using Code._CelestialObjects;
using Code._CelestialObjects.BlackHole;
using Code._CelestialObjects.Planet;
using Code._CelestialObjects.Star;
using Code._Galaxy;
using Code._Galaxy._SolarSystem;
using Code._Galaxy.GalaxyComponents;
using Code.TextureGen;

namespace Code._Factions.FactionTypes {
    public class TechnologyFaction : Faction {
        public TechnologyFaction(SolarSystem homeSystem) : base(homeSystem, FactionTypeEnum.Technology) {
        }

        public static int BlackHoleValue = 30;
        public static int StarT9Value = 20;
        public static int StarT8Value = 15;
        public static int StarT7Value = 10;
        public static int OrganicWorldValue = 30;
        
        public static int GetTechnologyFactionSystemDesire(SolarSystem system) {
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(BlackHole)) {
                    desireValue += TechnologyFaction.BlackHoleValue * (int)body.Tier;
                }
                else if (body.GetType() == typeof(Star) && body.Tier == Body.BodyTier.T9) {
                    desireValue += TechnologyFaction.StarT9Value * (int)body.Tier;
                }
                else if (body.GetType() == typeof(Star) && body.Tier == Body.BodyTier.T8) {
                    desireValue += TechnologyFaction.StarT8Value * (int)body.Tier;
                }
                else if (body.GetType() == typeof(Star)) {
                    desireValue += TechnologyFaction.StarT7Value * (int)body.Tier;
                }
                else if (body.GetType() == typeof(Planet)) {
                    Planet planet = (Planet)body;
                    if (planet.PlanetGen.GetType() == typeof(EarthWorldGen) || planet.PlanetGen.GetType() == typeof(WaterWorldGen)) {
                        desireValue += TechnologyFaction.OrganicWorldValue * (int)body.Tier;
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