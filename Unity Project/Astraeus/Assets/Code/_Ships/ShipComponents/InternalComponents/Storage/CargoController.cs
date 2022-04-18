using System;
using System.Collections.Generic;
using System.Linq;
using Code._Cargo;
using Code._Cargo.ProductTypes.Ships;
using Code._Galaxy._SolarSystem._CelestialObjects.Star;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using UnityEngine;

namespace Code._Ships.ShipComponents.InternalComponents.Storage {
    public class CargoController {
        private List<CargoBay> _cargoBays;

        public CargoController(List<CargoBay> cargoBays) {
            _cargoBays = cargoBays;
        }

        public List<Type> GetCargoTypes() {
            List<Type> cargoTypes = new List<Type>();
            foreach (CargoBay cargoBay in _cargoBays) {
                foreach (Cargo cargo in cargoBay.StoredCargo) {
                    if (!cargoTypes.Contains(cargo.GetType())) {
                        cargoTypes.Add(cargo.GetType());
                    }
                }
            }

            return cargoTypes;
        }

        public void AddCargo(List<Cargo> newCargo) {
            if (newCargo.Count <= GetFreeCargoSpace()) {
                for (int i = 0; i < _cargoBays.Count; i++) {
                    for (int j = _cargoBays[i].StoredCargo.Count; j < _cargoBays[i].CargoVolume; j++) {
                        if (newCargo.Count > 0) {
                            Cargo cargo = newCargo[0];
                            _cargoBays[i].StoredCargo.Add(cargo);
                            newCargo.Remove(cargo);
                        }
                        else {
                            break;
                        }
                    }
                }
            }
        }

        public void RemoveCargo(List<Cargo> cargoToRemove) {
            while (cargoToRemove.Count>0) {
                Cargo currentCargo = cargoToRemove[0];
                for (int i = 0; i < _cargoBays.Count;i++) {
                    if (_cargoBays[i].StoredCargo.Contains(currentCargo)) {
                        _cargoBays[i].StoredCargo.Remove(currentCargo);
                        cargoToRemove.Remove(currentCargo);
                        break;
                    }
                }
            }
        }

        public int GetMaxCargoSpace() {
            int cargoSpace = 0;
            foreach (CargoBay cargoBay in _cargoBays) {
                cargoSpace += cargoBay.CargoVolume;
            }

            return cargoSpace;
        }

        public int GetUsedCargoSpace() {
            int storedCargo = 0;
            foreach (CargoBay cargoBay in _cargoBays) {
                storedCargo += cargoBay.StoredCargo.Count;
            }

            return storedCargo;
        }

        public int GetFreeCargoSpace() {
            return GetMaxCargoSpace() - GetUsedCargoSpace();
        }

        public List<Cargo> GetCargoOfType(Type cargoType) {
            List<Cargo> cargo = new List<Cargo>();
            foreach (CargoBay cargoBay in _cargoBays){
                cargo.AddRange(cargoBay.StoredCargo.Where(c=> c.GetType()==cargoType));
            }

            return cargo;
        }
        public List<Cargo> GetCargoOfType(Type cargoType, int amount)  {
            List<Cargo> cargo = GetCargoOfType(cargoType);
            cargo.RemoveRange(0, cargo.Count-amount);
            return cargo;
        }

        private float fuelCountToAdd = 0;
        private float timeToGetFuel = 0;
        public void FuelScoop(Star star) {
            fuelCountToAdd += (float)star.Tier/2 * Time.deltaTime;
            timeToGetFuel += Time.deltaTime;
            
            while (fuelCountToAdd > 1 && GetFreeCargoSpace() > 0) {
                Debug.Log(timeToGetFuel);
                AddCargo(new List<Cargo>(){new Fuel()});
                fuelCountToAdd--;
            }
        }
    }
}