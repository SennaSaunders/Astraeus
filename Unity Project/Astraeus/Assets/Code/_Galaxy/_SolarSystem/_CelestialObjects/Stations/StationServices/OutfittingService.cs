using System;
using System.Collections.Generic;
using System.Linq;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Storage;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class OutfittingService : StationService {
        private List<(Type componentType,ShipComponentTier tier)> AvailableComponents { get; } = new List<(Type, ShipComponentTier)>();
        
        protected override void SetGUIStrings() {
            ServiceName = "Outfitting";
            GUIPath = "GUIPrefabs/Station/Services/Outfitting/OutfittingGUI";
        }

        public List<(Type type, ShipComponentTier tier)> GetComponentsOfType(Type type) {
            return AvailableComponents.Where(c => c.componentType.IsSubclassOf(type) || c.componentType == type).ToList();
        }
        
        public void SetAllAvailableComponents() {
            AddAllPowerPlants();
            AddAllThrusters();
            AddAllWeapons();
            AddAvailableComponents<CargoBay>();
        }
        
        public void AddAvailableComponents<T>() where T: ShipComponent {
            for (int i = 0; i < Enum.GetValues(typeof(ShipComponentTier)).Length; i++) {
                AvailableComponents.Add((typeof(T), (ShipComponentTier)i));
            }
        }

        //power plants
        private void AddAllPowerPlants() {
            AddAvailableComponents<PowerPlantHighRecharge>();
            AddAvailableComponents<PowerPlantHighCapacity>();
            AddAvailableComponents<PowerPlantBalanced>();
        }
        
        
        //thrusters
        private void AddAllThrusters() {
            AddAvailableComponents<PrimitiveThruster>();
            AddAvailableComponents<TechThruster>();
            AddAvailableComponents<IndustrialThruster>();
            AddAvailableComponents<ManoeuvringThruster>();
        }

        //weapons
        private void AddAllWeapons() {
            AddAvailableComponents<LaserCannon>();
            AddAvailableComponents<BallisticCannon>();
            AddAvailableComponents<Railgun>();
        }
    }
}