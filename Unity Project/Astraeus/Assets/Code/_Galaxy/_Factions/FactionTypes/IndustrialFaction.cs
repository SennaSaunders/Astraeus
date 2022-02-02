using System.Collections.Generic;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code.TextureGen;

namespace Code._Galaxy._Factions.FactionTypes {
    public class IndustrialFaction : Faction {
        public IndustrialFaction(SolarSystem homeSystem) : base(homeSystem, FactionType.Industrial) {
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

        public override List<(Weapon weapon, int spawnWeighting)> GetAllowedWeapons(ShipComponentTier tier) {
            List<(Weapon weapon, int spawnWeighting)> weapons = new List<(Weapon weapon, int spawnWeighting)>();
            weapons.Add((new BallisticCannon(tier), 1));
            weapons.Add((new LaserCannon(tier), 1));

            return weapons;
        }

        public override List<(MainThruster mainThruster, int spawnWeighting)> GetAllowedMainThrusters(ShipComponentTier tier) {
            List<(MainThruster mainThruster, int spawnWeighting)> thrusters = new List<(MainThruster mainThruster, int spawnWeighting)>();
            thrusters.Add((new PrimitiveThruster(tier), 1));
            thrusters.Add((new IndustrialThruster(tier), 10));

            return thrusters;
        }

        public override List<(PowerPlant powerPlant, int spawnWeighting)> GetAllowedPowerPlants(ShipComponentTier tier) {
            List<(PowerPlant powerPlant, int spawnWeighting)> powerPlants = new List<(PowerPlant powerPlant, int spawnWeighting)>();
            powerPlants.Add((new PowerPlantHighCapacity(tier), 3));
            powerPlants.Add((new PowerPlantBalanced(tier), 1));
            
            return powerPlants;
        }
    }
}