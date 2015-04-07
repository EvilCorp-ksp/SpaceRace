using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
    public class SRTraining
    {
        private static bool unlocked = false;
        public static void MissionSim(Simulation sim, string name)
        {
            foreach (ProtoCrewMember c in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                if (c.name == name)
                {
                    FlightLog.Entry flight = new FlightLog.Entry(0, sim.type, sim.body);
                    c.careerLog.AddEntry(flight);
                }
            }
        }

        public static void BuildList()
        {
            SpaceRaceMain.simulations.Clear();
            SpaceRaceMain.simulations.Add(new Simulation { body = "Kerbin", type = FlightLog.EntryType.Flight, description = "Flight over Kerbin" });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Kerbin", type = FlightLog.EntryType.Orbit, description = "Orbit around Kerbin" });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Mun", type = FlightLog.EntryType.Flight, description = "Flight over the Mun" });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Mun", type = FlightLog.EntryType.Orbit, description = "Orbit around the Mun" });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Mun", type = FlightLog.EntryType.Land, description = "Landing on the Mun" });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Minmus", type = FlightLog.EntryType.Flight, description = "Flight over Minmus" });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Minmus", type = FlightLog.EntryType.Orbit, description = "Orbit around Minmus" });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Minmus", type = FlightLog.EntryType.Land, description = "Landing on Minmus" });
            if (unlocked == true)
            {
                SpaceRaceMain.simulations.Add(new Simulation { body = "Duna", type = FlightLog.EntryType.Flight, description = "Flight over Duna" });
                SpaceRaceMain.simulations.Add(new Simulation { body = "Duna", type = FlightLog.EntryType.Orbit, description = "Orbit around Duna" });
                SpaceRaceMain.simulations.Add(new Simulation { body = "Duna", type = FlightLog.EntryType.Land, description = "Landing on Duna" });

            }
        }
}
    public class Simulation
    {
        public string body { get; set; }
        public string description { get; set; }
        public int xp { get; set; }
        public FlightLog.EntryType type { get; set; }
    }

    
  
}
