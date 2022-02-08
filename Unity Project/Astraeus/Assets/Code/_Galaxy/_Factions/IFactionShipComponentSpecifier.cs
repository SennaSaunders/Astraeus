using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;

namespace Code._Galaxy._Factions {
    public interface IFactionShipComponentSpecifier {
        public List<(Weapon weapon, int spawnWeighting)> GetAllowedWeapons(ShipComponentTier tier);
        public List<(MainThruster mainThruster, int spawnWeighting)> GetAllowedMainThrusters(ShipComponentTier tier);
        public List<(PowerPlant powerPlant, int spawnWeighting)> GetAllowedPowerPlants(ShipComponentTier tier);
    }
}