using System.Collections.Generic;
using System.Linq;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Ships.ShipComponents.InternalComponents.Storage;
using UnityEngine;

namespace Code._Ships.Controllers {
    public abstract class ShipController : MonoBehaviour {
        private Ship _ship;
        protected ThrusterController ThrusterController;
        private List<WeaponController> _weaponControllers;

        public void Setup(Ship ship) {
            _ship = ship;

            List<MainThruster> mainThrusters = _ship.ShipHull.MainThrusterComponents.Select(tc => tc.concreteComponent).Where(tc => tc != null).ToList();
            ManoeuvringThruster manoeuvringThrusters = _ship.ShipHull.ManoeuvringThrusterComponents.concreteComponent;
            List<float> centerOffsets = _ship.ShipHull.ManoeuvringThrusterComponents.thrusters.Select(t => t.centerOffset).ToList();
            ThrusterController = new ThrusterController(mainThrusters, (manoeuvringThrusters, centerOffsets), GetShipMass());
        }

        private void Update() {
            if (_ship.Active) {
                Thrust();
                Turn();
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

            Vector3 velocity = ThrusterController.velocity;
            Vector3 mappedVelocity = new Vector3(velocity.y, -velocity.x, velocity.z);
            
            gameObject.transform.Translate( mappedVelocity* Time.deltaTime, Space.World);
            
        }

        public void Turn() {
            var turnDir = GetTurnDirection();

            if (turnDir != 0) {
                ThrusterController.TurnShip(Time.deltaTime, turnDir);
            }
            else if(ThrusterController.angularVelocity!=0){
                ThrusterController.StopTurn(Time.deltaTime);
            }

            var angularVelocity = ThrusterController.angularVelocity;
            if (angularVelocity != 0) {
                Vector3 rotationVector = new Vector3(0, 0, ThrusterController.angularVelocity);
                gameObject.transform.Rotate(rotationVector * Time.deltaTime, Space.World);
            }
        }

        public abstract void AimWeapon(Vector2 target);

        public abstract Vector2 GetThrustVector();

        public abstract float GetTurnDirection();
    }
}