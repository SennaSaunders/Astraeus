using System.Collections.Generic;
using Code._Cargo.ProductTypes.Ships;
using UnityEngine;

namespace Code._Ships.ShipComponents.InternalComponents.Power_Plants {
    public class PowerPlantController {
        public PowerPlantController(List<PowerPlant> powerPlants) {
            _powerPlants = powerPlants;
        }

        public List<PowerPlant> _powerPlants;

        public float DrainPower(float powerRequested) {
            if (powerRequested > 0) {
                //load balancing split the load equally between all power plants relative to their current total power
                float totalCurrentEnergyCapacity = 0;

                foreach (PowerPlant powerPlant in _powerPlants) {
                    totalCurrentEnergyCapacity += powerPlant.CurrentEnergy;
                }
                // Debug.Log("Power level: " + totalCurrentEnergyCapacity + "Power Requested: "+powerRequested);

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
                
                return powerProvided / powerRequested;
            }

            return 0;
        }

        public void ChargePowerPlant(float deltaTime, List<Fuel> fuel) {
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
                            currentFuel.Capacity = residualEnergy / currentFuel.MaxEnergy;
                        }
                    }
                }

                powerPlant.CurrentEnergy += usedFuelEnergy;
            }
        }
    }
}