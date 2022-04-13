using System;

namespace Code._Cargo.ProductTypes.Commodity.Organic {
    public abstract class OrganicProduct : Commodity {
        protected OrganicProduct(string name, int basePrice, Type mainPlanetGenType, int mainProductionRate, Type secondaryPlanetGenType, int secondaryProductionRate) : base(name, basePrice, mainPlanetGenType, mainProductionRate, secondaryPlanetGenType, secondaryProductionRate) {
        }
    }
}