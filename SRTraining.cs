using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
    public class SRTraining
    {
        public static bool unlocked = false;
        public static void MissionSim(Simulation sim, string name)
        {
            foreach (ProtoCrewMember c in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                if (c.name == name)
                {
                    FlightLog.Entry flight = new FlightLog.Entry(999, sim.type, sim.body);
                    c.careerLog.AddEntry(flight);
                }
            }
        }

        public static void BuildList()
        {
            SpaceRaceMain.simulations.Clear();
            SpaceRaceMain.simulations.Add(new Simulation { body = "Kerbin", type = FlightLog.EntryType.Flight, description = "Flight over Kerbin", xp = 1 });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Kerbin", type = FlightLog.EntryType.Orbit, description = "Orbit around Kerbin", xp = 2 });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Mun", type = FlightLog.EntryType.Orbit, description = "Orbit around the Mun", xp = 3 });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Mun", type = FlightLog.EntryType.Land, description = "Landing on the Mun", xp = 4.6f});
            SpaceRaceMain.simulations.Add(new Simulation { body = "Minmus", type = FlightLog.EntryType.Orbit, description = "Orbit around Minmus", xp = 3.75f });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Minmus", type = FlightLog.EntryType.Land, description = "Landing on Minmus", xp = 5.75f });
            if (unlocked == true)
            {
                SpaceRaceMain.simulations.Add(new Simulation { body = "Duna", type = FlightLog.EntryType.Flight, description = "Flight over Duna", xp = 10f });
                SpaceRaceMain.simulations.Add(new Simulation { body = "Duna", type = FlightLog.EntryType.Orbit, description = "Orbit around Duna", xp = 7.5f });
                SpaceRaceMain.simulations.Add(new Simulation { body = "Duna", type = FlightLog.EntryType.Land, description = "Landing on Duna", xp = 11.5f });

            }
        }
}
    public class Simulation
    {
        public string body { get; set; }
        public string description { get; set; }
        public float xp { get; set; }
        public FlightLog.EntryType type { get; set; }
    }

    
  
}
