using System;
using System.Collections.Generic;
using Code._Cargo.ProductTypes.Commodity.Exotic;
using Code._Cargo.ProductTypes.Commodity.Industrial;
using Code._Cargo.ProductTypes.Commodity.Organic;
using Code._Cargo.ProductTypes.Commodity.Tech;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code.TextureGen;

namespace Code._Galaxy._Factions.FactionTypes {
    public class IndustrialFaction : Faction {
        public IndustrialFaction(SolarSystem homeSystem) : base(homeSystem, FactionType.Industrial) {
        }

        public static int WaterWorldDesire = 15;
        public static int RockyWorldDesire = 30;
        public static int BlackHoleDesire = -50;
        public static int GetIndustrialCelestialBodyDesire(CelestialBody celestialBody) {
            List<(Type type, Body.BodyTier tier, int desire)> desiredTypes = new List<(Type type, Body.BodyTier tier, int desire)>() {
                (typeof(WaterWorldGen),0, 15),
                (typeof(RockyWorldGen),0, 30),
                (typeof(BlackHole),0, -50)
            };
            return GetCelestialBodyDesireValue(desiredTypes,celestialBody);
        }

        public override List<(Type weaponType, int spawnWeighting)> GetAllowedWeapons() {
            List<(Type weaponType, int spawnWeighting)> weapons = new List<(Type weaponType, int spawnWeighting)>();
            weapons.Add((typeof(BallisticCannon), 1));
            weapons.Add((typeof(LaserCannon), 1));

            return weapons;
        }

        public override List<(Type mainThrusterType, int spawnWeighting)> GetAllowedMainThrusters() {
            List<(Type mainThrusterType, int spawnWeighting)> thrusters = new List<(Type mainThrusterType, int spawnWeighting)>();
            thrusters.Add((typeof(PrimitiveThruster), 1));
            thrusters.Add((typeof(IndustrialThruster), 10));

            return thrusters;
        }

        public override List<(Type powerPlantType, int spawnWeighting)> GetAllowedPowerPlants() {
            List<(Type powerPlantType, int spawnWeighting)> powerPlants = new List<(Type powerPlantType, int spawnWeighting)>();
            powerPlants.Add((typeof(PowerPlantHighCapacity), 3));
            powerPlants.Add((typeof(PowerPlantBalanced), 1));
            
            return powerPlants;
        }
        public override List<(Type shieldType, int spawnWeighting)> GetAllowedShields() {
            return new List<(Type shieldType, int spawnWeighting)>() { (typeof( ShieldBalanced), 5), (typeof( ShieldHighCapacity), 2) };
        }

        public override List<(Type productType, float productionMult, float priceMult)> GetProductionMultipliers() {
            return new List<(Type productType, float productionMult, float priceMult)>() { (typeof(OrganicProduct), .5f, 1.5f), (typeof(IndustrialProduct), 5, 1), (typeof(TechProduct), .5f, 3), (typeof(ExoticProduct), 1, 1) };
        }
    }
}