﻿using System;
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
            foreach (ScienceProject project in SpaceRaceMain.researchProjects)
            {
                SRUtilities.researchList.Add(project.TechName + ";" + project.techID + ";" + project.KerbalAssigned + /*";" + project.UTTimeStarted + */";" + project.UTTimeCompleted.ToString() + ";" + project.InProgress + ";" + project.Cost);
                Debug.Log("SpaceRace: Added to research list for saving: " + project.TechName + ";" + project.techID + ";" + project.KerbalAssigned + ";" + project.UTTimeCompleted.ToString() + ";" + project.InProgress + ";" + project.Cost);
            }
        }

        public static ScienceProject RebuildProjects(string projects)
        {
            ScienceProject project = new ScienceProject();
            string[] processor = projects.Split(';');
            Debug.Log("SpaceRace: Cleared researchProjects");
            ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.Crew.FirstOrDefault(c => c.name == processor[2]);
            Debug.Log("SpaceRace: Loaded assigned crew");
            project.TechName = processor[0];
            Debug.Log("SpaceRace: Loaded TechName");
            project.techID = processor[1];
            Debug.Log("SpaceRace: Loaded TechNode: " + processor[1]);
            project.KerbalAssigned = processor[2];
            Debug.Log("SpaceRace: Loaded KerbalAssigned");
            project.UTTimeCompleted = Convert.ToDouble(processor[3]);
            Debug.Log("SpaceRace: Loaded UTTimeCompleted");
            project.InProgress = Boolean.Parse(processor[4]);
            Debug.Log("SpaceRace: Loaded InProgress");
            project.Cost = Convert.ToInt32(processor[5]);
            Debug.Log("SpaceRace: Loaded Cost");
            Debug.Log("SpaceRace: Loading science project to list from file.");
            Debug.Log("SpaceRace: Project " + project.TechName + " loaded. Kerbal assigned: " + crew.name + ".");
            SpaceRaceMain.researchProjects.Add(project);
            Debug.Log("Added project to projects list");
            crew.rosterStatus = ProtoCrewMember.RosterStatus.Assigned;
            Debug.Log("Re-assigned crewmember to project");
            Debug.Log(project.techID);
            return project;
        }

        public static bool CheckResearchProject(string id)
        {
            bool check = false;
            foreach (ScienceProject project in SpaceRaceMain.researchProjects)
            {
                if (project.techID == id)
                {
                    check = true;
                }
            }
            return check;
        }

        public static void CheckCompletedProjects()
        {
            foreach (ScienceProject project in SpaceRaceMain.researchProjects)
            {
                if (Planetarium.GetUniversalTime() >= project.UTTimeCompleted && project.Completed == false)
                {
                    Unlock(project);
                    ReassignCrew(project);
                    //SpaceRaceMain.researchProjects.Remove(project);
                }
            }
        }

        public static void StartResearch(string staff, double timeending, string techid, bool inprogress)
        {
            foreach (ScienceProject p in SpaceRaceMain.researchProjects)
            {
                if (p.techID == techid)
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

        public static void ReassignCrew(ScienceProject project)
        {
            foreach (ProtoCrewMember crew in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                if (crew.name == project.KerbalAssigned)
                {
                    crew.rosterStatus = ProtoCrewMember.RosterStatus.Available;
                }
            }
        }

        public static void Lock(ScienceProject project)
        {
            project.pNode.state = RDTech.State.Unavailable;
            ResearchAndDevelopment.Instance.SetTechState(project.techID, project.pNode);
            Debug.Log("SpaceRace: Locked project.");
        }

        public static void Unlock(ScienceProject project)
        {
            project.pNode.state = RDTech.State.Available;
            ResearchAndDevelopment.Instance.SetTechState(project.techID, project.pNode);
            project.Completed = true;
            Debug.Log("SpaceRace: Unlocked project.");
        }
    }

    public class ScienceProject : RDTech
    {
        public string KerbalAssigned { get; set; } //Kerbal assigned to project. Kerbal is unavailable during project time.
        public double UTTimeCompleted { get; set; } //Exact time project will complete and the part and Kerbal assigned will both become available.
        //public double UTTimeStarted { get; set; }
        public bool InProgress { get; set; } //Boolean for progress status.
        public bool Completed { get; set; }
        public int Cost { get; set; }
        public string TechName { get; set; }
        public ProtoTechNode pNode;
        

        //public void Lock()
        //{
        //    if (this.state == RDTech.State.Available)
        //    {
        //        state = RDTech.State.Unavailable;
        //        ProtoTechNode node = ResearchAndDevelopment.Instance.GetTechState(techID);
        //        node.state = state;
        //        ResearchAndDevelopment.Instance.SetTechState(techID, ResearchAndDevelopment.Instance.GetTechState(techID));
        //        Debug.Log("SpaceRace: Locked project.");
        //    }
        //}
    }
}
