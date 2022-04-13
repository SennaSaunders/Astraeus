using Code.TextureGen;

namespace Code._Cargo.ProductTypes.Commodity.Organic.Types {
    public class Water : OrganicProduct{
        public Water() :base("Water", 50, typeof(WaterWorldGen), 1300, typeof(EarthWorldGen), 500) {
        }
    }
}