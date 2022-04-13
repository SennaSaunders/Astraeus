using Code.TextureGen;

namespace Code._Cargo.ProductTypes.Commodity.Industrial.Types {
    public class RareMetals : IndustrialProduct {
        public RareMetals() : base("Rare Metals", 1000, typeof(RockyWorldGen), 200, typeof(EarthWorldGen), 100) {
        }
    }
}