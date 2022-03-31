using System;
using System.Collections.Generic;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code.TextureGen;

namespace Code._Galaxy._Factions.FactionTypes {
    public class PirateFaction : Faction {
        public PirateFaction(SolarSystem homeSystem) : base(homeSystem, FactionType.Pirate) {
        }

        public static int GetPirateFactionCelestialBodyDesire(CelestialBody celestialBody) {
            List<(Type type, Body.BodyTier tier, int desire)> desiredTypes = new List<(Type type, Body.BodyTier tier, int desire)>(){(typeof(EarthWorldGen), 0,-10)};
            return GetCelestialBodyDesireValue(desiredTypes,celestialBody);
        }

        public override List<(Type weaponType, int spawnWeighting)> GetAllowedWeapons() {
            return new List<(Type weaponType, int spawnWeighting)> { (typeof(Railgun), 1), (typeof(BallisticCannon), 10), (typeof(LaserCannon), 5) };
        }

        public override List<(Type mainThrusterType, int spawnWeighting)> GetAllowedMainThrusters() {
            return new List<(Type mainThruster, int spawnWeighting)> { (typeof(PrimitiveThruster), 1) };
        }

        public override List<(Type powerPlantType, int spawnWeighting)> GetAllowedPowerPlants() {
            return new List<(Type powerPlant, int spawnWeighting)> { (typeof(PowerPlantBalanced), 10), (typeof(PowerPlantHighRecharge), 1) };
        }

        public override List<(Type shieldType, int spawnWeighting)> GetAllowedShields() {
            return new List<(Type shield, int spawnWeighting)>() { (typeof(ShieldBalanced), 15), (typeof(ShieldHighRecharge), 1) };
        }
    }
}