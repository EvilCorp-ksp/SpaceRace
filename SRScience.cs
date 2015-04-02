using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace SpaceRace
{
    public class SRScience : MonoBehaviour
    {
        public static void BuildProjectList()
        {
            SRUtilities.researchList.Clear();
            foreach (ScienceProject project in SpaceRaceMain.Instance.researchProjects)
            {
                SRUtilities.researchList.Add(project.TechName + ";" + project.TechNode + ";" + project.KerbalAssigned + ";" + project.UTTimeCompleted.ToString() + ";" + project.InProgress + ";" + project.Cost);
                Debug.Log("SpaceRace: Added to research list for saving: " + project.TechName + ";" + project.TechNode + ";" + project.KerbalAssigned + ";" + project.UTTimeCompleted.ToString() + ";" + project.InProgress + ";" + project.Cost);
            }
        }

        public static bool CheckResearchProject(List<ScienceProject> p, string id)
        {
            return  p.FirstOrDefault(t => t.TechNode == id) != null;
        }

        public static void CheckCompletedProjects()
        {
            foreach (ScienceProject project in SpaceRaceMain.Instance.researchProjects)
            {
                if (Planetarium.GetUniversalTime() >= project.UTTimeCompleted)
                {
                    CompleteResearchProject(project.KerbalAssigned, project.TechNode, project.node);
                }
                //else
                //{
                //    project.Lock();
                //}
            }
        }

        public static void StartResearch(string staff, double timeending, string techid, bool inprogress)
        {
            foreach (ScienceProject p in SpaceRaceMain.Instance.researchProjects)
            {
                if (p.TechNode == techid)
                {
                    p.KerbalAssigned = staff;
                    p.UTTimeCompleted = timeending;
                    p.InProgress = inprogress;
                }
            }
        }

        public static double CalcTime(int level, int sciencecost)
        {
            if (level == 0)
            {
                level = 1;
            }
            double result = Math.Round(Planetarium.GetUniversalTime(), 0, MidpointRounding.AwayFromZero);
            result += (21600 * sciencecost) / level;
            return result;
        }

 
        public static void CompleteResearchProject(string staff, string tech, ProtoTechNode node)
        {
            ScienceProject project = SpaceRaceMain.Instance.researchProjects.FirstOrDefault(p => p.TechNode == tech);
            ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.Crew.FirstOrDefault(c => c.name == staff);
            project.node.state = RDTech.State.Available;
            ResearchAndDevelopment.Instance.SetTechState(project.TechNode, project.node);
            crew.rosterStatus = ProtoCrewMember.RosterStatus.Available;
            int index = SpaceRaceMain.Instance.researchProjects.FindIndex(i => i.TechNode == tech);
            SpaceRaceMain.Instance.researchProjects.RemoveAt(index);
         }    
    }


    public class ScienceProject
    {
        public string KerbalAssigned { get; set; } //Kerbal assigned to project. Kerbal is unavailable during project time.
        public double UTTimeCompleted { get; set; } //Exact time project will complete and the part and Kerbal assigned will both become available.
        public bool InProgress { get; set; } //Boolean for progress status.
        public string TechNode { get; set; } //Tech node internal name 
        public string TechName { get; set; }
        public ProtoTechNode node  = new ProtoTechNode(); 
        public int Cost { get; set; }
        public void Unlock()
        {
            node.state = RDTech.State.Available;
            ResearchAndDevelopment.Instance.SetTechState(TechNode, node);
        }
        public void Lock()
        {
            //if (node.state == RDTech.State.Available)
            //{
            Debug.Log(node.techID);
            node.state = RDTech.State.Unavailable;
            ResearchAndDevelopment.Instance.SetTechState(node.techID, node);
            Debug.Log("SpaceRace: Locked project.");
            //}
        }
        public bool CheckList()
        {
            return SpaceRaceMain.Instance.researchProjects.FirstOrDefault(t => t.TechNode == this.TechNode) != null;
        }
    }
}
