using System.Collections.Generic;
using System.Linq;
using Code._Cargo;
using Code._Cargo.ProductTypes.Ships;
using Code._Galaxy._SolarSystem;
using Code._GameControllers;
using Code._Ships.ShipComponents.InternalComponents.Storage;

namespace Code._Ships.ShipComponents.InternalComponents.JumpDrives {
    public class JumpDriveController {
        private List<JumpDrive> _jumpDrives;
        private CargoController _cargoController;

        public JumpDriveController(List<JumpDrive> jumpDrives, CargoController cargoController) {
            _jumpDrives = jumpDrives;
            _cargoController = cargoController;
        }

        public float GetMaxRange() {
            float range = 0;

            foreach (JumpDrive jumpDrive in _jumpDrives) {
                range += jumpDrive.JumpRange;
            }

            return range;
        }

        private float GetJumpDistance(SolarSystem destination) {
            return (GameController._galaxyController.activeSystemController._solarSystem.Coordinate - destination.Coordinate).magnitude;
        }

        public float CalculateFuelEnergyUse(float distance) {
            return JumpDrive.EnergyPerLY * distance;
        }

        public void Jump(float distance) {
            SpendFuel(CalculateFuelEnergyUse(distance));
        }

        private void SpendFuel(float energySpent) {
            List<Fuel> fuelSpent = new List<Fuel>();
            List<Fuel> availableFuel = _cargoController.GetCargoOfType(typeof(Fuel)).Cast<Fuel>().ToList();

            while (energySpent > 0 && availableFuel.Count > 0) { //shouldn't be calling this if there isn't enough fuel energy but just in case checking against count
                Fuel fuel = availableFuel[0];
                energySpent -= fuel.GetCurrentEnergy();
                if (energySpent > 0) {
                    fuelSpent.Add(fuel);
                    availableFuel.Remove(fuel);
                }
                else {
                    fuel.Capacity = (fuel.Capacity * fuel.GetCurrentEnergy() + energySpent) / Fuel.MaxEnergy;
                }
            }

            _cargoController.RemoveCargo(fuelSpent.Cast<Cargo>().ToList());
        }

        private float CalculateTotalFuelEnergy() {
            List<Fuel> storedFuel = _cargoController.GetCargoOfType(typeof(Fuel)).Cast<Fuel>().ToList();
            float energy = 0;
            foreach (Fuel fuel in storedFuel) {
                energy += fuel.GetCurrentEnergy();
            }

            return energy;
        }

        public JumpStatus CanJump(SolarSystem destination) {
            //check distance
            float jumpDistance = GetJumpDistance(destination);
            bool distanceOk = jumpDistance < GetMaxRange();
            //check fuel
            bool enoughFuel = CalculateTotalFuelEnergy() > CalculateFuelEnergyUse(jumpDistance);

            return !distanceOk ? JumpStatus.TooFar : !enoughFuel ? JumpStatus.InsufficientFuel : JumpStatus.CanJump;
        }

        public enum JumpStatus {
            InsufficientFuel,
            TooFar,
            CanJump
        }
    }
}