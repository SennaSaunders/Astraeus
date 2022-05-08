namespace Code._Cargo.ProductTypes.Ships {
    public class Fuel : Cargo {
        //variable max energy allows for different types of fuel
        //don't know if necessary - could add depth to factions or be needless complication
        public Fuel() {
            Capacity = 1;
        }

        public const float MaxEnergy = 1000;
        public float Capacity { get; set; } // defines how full this unit of fuel is from 0 - 1
        public static int fuelPrice = 5;
        public override float GetMass() {
            return mass * Capacity;
        }

        public float GetCurrentEnergy() {
            return MaxEnergy * Capacity;
        }
    }
}