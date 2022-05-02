namespace Code._Ships.ShipComponents.InternalComponents.Shields {
    public abstract class Shield : InternalComponent {
        protected Shield(string nameSpecifier, ShipComponentTier componentSize, float baseMass, float strengthCapacity, float rechargeRate, float rechargeEnergy, float damageRecoveryTime, float depletionRecoveryTime, int basePrice) : base(nameSpecifier + " Shield", componentSize, baseMass, basePrice) {
            StrengthCapacity = GetTierMultipliedValue(strengthCapacity, componentSize);
            CurrentStrength = StrengthCapacity;
            RechargeRate = GetTierMultipliedValue(rechargeRate, componentSize);
            RechargeEnergy = GetTierMultipliedValue(rechargeEnergy, componentSize);
            DamageRecoveryTime = damageRecoveryTime;
            DepletionRecoveryTime = depletionRecoveryTime;
        }

        public float StrengthCapacity { get; }
        public float CurrentStrength;
        public float RechargeRate{ get; }
        public float RechargeEnergy{ get; }

        public float DamageRecoveryTime { get;}
        public float CurrentRecoveryTime = 0;
        public float DepletionRecoveryTime { get; }
        public float CurrentDepletionTime = 0;
        public bool Depleted = false;


    }
    
    public class ShieldHighRecharge:Shield {
        public ShieldHighRecharge(ShipComponentTier componentSize) : base("High Recharge", componentSize, 300, 250, 10, 20,1.5f, 10, 3000) {
        }
    }

    public class ShieldHighCapacity : Shield {
        public ShieldHighCapacity(ShipComponentTier componentSize) : base("High Capacity", componentSize, 400, 500, 5,7.5f, .5f, 20, 2500) {
        }
    }

    public class ShieldBalanced : Shield {
        public ShieldBalanced(ShipComponentTier componentSize) : base("Balanced", componentSize, 200, 325, 7.5f, 5,1, 15, 1000) {
        }
    }
}