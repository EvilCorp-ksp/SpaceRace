using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

namespace SpaceRace
{
    class SREngineering : MonoBehaviour
    {
        public static void StartEngineeringProject(string staff, int cost, AvailablePart part, bool status)
        {
            EngineeringProject project = new EngineeringProject();
            project = SpaceRaceMain.engineeringProjects.FirstOrDefault(p => p.Part == part);
            project.KerbalAssigned = staff;
            ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.Crew.FirstOrDefault(c => c.name == staff);
            project.UTTimeCompleted = Planetarium.GetUniversalTime() + CalcTime(crew.experienceLevel, part.entryCost);
            project.InProgress = status;
        }

        public static void CheckCompletedProjects()
        {
            foreach (EngineeringProject project in SpaceRaceMain.engineeringProjects)
            {
                if (Planetarium.GetUniversalTime() >= project.UTTimeCompleted && project.Completed == false)
                {
                    UnHidePart(project);
                    ReassignCrew(project);
                    project.Completed = true;
                    //SpaceRaceMain.researchProjects.Remove(project);
                }
            }
        }

        public static void ReassignCrew(EngineeringProject project)
        {
            foreach (ProtoCrewMember crew in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                if (crew.name == project.KerbalAssigned)
                {
                    crew.rosterStatus = ProtoCrewMember.RosterStatus.Available;
                }
            }
        }

        public static void HideParts()
        {
            foreach (EngineeringProject project in SpaceRaceMain.engineeringProjects)
            {
                if (project.Completed != true)
                {
                    project.Part.category = PartCategories.none;
                }
            }
        }

        public static void UnHidePart(EngineeringProject project)
        {
            AvailablePart part = PartLoader.LoadedPartsList.FirstOrDefault(p => p.name == project.Part.name);
            part.category = project.Category;
        }

        public static double CalcTime(int level, int partcost)
        {
            if (level == 0)
            {
                level = 1;
            }
            return (partcost * 50) / level;
        }
    }

    public class EngineeringProject
    {
        public string KerbalAssigned { get; set; } //Kerbal assigned to project. Kerbal is unavailable during project time.
        public double UTTimeCompleted { get; set; } //Exact time project will complete and the part and Kerbal assigned will both become available.
        public bool Completed { get; set; }
        public string Name { get; set; }
        public PartCategories Category { get; set; }
        public AvailablePart Part { get; set; } //Copy of the part itself.
        public bool InProgress { get; set; }
    }
}
