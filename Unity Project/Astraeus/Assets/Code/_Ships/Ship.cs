using Code._Ships.Hulls;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using UnityEngine;

namespace Code._Ships {
    public class Ship : MonoBehaviour {
        public Hull ShipHull;
        private ThrustersController _thrustersController;
        public Vector2 velocity;
        public bool Active { get; set; } = false;
        public bool PlayerControlled { get; set; } = false;

        private void Update() {
            if (Active && PlayerControlled) {
                PlayerFlyShip();
            }
        }

        private void PlayerFlyShip() {
            velocity = _thrustersController.velocity;
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
        
                _thrustersController.FireThrusters(moveVector, Time.deltaTime);
            }
        
            if (turnLeft && !turnRight) {
                _thrustersController.TurnShip(Time.deltaTime, 1);
            }
            
            if (!turnLeft && turnRight) {
                _thrustersController.TurnShip(Time.deltaTime, -1);
            }
        
            transform.Translate(_thrustersController.velocity * Time.deltaTime, relativeTo:Space.World);
            var rotation =Quaternion.Euler(0,0,_thrustersController.facingAngle);
            transform.rotation = rotation;
            UnityEngine.Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -30);
        }

        // private void SetupShipControllers() {
        //     _thrustersController = new ThrustersController(GetShipComponents<MainThruster>(), GetShipComponents<ManoeuvringThruster>(), GetShipMass());
        //     List<Weapon> weapons = GetShipComponents<Weapon>();
        //     foreach (Weapon weapon in weapons) {
        //         _weaponControllers.Add(new WeaponController(weapon));
        //     }
        // }

        // private float GetShipMass() {
        //     float shipMass = 0;
        //     shipMass += ShipHull.HullMass;
        //     foreach (ShipComponent component in ShipComponents) {
        //         shipMass += component.ComponentMass;
        //
        //         if (component.GetType() == typeof(CargoBay)) {
        //             shipMass += ((CargoBay)component).GetCargoMass();
        //         }
        //     }
        //
        //     return shipMass;
        // }
        //
        // private List<T> GetShipComponents<T>() {
        //     List<T> componentList = new List<T>();
        //     foreach (ShipComponent shipComponent in ShipComponents) {
        //         if (shipComponent.GetType() == typeof(T)) {
        //             componentList.Add((T)Convert.ChangeType(shipComponent, typeof(T)));
        //         }
        //     }
        //
        //     return componentList;
        // }
    }
}