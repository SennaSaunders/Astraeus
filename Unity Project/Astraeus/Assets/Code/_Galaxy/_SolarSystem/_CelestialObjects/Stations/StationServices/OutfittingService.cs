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
        
        protected override void SetGUIStrings() {
            serviceName = "Outfitting";
            guiString = "GUIPrefabs/Station/Services/Outfitting/OutfittingGUI";
        }

        // private List<ShipComponent> AvailableComponents { get; } = new List<ShipComponent>();
        private List<(Type componentType,ShipComponentTier tier)> AvailableComponents { get; } = new List<(Type, ShipComponentTier)>();


        public void AddAvailableComponents(List<Type> types, ShipComponentTier tier) {
            foreach (Type type in types) {
                AvailableComponents.Add((type, tier));
            };
        }
        
        public List<(Type type, ShipComponentTier tier)> GetComponentsOfType(Type type) {
            return AvailableComponents.Where(c => c.componentType.IsSubclassOf(type) || c.componentType == type).ToList();
        }

        //generate a list of all components for ship combat test
        public void SetAllAvailableComponents() {
            AddAllPowerPlants();
            AddAllThrusters();
            AddAllWeapons();
            AddAllCargoBays();
        }

        //power plants
        private void AddAllPowerPlants() {
            AddPowerPlantHighRecharge();
            AddPowerPlantHighCapacity();
            AddPowerPlantBalanced();
        }

        private void AddPowerPlantHighRecharge() {
            AvailableComponents.Add((typeof(PowerPlantHighRecharge),ShipComponentTier.T1));
            AvailableComponents.Add((typeof(PowerPlantHighRecharge),ShipComponentTier.T2));
            AvailableComponents.Add((typeof(PowerPlantHighRecharge),ShipComponentTier.T3));
            AvailableComponents.Add((typeof(PowerPlantHighRecharge),ShipComponentTier.T4));
            AvailableComponents.Add((typeof(PowerPlantHighRecharge),ShipComponentTier.T5));
        }
        
        private void AddPowerPlantHighCapacity() {
            AvailableComponents.Add((typeof(PowerPlantHighCapacity),ShipComponentTier.T1));
            AvailableComponents.Add((typeof(PowerPlantHighCapacity),ShipComponentTier.T2));
            AvailableComponents.Add((typeof(PowerPlantHighCapacity),ShipComponentTier.T3));
            AvailableComponents.Add((typeof(PowerPlantHighCapacity),ShipComponentTier.T4));
            AvailableComponents.Add((typeof(PowerPlantHighCapacity),ShipComponentTier.T5));
        }
        
        private void AddPowerPlantBalanced() {
            AvailableComponents.Add((typeof(PowerPlantBalanced),ShipComponentTier.T1));
            AvailableComponents.Add((typeof(PowerPlantBalanced),ShipComponentTier.T2));
            AvailableComponents.Add((typeof(PowerPlantBalanced),ShipComponentTier.T3));
            AvailableComponents.Add((typeof(PowerPlantBalanced),ShipComponentTier.T4));
            AvailableComponents.Add((typeof(PowerPlantBalanced),ShipComponentTier.T5));
        }
        //thrusters
        private void AddAllThrusters() {
            AddAllPrimitiveThrusters();
            AddAllTechThrusters();
            AddAllIndustrialThrusters();
            AddAllManoeuvringThrusters();
        }

        private void AddAllPrimitiveThrusters() {
            AvailableComponents.Add((typeof(PrimitiveThruster),ShipComponentTier.T1));
            AvailableComponents.Add((typeof(PrimitiveThruster),ShipComponentTier.T2));
            AvailableComponents.Add((typeof(PrimitiveThruster),ShipComponentTier.T3));
            AvailableComponents.Add((typeof(PrimitiveThruster),ShipComponentTier.T4));
            AvailableComponents.Add((typeof(PrimitiveThruster),ShipComponentTier.T5));
        }

        private void AddAllTechThrusters() {
            AvailableComponents.Add((typeof(TechThruster),ShipComponentTier.T1));
            AvailableComponents.Add((typeof(TechThruster),ShipComponentTier.T2));
            AvailableComponents.Add((typeof(TechThruster),ShipComponentTier.T3));
            AvailableComponents.Add((typeof(TechThruster),ShipComponentTier.T4));
            AvailableComponents.Add((typeof(TechThruster),ShipComponentTier.T5));
        }

        private void AddAllIndustrialThrusters() {
            AvailableComponents.Add((typeof(IndustrialThruster),ShipComponentTier.T1));
            AvailableComponents.Add((typeof(IndustrialThruster),ShipComponentTier.T2));
            AvailableComponents.Add((typeof(IndustrialThruster),ShipComponentTier.T3));
            AvailableComponents.Add((typeof(IndustrialThruster),ShipComponentTier.T4));
            AvailableComponents.Add((typeof(IndustrialThruster),ShipComponentTier.T5));
        }

        //weapons
        private void AddAllWeapons() {
            AddAllLaserCannons();
            AddAllBallisticCannons();
            AddAllRailguns();
        }

        private void AddAllRailguns() {
            AvailableComponents.Add((typeof(Railgun),ShipComponentTier.T1));
            AvailableComponents.Add((typeof(Railgun),ShipComponentTier.T2));
            AvailableComponents.Add((typeof(Railgun),ShipComponentTier.T3));
            AvailableComponents.Add((typeof(Railgun),ShipComponentTier.T4));
            AvailableComponents.Add((typeof(Railgun),ShipComponentTier.T5));
        }
        
        private void AddAllLaserCannons() {
            AvailableComponents.Add((typeof(LaserCannon),ShipComponentTier.T1));
            AvailableComponents.Add((typeof(LaserCannon),ShipComponentTier.T2));
            AvailableComponents.Add((typeof(LaserCannon),ShipComponentTier.T3));
            AvailableComponents.Add((typeof(LaserCannon),ShipComponentTier.T4));
            AvailableComponents.Add((typeof(LaserCannon),ShipComponentTier.T5));
        }
        
        private void AddAllBallisticCannons() {
            AvailableComponents.Add((typeof(BallisticCannon),ShipComponentTier.T1));
            AvailableComponents.Add((typeof(BallisticCannon),ShipComponentTier.T2));
            AvailableComponents.Add((typeof(BallisticCannon),ShipComponentTier.T3));
            AvailableComponents.Add((typeof(BallisticCannon),ShipComponentTier.T4));
            AvailableComponents.Add((typeof(BallisticCannon),ShipComponentTier.T5));
        }

        public void AddAllCargoBays() {
            AvailableComponents.Add((typeof(CargoBay),ShipComponentTier.T1));
            AvailableComponents.Add((typeof(CargoBay),ShipComponentTier.T2));
            AvailableComponents.Add((typeof(CargoBay),ShipComponentTier.T3));
            AvailableComponents.Add((typeof(CargoBay),ShipComponentTier.T4));
            AvailableComponents.Add((typeof(CargoBay),ShipComponentTier.T5));
        }

        public void AddAllManoeuvringThrusters() {
            AvailableComponents.Add((typeof(ManoeuvringThruster),ShipComponentTier.T1));
            AvailableComponents.Add((typeof(ManoeuvringThruster),ShipComponentTier.T2));
            AvailableComponents.Add((typeof(ManoeuvringThruster),ShipComponentTier.T3));
            AvailableComponents.Add((typeof(ManoeuvringThruster),ShipComponentTier.T4));
            AvailableComponents.Add((typeof(ManoeuvringThruster),ShipComponentTier.T5));
        }
    }
}