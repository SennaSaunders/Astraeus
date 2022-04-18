using System;
using System.Collections.Generic;
using Code._Cargo.ProductTypes.Commodity.Exotic;
using Code._Cargo.ProductTypes.Commodity.Industrial;
using Code._Cargo.ProductTypes.Commodity.Organic;
using Code._Cargo.ProductTypes.Commodity.Tech;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Galaxy._SolarSystem._CelestialObjects.Star;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code.TextureGen;

namespace Code._Galaxy._Factions.FactionTypes {
    public class TechnologyFaction : Faction {
        public TechnologyFaction(SolarSystem homeSystem) : base(homeSystem, FactionType.Technology) {
        }
        
        public static int GetTechnologyFactionCelestialBodyDesire(CelestialBody celestialBody) {
            List<(Type type, Body.BodyTier tier, int desire)> desiredTypes = new List<(Type type, Body.BodyTier tier, int desire)>() {
                (typeof(BlackHole), 0, 30),
                (typeof(Star),Body.BodyTier.T9, 20),
                (typeof(Star), Body.BodyTier.T8, 15),
                (typeof(Star),Body.BodyTier.T7, 10),
                (typeof(EarthWorldGen), 0, 30),
                (typeof(WaterWorldGen), 0,30)
            };
            return GetCelestialBodyDesireValue(desiredTypes,celestialBody);
        }

        public override List<(Type weaponType, int spawnWeighting)> GetAllowedWeapons() {
            return new List<(Type weaponType, int spawnWeighting)> { (typeof(Railgun), 1), (typeof(Laser), 5) };
        }

        public override List<(Type mainThrusterType, int spawnWeighting)> GetAllowedMainThrusters() {
            return new List<(Type mainThrusterType, int spawnWeighting)> { (typeof(TechThruster), 1) };
        }

        public override List<(Type powerPlantType, int spawnWeighting)> GetAllowedPowerPlants() {
            return new List<(Type powerPlantType, int spawnWeighting)> { (typeof(PowerPlantHighCapacity), 1), (typeof(PowerPlantHighRecharge), 3) };
        }

        public override List<(Type shieldType, int spawnWeighting)> GetAllowedShields() {
            return new List<(Type shieldType, int spawnWeighting)>() { (typeof(ShieldHighRecharge), 1), (typeof(ShieldHighCapacity), 1) };
        }

        public override List<(Type productType, float productionMult, float priceMult)> GetProductionMultipliers() {
            return new List<(Type productType, float productionMult, float priceMult)>() { (typeof(OrganicProduct), .2f, 3), (typeof(IndustrialProduct), 1, 4), (typeof(TechProduct), 5, 1), (typeof(ExoticProduct), 1, 1) };
        }
    }
}