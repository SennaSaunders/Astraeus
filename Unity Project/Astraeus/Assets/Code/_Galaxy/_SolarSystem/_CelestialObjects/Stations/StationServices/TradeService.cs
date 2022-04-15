using System;
using System.Collections.Generic;
using Code._Cargo.ProductTypes.Commodity;
using Code._Cargo.ProductTypes.Commodity.Industrial;
using Code._Cargo.ProductTypes.Commodity.Industrial.Types;
using Code._Cargo.ProductTypes.Commodity.Organic;
using Code._Cargo.ProductTypes.Commodity.Organic.Types;
using Code._Cargo.ProductTypes.Commodity.Tech;
using Code._Cargo.ProductTypes.Commodity.Tech.Types;
using Code._Galaxy._Factions;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class TradeService : StationService {
        private Faction _faction;
        private SolarSystem _solarSystem;
        public List<(Type productType, int quantity, int price)> Products;
        public List<(Type productType, int quantity, int price)> PlayerProducts;

        public TradeService(Faction faction, SolarSystem solarSystem) {
            _faction = faction;
            _solarSystem = solarSystem;
            SetupProducts();
        }

        protected override void SetGUIStrings() {
            ServiceName = "Trade";
            GUIPath = "GUIPrefabs/Station/Services/Trade/TradeGUI";
        }

        private void SetupProducts() {
            if (Products == null) {
                Products = new List<(Type productType, int quantity, int price)>();
                List<(Type productType, float productionMult, float priceMult)> productModifiers = _faction.GetProductionMultipliers();
                foreach ((Type productType, float productionMult, float priceMult) productModifier in productModifiers) {
                    
                        if (productModifier.productType == typeof(OrganicProduct)) {
                            SetupOrganics(productModifier.productionMult, productModifier.priceMult);
                        }
                        else if (productModifier.productType == typeof(IndustrialProduct)) {
                            SetupIndustrialProducts(productModifier.productionMult, productModifier.priceMult);
                        }
                        else if (productModifier.productType == typeof(TechProduct)) {
                            SetupTechProducts(productModifier.productionMult, productModifier.priceMult);
                        }
                    
                }
            }
        }

        private void SetupProductsOfType(Type productType, float productionMult, float priceMult) {
            Commodity commodity = (Commodity)Activator.CreateInstance(productType);
            //iterate through planets
            int price = (int)(priceMult * commodity.BasePrice);
            int quantity = 0;
            foreach (Body body in _solarSystem.Bodies) {
                if (body.GetType() == typeof(Planet.Planet)) {
                    Type planetGenType = ((Planet.Planet)body).PlanetGen.GetType();

                    if (planetGenType == commodity.MainPlanetType) {
                        quantity += (int)(productionMult * commodity.MainProductionRate);
                    }
                    else if (planetGenType == commodity.SecondaryPlanetType) {
                        quantity += (int)(productionMult * commodity.SecondaryProductionRate);
                    }
                }
            }
            
            Products.Add((productType, quantity, price));
        }

        private void SetupOrganics(float productionMult, float priceMult) {
            List<Type> organicTypes = GetOrganicProductTypes();
            foreach (Type organicType in organicTypes) {
                SetupProductsOfType(organicType, productionMult, priceMult);
            }
        }

        private void SetupIndustrialProducts(float productionMult, float priceMult) {
            List<Type> industrialTypes = GetIndustrialProductTypes();
            foreach (Type industrialType in industrialTypes) {
                SetupProductsOfType(industrialType, productionMult, priceMult);
            }
        }

        private void SetupTechProducts(float productionMult, float priceMult) {
            List<Type> techTypes = GetTechProductTypes();
            foreach (Type techType in techTypes) {
                SetupProductsOfType(techType, productionMult, priceMult);
            }
        }


        private List<Type> GetOrganicProductTypes() {
            return new List<Type>() {
                typeof(Vegetables),
                typeof(Fish),
                typeof(Meat),
                typeof(NaturalFiber),
                typeof(Water)
            };
        }

        private List<Type> GetIndustrialProductTypes() {
            return new List<Type>() {
                typeof(SyntheticPolymers),
                typeof(Lumber),
                typeof(RawMinerals),
                typeof(Metals),
                typeof(RareMetals),
                typeof(BasicComposites),
                typeof(MechanicalParts),
                typeof(Machinery)
            };
        }

        private List<Type> GetTechProductTypes() {
            return new List<Type>() {
                typeof(NanoCarbons),
                typeof(AdvancedComposites),
                typeof(Computers),
                typeof(ElectricalComponents),
                typeof(Silicon)
            };
        }
    }
}