using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceRace
{
    public class SRTraining
    {
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
            SpaceRaceMain.simulations.Add(new Simulation { body = "Kerbin", type = FlightLog.EntryType.Flight });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Kerbin", type = FlightLog.EntryType.Orbit });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Kerbin", type = FlightLog.EntryType.Land });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Mun", type = FlightLog.EntryType.Flight });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Mun", type = FlightLog.EntryType.Orbit });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Mun", type = FlightLog.EntryType.Land });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Minmus", type = FlightLog.EntryType.Flight });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Minmus", type = FlightLog.EntryType.Orbit });
            SpaceRaceMain.simulations.Add(new Simulation { body = "Minmus", type = FlightLog.EntryType.Land });
        }
}
    public class Simulation
    {
        public string body { get; set; }
        public FlightLog.EntryType type { get; set; }
    }

    
  
}
