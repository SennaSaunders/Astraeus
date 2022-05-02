using System.Collections.Generic;
using Code._Cargo;

namespace Code._Ships.ShipComponents.InternalComponents.Storage {
    public class CargoBay : InternalComponent{
        public CargoBay(ShipComponentTier componentSize) : base("Cargo Bay", componentSize, 0, 500) {
            CargoVolume = (int)GetTierMultipliedValue(BaseCargoVolume, componentSize);
            StoredCargo = new List<Cargo>();
        }

        private static int BaseCargoVolume = 500;
        public int CargoVolume;
        public List<Cargo> StoredCargo;

        public float GetCargoMass() {
            return 0;
        }
    }
}