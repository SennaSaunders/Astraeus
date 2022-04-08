using System.Collections.Generic;
using System.Linq;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Storage;
using Code._Utility;
using UnityEngine;

namespace Code._Ships.Controllers {
    public abstract class ShipController : MonoBehaviour {
        private Ship _ship;
        public ThrusterController ThrusterController;
        private List<WeaponController> _weaponControllers;
        private PowerPlantController _powerPlantController;
        private List<Ship> hostiles;

        public void Setup(Ship ship) {
            _ship = ship;
            List<PowerPlant> powerPlants = new List<PowerPlant>();
            foreach (var internalSlot in _ship.ShipHull.InternalComponents) {
                if (internalSlot.concreteComponent != null) {
                    InternalComponent component = internalSlot.concreteComponent;
                    if (component.GetType().IsSubclassOf(typeof(PowerPlant))) {
                        powerPlants.Add((PowerPlant)component);
                    }
                }
            }
            _powerPlantController = new PowerPlantController(powerPlants);
            
            List<MainThruster> mainThrusters = _ship.ShipHull.MainThrusterComponents.Select(tc => tc.concreteComponent).Where(tc => tc != null).ToList();
            ManoeuvringThruster manoeuvringThrusters = _ship.ShipHull.ManoeuvringThrusterComponents.concreteComponent;
            List<float> centerOffsets = _ship.ShipHull.ManoeuvringThrusterComponents.thrusters.Select(t => t.centerOffset).ToList();

            ThrusterController = new ThrusterController(mainThrusters, (manoeuvringThrusters, centerOffsets), GetShipMass(), _powerPlantController);
            _weaponControllers = new List<WeaponController>();
            foreach (var weaponComponent in ship.ShipHull.WeaponComponents) {
                Weapon weapon = weaponComponent.concreteComponent;
                if (weaponComponent.concreteComponent != null) {
                    var weaponGameObject = weapon.InstantiatedGameObject;
                    GameObject spindle = FindChildGameObject.FindChild(weaponGameObject,"TurretSpindle");

                    var weaponController = spindle.gameObject.AddComponent<WeaponController>();
                    weaponController.Setup(weapon, _powerPlantController);
                    _weaponControllers.Add(weaponController);
                }
            }
        }

        private void Update() {
            if (_ship.Active) {
                Thrust();
                Turn();
                AimWeapons();
                FireCheck();
            }
        }

        private float GetShipMass() {
            float mass = _ship.ShipHull.HullMass;

            foreach (InternalComponent internalComponent in _ship.ShipHull.InternalComponents.Select(ic => ic.concreteComponent)) {
                if (internalComponent != null) {
                    mass += internalComponent.ComponentMass;
                    if (internalComponent.GetType() == typeof(CargoBay)) {
                        mass += ((CargoBay)internalComponent).GetCargoMass();
                    }
                }
            }

            foreach (Thruster thruster in _ship.ShipHull.MainThrusterComponents.Select(tc => tc.concreteComponent)) {
                if (thruster != null) {
                    mass += thruster.ComponentMass;
                }
            }

            foreach (Weapon weapon in _ship.ShipHull.WeaponComponents.Select(wc => wc.concreteComponent)) {
                if (weapon != null) {
                    mass += weapon.ComponentMass;
                }
            }

            return mass;
        }

        public void Thrust() {
            Vector2 thrustVector = GetThrustVector();
            if (thrustVector != new Vector2()) {
                ThrusterController.FireThrusters(thrustVector, Time.deltaTime, gameObject.transform.localRotation.eulerAngles.z);
            }

            gameObject.transform.Translate( ThrusterController.Velocity* Time.deltaTime, Space.World);
        }

        public void Turn() {
            var turnDir = GetTurnDirection();

            if (turnDir != 0) {
                ThrusterController.TurnShip(Time.deltaTime, turnDir);
            }
            else if(ThrusterController.AngularVelocity!=0){
                ThrusterController.StopTurn(Time.deltaTime);
            }

            var angularVelocity = ThrusterController.AngularVelocity;
            if (angularVelocity != 0) {
                Vector3 rotationVector = new Vector3(0, 0, ThrusterController.AngularVelocity);
                gameObject.transform.Rotate(rotationVector * Time.deltaTime, Space.World);
            }
        }

        public void AimWeapons(Vector2 target) {
            foreach (WeaponController weaponController in _weaponControllers) {
                weaponController.TurnWeapon(target, transform.rotation);
            }
        }
        public abstract void AimWeapons();
        public abstract void FireCheck();

        public void FireWeapons() {
            foreach (WeaponController weaponController in _weaponControllers) {
                weaponController.Fire(ThrusterController.Velocity);
            }
        }
        public abstract Vector2 GetThrustVector();

        public abstract float GetTurnDirection();
    }
}