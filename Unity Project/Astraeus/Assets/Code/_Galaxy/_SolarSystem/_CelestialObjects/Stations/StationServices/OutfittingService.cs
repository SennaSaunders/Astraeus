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

        private List<ShipComponent> AvailableComponents { get; } = new List<ShipComponent>();


        public void AddAvailableComponents(List<ShipComponent> components) {
            AvailableComponents.AddRange(components);
        }
        
        public List<T> GetComponentsOfType<T>() {
            return AvailableComponents.Where(c => c.GetType().IsSubclassOf(typeof(T)) || c.GetType() == typeof(T)).Cast<T>().ToList();
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
            AvailableComponents.Add(new PowerPlantHighRecharge(ShipComponentTier.T1));
            AvailableComponents.Add(new PowerPlantHighRecharge(ShipComponentTier.T2));
            AvailableComponents.Add(new PowerPlantHighRecharge(ShipComponentTier.T3));
            AvailableComponents.Add(new PowerPlantHighRecharge(ShipComponentTier.T4));
            AvailableComponents.Add(new PowerPlantHighRecharge(ShipComponentTier.T5));
        }
        
        private void AddPowerPlantHighCapacity() {
            AvailableComponents.Add(new PowerPlantHighCapacity(ShipComponentTier.T1));
            AvailableComponents.Add(new PowerPlantHighCapacity(ShipComponentTier.T2));
            AvailableComponents.Add(new PowerPlantHighCapacity(ShipComponentTier.T3));
            AvailableComponents.Add(new PowerPlantHighCapacity(ShipComponentTier.T4));
            AvailableComponents.Add(new PowerPlantHighCapacity(ShipComponentTier.T5));
        }
        
        private void AddPowerPlantBalanced() {
            AvailableComponents.Add(new PowerPlantBalanced(ShipComponentTier.T1));
            AvailableComponents.Add(new PowerPlantBalanced(ShipComponentTier.T2));
            AvailableComponents.Add(new PowerPlantBalanced(ShipComponentTier.T3));
            AvailableComponents.Add(new PowerPlantBalanced(ShipComponentTier.T4));
            AvailableComponents.Add(new PowerPlantBalanced(ShipComponentTier.T5));
        }
        //thrusters
        private void AddAllThrusters() {
            AddAllPrimitiveThrusters();
            AddAllTechThrusters();
            AddAllIndustrialThrusters();
            AddAllManoeuvringThrusters();
        }

        private void AddAllPrimitiveThrusters() {
            AvailableComponents.Add(new PrimitiveThruster(ShipComponentTier.T1));
            AvailableComponents.Add(new PrimitiveThruster(ShipComponentTier.T2));
            AvailableComponents.Add(new PrimitiveThruster(ShipComponentTier.T3));
            AvailableComponents.Add(new PrimitiveThruster(ShipComponentTier.T4));
            AvailableComponents.Add(new PrimitiveThruster(ShipComponentTier.T5));
        }

        private void AddAllTechThrusters() {
            AvailableComponents.Add(new TechThruster(ShipComponentTier.T1));
            AvailableComponents.Add(new TechThruster(ShipComponentTier.T2));
            AvailableComponents.Add(new TechThruster(ShipComponentTier.T3));
            AvailableComponents.Add(new TechThruster(ShipComponentTier.T4));
            AvailableComponents.Add(new TechThruster(ShipComponentTier.T5));
        }

        private void AddAllIndustrialThrusters() {
            AvailableComponents.Add(new IndustrialThruster(ShipComponentTier.T1));
            AvailableComponents.Add(new IndustrialThruster(ShipComponentTier.T2));
            AvailableComponents.Add(new IndustrialThruster(ShipComponentTier.T3));
            AvailableComponents.Add(new IndustrialThruster(ShipComponentTier.T4));
            AvailableComponents.Add(new IndustrialThruster(ShipComponentTier.T5));
        }

        //weapons
        private void AddAllWeapons() {
            AddAllLaserCannons();
            AddAllBallisticCannons();
            AddAllRailguns();
        }

        private void AddAllRailguns() {
            AvailableComponents.Add(new Railgun(ShipComponentTier.T1));
            AvailableComponents.Add(new Railgun(ShipComponentTier.T2));
            AvailableComponents.Add(new Railgun(ShipComponentTier.T3));
            AvailableComponents.Add(new Railgun(ShipComponentTier.T4));
            AvailableComponents.Add(new Railgun(ShipComponentTier.T5));
        }
        
        private void AddAllLaserCannons() {
            AvailableComponents.Add(new LaserCannon(ShipComponentTier.T1));
            AvailableComponents.Add(new LaserCannon(ShipComponentTier.T2));
            AvailableComponents.Add(new LaserCannon(ShipComponentTier.T3));
            AvailableComponents.Add(new LaserCannon(ShipComponentTier.T4));
            AvailableComponents.Add(new LaserCannon(ShipComponentTier.T5));
        }
        
        private void AddAllBallisticCannons() {
            AvailableComponents.Add(new BallisticCannon(ShipComponentTier.T1));
            AvailableComponents.Add(new BallisticCannon(ShipComponentTier.T2));
            AvailableComponents.Add(new BallisticCannon(ShipComponentTier.T3));
            AvailableComponents.Add(new BallisticCannon(ShipComponentTier.T4));
            AvailableComponents.Add(new BallisticCannon(ShipComponentTier.T5));
        }

        public void AddAllCargoBays() {
            AvailableComponents.Add(new CargoBay(ShipComponentTier.T1));
            AvailableComponents.Add(new CargoBay(ShipComponentTier.T2));
            AvailableComponents.Add(new CargoBay(ShipComponentTier.T3));
            AvailableComponents.Add(new CargoBay(ShipComponentTier.T4));
            AvailableComponents.Add(new CargoBay(ShipComponentTier.T5));
        }

        public void AddAllManoeuvringThrusters() {
            AvailableComponents.Add(new ManoeuvringThruster(ShipComponentTier.T1));
            AvailableComponents.Add(new ManoeuvringThruster(ShipComponentTier.T2));
            AvailableComponents.Add(new ManoeuvringThruster(ShipComponentTier.T3));
            AvailableComponents.Add(new ManoeuvringThruster(ShipComponentTier.T4));
            AvailableComponents.Add(new ManoeuvringThruster(ShipComponentTier.T5));
        }
    }
}