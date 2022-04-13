using System;

namespace Code._Cargo.ProductTypes.Commodity.Tech {
    public abstract class TechProduct : Commodity {
        protected TechProduct(string name, int basePrice, Type mainPlanetGenType, int mainProductionRate, Type secondaryPlanetGenType, int secondaryProductionRate) : base(name, basePrice, mainPlanetGenType, mainProductionRate, secondaryPlanetGenType, secondaryProductionRate) {
        }
    }
}