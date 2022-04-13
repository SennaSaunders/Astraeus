using System;
using Code.TextureGen;

namespace Code._Cargo.ProductTypes.Commodity {
    public abstract class Commodity : Cargo {
        public string Name { get; }
        public int BasePrice { get; }
        public Type MainPlanetType { get; }
        public int MainProductionRate { get; }
        public Type SecondaryPlanetType { get; }
        public int SecondaryProductionRate { get; }

        protected Commodity(string name, int basePrice, Type mainPlanetGenType, int mainProductionRate, Type secondaryPlanetGenType, int secondaryProductionRate) {
            Name = name;
            BasePrice = basePrice;
            MainPlanetType = mainPlanetGenType != null ? mainPlanetGenType.IsSubclassOf(typeof(PlanetGen)) ? mainPlanetGenType : null : null;
            MainProductionRate = mainProductionRate;
            SecondaryPlanetType = secondaryPlanetGenType != null ? secondaryPlanetGenType.IsSubclassOf(typeof(PlanetGen)) ? secondaryPlanetGenType : null : null;
            SecondaryProductionRate = secondaryProductionRate;
        }

        public override float GetMass() {
            return mass;
        }
    }
}