using System;

namespace Code._Cargo.ProductTypes.Commodity.Industrial {
    public abstract class IndustrialProduct : Commodity {
        protected IndustrialProduct(string name, int basePrice, Type mainPlanetGenType, int mainProductionRate, Type secondaryPlanetGenType, int secondaryProductionRate) : base(name, basePrice, mainPlanetGenType, mainProductionRate, secondaryPlanetGenType, secondaryProductionRate) {
        }
    }
}