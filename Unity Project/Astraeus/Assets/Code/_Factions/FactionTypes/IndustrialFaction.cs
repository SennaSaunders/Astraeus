using System.Collections.Generic;
using Code._CelestialObjects;
using Code._CelestialObjects.BlackHole;
using Code._CelestialObjects.Planet;
using Code._Galaxy._SolarSystem;
using Code.TextureGen;

namespace Code._Factions.FactionTypes {
    public class IndustrialFaction : Faction {
        public IndustrialFaction(SolarSystem homeSystem, List<SolarSystem> systems) : base(homeSystem, FactionTypeEnum.Industrial, systems) {
        }

        public static int WaterWorldDesire = 15;
        public static int RockyWorldDesire = 30;
        public static int BlackHoleDesire = -50;

        public static int GetIndustrialSystemDesire(SolarSystem system) {
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(Planet)) {
                    Planet planet = (Planet)body;
                    if (planet.PlanetGen.GetType() == typeof(WaterWorldGen)) {
                        desireValue += IndustrialFaction.WaterWorldDesire * (int)planet.Tier;
                    }
                    else if (planet.PlanetGen.GetType() == typeof(RockyWorldGen)) {
                        desireValue += IndustrialFaction.RockyWorldDesire * (int)planet.Tier;
                    }
                    else {
                        desireValue += (int)body.Tier * (int)body.Tier;
                    }
                }
                else if (body.GetType() == typeof(BlackHole)) {
                    desireValue += IndustrialFaction.BlackHoleDesire * (int)body.Tier;
                }
            }

            return desireValue;
        }
    }
}