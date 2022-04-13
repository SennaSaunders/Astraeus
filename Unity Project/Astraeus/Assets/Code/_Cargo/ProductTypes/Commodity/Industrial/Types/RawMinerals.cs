using Code.TextureGen;

namespace Code._Cargo.ProductTypes.Commodity.Industrial.Types {
    public class RawMinerals : IndustrialProduct {
        public RawMinerals() : base("Raw Minerals", 500, typeof(RockyWorldGen), 300, typeof(EarthWorldGen), 100) {
        }
    }
}