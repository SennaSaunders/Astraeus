namespace Code._Ships.ShipComponents.InternalComponents.Power_Plants {
    public abstract class PowerPlant : InternalComponent {
        private static float baseMass = 300;
        
        public float EnergyCapacity;
        public float CurrentEnergy;
        public float RechargeRate;
        
        public float DepletionRecoveryTime;
        public float CurrentDepletionTime;
        public bool Depleted = false;
        
        public PowerPlant(string name, ShipComponentTier componentSize, float baseEnergyCapacity, float baseRechargeRate, int basePrice) : base(name + " Power Plant", componentSize, baseMass, basePrice) {
            EnergyCapacity = GetTierMultipliedValue(baseEnergyCapacity, componentSize);
            CurrentEnergy = EnergyCapacity;
            RechargeRate = GetTierMultipliedValue(baseRechargeRate, componentSize);
        }
    }

    public class PowerPlantHighRecharge : PowerPlant {
        private static float baseEnergyCapacity = 500;
        private static float baseRechargeRate = 200;

        public PowerPlantHighRecharge(ShipComponentTier componentSize) : base("High Recharge",componentSize, baseEnergyCapacity, baseRechargeRate, 3000) {
        }
    }
    
    public class PowerPlantHighCapacity : PowerPlant {
        private static float baseEnergyCapacity = 1500;
        private static float baseRechargeRate = 100;

        public PowerPlantHighCapacity(ShipComponentTier componentSize) : base("High Capacity",componentSize, baseEnergyCapacity, baseRechargeRate, 2500) {
        }
    }
    
    public class PowerPlantBalanced : PowerPlant {
        private static float baseEnergyCapacity = 750;
        private static float baseRechargeRate = 150;

        public PowerPlantBalanced(ShipComponentTier componentSize) : base("Balanced",componentSize, baseEnergyCapacity, baseRechargeRate, 1500) {
        }
    }
}