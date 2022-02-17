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
    public class MilitaryFaction : Faction {
        public MilitaryFaction(SolarSystem homeSystem) : base(homeSystem, FactionType.Military) {
        }

        public static int GetMilitaryFactionCelestialBodyDesire(CelestialBody celestialBody) {
            List<(Type type, Body.BodyTier tier, int desire)> desiredTypes = new List<(Type type, Body.BodyTier tier, int desire)>() { (typeof(EarthWorldGen), 0, 20) };
            return GetCelestialBodyDesireValue(desiredTypes,celestialBody);
        }

        public override List<(Weapon weapon, int spawnWeighting)> GetAllowedWeapons(ShipComponentTier tier) {
            List<(Weapon weapon, int spawnWeighting)> weapons = new List<(Weapon weapon, int spawnWeighting)>();
            weapons.Add((new Railgun(tier), 1));
            weapons.Add((new BallisticCannon(tier), 2));
            weapons.Add((new LaserCannon(tier), 2));

            return weapons;
        }

        public override List<(MainThruster mainThruster, int spawnWeighting)> GetAllowedMainThrusters(ShipComponentTier tier) {
            List<(MainThruster mainThruster, int spawnWeighting)> thrusters = new List<(MainThruster mainThruster, int spawnWeighting)>();
            thrusters.Add((new TechThruster(tier), 1));
            thrusters.Add((new PrimitiveThruster(tier), 5));

            return thrusters;
        }

        public override List<(PowerPlant powerPlant, int spawnWeighting)> GetAllowedPowerPlants(ShipComponentTier tier) {
            List<(PowerPlant powerPlant, int spawnWeighting)> powerPlants = new List<(PowerPlant powerPlant, int spawnWeighting)>();
            powerPlants.Add((new PowerPlantHighCapacity(tier), 10));
            powerPlants.Add((new PowerPlantHighRecharge(tier), 1));

            return powerPlants;
        }

        public override List<(Shield shield, int spawnWeighting)> GetAllowedShields(ShipComponentTier tier) {
            return new List<(Shield shield, int spawnWeighting)>() { (new ShieldBalanced(tier), 10), (new ShieldHighCapacity(tier), 5), (new ShieldHighRecharge(tier), 2) };
        }
    }
}