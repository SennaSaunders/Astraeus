namespace Code._Ships.ShipComponents.InternalComponents.Shields {
    public abstract class Shield : InternalComponent {
        protected Shield(string nameSpecifier, ShipComponentTier componentSize, float baseMass, float strengthCapacity, float rechargeRate, float damageRecoveryTime, float depletionRecoveryTime) : base(nameSpecifier + " Shield", componentSize, baseMass) {
            StrengthCapacity = GetTierMultipliedValue(strengthCapacity, componentSize);
            CurrentStrength = StrengthCapacity;
            RechargeRate = GetTierMultipliedValue(rechargeRate, componentSize);
            DamageRecoveryTime = damageRecoveryTime;
            DepletionRecoveryTime = depletionRecoveryTime;
        }

        public float StrengthCapacity;
        public float CurrentStrength;
        public float RechargeRate;

        public float DamageRecoveryTime;
        public float DepletionRecoveryTime;


    }
    
    public class ShieldHighRecharge:Shield {
        public ShieldHighRecharge(ShipComponentTier componentSize) : base("High Recharge", componentSize, 300, 250, 10, 1.5f, 10) {
        }
    }

    public class ShieldHighCapacity : Shield {
        public ShieldHighCapacity(ShipComponentTier componentSize) : base("High Capacity", componentSize, 400, 500, 5, .5f, 20) {
        }
    }

    public class ShieldBalanced : Shield {
        public ShieldBalanced(ShipComponentTier componentSize) : base("Balanced", componentSize, 200, 325, 7.5f, 1, 15) {
        }
    }
}