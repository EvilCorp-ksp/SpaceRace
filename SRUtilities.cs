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
                    SpaceRaceMain.lastLoaded = true;
                }
                SpaceRaceMain.Instance.researchProjects.Clear();
                foreach (string line in SRUtilities.researchList)
                {
                    SRScience.RebuildProjects(line);
                }
            }
        }
    }
}


