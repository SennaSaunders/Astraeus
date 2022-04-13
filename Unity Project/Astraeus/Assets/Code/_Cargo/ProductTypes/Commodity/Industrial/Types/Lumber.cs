using Code.TextureGen;

namespace Code._Cargo.ProductTypes.Commodity.Industrial.Types {
    public class Lumber :IndustrialProduct{
        public Lumber() : base("Lumber", 1000, typeof(EarthWorldGen), 200, null, 0) {
        }
    }
}