using System;
using System.Collections.Generic;
using System.Linq;
using Code._Cargo;
using Code._Cargo.ProductTypes.Commodity;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;

namespace Code.Missions {
    public class TradeMission : Mission {
        public TradeMission(SpaceStation missionPickupLocation, Faction missionGiver) : base(missionPickupLocation, missionGiver) {
            SetMissionCargo();
            SetDeliveryDestination(missionGiver);
            CargoQuota = GetQuota();
            RewardCredits = GetRewardCredits(Cargo);
        }


        public int CargoQuota { get; }
        public int SuppliedCargo { get; private set; }
        public int CargoDelivered { get; private set; }
        public Commodity Cargo { get; private set; }
        public SpaceStation Destination { get; private set; }


        private void SetMissionCargo() {
            TradeService tradeService = (TradeService)MissionPickupLocation.StationServices.Find(service => service.GetType() == typeof(TradeService));
            List<(Type productType, int quantity, int price)> potentialProducts = tradeService.Products.Where(p => p.quantity > 0).ToList(); 
            Type productType = potentialProducts[r.Next(potentialProducts.Count)].productType;
            Cargo = (Commodity)Activator.CreateInstance(productType);
        }

        public bool PickupCargo(int qty) {
            if (qty <= GameController.PlayerShipController.CargoController.GetFreeCargoSpace()) {
                List<Cargo> cargos = new List<Cargo>();
                for (int i = 0; i < qty; i++) {
                    cargos.Add(Cargo);
                }
                SuppliedCargo += cargos.Count;
                GameController.PlayerShipController.CargoController.AddCargo(cargos);
                
                return true;
            }

            return false;
        }

        public bool AttemptDelivery(int qty) {
            List<Cargo> cargo = GameController.PlayerShipController.CargoController.GetCargoOfType(Cargo.GetType());
            while (cargo.Count > qty) {
                cargo.RemoveAt(cargo.Count - 1);
            }

            if (cargo.Count > 0) {
                DeliverCargo(cargo.Count);
                GameController.PlayerShipController.CargoController.RemoveCargo(cargo);
                return true;
            }

            return false;
        }

        private void DeliverCargo(int qty) {
            CargoDelivered += qty;
        }

        private int GetQuota() {
            int playerCargoSpace = GameController.PlayerShipController.CargoController.GetMaxCargoSpace();
            float maxMult = 1.5f;
            int multiplesOf = 20;
            int min = multiplesOf;
            int quota = r.Next(min, (int)(playerCargoSpace * maxMult));
            quota -= quota % multiplesOf;
            return quota;
        }

        private int GetRewardCredits(Cargo cargo) {
            TradeService tradeService = (TradeService)Destination.StationServices.Find(service => service.GetType() == typeof(TradeService));
            var tradeProduct = tradeService.Products.Find(product => product.productType == cargo.GetType());
            int cargoPrice = tradeProduct.price;
            
            return (CargoQuota) * (cargoPrice / 15 + 50);
        }

        private void SetDeliveryDestination(Faction faction) {
            bool internalTrade = false;
            float internalTradeChance = .5f;
            //deliver within faction
            if (faction.Systems.Count > 1) {
                internalTrade = r.NextDouble() < internalTradeChance;
            }

            SolarSystem destinationSystem;

            if (internalTrade) { //choose a destination that doesnt contain the mission giver location
                List<SolarSystem> solarSystems = faction.Systems.Where(s => !s.Bodies.Contains(MissionPickupLocation)).ToList();
                destinationSystem = solarSystems[r.Next(solarSystems.Count)];
            }
            else {
                Faction.FactionType factionType = faction.factionType;
                List<Faction> deliveryFactions = GameController.GalaxyController.GetFactions();
                deliveryFactions.Remove(faction);
                if (factionType == Faction.FactionType.Military) { //deliver to all types except military & pirate
                    deliveryFactions = deliveryFactions.Where(f => f.factionType != Faction.FactionType.Military && f.factionType != Faction.FactionType.Pirate).ToList();
                }
                else if (faction.factionType == Faction.FactionType.Pirate) { //only deliver to other pirate factions
                    deliveryFactions = deliveryFactions.Where(f => f.factionType == Faction.FactionType.Pirate).ToList();
                }
                else { //deliver to all except pirate
                    deliveryFactions = deliveryFactions.Where(f => f.factionType != Faction.FactionType.Pirate).ToList();
                }

                Faction chosenFaction = deliveryFactions[r.Next(deliveryFactions.Count)]; //choose a random faction from the possible factions
                destinationSystem = chosenFaction.Systems[r.Next(chosenFaction.Systems.Count)];
            }

            Destination = GetSystemSpaceStation(destinationSystem);
        }

        private SpaceStation GetSystemSpaceStation(SolarSystem solarSystem) {
            return (SpaceStation)solarSystem.Bodies.Find(b => b.GetType() == typeof(SpaceStation));
        }
    }
}