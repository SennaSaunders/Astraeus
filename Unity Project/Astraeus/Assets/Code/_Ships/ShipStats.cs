using System.Collections.Generic;
using System.Linq;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Ships.ShipComponents.InternalComponents.JumpDrives;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code._Ships.ShipComponents.InternalComponents.Storage;

namespace Code._Ships {
    public static class ShipStats {
        public static float GetShipMass(Ship ship, bool includeCargo) {
            float mass = ship.ShipHull.HullMass;
            foreach (InternalComponent component in ship.ShipHull.InternalComponents.Select(ic => ic.concreteComponent)) {
                if (component != null) {
                    mass += component.ComponentMass;
                    if (includeCargo) {
                        if (component.GetType() == typeof(CargoBay)) {
                            mass += ((CargoBay)component).GetCargoMass();
                        }
                    }
                }
            }

            foreach (Weapon component in ship.ShipHull.WeaponComponents.Select(ic => ic.concreteComponent)) {
                if (component != null) {
                    mass += component.ComponentMass;
                }
            }

            foreach (MainThruster component in ship.ShipHull.MainThrusterComponents.Select(ic => ic.concreteComponent)) {
                if (component != null) {
                    mass += component.ComponentMass;
                }
            }

            ManoeuvringThruster manoeuvringThruster = ship.ShipHull.ManoeuvringThrusterComponents.concreteComponent;
            if (manoeuvringThruster != null) {
                mass += manoeuvringThruster.ComponentMass * ship.ShipHull.ManoeuvringThrusterComponents.parentTransformNames.Count;
            }


            return mass;
        }

        public static float GetPowerCapacity(Ship ship) {
            float capacity = 0;
            foreach (PowerPlant powerPlant in ship.ShipHull.InternalComponents.Select(ic => ic.concreteComponent).Where(ic => ic != null && ic.GetType().IsSubclassOf(typeof(PowerPlant))).Cast<PowerPlant>()) {
                capacity += powerPlant.EnergyCapacity;
            }

            return capacity;
        }

        public static float GetPowerRecharge(Ship ship) {
            float recharge = 0;
            foreach (PowerPlant powerPlant in ship.ShipHull.InternalComponents.Select(ic => ic.concreteComponent).Where(ic => ic != null && ic.GetType().IsSubclassOf(typeof(PowerPlant))).Cast<PowerPlant>()) {
                recharge += powerPlant.RechargeRate;
            }

            return recharge;
        }

        public static float GetDPS(Ship ship) {
            float dps = 0;
            foreach (Weapon weapon in ship.ShipHull.WeaponComponents.Select(wc => wc.concreteComponent).Where(wc => wc != null)) {
                float shotsPerSecond = 1 / weapon.FireDelay;
                dps += shotsPerSecond * weapon.Damage;
            }

            return dps;
        }

        public static float GetWeaponPowerPerSecond(Ship ship) {
            float powerPerSecond = 0;
            foreach (Weapon weapon in ship.ShipHull.WeaponComponents.Select(wc => wc.concreteComponent).Where(wc => wc != null)) {
                float shotsPerSecond = 1 / weapon.FireDelay; //reciprocal of fire delay time == shots per second
                powerPerSecond += shotsPerSecond * weapon.PowerDraw;
            }

            return powerPerSecond;
        }

        public static float GetMainThrusterForce(Ship ship) {
            float mainForce = 0;
            foreach (MainThruster component in ship.ShipHull.MainThrusterComponents.Select(ic => ic.concreteComponent)) {
                if (component != null) {
                    mainForce += component.Force;
                }
            }

            return mainForce;
        }

        public static float GetMainThrusterPowerPerSecond(Ship ship) {
            float powerPerSecond = 0;
            foreach (MainThruster component in ship.ShipHull.MainThrusterComponents.Select(ic => ic.concreteComponent)) {
                if (component != null) {
                    powerPerSecond += component.PowerDraw;
                }
            }

            return powerPerSecond;
        }

        public static float GetManoeuvringThrusterForce(Ship ship) {
            float manoeuvringForce = 0;
            ManoeuvringThruster manoeuvringThruster = ship.ShipHull.ManoeuvringThrusterComponents.concreteComponent;
            if (manoeuvringThruster != null) {
                manoeuvringForce = manoeuvringThruster.Force * ship.ShipHull.ManoeuvringThrusterComponents.parentTransformNames.Count / 2;
            }

            return manoeuvringForce;
        }

        public static float GetManoeuvringThrusterPowerPerSecond(Ship ship) {
            float powerPerSecond = 0;
            ManoeuvringThruster manoeuvringThruster = ship.ShipHull.ManoeuvringThrusterComponents.concreteComponent;
            if (manoeuvringThruster != null) {
                powerPerSecond += manoeuvringThruster.PowerDraw * ship.ShipHull.ManoeuvringThrusterComponents.parentTransformNames.Count / 2;
            }

            return powerPerSecond;
        }

        public static float GetShieldCapacity(Ship ship) {
            List<Shield> shields = ship.ShipHull.InternalComponents.Select(ic => ic.concreteComponent).Where(ic => ic != null && ic.GetType().IsSubclassOf(typeof(Shield))).Cast<Shield>().ToList();
            float shieldCapacity = 0;
            //capacity
            foreach (Shield shield in shields) {
                shieldCapacity += shield.StrengthCapacity;
            }

            return shieldCapacity;
        }

        public static float GetShieldRechargeRate(Ship ship) {
            List<Shield> shields = ship.ShipHull.InternalComponents.Select(ic => ic.concreteComponent).Where(ic => ic != null && ic.GetType().IsSubclassOf(typeof(Shield))).Cast<Shield>().ToList();
            float rechargeRate = 0;
            //capacity
            foreach (Shield shield in shields) {
                rechargeRate += shield.RechargeRate;
            }

            return rechargeRate;
        }

        public static float GetShieldPowerPerSecond(Ship ship) {
            float powerPerSecond = 0;
            foreach (Shield shield in ship.ShipHull.InternalComponents.Select(ic => ic.concreteComponent).Where(ic => ic != null && ic.GetType().IsSubclassOf(typeof(Shield))).Cast<Shield>()) {
                powerPerSecond += shield.RechargePower;
            }

            return powerPerSecond;
        }

        public static float GetJumpRange(Ship ship) {
            List<JumpDrive> jumpDrives = ship.ShipHull.InternalComponents.Select(ic => ic.concreteComponent).Where(ic => ic != null && ic.GetType() == typeof(JumpDrive)).Cast<JumpDrive>().ToList();
            float jumpRange = 0;
            foreach (JumpDrive jumpDrive in jumpDrives) {
                jumpRange += jumpDrive.JumpRange;
            }

            return jumpRange;
        }
        
        public static int GetUsedCargoSpace(Ship ship) {
            List<CargoBay> cargoBays = ship.ShipHull.InternalComponents.Select(ic => ic.concreteComponent).Where(ic => ic != null && ic.GetType() == typeof(CargoBay)).Cast<CargoBay>().ToList();
            int usedCargoSpace = 0;
            foreach (CargoBay cargoBay in cargoBays) {
                usedCargoSpace += cargoBay.StoredCargo.Count;
            }

            return usedCargoSpace;
        }

        public static int GetMaxCargoSpace(Ship ship) {
            List<CargoBay> cargoBays = ship.ShipHull.InternalComponents.Select(ic => ic.concreteComponent).Where(ic => ic != null && ic.GetType() == typeof(CargoBay)).Cast<CargoBay>().ToList();
            int maxCargoSpace = 0;
            foreach (CargoBay cargoBay in cargoBays) {
                maxCargoSpace += cargoBay.CargoVolume;
            }

            return maxCargoSpace;
        }

        public static int GetShipValue(Ship ship) {
            int shipValue = ship.ShipHull.HullPrice;
            foreach (var component in ship.ShipHull.InternalComponents) {
                if (component.concreteComponent != null) {
                    shipValue += component.concreteComponent.ComponentPrice;
                }
            }
            foreach (var component in ship.ShipHull.WeaponComponents) {
                if (component.concreteComponent != null) {
                    shipValue += component.concreteComponent.ComponentPrice;
                }
            }
            
            foreach (var component in ship.ShipHull.MainThrusterComponents) {
                if (component.concreteComponent != null) {
                    shipValue += component.concreteComponent.ComponentPrice;
                }
            }

            ManoeuvringThruster manoeuvringThruster = ship.ShipHull.ManoeuvringThrusterComponents.concreteComponent;
            if (manoeuvringThruster != null) {
                shipValue += manoeuvringThruster.ComponentPrice;
            }

            return shipValue;
        }
    }
}