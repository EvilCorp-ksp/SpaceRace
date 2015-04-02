using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace SpaceRace
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, new GameScenes[] { GameScenes.FLIGHT, GameScenes.SPACECENTER })]
    class SRUtilities : ScenarioModule
    {
        public static List<string> researchList = new List<string>();
        public static List<string> engineeringList = new List<string>();

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            LoadData();
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            SaveData();
        }

        public void SaveData()
        {
            Debug.Log("SpaceRace: Firing SaveData.");
            //if (researchList.Count >= 1)
            //{
            Debug.Log("SpaceRace: Firing BuildProjectList from SaveData");
            SRScience.BuildProjectList();
            using (StreamWriter writer = new StreamWriter(SpaceRaceMain.spaceracefolder + "RnDList"))
            {
                foreach (string line in researchList)
                {
                    writer.WriteLine(line);
                    Debug.Log("SpaceRace: SaveData - Writing line: " + line);
                }
                writer.Close();
            }
            //}
        }

        public void LoadData()
        {
            researchList.Clear();
            Debug.Log("SpaceRace: Calling LoadData");
            if (File.Exists(SpaceRaceMain.spaceracefolder + "RnDList"))
            {
                using (StreamReader reader = new StreamReader(SpaceRaceMain.spaceracefolder + "RnDList"))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        researchList.Add(line);
                    }
                    Debug.Log(String.Format("SpaceRace: Loaded {0} lines into researchList", researchList.Count));
                    reader.Close();
                }
            }

            foreach (string line in researchList)
            {
                string[] processor = line.Split(';');
                SpaceRaceMain.Instance.researchProjects.Clear();
                Debug.Log("SpaceRace: Cleared researchProjects");
                ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.Crew.FirstOrDefault(c => c.name == processor[2]);
                Debug.Log("SpaceRace: Loaded assigned crew");
                ScienceProject project = new ScienceProject();// { node = ResearchAndDevelopment.Instance.GetTechState(processor[1]), UTTimeCompleted = Convert.ToDouble(processor[4]), KerbalAssigned = processor[2], TechNode = processor[1], TechName = processor[0], Cost = Convert.ToInt32(processor[5]), InProgress = Boolean.Parse(processor[4]) };
                project.node = ResearchAndDevelopment.Instance.GetTechState(processor[1]);
                Debug.Log("SpaceRace: Loaded ProtoTechNode");
                project.UTTimeCompleted = Convert.ToDouble(processor[3]);
                Debug.Log("SpaceRace: Loaded UTTimeCompleted");
                project.KerbalAssigned = processor[2];
                Debug.Log("SpaceRace: Loaded KerbalAssigned");
                project.TechNode = processor[1];
                Debug.Log("SpaceRace: Loaded TechNode: " + processor[1]);
                project.TechName = processor[0];
                Debug.Log("SpaceRace: Loaded TechName");
                project.Cost = Convert.ToInt32(processor[5]);
                Debug.Log("SpaceRace: Loaded Cost");
                project.InProgress = Boolean.Parse(processor[4]);
                Debug.Log("SpaceRace: Loaded InProgress");
                if (!project.CheckList())
                {
                    Debug.Log("SpaceRace: Loading science project to list from file.");
                    Debug.Log("SpaceRace: Project " + project.TechName + " loaded. Kerbal assigned: " + crew.name + ".");
                    SpaceRaceMain.Instance.researchProjects.Add(project);
                    Debug.Log("Added project to projects list");
                    crew.rosterStatus = ProtoCrewMember.RosterStatus.Assigned;
                    Debug.Log("Re-assigned crewmember to project");
                    //if (project.node.state == RDTech.State.Available)
                    //{
                    project.Lock();
                    Debug.Log("Locked project");
                    //}
                }
            }
        }
    }
}


