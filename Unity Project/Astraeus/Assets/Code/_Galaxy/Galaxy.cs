using System.Collections.Generic;
using Code._Factions;
using Code._Galaxy._SolarSystem;

namespace Code._Galaxy {
    public class Galaxy {
        public List<SolarSystem> Systems { get; }
        public List<Faction> Factions { get; set; }
        public List<Sector> Sectors { get; }

        public Galaxy(List<SolarSystem> systems, List<Sector> sectors) {
            Systems = systems;
            Sectors = sectors;
        }
    }
}
