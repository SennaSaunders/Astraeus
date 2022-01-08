using System.Collections.Generic;
using Code._CelestialObjects;
using Code._CelestialObjects.Planet;
using Code._Galaxy;
using Code._Galaxy._SolarSystem;
using Code._Galaxy.GalaxyComponents;
using Code.TextureGen;

namespace Code._Factions.FactionTypes {
    public class PirateFaction : Faction {
        public PirateFaction(SolarSystem homeSystem) : base(homeSystem, FactionTypeEnum.Pirate) {
        }

        public static int EarthlikeDesire = -10;
        
        public static int GetPirateFactionSystemDesire(SolarSystem system) {
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(Planet)) {
                    Planet planet = (Planet)body;
                    if (planet.PlanetGen.GetType() == typeof(EarthWorldGen)) {
                        desireValue += PirateFaction.EarthlikeDesire * (int)planet.Tier;
                    }
                    else {
                        desireValue += (int)planet.Tier;
                    }
                }
            }

            return desireValue;
        }
    }
}