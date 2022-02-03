using System;
using System.Collections.Generic;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Galaxy._SolarSystem._CelestialObjects.Star;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
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

        public override List<(Weapon weapon, int spawnWeighting)> GetAllowedWeapons(ShipComponentTier tier) {
            List<(Weapon weapon, int spawnWeighting)> weapons = new List<(Weapon weapon, int spawnWeighting)>();
            weapons.Add((new Railgun(tier), 1));
            weapons.Add((new LaserCannon(tier), 5));

            return weapons;
        }

        public override List<(MainThruster mainThruster, int spawnWeighting)> GetAllowedMainThrusters(ShipComponentTier tier) {
            List<(MainThruster mainThruster, int spawnWeighting)> thrusters = new List<(MainThruster mainThruster, int spawnWeighting)>();
            thrusters.Add((new TechThruster(tier), 1));

            return thrusters;
        }

        public override List<(PowerPlant powerPlant, int spawnWeighting)> GetAllowedPowerPlants(ShipComponentTier tier) {
            List<(PowerPlant powerPlant, int spawnWeighting)> powerPlants = new List<(PowerPlant powerPlant, int spawnWeighting)>();
            powerPlants.Add((new PowerPlantHighCapacity(tier), 1));
            powerPlants.Add((new PowerPlantHighRecharge(tier), 3));

            return powerPlants;
        }
    }
}