using System;
using System.Collections.Generic;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
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

        public override List<(Type weaponType, int spawnWeighting)> GetAllowedWeapons() {
            List<(Type weaponType, int spawnWeighting)> weapons = new List<(Type weaponType, int spawnWeighting)> { (typeof(BallisticCannon), 1), (typeof(LaserCannon), 1) };
            return weapons;
        }

        public override List<(Type mainThrusterType, int spawnWeighting)> GetAllowedMainThrusters() {
            List<(Type mainThrusterType, int spawnWeighting)> thrusters = new List<(Type mainThrusterType, int spawnWeighting)> { (typeof(PrimitiveThruster), 1), (typeof(IndustrialThruster), 1) };
            return thrusters;
        }

        public override List<(Type powerPlantType, int spawnWeighting)> GetAllowedPowerPlants() {
            List<(Type powerPlantType, int spawnWeighting)> powerPlants = new List<(Type powerPlantType, int spawnWeighting)> { (typeof(PowerPlantBalanced), 1) };
            return powerPlants;
        }
        public override List<(Type shieldType, int spawnWeighting)> GetAllowedShields() {
            return new List<(Type shieldType, int spawnWeighting)>() { (typeof(ShieldBalanced), 10) };
        }
    }
}