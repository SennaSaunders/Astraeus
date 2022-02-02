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

        public override List<(Weapon weapon, int spawnWeighting)> GetAllowedWeapons(ShipComponentTier tier) {
            List<(Weapon weapon, int spawnWeighting)> weapons = new List<(Weapon weapon, int spawnWeighting)>();
            weapons.Add((new BallisticCannon(tier), 1));
            weapons.Add((new LaserCannon(tier), 1));

            return weapons;
        }

        public override List<(MainThruster mainThruster, int spawnWeighting)> GetAllowedMainThrusters(ShipComponentTier tier) {
            List<(MainThruster mainThruster, int spawnWeighting)> thrusters = new List<(MainThruster mainThruster, int spawnWeighting)>();
            thrusters.Add((new PrimitiveThruster(tier), 1));
            thrusters.Add((new IndustrialThruster(tier), 1));

            return thrusters;
        }

        public override List<(PowerPlant powerPlant, int spawnWeighting)> GetAllowedPowerPlants(ShipComponentTier tier) {
            List<(PowerPlant powerPlant, int spawnWeighting)> powerPlants = new List<(PowerPlant powerPlant, int spawnWeighting)>();
            powerPlants.Add((new PowerPlantBalanced(tier), 1));

            return powerPlants;
        }
    }
}