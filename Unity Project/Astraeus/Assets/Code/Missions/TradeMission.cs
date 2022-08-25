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
        public TradeMission(SpaceStation missionPickupLocation, Faction missionGiver, SpaceStation deliveryDestination, Commodity missionCargo, int cargoQuota, int rewardCredits) : base(missionPickupLocation, missionGiver) {
            Destination = deliveryDestination;
            Cargo = missionCargo;
            CargoQuota = cargoQuota;
            RewardCredits = rewardCredits;
        }

        public int CargoQuota { get; }
        public int SuppliedCargo { get; private set; }
        public int CargoDelivered { get; private set; }
        public Commodity Cargo { get; }
        public SpaceStation Destination { get; }

        public bool PickupCargo(int qty) {
            if (qty <= _gameController.PlayerShipController.CargoController.GetFreeCargoSpace()) {
                List<Cargo> cargos = new List<Cargo>();
                for (int i = 0; i < qty; i++) {
                    cargos.Add(Cargo);
                }
                SuppliedCargo += cargos.Count;
                _gameController.PlayerShipController.CargoController.AddCargo(cargos);
                
                return true;
            }

            return false;
        }

        public bool AttemptDelivery(int qty) {
            List<Cargo> cargo = _gameController.PlayerShipController.CargoController.GetCargoOfType(Cargo.GetType());
            while (cargo.Count > qty) {
                cargo.RemoveAt(cargo.Count - 1);
            }

            if (cargo.Count > 0) {
                DeliverCargo(cargo.Count);
                _gameController.PlayerShipController.CargoController.RemoveCargo(cargo);
                return true;
            }

            return false;
        }

        private void DeliverCargo(int qty) {
            CargoDelivered += qty;
        }
    }
}