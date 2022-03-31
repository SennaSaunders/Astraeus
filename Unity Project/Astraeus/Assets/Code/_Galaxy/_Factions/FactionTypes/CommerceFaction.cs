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

namespace Code._Galaxy._Factions.FactionTypes {
    public class CommerceFaction : Faction {
        public CommerceFaction(SolarSystem homeSystem) : base(homeSystem, FactionType.Commerce) {
        }

        public static int GetCommerceFactionCelestialBodyDesire(CelestialBody celestialBody) {
            List<(Type type, Body.BodyTier tier, int desire)> desiredTypes = new List<(Type type, Body.BodyTier tier, int desire)>() { (typeof(BlackHole),0, -100) };
            return GetCelestialBodyDesireValue(desiredTypes,celestialBody);
        }

        public override List<(Type weaponType, int spawnWeighting)> GetAllowedWeapons() {
            List<(Type weaponType, int spawnWeighting)> weapons = new List<(Type weapon, int spawnWeighting)>();
            weapons.Add((typeof(BallisticCannon), 1));
            weapons.Add((typeof(LaserCannon), 1));

            return weapons;
        }

        public override List<(Type mainThrusterType, int spawnWeighting)> GetAllowedMainThrusters() {
            List<(Type mainThrusterType, int spawnWeighting)> thrusters = new List<(Type mainThrusterType, int spawnWeighting)>();
            thrusters.Add((typeof(PrimitiveThruster), 1));
            thrusters.Add((typeof(IndustrialThruster), 1));

            return thrusters;
        }

        public override List<(Type powerPlantType, int spawnWeighting)> GetAllowedPowerPlants() {
            List<(Type powerPlantType, int spawnWeighting)> powerPlants = new List<(Type powerPlantType, int spawnWeighting)>();
            powerPlants.Add((typeof(PowerPlantBalanced), 1));

            return powerPlants;
        }

        public override List<(Type shieldType, int spawnWeighting)> GetAllowedShields() {
            return new List<(Type shield, int spawnWeighting)>() { (typeof(ShieldBalanced), 10) };
        }
    }
}