using Code.TextureGen;

namespace Code._Cargo.ProductTypes.Commodity.Industrial.Types {
    public class Metals : IndustrialProduct {
        public Metals() : base("Metals", 250, typeof(RockyWorldGen), 500, typeof(EarthWorldGen), 300) {
        }
    }
}