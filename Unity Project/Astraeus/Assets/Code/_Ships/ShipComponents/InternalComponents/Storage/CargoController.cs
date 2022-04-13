using System.Collections.Generic;
using System.Linq;
using Code._Cargo;

namespace Code._Ships.ShipComponents.InternalComponents.Storage {
    public class CargoController {
        private List<CargoBay> _cargoBays;

        public CargoController(List<CargoBay> cargoBays) {
            _cargoBays = cargoBays;
        }

        public bool AddCargo(List<Cargo> newCargo) {
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
                return true;
            }
            return false;
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

        public List<T> GetCargoOfType<T>() where T : Cargo {
            List<T> cargo = new List<T>();
            foreach (CargoBay cargoBay in _cargoBays) {
                cargo.AddRange(cargoBay.StoredCargo.Where(c => c.GetType() == typeof(T)).Cast<T>().ToList());
            }

            return cargo;
        }

        public List<T> GetCargoOfType<T>(int amount) where T : Cargo {
            List<T> cargo = GetCargoOfType<T>();
            cargo.RemoveRange(0, cargo.Count-amount);
            return cargo;
        }

    }
}