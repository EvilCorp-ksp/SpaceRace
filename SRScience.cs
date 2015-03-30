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
        public static List<ScienceProject> researchProjects = new List<ScienceProject>();

        public static void BuildProjectList()
        {
            SRUtilities.researchList.Clear();
            foreach (ScienceProject project in researchProjects)
            {
                SRUtilities.researchList.Add(project.TechName + ";" + project.TechNode + ";" + project.KerbalAssigned + ";" + project.UTTimeCompleted.ToString() + ";" + project.InProgress);
            }
        }

        public static bool CheckResearchProject(List<ScienceProject> p, string id)
        {
            return  p.FirstOrDefault(t => t.TechNode == id) != null;
        }

        public static void CheckCompletedProjects()
        {
            foreach (ScienceProject project in researchProjects)
            {
                if (Planetarium.GetUniversalTime() >= project.UTTimeCompleted)
                {

                    CompleteResearchProject(project.KerbalAssigned, project.TechNode, project.node);
                }
                else
                {
                    project.Lock();
                }
            }
        }

        public static void StartResearch(string staff, double timeending, string techid, bool inprogress)
        {
            foreach (ScienceProject p in researchProjects)
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
            Debug.Log("Starting CalcTime");
            double result = Math.Round(Planetarium.GetUniversalTime(), 0, MidpointRounding.AwayFromZero);
            Debug.Log("Result 1: " + result);
            result += (21600 * sciencecost) / level;
            Debug.Log("Result 2: " + result);
            return result;
        }

 
        public static void CompleteResearchProject(string staff, string tech, ProtoTechNode node)
        {
            ScienceProject project = researchProjects.FirstOrDefault(p => p.TechNode == tech);
            ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.Crew.FirstOrDefault(c => c.name == staff);
            project.Unlock();
            crew.rosterStatus = ProtoCrewMember.RosterStatus.Available;
            int index = researchProjects.FindIndex(i => i.TechNode == tech);
            researchProjects.RemoveAt(index);
         }    
    }


    public class ScienceProject
    {
        public string KerbalAssigned { get; set; } //Kerbal assigned to project. Kerbal is unavailable during project time.
        public double UTTimeCompleted { get; set; } //Exact time project will complete and the part and Kerbal assigned will both become available.
        public bool InProgress { get; set; } //Boolean for progress status.
        public string TechNode { get; set; } //Tech node internal name 
        public string TechName { get; set; }
        public ProtoTechNode node { get; set; } //Node itself
        public int Cost { get; set; }
        public void Unlock()
        {
            node.state = RDTech.State.Available;
            ResearchAndDevelopment.Instance.SetTechState(TechNode, node);
        }
        public void Lock()
        {
            node.state = RDTech.State.Unavailable;
            ResearchAndDevelopment.Instance.SetTechState(TechNode, node);
        }
        public bool CheckList()
        {
            return SRScience.researchProjects.FirstOrDefault(t => t.TechNode == this.TechNode) != null;
        }
    }
}
