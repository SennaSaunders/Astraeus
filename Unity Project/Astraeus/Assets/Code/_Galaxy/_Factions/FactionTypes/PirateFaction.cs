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

        public override List<(Weapon weapon, int spawnWeighting)> GetAllowedWeapons(ShipComponentTier tier) {
            return new List<(Weapon weapon, int spawnWeighting)> { (new Railgun(tier), 1), (new BallisticCannon(tier), 10), (new LaserCannon(tier), 5) };
        }

        public override List<(MainThruster mainThruster, int spawnWeighting)> GetAllowedMainThrusters(ShipComponentTier tier) {
            return new List<(MainThruster mainThruster, int spawnWeighting)> { (new PrimitiveThruster(tier), 1) };
        }

        public override List<(PowerPlant powerPlant, int spawnWeighting)> GetAllowedPowerPlants(ShipComponentTier tier) {
            return new List<(PowerPlant powerPlant, int spawnWeighting)> { (new PowerPlantBalanced(tier), 10), (new PowerPlantHighRecharge(tier), 1) };
        }

        public override List<(Shield shield, int spawnWeighting)> GetAllowedShields(ShipComponentTier tier) {
            return new List<(Shield shield, int spawnWeighting)>() { (new ShieldBalanced(tier), 15), (new ShieldHighRecharge(tier), 1) };
        }
    }
}