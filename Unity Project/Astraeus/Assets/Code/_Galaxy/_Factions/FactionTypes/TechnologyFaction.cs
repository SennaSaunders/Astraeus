using System.Collections.Generic;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;
using Code._Galaxy._SolarSystem._CelestialObjects.Star;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code.TextureGen;

namespace Code._Galaxy._Factions.FactionTypes {
    public class TechnologyFaction : Faction {
        public TechnologyFaction(SolarSystem homeSystem) : base(homeSystem, FactionType.Technology) {
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

        public override List<(Weapon weapon, int spawnWeighting)> GetAllowedWeapons(ShipComponentTier tier) {
            List<(Weapon weapon, int spawnWeighting)> weapons = new List<(Weapon weapon, int spawnWeighting)>();
            weapons.Add((new Railgun(tier), 1));
            weapons.Add((new LaserCannon(tier), 5));

            return weapons;
        }

        public override List<(MainThruster mainThruster, int spawnWeighting)> GetAllowedMainThrusters(ShipComponentTier tier) {
            List<(MainThruster mainThruster, int spawnWeighting)> thrusters = new List<(MainThruster mainThruster, int spawnWeighting)>();
            thrusters.Add((new TechThruster(tier), 1));

            return thrusters;
        }

        public override List<(PowerPlant powerPlant, int spawnWeighting)> GetAllowedPowerPlants(ShipComponentTier tier) {
            List<(PowerPlant powerPlant, int spawnWeighting)> powerPlants = new List<(PowerPlant powerPlant, int spawnWeighting)>();
            powerPlants.Add((new PowerPlantHighCapacity(tier), 1));
            powerPlants.Add((new PowerPlantHighRecharge(tier), 3));

            return powerPlants;
        }
    }
}