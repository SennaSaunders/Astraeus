using System.Collections.Generic;
using Code._Cargo.ProductTypes.Ships;
using Code.GUI.ObserverPattern;

namespace Code._Ships.ShipComponents.InternalComponents.Power_Plants {
    public class PowerPlantController : ISubject<IItemObserver<float>> {
        public PowerPlantController(List<PowerPlant> powerPlants) {
            _powerPlants = powerPlants;
        }

        public List<PowerPlant> _powerPlants;
        private List<IItemObserver<float>> _observers = new List<IItemObserver<float>>();

        private float GetCurrentEnergy() {
            float totalCurrentEnergyCapacity = 0;

            foreach (PowerPlant powerPlant in _powerPlants) {
                totalCurrentEnergyCapacity += powerPlant.CurrentEnergy;
            }

            return totalCurrentEnergyCapacity;
        }

        private float GetTotalEnergyCapacity() {
            float energyCapacity = 0;

            foreach (PowerPlant powerPlant in _powerPlants) {
                energyCapacity += powerPlant.EnergyCapacity;
            }

            return energyCapacity;
        }

        public float DrainPower(float powerRequested) {
            if (powerRequested > 0) {
                //load balancing split the load equally between all power plants relative to their current total power
                float totalCurrentEnergyCapacity = GetCurrentEnergy();

                List<(float energyRequested, PowerPlant powerPlant)> powerDrainedPerPowerPlant = new List<(float energyRequested, PowerPlant powerPlant)>();

                foreach (PowerPlant powerPlant in _powerPlants) {
                    float energy = powerRequested * (powerPlant.CurrentEnergy / totalCurrentEnergyCapacity); //sets the amount of power each power plant will need
                    powerDrainedPerPowerPlant.Add((energy, powerPlant));
                }

                float powerProvided = 0;
                foreach ((float energyRequested, PowerPlant powerPlant) powerPlant in powerDrainedPerPowerPlant) {
                    if (!powerPlant.powerPlant.Depleted) { //if not depleted
                        if (powerPlant.powerPlant.CurrentEnergy - powerPlant.energyRequested > 0) {
                            powerPlant.powerPlant.CurrentEnergy -= powerPlant.energyRequested;
                            powerProvided += powerPlant.energyRequested;
                        }
                        else {
                            powerProvided += powerPlant.powerPlant.CurrentEnergy;
                            powerPlant.powerPlant.CurrentEnergy = 0;
                            powerPlant.powerPlant.Depleted = true;
                        }
                    }
                }

                NotifyObservers();

                return powerProvided / powerRequested;
            }

            return 0;
        }

        public List<Fuel> ChargePowerPlant(float deltaTime, List<Fuel> fuel) {
            List<Fuel> depletedFuel = new List<Fuel>();
            foreach (PowerPlant powerPlant in _powerPlants) {
                if (powerPlant.Depleted) {
                    powerPlant.CurrentDepletionTime += deltaTime; // add time to depletion timer
                    if (powerPlant.CurrentDepletionTime > powerPlant.DepletionRecoveryTime) { // if recovered from depletion
                        powerPlant.CurrentDepletionTime = 0;
                        powerPlant.Depleted = false;
                    }
                }

                float theoreticalRecharge = powerPlant.RechargeRate * deltaTime;
                float spareCapacity = powerPlant.EnergyCapacity - powerPlant.CurrentEnergy;

                float maxEnergyCharged = theoreticalRecharge <= spareCapacity ? theoreticalRecharge : spareCapacity;
                float usedFuelEnergy = 0;

                while (usedFuelEnergy < maxEnergyCharged && fuel.Count > 0) {
                    Fuel currentFuel = fuel[0];
                    float currentFuelEnergy = currentFuel.GetCurrentEnergy();

                    if (usedFuelEnergy + currentFuelEnergy <= maxEnergyCharged) {
                        usedFuelEnergy += currentFuelEnergy;
                        fuel.Remove(currentFuel);
                        depletedFuel.Add(currentFuel);
                    }
                    else {
                        float epsilon = 0.0001f;
                        float diff = maxEnergyCharged - usedFuelEnergy;
                        if (diff < epsilon && diff > -epsilon) {
                            usedFuelEnergy = maxEnergyCharged;
                        }
                        else {
                            float residualEnergy = usedFuelEnergy + currentFuelEnergy - maxEnergyCharged;
                            usedFuelEnergy += currentFuelEnergy - residualEnergy;
                            currentFuel.Capacity = residualEnergy / Fuel.MaxEnergy;
                        }
                    }
                }

                powerPlant.CurrentEnergy += usedFuelEnergy;
            }
            NotifyObservers();
            return depletedFuel;
        }

        public void AddObserver(IItemObserver<float> observer) {
            _observers.Add(observer);
        }

        public void RemoveObserver(IItemObserver<float> observer) {
            _observers.Remove(observer);
        }

        public void ClearObservers() {
            _observers = new List<IItemObserver<float>>();
        }

        public void NotifyObservers() {
            float value = GetCurrentEnergy() / GetTotalEnergyCapacity();
            foreach (IItemObserver<float> observer in _observers) {
                observer.UpdateSelf(value);
            }
        }
    }
}