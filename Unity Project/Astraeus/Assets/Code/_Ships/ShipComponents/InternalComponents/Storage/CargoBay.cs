namespace Code._Ships.ShipComponents.InternalComponents.Storage {
    public class CargoBay : InternalComponent{
        public CargoBay(ShipComponentTier componentSize) : base("Cargo Bay", componentSize, 0) {
            CargoVolume = (int)GetTierMultipliedValue(BaseCargoVolume, componentSize);
        }

        private static int BaseCargoVolume = 500;
        public int CargoVolume;

        public float GetCargoMass() {
            return 0;
        }
    }
}