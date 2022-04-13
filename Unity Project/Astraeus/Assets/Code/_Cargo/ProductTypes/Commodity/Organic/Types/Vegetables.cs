using Code.TextureGen;

namespace Code._Cargo.ProductTypes.Commodity.Organic.Types {
    public class Vegetables : OrganicProduct{
        public Vegetables() : base("Vegetables", 70, typeof(EarthWorldGen), 800, typeof(WaterWorldGen), 300) {
        }
    }
}