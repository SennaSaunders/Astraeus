using System.Collections.Generic;
using Code._Factions;
using Code._Galaxy._SolarSystem;

namespace Code._Galaxy.GalaxyComponents {
    public class Sector {
        public Tile SectorTile { get; }
        public List<SolarSystem> Systems { get; private set; }
        public List<Faction> Factions { get; } = new List<Faction>();

        public Sector(Tile sectorTile) {
            this.SectorTile = sectorTile;
        }

        public void SetSolarSystems(List<SolarSystem> solarSystems) {
            Systems = solarSystems;
        }
    }
}