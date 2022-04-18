using System.Collections.Generic;
using Code._Cargo.ProductTypes.Ships;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using NUnit.Framework;
using UnityEngine.TestTools.Utils;

namespace Tests {
    public class PowerPlantTest {
        private static List<PowerPlant> _powerPlants;
        private static List<Fuel> _fuel;
        private static int fuelUnits = 1000;
        private static PowerPlantController _powerPlantController;

        public static void SetupLowTier() {
            SetupLowTierPowerPlants();
            SetupPowerPlantController();
            SetupFuel();
        }

        public static void SetupHighTier() {
            SetupHighTierPowerPlants();
            SetupPowerPlantController();
            SetupFuel();
        }
        
        public static void SetupAllTiers() {
            SetupAllPowerPlantsAllTiers();
            SetupPowerPlantController();
            SetupFuel();
        }

        private static void SetupLowTierPowerPlants() {
            _powerPlants = new List<PowerPlant>() {
                new PowerPlantBalanced(ShipComponentTier.T1),
                new PowerPlantHighCapacity(ShipComponentTier.T1),
                new PowerPlantHighRecharge(ShipComponentTier.T1)
            };
        }

        private static void SetupHighTierPowerPlants() {
            _powerPlants = new List<PowerPlant>() {
                new PowerPlantBalanced(ShipComponentTier.T5),
                new PowerPlantHighCapacity(ShipComponentTier.T5),
                new PowerPlantHighRecharge(ShipComponentTier.T5)
            };
        }

        private static void SetupAllPowerPlantsAllTiers() {
            _powerPlants = new List<PowerPlant>() {
                new PowerPlantBalanced(ShipComponentTier.T1),
                new PowerPlantBalanced(ShipComponentTier.T2),
                new PowerPlantBalanced(ShipComponentTier.T3),
                new PowerPlantBalanced(ShipComponentTier.T4),
                new PowerPlantBalanced(ShipComponentTier.T5),
                
                new PowerPlantHighCapacity(ShipComponentTier.T1),
                new PowerPlantHighCapacity(ShipComponentTier.T2),
                new PowerPlantHighCapacity(ShipComponentTier.T3),
                new PowerPlantHighCapacity(ShipComponentTier.T4),
                new PowerPlantHighCapacity(ShipComponentTier.T5),
                
                new PowerPlantHighRecharge(ShipComponentTier.T1),
                new PowerPlantHighRecharge(ShipComponentTier.T2),
                new PowerPlantHighRecharge(ShipComponentTier.T3),
                new PowerPlantHighRecharge(ShipComponentTier.T4),
                new PowerPlantHighRecharge(ShipComponentTier.T5)
            };
        }

        private static void SetupFuel() {
            _fuel = new List<Fuel>();

            for (int i = 0; i < fuelUnits; i++) {
                _fuel.Add(new Fuel());
            }
        }

        private static void SetupPowerPlantController() {
            _powerPlantController = new PowerPlantController(_powerPlants);
        }

        private float GetMaxEnergyCapacity() {
            float maxEnergyCapacity = 0;
            foreach (PowerPlant powerPlant in _powerPlantController._powerPlants) {
                maxEnergyCapacity += powerPlant.EnergyCapacity;
            }

            return maxEnergyCapacity;
        }

        private float GetCurrentEnergyCapacity() {
            float currentEnergyCapacity = 0;
            foreach (PowerPlant powerPlant in _powerPlantController._powerPlants) {
                currentEnergyCapacity += powerPlant.CurrentEnergy;
            }

            return currentEnergyCapacity;
        }

        private float GetFuelMaxEnergy() {
            float fuelEnergy = 0;
            foreach (Fuel fuel in _fuel) {
                fuelEnergy += fuel.GetCurrentEnergy();
            }

            return fuelEnergy;
        }

        private float GetTotalRechargeRate() {
            float recharge = 0;
            foreach (PowerPlant powerPlant in _powerPlants) {
                recharge += powerPlant.RechargeRate;
            }

            return recharge;
        }

        private void RunPowerPlantInitTest() {
            foreach (PowerPlant powerPlant in _powerPlants) {
                Assert.AreEqual(powerPlant.EnergyCapacity, powerPlant.CurrentEnergy);
                Assert.AreEqual(0, powerPlant.CurrentDepletionTime);
            }
        }


        [Test]
        public void PowerPlantInitTest() {
            SetupLowTier();
            RunPowerPlantInitTest();
            
            SetupHighTier();
            RunPowerPlantInitTest();
            
            SetupAllTiers();
            RunPowerPlantInitTest();
        }

        public void RunPowerPlantDrainTest(float powerRequested) {
            float startEnergyCapacity = GetCurrentEnergyCapacity();
            float expectedEffectiveness = startEnergyCapacity/powerRequested > 1 ? 1 : startEnergyCapacity/powerRequested;
            float actualEffectiveness = _powerPlantController.DrainPower(powerRequested);
            Assert.That(actualEffectiveness, Is.EqualTo(expectedEffectiveness).Using(FloatEqualityComparer.Instance));
            
            float expectedCurrentEnergy = startEnergyCapacity - powerRequested > 0 ? startEnergyCapacity - powerRequested : 0;
            float currentEnergyCapacity = GetCurrentEnergyCapacity();
            Assert.That(currentEnergyCapacity, Is.EqualTo(expectedCurrentEnergy).Using(FloatEqualityComparer.Instance));
        }

        [Test]
        public void PowerPlantDrainTest() {
            SetupLowTier();
            RunPowerPlantDrainTest(1000);
            RunPowerPlantDrainTest(10000);
            RunPowerPlantDrainTest(1);
            
            SetupHighTier();
            RunPowerPlantDrainTest(1000);
            RunPowerPlantDrainTest(10000);
            RunPowerPlantDrainTest(1);
            
            SetupAllTiers();
            RunPowerPlantDrainTest(1000);
            RunPowerPlantDrainTest(10000);
            RunPowerPlantDrainTest(1);
        }

        private void RunPowerPlantRechargeTest(float powerDrain, float deltaTime) {
            _powerPlantController.DrainPower(powerDrain);

            float startEnergy = GetCurrentEnergyCapacity();

            float totalRechargeRate = 0;
            foreach (PowerPlant powerPlant in _powerPlants) {
                float rechargeRate = powerPlant.RechargeRate;
                float spareCapacity = powerPlant.EnergyCapacity - powerPlant.CurrentEnergy;
                float maxRecharge = rechargeRate * deltaTime < spareCapacity ? rechargeRate * deltaTime : spareCapacity;
                totalRechargeRate += maxRecharge;
            }

            float fuelMaxEnergy = GetFuelMaxEnergy();
            float expectedRecharge = totalRechargeRate < fuelMaxEnergy ? totalRechargeRate : fuelMaxEnergy;

            _powerPlantController.ChargePowerPlant(deltaTime, _fuel);
            
            float endEnergy = GetCurrentEnergyCapacity();
            float actualRecharge = endEnergy - startEnergy;

            Assert.That(actualRecharge, Is.EqualTo(expectedRecharge).Using(FloatEqualityComparer.Instance));
            Assert.That(fuelMaxEnergy-actualRecharge, Is.EqualTo(GetFuelMaxEnergy()).Using(FloatEqualityComparer.Instance));
        }

        [Test]
        public void PowerPlantRechargeTest() {
            SetupLowTier();
            RunPowerPlantRechargeTest(1500, 1);
            RunPowerPlantRechargeTest(1500, 5);
            RunPowerPlantRechargeTest(200, 10);
            RunPowerPlantRechargeTest(1000, 100);
            
            SetupHighTier();
            RunPowerPlantRechargeTest(1500, 1);
            RunPowerPlantRechargeTest(1500, 2);
            RunPowerPlantRechargeTest(200, 5);
            RunPowerPlantRechargeTest(1000, 10);
            
            SetupAllTiers();
            RunPowerPlantRechargeTest(1500, .5f);
            RunPowerPlantRechargeTest(10000, .1f);
            RunPowerPlantRechargeTest(60000, 5);
            RunPowerPlantRechargeTest(1000, 3);
            RunPowerPlantRechargeTest(10000, 40);
        }
    }
}