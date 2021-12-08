using System.Collections.Generic;
using Code._Galaxy._SolarSystem;

namespace Code._Galaxy {
    public class Sector {
        public GalaxyGenerator.Tile SectorTile { get; }
        public List<SolarSystem> Systems { get; private set; }

        public Sector(GalaxyGenerator.Tile sectorTile) {
            this.SectorTile = sectorTile;
        }

        public void SetSolarSystems(List<SolarSystem> solarSystems) {
            Systems = solarSystems;
        }
    }
}