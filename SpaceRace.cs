using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;
using SpaceRace;

namespace SpaceRace
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class SpaceRaceMain : MonoBehaviour
    {
        public static String save_folder = "";
        public static string spaceracefolder = "";
        public ApplicationLauncherButton button;
        private Rect mainWindow = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 62, 400, 125);
        private Rect trainingWindow = new Rect(Screen.width / 8 - 50, Screen.height / 4 + 100, 400, 300);
        private Rect researchWindow = new Rect(Screen.width / 8, Screen.height / 2, 400, 300);
        private Rect engineeringWindow = new Rect(Screen.width / 8, Screen.height / 4 + 100, 800, 400);
        private Rect rivalWindow = new Rect(Screen.width / 8, Screen.height / 4 + 100, 400, 125);
        private Rect kerbalDetails = new Rect(200, 200, 400, 275);
        private Rect scienceProjects = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 100, 400, 200);
        private Rect assignKerbal = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200);
        private Rect pickSim = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200);
        private ProtoCrewMember kerbCrew;
        public static SpaceRaceMain Instance;
        private ScienceProject nodeBuffer;
        List<ProtoCrewMember> crewMembers = new List<ProtoCrewMember>();
        public static List<ScienceProject> researchProjects = new List<ScienceProject>();
        public static List<Simulation> simulations = new List<Simulation>();
        public int simIndex = 0;
        Vector2 scrollPositionName = Vector2.zero;
        Vector2 scrollPositionENG = Vector2.zero;
        Vector2 scrollPositionRES = Vector2.zero;
        public static int cnCounter = 0;
        public static bool lastLoaded = false;
        public int currentScience = 0;

        void Update()
        {
            if (HighLogic.LoadedSceneHasPlanetarium)
            {
                SRScience.CheckCompletedProjects();
            }
        }

        public void Start()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }
            Instance = this;
            save_folder = GetRootPath() + "/saves/" + HighLogic.SaveFolder + "/";
            Directory.CreateDirectory(SpaceRaceMain.save_folder + "/SpaceRace");
            spaceracefolder = save_folder + "/SpaceRace/";
            Debug.Log("SpaceRace: Folder set to " + spaceracefolder);
            Debug.Log("SpaceRace: Adding events");
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            GameEvents.OnTechnologyResearched.Add(TriggerResearch);
            GameEvents.onGUIRnDComplexDespawn.Add(TriggerLock);
            GameEvents.OnPartPurchased.Add(AddEngineeringProject);
            SRTraining.BuildList();
        }

        public void AddEngineeringProject(AvailablePart ap)
        {
            Debug.Log("Firing AddEngineeringProject");
            ScienceProject sp = new ScienceProject();
            Debug.Log("Verifying research project is complete");
            sp = researchProjects.FirstOrDefault(s => s.techID == ap.TechRequired);
            if (sp != null)
            {
                ScreenMessages.PostScreenMessage("Technology required is still being researched!", 5.0f, ScreenMessageStyle.UPPER_LEFT);
                Funding.Instance.AddFunds(ap.entryCost, TransactionReasons.RnDPartPurchase);
                sp.partsPurchased.Remove(ap);
                sp.state = RDTech.State.Unavailable;
                ResearchAndDevelopment.Instance.SetTechState(sp.techID, ResearchAndDevelopment.Instance.GetTechState(sp.techID));
            }
            else
            {
                SREngineering.EngineeringProject project = new SREngineering.EngineeringProject();
                project = SREngineering.engineeringProjects.FirstOrDefault(p => p.Part == ap);
                if (project == null)
                {
                    SREngineering.engineeringProjects.Add(new SREngineering.EngineeringProject { Part = ap, Category = ap.category });
                }
            }
            SREngineering.HideParts();
        }

        public void TriggerLock()
        {
            Debug.Log("SpaceRace: Fired TriggerLock.");
            Debug.Log(researchProjects.Count + " projects in the list");
            foreach (ScienceProject project in researchProjects)
            {
                SRScience.Lock(project);
            }
        }

        public void TriggerResearch(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> result)
        {
            
            Debug.Log("SpaceRace: Purchased tech, firing event.");
            if (result.host != null && result.target == RDTech.OperationResult.Successful)
            {
                result.host.state = RDTech.State.Unavailable;
                ScienceProject project = new ScienceProject() { partsPurchased = ResearchAndDevelopment.Instance.GetTechState(result.host.techID).partsPurchased, UTTimeCompleted = 9999999999f, KerbalAssigned = "None", techID = result.host.techID, TechName = result.host.title, Cost = result.host.scienceCost, InProgress = false, state = RDTech.State.Unavailable, Completed = false };
                if (SRScience.CheckResearchProject(result.host.techID) == false)
                {
                    Debug.Log("SpaceRace: Adding science project to list.");
                    researchProjects.Add(project);
                    ScreenMessages.PostScreenMessage("Science project added to list! Please assign a Scientist to research.", 5.0f, ScreenMessageStyle.UPPER_LEFT);
                    SRScience.Lock(project);
                }
                else 
                {
                    foreach (ScienceProject p in researchProjects)
                    {
                        if (p.techID == result.host.techID)
                        {
                            project = p;
                        }
                    }
                    //project = researchProjects.FirstOrDefault(p => p.techID == result.host.techID);
                    if (project.InProgress == true)
                    {
                        ResearchAndDevelopment.Instance.AddScience(result.host.scienceCost, TransactionReasons.RnDTechResearch);
                        ScreenMessages.PostScreenMessage("Already begun, research will complete in " + FormatTime(CalcTimeLeft(project.UTTimeCompleted)) + ".", 5.0f, ScreenMessageStyle.UPPER_LEFT);
                        Debug.Log("Passed UT time of " + CalcTimeLeft(project.UTTimeCompleted));
                        if (project.state == RDTech.State.Available)
                        {
                            SRScience.Lock(project);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("SpaceRace: Unknown error in Research Center");
            }
        }

        public double CalcTimeLeft(double uttime)
        {
            return Math.Round(uttime - Planetarium.GetUniversalTime(), 0, MidpointRounding.AwayFromZero);
        }

        public string FormatTime(double uttime)
        {
            string time = "";
            string days = Math.Floor(uttime / 21600).ToString();
            uttime = uttime % 21600;
            string hours = Math.Floor(uttime / 3600).ToString();
            uttime = uttime % 3600;
            string minutes = Math.Floor(uttime / 60).ToString();
            uttime = Math.Round(uttime % 60, 0, MidpointRounding.AwayFromZero);
            time = days + ":" + hours + ":" + minutes + ":" + uttime.ToString();
            return time;
        }

        private void ScienceWindow(int windowId)
        {
            GUILayout.BeginVertical();
            scrollPositionRES = GUILayout.BeginScrollView(scrollPositionRES, HighLogic.Skin.scrollView);
            foreach (ScienceProject project in researchProjects)
            {
                if (project.Completed == false)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(project.TechName, HighLogic.Skin.button, GUILayout.Width(150)))
                    {

                    }
                    if (GUILayout.Button(project.KerbalAssigned, HighLogic.Skin.button, GUILayout.Width(150), GUILayout.MinWidth(150), GUILayout.MaxWidth(150)))
                    {
                        nodeBuffer = project;
                        RenderingManager.AddToPostDrawQueue(0, AssignKerbDraw);
                    }
                    GUILayout.FlexibleSpace();
                    if (project.InProgress == true)
                    {
                        GUILayout.Label(FormatTime(project.UTTimeCompleted - Planetarium.GetUniversalTime()), HighLogic.Skin.label, GUILayout.Width(100));
                    }
                    else
                    {
                        GUILayout.Label("0:0:0:0", HighLogic.Skin.label, GUILayout.Width(100));
                    }
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndScrollView();
            if (GUILayout.Button("Close", HighLogic.Skin.button))
            {
                RenderingManager.RemoveFromPostDrawQueue(0, ScienceDraw);
                RenderingManager.AddToPostDrawQueue(0, OnDraw);
            }
            GUILayout.EndVertical();
        }

        private void ScienceDraw()
        {
            scienceProjects = GUILayout.Window(84625, scienceProjects, ScienceWindow, "Pick a project to assign", HighLogic.Skin.window);
        }

        private void AssignKerbDraw()
        {
            assignKerbal = GUILayout.Window(98351, assignKerbal, AssignKerbal, "Assign Kerbal to project", HighLogic.Skin.window);
        }

        private void SimulationDraw()
        {
            pickSim = GUILayout.Window(76523, pickSim, PickSim, "Pick Mission Simulation", HighLogic.Skin.window);
        }

        private void PickSim(int windowID)
        {
            GUILayout.BeginVertical();
            scrollPositionRES = GUILayout.BeginScrollView(scrollPositionRES, false, true);
            foreach (Simulation sim in simulations)
            {
                if (GUILayout.Button(sim.description, HighLogic.Skin.button))
                {
                    simIndex = simulations.IndexOf(sim);
                    RenderingManager.RemoveFromPostDrawQueue(0, SimulationDraw);
                }
            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("Close", HighLogic.Skin.button))
            {
                RenderingManager.RemoveFromPostDrawQueue(0, SimulationDraw);
            }
            GUILayout.EndVertical();
        }

        private void AssignKerbal(int windowID)
        {
            GUILayout.BeginVertical();
            scrollPositionRES = GUILayout.BeginScrollView(scrollPositionRES, false, true);
            foreach (ProtoCrewMember crew in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                if (crew.rosterStatus == ProtoCrewMember.RosterStatus.Available && crew.experienceTrait.TypeName == "Scientist")
                {
                    if (GUILayout.Button(crew.name, HighLogic.Skin.button))
                    {
                        ScienceProject project = new ScienceProject();
                        project = researchProjects.FirstOrDefault(p => p.techID == nodeBuffer.techID);

                        if (project.InProgress == false)
                        {    
                            SRScience.StartResearch(crew.name, SRScience.CalcTime(crew.experienceLevel, project.Cost), project.techID, true);
                            crew.rosterStatus = ProtoCrewMember.RosterStatus.Assigned;
                            RenderingManager.RemoveFromPostDrawQueue(0, AssignKerbDraw);
                        }
                    }
                }
            }


            GUILayout.EndScrollView();
            if (GUILayout.Button("Close", HighLogic.Skin.button))
            {
                RenderingManager.RemoveFromPostDrawQueue(0, AssignKerbDraw);
            }
            GUILayout.EndVertical();
        }

        void OnGUIAppLauncherReady()
        {
            {
                this.button = ApplicationLauncher.Instance.AddModApplication(
                    onAppLauncherToggleOn,
                    onAppLauncherToggleOff,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.SPACECENTER,
                    (Texture)GameDatabase.Instance.GetTexture("EvilCorp/Textures/SRlogo2", false));
            }
        }

        public void DestroyButtons()
        {
            ApplicationLauncher.Instance.RemoveModApplication(button);
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
        }

        public static String GetRootPath()
        {
            String path = KSPUtil.ApplicationRootPath;
            path = path.Replace("\\", "/");
            if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
            return path;
        }

        void onAppLauncherToggleOn()
        {
            Debug.Log("SpaceRace: Open Menu");
            RenderingManager.AddToPostDrawQueue(0, OnDraw);
        }

        void onAppLauncherToggleOff()
        {
            Debug.Log("SpaceRace: Close Menu");
            RenderingManager.RemoveFromPostDrawQueue(0, OnDraw);
        }

        private void OnDraw()
        {
            mainWindow = GUILayout.Window(65832, mainWindow, OnWindow, "Space Race", HighLogic.Skin.window);
        }

        public void OnDestroy()
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            GameEvents.OnTechnologyResearched.Remove(TriggerResearch);
            GameEvents.onGUIRnDComplexDespawn.Remove(TriggerLock);
            GameEvents.OnPartPurchased.Remove(AddEngineeringProject);
            if (HighLogic.LoadedSceneHasPlanetarium)
            {
                ApplicationLauncher.Instance.RemoveModApplication(button);
            }
        }

        void OnGUI()
        {
            GUI.skin = HighLogic.Skin;
        }

        private void TrainingDraw()
        {
            trainingWindow = GUILayout.Window(87465, trainingWindow, TrainingWindow, "Training Center", HighLogic.Skin.window);
        }

        private void TrainingWindow(int windowId)
        {
            GUILayout.BeginHorizontal();
            scrollPositionName = GUILayout.BeginScrollView(scrollPositionName, false, true); 
            GUILayout.BeginVertical();
            foreach (ProtoCrewMember k in HighLogic.CurrentGame.CrewRoster.Crew)
            {
               GUILayout.BeginHorizontal();
               GUILayout.Label(k.name , GUILayout.Width(150));
               GUILayout.Label("Level " + k.experienceLevel.ToString(), GUILayout.Width(60));
               GUILayout.Label(k.experienceTrait.TypeName, GUILayout.Width(60));
               GUILayout.FlexibleSpace();
               if (GUILayout.Button("Details", HighLogic.Skin.button, GUILayout.Width(50)))
               {
                   kerbCrew = k;
                   RenderingManager.AddToPostDrawQueue(0, KerbDraw);
               }
               GUILayout.FlexibleSpace();
               GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Close"))
            {
                RenderingManager.RemoveFromPostDrawQueue(0, TrainingDraw);
            }
            GUI.DragWindow();
        }

        private void KerbDraw()
        {
            kerbalDetails = GUILayout.Window(34562, kerbalDetails, DetailsWindow, "Kerbal Details", HighLogic.Skin.window);
        }

        private void DetailsWindow(int windowId)
        {
            float t = (kerbCrew.experienceLevel + 1) * 1000;
            switch (kerbCrew.experienceLevel)
            {
                case 0:
                    break;
                case 1:
                    t = 3000;
                    break;
                case 2:
                    t = 6000;
                    break;
                case 3:
                    t = 10000;
                    break;
                case 4:
                    t = 15000;
                    break;
                default:
                    break;
            }

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label(kerbCrew.name);
            GUILayout.Label("\nLevel " + kerbCrew.experienceLevel + " " + kerbCrew.experienceTrait.TypeName,HighLogic.Skin.label ,GUILayout.ExpandWidth(true));
            GUILayout.Label(kerbCrew.experienceTrait.Description);
            GUILayout.Label("Traits: ");
            GUILayout.Label(kerbCrew.experienceTrait.DescriptionEffects);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            if (kerbCrew.experienceLevel < 3)
            {
                GUILayout.Label("Mission Simulation: " + t);
                if (Funding.Instance.Funds >= t)
                {
                    if (GUILayout.Button(simulations[simIndex].description, HighLogic.Skin.button))
                    {
                        RenderingManager.AddToPostDrawQueue(0, SimulationDraw);
                    }
                    if (GUILayout.Button("Train", HighLogic.Skin.button))
                    {
                        SRTraining.MissionSim(simulations[simIndex], kerbCrew.name);
                        kerbCrew.ArchiveFlightLog();
                        Funding.Instance.AddFunds(-t, 0);
                        simIndex = 0;
                    }
                }
                else if (Funding.Instance.Funds < t)
                {
                    GUILayout.Label("Not enough funds for training!", HighLogic.Skin.label);
                }
            }
            else if (kerbCrew .experienceLevel >= 5)
            {
                GUILayout.Label("Maximum level, no more \ntraining possible", HighLogic.Skin.label);
            }
            //GUILayout.Label("Traits: ", HighLogic.Skin.label);
            //GUILayout.Label(kerbCrew.experienceTrait.DescriptionEffects, HighLogic.Skin.label);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", HighLogic.Skin.button))
            {
                RenderingManager.RemoveFromPostDrawQueue(0, KerbDraw);
            }
            GUI.DragWindow();
        }

        private void ResearchDraw()
        {
            researchWindow = GUILayout.Window(76132, researchWindow, ResearchWindow, "Research Center", HighLogic.Skin.window);
        }

        private void ResearchWindow(int windowId)
        {
            scrollPositionRES = GUILayout.BeginScrollView(scrollPositionRES, false, true);
            GUILayout.BeginVertical();
            if (GUILayout.Button("Open Project List", HighLogic.Skin.button))
            {
                RenderingManager.AddToPostDrawQueue(0, ScienceDraw);
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
#if DEBUG
            GUILayout.BeginVertical();
            if (GUILayout.Button("Give 50 Science", HighLogic.Skin.button))
            {
                ResearchAndDevelopment.Instance.AddScience(50, 0);
            }
            if (GUILayout.Button("Close", HighLogic.Skin.button))
            {
                RenderingManager.RemoveFromPostDrawQueue(0, ResearchDraw);
            }

            GUILayout.EndVertical();
#endif
            GUI.DragWindow();
        }

        private void EngineeringDraw()
        {
            engineeringWindow = GUILayout.Window(15623, engineeringWindow, EngineeringWindow, "Engineering", HighLogic.Skin.window);
        }

        private void EngineeringWindow(int windowId)
        {
            scrollPositionENG = GUILayout.BeginScrollView(scrollPositionENG, false, true);
            GUILayout.BeginVertical();
            foreach (SREngineering.EngineeringProject project in SREngineering.engineeringProjects)
            {
                AvailablePart part = PartLoader.LoadedPartsList.FirstOrDefault(p => p.name == project.Part.name);
                GUILayout.Label(project.Part.name + " " + project.Part.entryCost + " " + project.Part.category.ToString() + " " + project.Category.ToString(), HighLogic.Skin.label);
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            if (GUILayout.Button("Close", HighLogic.Skin.button))
            {
                RenderingManager.RemoveFromPostDrawQueue(0, EngineeringDraw);
            }
            GUI.skin = HighLogic.Skin;
            GUI.DragWindow();
        }

        private void RivalDraw()
        {
            rivalWindow = GUILayout.Window(65324, rivalWindow, RivalWindow, "Rival Space Center");
        }

        private void RivalWindow(int windowId)
        {
            
        }

        private void OnWindow(int windowId)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Training Center", HighLogic.Skin.button))
            {
                RenderingManager.AddToPostDrawQueue(0, TrainingDraw);
                RenderingManager.RemoveFromPostDrawQueue(0, OnDraw);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Research Center", HighLogic.Skin.button))
            {
                RenderingManager.AddToPostDrawQueue(0, ScienceDraw);
                RenderingManager.RemoveFromPostDrawQueue(0, OnDraw);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Engineering Department", HighLogic.Skin.button))
            {
                RenderingManager.AddToPostDrawQueue(0, EngineeringDraw);
                RenderingManager.RemoveFromPostDrawQueue(0, OnDraw);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("View Rival Space Center", HighLogic.Skin.button))
            {
                RenderingManager.RemoveFromPostDrawQueue(0, OnDraw);
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

     }
}
