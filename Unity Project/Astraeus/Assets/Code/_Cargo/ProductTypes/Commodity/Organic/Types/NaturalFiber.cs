using Code.TextureGen;

namespace Code._Cargo.ProductTypes.Commodity.Organic.Types {
    public class NaturalFiber :OrganicProduct{
        public NaturalFiber() :base("Natural Fiber", 400, typeof(EarthWorldGen), 400, typeof(WaterWorldGen), 200) {
        }
    }
}