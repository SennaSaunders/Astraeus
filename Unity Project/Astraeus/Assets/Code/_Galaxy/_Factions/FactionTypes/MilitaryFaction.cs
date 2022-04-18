using System;
using System.Collections.Generic;
using Code._Cargo.ProductTypes.Commodity.Exotic;
using Code._Cargo.ProductTypes.Commodity.Industrial;
using Code._Cargo.ProductTypes.Commodity.Organic;
using Code._Cargo.ProductTypes.Commodity.Tech;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code.TextureGen;

namespace Code._Galaxy._Factions.FactionTypes {
    public class MilitaryFaction : Faction {
        public MilitaryFaction(SolarSystem homeSystem) : base(homeSystem, FactionType.Military) {
        }

        public static int GetMilitaryFactionCelestialBodyDesire(CelestialBody celestialBody) {
            List<(Type type, Body.BodyTier tier, int desire)> desiredTypes = new List<(Type type, Body.BodyTier tier, int desire)>() { (typeof(EarthWorldGen), 0, 20) };
            return GetCelestialBodyDesireValue(desiredTypes,celestialBody);
        }

        public override List<(Type weaponType, int spawnWeighting)> GetAllowedWeapons( ) {
            List<(Type weapon, int spawnWeighting)> weapons = new List<(Type weapon, int spawnWeighting)>();
            weapons.Add((typeof(Railgun), 1));
            weapons.Add((typeof(BallisticCannon), 2));
            weapons.Add((typeof(Laser), 2));

            return weapons;
        }

        public override List<(Type mainThrusterType, int spawnWeighting)> GetAllowedMainThrusters( ) {
            List<(Type mainThrusterType, int spawnWeighting)> thrusters = new List<(Type mainThruster, int spawnWeighting)>();
            thrusters.Add((typeof(TechThruster), 1));
            thrusters.Add((typeof(PrimitiveThruster), 5));

            return thrusters;
        }

        public override List<(Type powerPlantType, int spawnWeighting)> GetAllowedPowerPlants( ) {
            List<(Type powerPlantType, int spawnWeighting)> powerPlants = new List<(Type powerPlant, int spawnWeighting)>();
            powerPlants.Add((typeof(PowerPlantHighCapacity), 10));
            powerPlants.Add((typeof(PowerPlantHighRecharge), 1));

            return powerPlants;
        }

        public override List<(Type shieldType, int spawnWeighting)> GetAllowedShields( ) {
            return new List<(Type shieldType, int spawnWeighting)>() { (typeof(ShieldBalanced), 10), (typeof( ShieldHighCapacity), 5), (typeof( ShieldHighRecharge), 2) };
        }

        public override List<(Type productType, float productionMult, float priceMult)> GetProductionMultipliers() {
            return new List<(Type productType, float productionMult, float priceMult)>() { (typeof(OrganicProduct), 1, 2), (typeof(IndustrialProduct), 1, 1), (typeof(TechProduct), 1, 3), (typeof(ExoticProduct), 1, 1) };
        }
    }
}