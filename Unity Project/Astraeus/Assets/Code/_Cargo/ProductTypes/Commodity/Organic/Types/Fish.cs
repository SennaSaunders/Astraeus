using Code.TextureGen;

namespace Code._Cargo.ProductTypes.Commodity.Organic.Types {
    public class Fish : OrganicProduct {
        public Fish() : base("Fish", 150, typeof(WaterWorldGen), 500, typeof(EarthWorldGen), 200) {
        }
    }
}