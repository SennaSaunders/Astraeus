using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using UnityEngine;
using UnityEngine.EventSystems;

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

        protected override void AimWeapons() {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (hit.collider != null) {
                    Vector2 target = hit.point;
                    AimWeapons(target);
                }
            }
        }

        public void AimWeapons(Vector2 aimTarget) {
            foreach (WeaponController weaponController in WeaponControllers) {
                weaponController.TurnWeapon(aimTarget, transform.rotation);
            }
        }

        protected override void FireCheck() {
            if (Input.GetMouseButton(0)) {
                BaseInputModule baseInputModule = FindObjectOfType<BaseInputModule>();
                if (!baseInputModule.IsPointerOverGameObject(-1)) {
                    FireWeapons();
                }
                
            }
        }

        protected override Vector2 GetThrustVector() {
            Vector2 forwards = Input.GetKey(KeyCode.W) ? Vector2.up : new Vector2();
            Vector2 back = Input.GetKey(KeyCode.S) ? Vector2.down : new Vector2();
            Vector2 left = Input.GetKey(KeyCode.A) ? Vector2.left : new Vector2();
            Vector2 right = Input.GetKey(KeyCode.D) ? Vector2.right : new Vector2();
            return forwards + back + left + right;
        }

        protected override float GetTurnDirection() {
            float left = Input.GetKey(KeyCode.Q) ? 1 : 0;
            float right = Input.GetKey(KeyCode.E) ? -1 : 0;
            return left + right;
        }
    }
}