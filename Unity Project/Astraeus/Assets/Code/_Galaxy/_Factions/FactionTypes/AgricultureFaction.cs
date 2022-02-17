using System;
using System.Collections.Generic;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code.TextureGen;

namespace Code._Galaxy._Factions.FactionTypes {
    public class AgricultureFaction : Faction {
        public AgricultureFaction(SolarSystem homeSystem) : base(homeSystem, FactionType.Agriculture) {
        }

        public static int GetAgricultureFactionCelestialBodyDesire(CelestialBody celestialBody) {
            List<(Type type, Body.BodyTier tier, int desire)> desiredTypes = new List<(Type type, Body.BodyTier tier, int desire)>() {
                (typeof(EarthWorldGen),0, 50),
                (typeof(WaterWorldGen),0, 50),
                (typeof(BlackHole), 0,-100)
            };
            return GetCelestialBodyDesireValue(desiredTypes,celestialBody);
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
        public override List<(Shield shield, int spawnWeighting)> GetAllowedShields(ShipComponentTier tier) {
            return new List<(Shield shield, int spawnWeighting)>() { (new ShieldBalanced(tier), 10) };
        }
        
    }
}