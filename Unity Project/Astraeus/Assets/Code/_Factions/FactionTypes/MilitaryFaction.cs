using System.Collections.Generic;
using Code._CelestialObjects;
using Code._CelestialObjects.Planet;
using Code._Galaxy;
using Code._Galaxy._SolarSystem;
using Code._Galaxy.GalaxyComponents;
using Code.TextureGen;

namespace Code._Factions.FactionTypes {
    public class MilitaryFaction : Faction {
        public MilitaryFaction(SolarSystem homeSystem) : base(homeSystem, FactionTypeEnum.Military) {
        }

        public static int EarthWorldDesire = 20;
        
        public static int GetMilitaryFactionSystemDesire(SolarSystem system) {
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(Planet)) { //ignore stars/black holes
                    Planet planet = (Planet)body;
                    if (planet.PlanetGen.GetType() == typeof(EarthWorldGen)) {
                        desireValue += MilitaryFaction.EarthWorldDesire * (int)planet.Tier;
                    }
                    else {
                        desireValue += (int)body.Tier;
                    }
                }
            }

            return desireValue;
        }
    }
}