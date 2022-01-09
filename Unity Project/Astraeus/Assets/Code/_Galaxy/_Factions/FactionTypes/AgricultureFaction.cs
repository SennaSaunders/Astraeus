using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;
using Code.TextureGen;

namespace Code._Galaxy._Factions.FactionTypes {
    public class AgricultureFaction : Faction {
        public AgricultureFaction(SolarSystem homeSystem) : base(homeSystem, FactionTypeEnum.Agriculture) {
        }

        public static int OrganicWorldDesire = 50;
        public static int BlackHoleDesire = -100;

        public static int GetAgricultureFactionSystemDesire(SolarSystem system) {
            //assign high values to earth-likes & water worlds - these are the most prized
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(Planet)) {
                    Planet planet = (Planet)body;
                    if (planet.PlanetGen.GetType() == typeof(EarthWorldGen) || planet.PlanetGen.GetType() == typeof(WaterWorldGen)) {
                        desireValue += AgricultureFaction.OrganicWorldDesire * (int)planet.Tier;
                    }
                    else {
                        desireValue += (int)body.Tier;
                    }
                }
                else if (body.GetType() == typeof(BlackHole)) {
                    desireValue += AgricultureFaction.BlackHoleDesire * (int)body.Tier;
                }
            }

            return desireValue;
        }
    }
}