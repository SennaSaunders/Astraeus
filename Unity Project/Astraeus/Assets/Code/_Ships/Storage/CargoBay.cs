namespace Code._Ships.Storage {
    public class CargoBay : ShipComponent{
        public CargoBay(ShipComponentTier componentSize) : base("Cargo Bay", ShipComponentType.Internal, componentSize, 0) {
            CargoVolume = (int)GetTierMultipliedStat(BaseCargoVolume, componentSize);
        }

        private static int BaseCargoVolume = 500;
        public int CargoVolume;

        public float GetCargoMass() {
            return 0;
        }
    }
}