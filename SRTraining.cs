using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
    public static class SRTraining
    {
        public static void TradeSchool(string name)
         {
             foreach (ProtoCrewMember c in HighLogic.CurrentGame.CrewRoster.Crew)
             {
                 
                 if (c.name == name)
                 {
                     FlightLog.Entry flight = new FlightLog.Entry(0, FlightLog.EntryType.Flight, "Jool");
                     c.careerLog.AddEntry(flight);
                     //if (c.experience < 2)
                     //{
                     //    c.experience = 2;
                     //    c.experienceLevel = 1;
                     //}
                     //else if (c.experience >= 2 && c.experience < 8)
                     //{
                     //    c.experience = 8;
                     //    c.experienceLevel = 2;
                     //}
                     //else if (c.experience >= 8 && c.experience < 16)
                     //{
                     //    c.experience = 16;
                     //    c.experienceLevel = 3;
                     //}
                     //else if (c.experience >= 16 && c.experience < 32)
                     //{
                     //    c.experience = 32;
                     //    c.experienceLevel = 4;
                     //}
                     //else if (c.experience >= 32 && c.experience < 64)
                     //{
                     //    c.experience = 64;
                     //    c.experienceLevel = 5;
                     //}
                 }
             }
         }

         public static void AddXP(string name, float xp)
        {
            foreach (ProtoCrewMember c in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                if (c.name == name)
                {
                    c.experience += xp;
                    if (c.experience < 2)
                    {
                        c.experienceLevel = 0;
                    }
                    else if (c.experience >= 2 && c.experience < 8)
                    {
                        c.experienceLevel = 1;
                    }
                    else if (c.experience >= 8 && c.experience < 16)
                    {
                        c.experienceLevel = 2;
                    }
                    else if (c.experience >= 16 && c.experience < 32)
                    {
                        c.experienceLevel = 3;
                    }
                    else if (c.experience >= 32 && c.experience < 64)
                    {
                        c.experienceLevel = 4;
                    }
                    else if (c.experience >= 64)
                    {
                        c.experienceLevel = 5;
                    }

                }
            }
        }
    }

   
}
