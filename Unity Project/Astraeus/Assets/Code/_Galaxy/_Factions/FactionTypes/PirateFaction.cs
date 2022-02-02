using System.Collections.Generic;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code.TextureGen;

namespace Code._Galaxy._Factions.FactionTypes {
    public class PirateFaction : Faction {
        public PirateFaction(SolarSystem homeSystem) : base(homeSystem, FactionType.Pirate) {
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

        public override List<(Weapon weapon, int spawnWeighting)> GetAllowedWeapons(ShipComponentTier tier) {
            List<(Weapon weapon, int spawnWeighting)> weapons = new List<(Weapon weapon, int spawnWeighting)>();
            weapons.Add((new Railgun(tier), 1));
            weapons.Add((new BallisticCannon(tier), 10));
            weapons.Add((new LaserCannon(tier), 5));

            return weapons;
        }

        public override List<(MainThruster mainThruster, int spawnWeighting)> GetAllowedMainThrusters(ShipComponentTier tier) {
            List<(MainThruster mainThruster, int spawnWeighting)> thrusters = new List<(MainThruster mainThruster, int spawnWeighting)>();
            thrusters.Add((new PrimitiveThruster(tier), 1));

            return thrusters;
        }

        public override List<(PowerPlant powerPlant, int spawnWeighting)> GetAllowedPowerPlants(ShipComponentTier tier) {
            List<(PowerPlant powerPlant, int spawnWeighting)> powerPlants = new List<(PowerPlant powerPlant, int spawnWeighting)>();
            powerPlants.Add((new PowerPlantBalanced(tier), 10));
            powerPlants.Add((new PowerPlantHighRecharge(tier), 1));

            return powerPlants;
        }
    }
}