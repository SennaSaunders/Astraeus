namespace Code._Cargo.ProductTypes.Commodity.Exotic {
    public abstract class ExoticProduct : Commodity {
        protected ExoticProduct(string name, int basePrice) : base(name, basePrice, null, 30, null, 0) {
        }
    }
}