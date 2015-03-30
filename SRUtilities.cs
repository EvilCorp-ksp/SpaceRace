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
            //LoadData();
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            SRScience.BuildProjectList();
            SaveData();
        }

        public void SaveData()
        {
            Debug.Log("SpaceRace: Calling SaveData");

            using (StreamWriter writer = new StreamWriter(SpaceRace.SpaceRaceMain.save_folder + "RnDList"))
            {
                foreach (string line in researchList)
                {
                    writer.WriteLine(line);
                    Debug.Log("SpaceRace: SaveData - Writing line: " + line);
                }
            }
        }

        public void LoadData()
        {
            researchList.Clear();
            Debug.Log("SpaceRace: Calling LoadData");
            using (StreamReader reader = new StreamReader(SpaceRace.SpaceRaceMain.save_folder + "RnDList"))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    researchList.Add(line);
                }
                Debug.Log(String.Format("SpaceRace: Loaded {0} lines into researchList", researchList.Count));
            }
        }
    }
}


