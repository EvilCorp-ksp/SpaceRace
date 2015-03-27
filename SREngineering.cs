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
        public static List<EngineeringProject> engineeringProjects = new List<EngineeringProject>();
        
        public class EngineeringProject
        {
            public string KerbalAssigned { get; set; } //Kerbal assigned to project. Kerbal is unavailable during project time.
            public double UTTimeCompleted { get; set; } //Exact time project will complete and the part and Kerbal assigned will both become available.
            public PartCategories Category { get; set; }
            public AvailablePart Part { get; set; } //Copy of the part itself.
        }

        

        public void StartEngineeringProject(string staff, int cost, AvailablePart part)
        {
            EngineeringProject project = new EngineeringProject();
            project = engineeringProjects.FirstOrDefault(p => p.Part == part);
            project.KerbalAssigned = staff;
            ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.Crew.FirstOrDefault(c => c.name == staff);
            project.UTTimeCompleted = Planetarium.GetUniversalTime() + CalcTime(crew.experienceLevel, part.entryCost);
        }

        public static void HideParts()
        {
            foreach (EngineeringProject project in engineeringProjects)
            {
                project.Part.category = PartCategories.none;
            }
        }

        public static double CalcTime(int level, int partcost)
        {
            return (partcost * 50) / level;
        }
    }
}
