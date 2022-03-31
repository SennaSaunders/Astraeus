using System;
using Code._Galaxy._SolarSystem;
using UnityEngine;

namespace Code._Ships.Controllers {
    public class PlayerShipController : ShipController {
        //takes user input and maps it to thrust/turn
        private Vector3 targetVector;
        private float targetRotation;
        private BoxCollider mouseCollider;
        private UnityEngine.Camera _camera;

        private void Awake() {
            _camera = UnityEngine.Camera.main;
            mouseCollider = gameObject.AddComponent<BoxCollider>();
            mouseCollider.size = new Vector2(1000, 1000);
        }

        public override void AimWeapons() {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            
            
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (hit.collider != null) {
                    Vector2 target = hit.point;
                    AimWeapons(target);
                }
            }
        }

        public override Vector2 GetThrustVector() {
            Vector2 forwards = Input.GetKey(KeyCode.W) ? Vector2.up : new Vector2();
            Vector2 backwards = Input.GetKey(KeyCode.S) ? Vector2.down : new Vector2();
            Vector2 left = Input.GetKey(KeyCode.Q) ? Vector2.left : new Vector2();
            Vector2 right = Input.GetKey(KeyCode.E) ? Vector2.right : new Vector2();

            return forwards + backwards + left + right;
        }

        public override float GetTurnDirection() {
            float left = Input.GetKey(KeyCode.A) ? 1 : 0;
            float right = Input.GetKey(KeyCode.D) ? -1 : 0;
            return left + right;
        }
    }
}