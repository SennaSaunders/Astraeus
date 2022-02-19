namespace Code._Products.ProductTypes.Ships.Common {
    public class Fuel : Product {
        public Fuel(float maxEnergy) {
            MaxEnergy = maxEnergy;
            Capacity = 1;
        }

        public float MaxEnergy { get; private set; }
        public float Capacity { get; set; } // defines how full this unit of fuel is from 0 - 1
        public override float GetMass() {
            return mass * Capacity;
        }

        public float GetCurrentEnergy() {
            return MaxEnergy * Capacity;
        }
    }
}