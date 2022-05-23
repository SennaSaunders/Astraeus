using Code._GameControllers;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Utility;
using Code.GUI.ShipGUI;
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

        public override void Setup(Ship ship) {
            base.Setup(ship);
            SetupPowerGUI();
            SetupFuelGUI();
        }

        public void ResetShields() {
            ShieldController.ResetShields();
        }

        private void SetupFuelGUI() {
            FuelObserver fuelObserver = GameObjectHelper.FindChild(GameController.GUIController.shipGUIController.guiGameObject, "FuelPanel").AddComponent<FuelObserver>();
            CargoController.AddObserver(fuelObserver);
        }

        private void SetupPowerGUI() {
            _powerPlantController.ClearObservers();
            GameObject powerGUI = Instantiate((GameObject)Resources.Load("GUIPrefabs/Ship/Power"), _shipHealthGUI.transform, false);
            ShipBarObserver powerObserver = GameObjectHelper.FindChild(powerGUI, "PowerBar").AddComponent<ShipBarObserver>();
            _powerPlantController.AddObserver(powerObserver);
            _powerPlantController.NotifyObservers();
        }

        protected override void AimWeapons() {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            LayerMask ignoreLayer = 1<<LayerMask.NameToLayer("LocalMap");
            ignoreLayer = ~ ignoreLayer;
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,ignoreLayer)) {
                if (hit.collider != null) {
                    Vector2 target = hit.point;
                    AimWeapons(target);
                }
            }
        }

        private void AimWeapons(Vector2 aimTarget) {
            foreach (WeaponController weaponController in WeaponControllers) {
                weaponController.TurnWeapon(aimTarget, transform.rotation);
            }
        }

        protected override void FireCheck() {
            if (!GameController.InLocalMap) {
                if (Input.GetMouseButton(0)) {
                    BaseInputModule baseInputModule = FindObjectOfType<BaseInputModule>();
                    if (!baseInputModule.IsPointerOverGameObject(-1)) {
                        FireWeapons();
                    }
                }
            }
        }

        protected override Vector2 GetThrustVector() {
            if (!GameController.InLocalMap) {
                Vector2 forwards = Input.GetKey(KeyCode.W) ? Vector2.up : new Vector2();
                Vector2 back = Input.GetKey(KeyCode.S) ? Vector2.down : new Vector2();
                Vector2 left = Input.GetKey(KeyCode.A) ? Vector2.left : new Vector2();
                Vector2 right = Input.GetKey(KeyCode.D) ? Vector2.right : new Vector2();
                return forwards + back + left + right;
            }
            return new Vector2();
        }

        protected override float GetTurnDirection() {
            if (!GameController.InLocalMap) {
                float left = Input.GetKey(KeyCode.Q) ? 1 : 0;
                float right = Input.GetKey(KeyCode.E) ? -1 : 0;
                return left + right;
            }

            return 0;
        }

        protected override void ShipDestroyed() {
            base.ShipDestroyed();
            GameObject projectiles = GameObject.Find("ProjectileHolder");
            for (int i = projectiles.transform.childCount; i > 0; i--) {
                Destroy(projectiles.transform.GetChild(i-1).gameObject);
            }
            
            RespawnGUI();
        }

        private void RespawnGUI() {
            GameController.GUIController.SetShipGUIActive(false);
            GameController.IsPaused = true;
            Instantiate((GameObject)Resources.Load("GUIPrefabs/Ship/ShipDestroyedGUI"));
        }
    }
}