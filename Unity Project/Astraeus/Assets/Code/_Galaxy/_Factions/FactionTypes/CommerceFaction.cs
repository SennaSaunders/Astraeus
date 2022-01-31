using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;

namespace Code._Galaxy._Factions.FactionTypes {
    public class CommerceFaction : Faction {
        public CommerceFaction(SolarSystem homeSystem) : base(homeSystem, FactionType.Commerce) {
        }

        public static int BlackHoleDesire = -100;

        public static int GetCommerceFactionSystemDesire(SolarSystem system) {
            //assign higher values to larger planets - so the more large planets the better
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(Planet)) {
                    desireValue += (int)body.Tier * (int)body.Tier;
                }
                else if (body.GetType() == typeof(BlackHole)) {
                    desireValue += CommerceFaction.BlackHoleDesire * (int)body.Tier;
                }
            }

            return desireValue;
        }
    }
}