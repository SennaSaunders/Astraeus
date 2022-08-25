using System;
using System.Collections.Generic;
using System.Linq;
using Code._Cargo;
using Code._Cargo.ProductTypes.Commodity;
using Code._Galaxy._Factions;
using Code._GameControllers;
using Code._Utility;
using Code.Missions;
using UnityEngine;
using Random = System.Random;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class MissionService : StationService {
        private Faction _faction;
        private SpaceStation _spaceStation;
        private GameController _gameController;
        private static Random r = new Random();

        public MissionService(Faction faction, SpaceStation spaceStation) {
            _faction = faction;
            _spaceStation = spaceStation;
        }

        private void GetGameController() {
            if (_gameController == null) {
                _gameController = GameObjectHelper.GetGameController();
            }
        }

        protected override void SetGUIStrings() {
            ServiceName = "Missions";
            GUIPath = "GUIPrefabs/Station/Services/Missions/MissionGUI";
        }

        public TradeMission GenTradeMission() {
            Commodity cargo = GetMissionCargo();
            SpaceStation destination = GetDeliveryDestination(_faction);
            int quota = GetQuota();
            return new TradeMission(_spaceStation, _faction, destination, cargo, quota, GetDeliveryRewardCredits(cargo, destination, quota));
        }

        private Commodity GetMissionCargo() {
            TradeService tradeService = (TradeService)_spaceStation.StationServices.Find(service => service.GetType() == typeof(TradeService));
            List<(Type productType, int quantity, int price)> potentialProducts = tradeService.Products.Where(p => p.quantity > 0).ToList();
            Type productType = potentialProducts[r.Next(potentialProducts.Count)].productType;
            return (Commodity)Activator.CreateInstance(productType);
        }

        private int GetQuota() {
            GetGameController();
            int playerCargoSpace = _gameController.PlayerShipController.CargoController.GetMaxCargoSpace();
            float maxMult = 1.5f;
            int multiplesOf = 20;
            int min = multiplesOf;
            int quota = r.Next(min, (int)(playerCargoSpace * maxMult));
            quota -= quota % multiplesOf;
            return quota;
        }

        private SpaceStation GetDeliveryDestination(Faction faction) {
            bool internalTrade = false;
            float internalTradeChance = .6f;
            //deliver within faction
            if (faction.Systems.Count > 1) {
                internalTrade = r.NextDouble() < internalTradeChance;
            }

            SolarSystem destinationSystem;

            if (internalTrade) { //choose a destination that doesnt contain the mission giver location
                List<SolarSystem> solarSystems = faction.Systems.Where(s => !s.Bodies.Contains(_spaceStation)).ToList();
                destinationSystem = solarSystems[r.Next(solarSystems.Count)];
            }
            else {
                Faction.FactionType factionType = faction.factionType;
                GetGameController();
                List<Faction> deliveryFactions = _gameController.GalaxyController.GetFactions();
                deliveryFactions.Remove(faction);
                if (factionType == Faction.FactionType.Military) { //deliver to all types except military & pirate
                    deliveryFactions = deliveryFactions.Where(f => f.factionType != Faction.FactionType.Military && f.factionType != Faction.FactionType.Pirate).ToList();
                }
                else if (faction.factionType == Faction.FactionType.Pirate) { //only deliver to other pirate factions
                    List<Faction> potentialTargetFactions = deliveryFactions.Where(f => f.factionType == Faction.FactionType.Pirate).ToList();
                    if (potentialTargetFactions.Count == 0) {
                        potentialTargetFactions = deliveryFactions.Where(f => f.factionType != Faction.FactionType.Military && f.factionType != Faction.FactionType.Pirate).ToList();
                    }

                    deliveryFactions = potentialTargetFactions;
                }
                else { //deliver to all except pirate
                    deliveryFactions = deliveryFactions.Where(f => f.factionType != Faction.FactionType.Pirate).ToList();
                }

                Faction chosenFaction = deliveryFactions[r.Next(deliveryFactions.Count)]; //choose a random faction from the possible factions
                destinationSystem = chosenFaction.Systems[r.Next(chosenFaction.Systems.Count)];
            }

            return GetSystemSpaceStation(destinationSystem);
        }

        private SpaceStation GetSystemSpaceStation(SolarSystem solarSystem) {
            return (SpaceStation)solarSystem.Bodies.Find(b => b.GetType() == typeof(SpaceStation));
        }

        private int GetDeliveryRewardCredits(Cargo cargo, SpaceStation destination, int quota) {
            TradeService tradeService = (TradeService)destination.StationServices.Find(service => service.GetType() == typeof(TradeService));
            var tradeProduct = tradeService.Products.Find(product => product.productType == cargo.GetType());
            int cargoPrice = tradeProduct.price;
            return (quota) * (cargoPrice / 15 + 50);
        }
    }
}