using Code.TextureGen;

namespace Code._Cargo.ProductTypes.Commodity.Organic.Types {
    public class Meat :OrganicProduct{
        public Meat() :base("Meat", 200, typeof(EarthWorldGen), 500, null, 0) {
        }
    }
}