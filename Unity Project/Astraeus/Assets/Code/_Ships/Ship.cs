using System;
using System.Collections.Generic;
using Code._Ships.Hulls;
using Code._Ships.Storage;
using Code._Ships.Thrusters;
using Code._Ships.Weapons;
using UnityEngine;

namespace Code._Ships {
    public class Ship : MonoBehaviour {
        public Hull ShipHull;
        public List<ShipComponent> ShipComponents; //list of all ship components - must be checked against hull to see if all components will fit
        private ThrusterController _thrusterController;
        private List<WeaponController> _weaponControllers = new List<WeaponController>();
        public Vector2 velocity;
        public float mainForce = 10000;
        public float manoeuvreForce = 2500;

        
        private void Start() {
            ShipHull = new Hull(null, 5000);
            ShipComponents = new List<ShipComponent>() { 
                new MainThruster(1, 300, mainForce, 5),
                new ManoeuvringThruster(1, 300, manoeuvreForce, 5) };
            SetupShipControllers();
        }

        private void Update() {
            velocity = _thrusterController.velocity;
            bool up = Input.GetKey(KeyCode.W);
            bool down = Input.GetKey(KeyCode.S);
            bool left = Input.GetKey(KeyCode.A);
            bool right = Input.GetKey(KeyCode.D);
            bool turnLeft = Input.GetKey(KeyCode.Q);
            bool turnRight = Input.GetKey(KeyCode.E);

            if (up || down || left || right) {
                Vector2 moveVector = new Vector2();
                if (up) {
                    moveVector += Vector2.up;
                }

                if (down) {
                    moveVector += Vector2.down;
                }

                if (left) {
                    moveVector += Vector2.left;
                }

                if (right) {
                    moveVector += Vector2.right;
                }

                _thrusterController.FireThrusters(moveVector, Time.deltaTime);
            }

            if (turnLeft && !turnRight) {
                _thrusterController.TurnShip(Time.deltaTime, 1);
            }
            
            if (!turnLeft && turnRight) {
                _thrusterController.TurnShip(Time.deltaTime, -1);
            }

            transform.Translate(_thrusterController.velocity * Time.deltaTime, relativeTo:Space.World);
            var rotation =Quaternion.Euler(0,0,_thrusterController.facingAngle);
            transform.rotation = rotation;
            UnityEngine.Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -30);
        }

        private void SetupShipControllers() {
            _thrusterController = new ThrusterController(GetShipComponents<MainThruster>(), GetShipComponents<ManoeuvringThruster>(), GetShipMass());
            List<Weapon> weapons = GetShipComponents<Weapon>();
            foreach (Weapon weapon in weapons) {
                _weaponControllers.Add(new WeaponController(weapon));
            }
        }

        private float GetShipMass() {
            float shipMass = 0;
            shipMass += ShipHull.HullMass;
            foreach (ShipComponent component in ShipComponents) {
                shipMass += component.ComponentMass;

                if (component.GetType() == typeof(CargoBay)) {
                    shipMass += ((CargoBay)component).GetCargoMass();
                }
            }

            return shipMass;
        }

        private List<T> GetShipComponents<T>() {
            List<T> componentList = new List<T>();
            foreach (ShipComponent shipComponent in ShipComponents) {
                if (shipComponent.GetType() == typeof(T)) {
                    componentList.Add((T)Convert.ChangeType(shipComponent, typeof(T)));
                }
            }

            return componentList;
        }
    }
}