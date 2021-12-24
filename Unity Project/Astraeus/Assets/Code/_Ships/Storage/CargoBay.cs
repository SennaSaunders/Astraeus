namespace Code._Ships.Storage {
    public class CargoBay : ShipComponent{
        public CargoBay(int componentSize,int mass) : base(ShipComponentType.Internal, componentSize, mass) {
        }

        public float GetCargoMass() {
            return 0;
        }
    }
}