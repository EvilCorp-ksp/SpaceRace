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
            SpaceRaceMain.researchProjects.Clear();

            ConfigNode rcn = node.GetNode("Rival");
            if (rcn != null)
            {
                SRRival.rival = SRRival.DecodeRival(rcn);
            }
            if (rcn == null)
            {
                SRRival.rival = SRRival.CreateNewRival();
            }

            ConfigNode spcn = node.GetNode("ScienceProjects");
            if (spcn != null)
            {
                foreach (ConfigNode n in spcn.GetNodes("ScienceProject"))
                {
                    ScienceProject project = new ScienceProject();
                    project.TechName = n.GetValue("Title");
                    project.techID = n.GetValue("techID");
                    project.KerbalAssigned = n.GetValue("KerbalAssigned");
                    project.UTTimeCompleted = Convert.ToDouble(n.GetValue("UTTimeCompleted"));
                    project.InProgress = Boolean.Parse(n.GetValue("InProgress"));
                    project.scienceCost = Convert.ToInt32(n.GetValue("ScienceCost"));
                    project.Completed = Boolean.Parse(n.GetValue("Completed"));
                    project.pNode = new ProtoTechNode(n.GetNode("TechNode"));
                    Debug.Log("pNode is " + project.pNode.techID);

                    SpaceRaceMain.researchProjects.Add(project);
                }
            }

            SpaceRaceMain.engineeringProjects.Clear();
            ConfigNode epcn = node.GetNode("EngineeringProjects");
            if (epcn != null)
            {
                foreach (ConfigNode n in epcn.GetNodes("EngineeringProject"))
                {
                    EngineeringProject project = new EngineeringProject();
                    project.Name = n.GetValue("Name");
                    project.KerbalAssigned = n.GetValue("KerbalAssigned");
                    project.UTTimeCompleted = Convert.ToDouble(n.GetValue("UTTimeCompleted"));
                    Debug.Log("SpaceRace: Loaded EngineeringProject " + project.Name + " with a UTTimeEnding of " + project.UTTimeCompleted);
                    project.Completed = Boolean.Parse(n.GetValue("Completed"));
                    project.InProgress = Boolean.Parse(n.GetValue("InProgress"));
                    project.Part = PartLoader.Instance.parts.FirstOrDefault(p => p.name == project.Name);
                    Debug.Log("SpaceRace: Loaded part " + project.Part.title);

                    SpaceRaceMain.engineeringProjects.Add(project);
                }
            }
            SREngineering.HideParts();
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            node.AddNode(SRRival.EncodeRival(SRRival.rival));

            ConfigNode spcn = new ConfigNode("ScienceProjects");
            foreach (ScienceProject project in SpaceRaceMain.researchProjects)
            {
                if (project.Completed == false)
                {
                    ConfigNode technode = new ConfigNode("TechNode");
                    ConfigNode temp = new ConfigNode("ScienceProject");
                    project.pNode.Save(technode);
                    temp.AddValue("Title", project.TechName);
                    temp.AddValue("techID", project.techID);
                    temp.AddValue("KerbalAssigned", project.KerbalAssigned);
                    temp.AddValue("UTTimeCompleted", project.UTTimeCompleted);
                    temp.AddValue("InProgress", project.InProgress);
                    temp.AddValue("ScienceCost", project.scienceCost);
                    temp.AddValue("Completed", project.Completed);

                    temp.AddNode(technode);
                    spcn.AddNode(temp);
                }

            }
            node.AddNode(spcn);

            ConfigNode epcn = new ConfigNode("EngineeringProjects");
            foreach (EngineeringProject project in SpaceRaceMain.engineeringProjects)
            {
                if (project.Completed == false)
                {
                    ConfigNode temp = new ConfigNode("EngineeringProject");
                    temp.AddValue("Name", project.Part.name);
                    temp.AddValue("KerbalAssigned", project.KerbalAssigned);
                    temp.AddValue("UTTimeCompleted", project.UTTimeCompleted);
                    temp.AddValue("Completed", project.Completed);
                    temp.AddValue("InProgress", project.InProgress);

                    epcn.AddNode(temp);
                }
            }
            node.AddNode(epcn);
            
        }

        //public void SaveData()
        //{
        //    Debug.Log("SpaceRace: Firing SaveData.");

        //    Debug.Log("SpaceRace: Firing BuildProjectList from SaveData");
        //    SRScience.BuildProjectList();
        //    using (StreamWriter writer = new StreamWriter(SpaceRaceMain.spaceracefolder + "RnDList"))
        //    {
        //        foreach (string line in researchList)
        //        {
        //            writer.WriteLine(line);
        //            Debug.Log("SpaceRace: SaveData - Writing line: " + line);
        //        }
        //        writer.Close();
        //    }
        //}

        //public void LoadData()
        //{

        //    researchList.Clear();
        //    Debug.Log("SpaceRace: Calling LoadData");
        //    if (File.Exists(SpaceRaceMain.spaceracefolder + "RnDList"))
        //    {
        //        using (StreamReader reader = new StreamReader(SpaceRaceMain.spaceracefolder + "RnDList"))
        //        {
        //            string line = "";
        //            while ((line = reader.ReadLine()) != null)
        //            {
        //                researchList.Add(line);
        //            }
        //            Debug.Log(String.Format("SpaceRace: Loaded {0} lines into researchList", researchList.Count));
        //            reader.Close();
        //            SpaceRaceMain.lastLoaded = true;
        //        }
        //        SpaceRaceMain.researchProjects.Clear();
        //        foreach (string line in SRUtilities.researchList)
        //        {
        //            SRScience.RebuildProjects(line);
        //        }
        //    }
        //}
    }
}


