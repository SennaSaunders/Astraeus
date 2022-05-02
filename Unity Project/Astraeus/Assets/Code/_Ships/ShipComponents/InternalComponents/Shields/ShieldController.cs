using System.Collections.Generic;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code.GUI.ObserverPattern;
using UnityEngine;

namespace Code._Ships.ShipComponents.InternalComponents.Shields {
    public class ShieldController : ISubject<IItemObserver<float>> {
        private List<Shield> _shields;
        private PowerPlantController _powerPlantController;
        private List<IItemObserver<float>> _observers = new List<IItemObserver<float>>();

        public ShieldController(List<Shield> shields, PowerPlantController powerPlantController) {
            _shields = shields;
            _powerPlantController = powerPlantController;
        }

        public void ChargeShields() {
            float time = Time.deltaTime;
            foreach (Shield shield in _shields) {
                if (shield.CurrentStrength < shield.StrengthCapacity) {
                    //check depletion
                    if (shield.Depleted) {
                        shield.CurrentDepletionTime -= time;
                        if (shield.CurrentDepletionTime <= 0) {
                            shield.Depleted = false;
                        }
                    }
                    else {
                        //count down damage recovery time
                        if (shield.CurrentRecoveryTime > 0) {
                            shield.CurrentRecoveryTime -= time;
                        }

                        //shield allowed to charge
                        if (shield.CurrentRecoveryTime <= 0) {
                            float chargeTillFull = shield.StrengthCapacity - shield.CurrentStrength;
                            float potentialCharge = shield.RechargeRate * time;
                            potentialCharge = potentialCharge < chargeTillFull ? potentialCharge : chargeTillFull;

                            float maxEnergyUsed = (potentialCharge / shield.RechargeRate) * shield.RechargeEnergy;
                            float effectiveness = _powerPlantController.DrainPower(maxEnergyUsed);
                            float actualCharge = potentialCharge * effectiveness;
                            shield.CurrentStrength += actualCharge;
                            NotifyObservers();
                        }
                    }
                }
            }
        }

        public float TakeDamage(float damage) {
            float damageLeft = damage;
            for (int i = 0; i < _shields.Count; i++) {
                Shield shield = _shields[i];
                if (shield.CurrentStrength > 0) {
                    //depleted
                    if (shield.CurrentStrength - damageLeft < 0) {
                        damageLeft -= shield.CurrentStrength;
                        shield.CurrentStrength = 0;
                        SetDepletedRecovery(shield);
                    }
                    else {
                        shield.CurrentStrength -= damageLeft;
                        SetDamagedRecovery(shield);
                        damageLeft = 0;
                        break;
                    }
                }
            }

            NotifyObservers();
            return damageLeft;
        }

        private void SetDepletedRecovery(Shield shield) {
            shield.Depleted = true;
            shield.CurrentDepletionTime = shield.DepletionRecoveryTime;
            shield.CurrentRecoveryTime = 0;
        }

        private void SetDamagedRecovery(Shield shield) {
            shield.CurrentRecoveryTime = shield.DamageRecoveryTime;
        }

        private float GetMaxShieldHealth() {
            float health = 0;
            foreach (Shield shield in _shields) {
                health += shield.StrengthCapacity;
            }

            return health;
        }

        private float GetCurrentShieldHealth() {
            float health = 0;
            foreach (Shield shield in _shields) {
                health += shield.CurrentStrength;
            }

            return health;
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
            float value = GetCurrentShieldHealth() / GetMaxShieldHealth();
            foreach (IItemObserver<float> observer in _observers) {
                observer.UpdateSelf(value);
            }
        }
    }
}