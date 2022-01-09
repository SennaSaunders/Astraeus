namespace Code._Ships.Power_Plants {
    public abstract class PowerPlant : ShipComponent {
        private static float baseMass = 300;
        
        public float EnergyCapacity;
        public float CurrentEnergy;
        public float RechargeRate;
        
        public static float DepletionRecoveryTime;
        public bool Depleted = false;
        
        public float DrainPower(float powerRequested) {
            float outputEffectiveness = 0; //modifies the effectiveness of the ship component requesting power

            if (!Depleted) { //if not depleted
                if (CurrentEnergy - powerRequested > 0) { //if there is enough power 
                    CurrentEnergy -= powerRequested;
                    outputEffectiveness = 1;
                }
                else { // get the relative power capacity left to  drain the capacity to 0, set Depleted to true
                    outputEffectiveness = EnergyCapacity / powerRequested;
                    CurrentEnergy = 0;
                    Depleted = true;
                }
            }
            return outputEffectiveness;
        }

        public PowerPlant(string name, ShipComponentTier componentSize, float baseEnergyCapacity, float baseRechargeRate) : base(name + " Power Plant",ShipComponentType.Internal, componentSize, baseMass) {
            EnergyCapacity = GetTierMultipliedStat(baseEnergyCapacity, componentSize);
            CurrentEnergy = EnergyCapacity;
            RechargeRate = GetTierMultipliedStat(baseRechargeRate, componentSize);
        }
    }

    public class PowerPlantHighRecharge : PowerPlant {
        private static float baseEnergyCapacity = 1000;
        private static float baseRechargeRate = 250;

        public PowerPlantHighRecharge(ShipComponentTier componentSize) : base("High Recharge",componentSize, baseEnergyCapacity, baseRechargeRate) {
        }
    }
    
    public class PowerPlantHighCapacity : PowerPlant {
        private static float baseEnergyCapacity = 3000;
        private static float baseRechargeRate = 100;

        public PowerPlantHighCapacity(ShipComponentTier componentSize) : base("High Capacity",componentSize, baseEnergyCapacity, baseRechargeRate) {
        }
    }
    
    public class PowerPlantBalanced : PowerPlant {
        private static float baseEnergyCapacity = 2000;
        private static float baseRechargeRate = 200;

        public PowerPlantBalanced(ShipComponentTier componentSize) : base("Balanced",componentSize, baseEnergyCapacity, baseRechargeRate) {
        }
    }
}