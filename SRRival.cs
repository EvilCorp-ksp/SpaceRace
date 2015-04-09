using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using UnityEngine;

namespace SpaceRace
{
    class SRRival
    {
        
        public static List<Events> eventList = new List<Events>();
        public static Rival rival = new Rival();
        
        public static void GenerateEventList()
        {
            eventList.Clear();
        }

        public Event PickEvent(string type)
        {
            Event ev = new Event();

            return ev;
        }

        public static void MinorEvent()
        {
            if (Planetarium.GetUniversalTime() >= rival.NextMinor)
            {
                int result = UnityEngine.Random.Range(0, 99);
                if (result >= 0 && result < 26)
                {
                    rival.PreviousMinor = Planetarium.GetUniversalTime();
                    rival.NextMinor = Planetarium.GetUniversalTime() + UnityEngine.Random.Range(216000, 864000);
                }

            }
        }

        public static void MajorEvent()
        {
            if (Planetarium.GetUniversalTime() / 21600 >= rival.NextMajor)
            {
                int result = UnityEngine.Random.Range(0, 99);
                if (result >= 0 && result < 35)
                {
                    rival.PreviousMajor =  Math.Floor(Planetarium.GetUniversalTime() / 21600);
                    rival.NextMinor = Math.Floor(Planetarium.GetUniversalTime() / 21600) + 106;
                }

            }
        }

        public static Rival CreateNewRival()
        {
            Rival r = new Rival()
            {
                Name = "Space-Z",
                CurrentScience = 0,
                Funds = 25000f,
                Reputation = 0f,
                PreviousMinor = Planetarium.GetUniversalTime(),
                PreviousMajor = Math.Floor(Planetarium.GetUniversalTime() / 21600),
                NextMinor = Planetarium.GetUniversalTime() + 216000,
                NextMajor = Math.Floor(Planetarium.GetUniversalTime() / 21600) + 106,
                Staff = HireStartingKerbals(),
                ScienceNodes = new List<string>() { "start" }
            };
            return r;
        }

        public static void LockStaff()
        {
            foreach (string staff in rival.Staff)
            {
                foreach (ProtoCrewMember crew in HighLogic.CurrentGame.CrewRoster.Applicants)
                {
                    if (crew.name == staff)
                    {
                        crew.rosterStatus = ProtoCrewMember.RosterStatus.Missing;
                    }
                }
                foreach (ProtoCrewMember crew in HighLogic.CurrentGame.CrewRoster.Crew)
                {
                    if (crew.name == staff)
                    {
                        crew.rosterStatus = ProtoCrewMember.RosterStatus.Missing;
                    }
                }
            }
        }
        
        public static ConfigNode EncodeRival(Rival r)
        {
            ConfigNode cn = new ConfigNode("Rival");
            ConfigNode cnStaff = new ConfigNode("Staff");
            ConfigNode cnScience = new ConfigNode("UnlockedScience");
            cn.AddValue("Name", r.Name);
            cn.AddValue("Science", r.CurrentScience);
            cn.AddValue("Funds", r.Funds);
            cn.AddValue("Reputation", r.Reputation);
            foreach (string s in r.Staff)
            {
                cnStaff.AddValue("StaffMember", s);
            }
            foreach (string s in r.ScienceNodes)
            {
                cnScience.AddValue("ScienceNode", s);
            }

            cn.AddNode(cnStaff);
            cn.AddNode(cnScience);
            return cn;
        }

        public static Rival DecodeRival(ConfigNode node)
        {
            ConfigNode cn = node.GetNode("Rival");
            ConfigNode cnScience = cn.GetNode("UnlockedScience");
            ConfigNode cnStaff = cn.GetNode("Staff");
            Rival temp = new Rival();
            temp.Name = cn.GetValue("Name");
            temp.CurrentScience = Convert.ToInt32(cn.GetValue("Science"));
            temp.Funds = Convert.ToDouble(cn.GetValue("Funds"));
            temp.Reputation = Convert.ToDouble(cn.GetValue("Reputation"));
            foreach (string s in cnScience.GetValues("ScienceNode"))
            {
                temp.ScienceNodes.Add(s);
            }
            foreach (string s in cnStaff.GetValues("StaffMember"))
            {
                temp.Staff.Add(s);
            }
            return temp;
        }

        public static List<string> HireStartingKerbals()
        {
            bool scientist = false;
            bool engineer = false;
            bool pilot = false;

            List<string> list = new List<string>();
            if (scientist == false)
            {
                ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.Applicants.FirstOrDefault(c => c.experienceTrait.TypeName == "Scientist");
                if (crew != null)
                    list.Add(crew.name);
                scientist = true;
            }
            if (engineer == false)
            {
                ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.Applicants.FirstOrDefault(c => c.experienceTrait.TypeName == "Engineer");
                if (crew != null)
                    list.Add(crew.name);
                engineer = true;
            }
            if (pilot == false)
            {
                ProtoCrewMember crew = HighLogic.CurrentGame.CrewRoster.Applicants.FirstOrDefault(c => c.experienceTrait.TypeName == "Pilot");
                if (crew != null)
                    list.Add(crew.name);
                engineer = true;
            }
            return list;
        }

    }

    public class Events
    {

    }

    public class Rival
    {
        public string Name {get; set;}
        public int CurrentScience { get; set; }
        public double Funds { get; set; }
        public double Reputation { get; set; }
        public double PreviousMinor { get; set; }
        public double NextMinor { get; set; }
        public double PreviousMajor { get; set; }
        public double NextMajor { get; set; }
        public List<string> Staff;
        public List<string> ScienceNodes;
      
    }
}
