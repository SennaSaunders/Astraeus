using Code._Cargo.ProductTypes.Ships;
using Code._Galaxy;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._GameControllers;
using Code._Ships.Controllers;
using Code._Ships.ShipComponents.InternalComponents.JumpDrives;
using Code._Utility;
using Code.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.Map {
    public class GalaxyMapGUIController : MonoBehaviour {
        private GalaxyController _galaxyController;
        private UnityEngine.Camera _camera;
        private SolarSystemController _selectedSystem;
        private GalaxyCameraController _galaxyCameraController;
        private GameObject _guiGameObject, _jumpInfoCard, _jumpRangeCircle;
        private JumpDriveController _jumpDriveController;

        private void Start() {
            GameController.GUIController.SetShipGUIActive(false);
            _galaxyController = GameController.GalaxyController;
            _jumpDriveController = GameController.CurrentShip.ShipObject.GetComponent<ShipController>().JumpDriveController;
            _camera = GameObject.FindWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            _galaxyCameraController = _camera.gameObject.GetComponent<GalaxyCameraController>();
            if (_galaxyCameraController == null) {
                _galaxyCameraController = _camera.gameObject.AddComponent<GalaxyCameraController>();
            }

            _galaxyCameraController.TakeCameraControl();
            _galaxyCameraController.SetupCamera(GameController.GalaxyWidth, GameController.GalaxyHeight);
            CenterOnLocation();
            DrawJumpDistanceCircle();
            SetupGUI();
        }

        private void Update() {
            SolarSystemRaycast();
        }

        private void SetupGUI() {
            _guiGameObject = Instantiate((GameObject)Resources.Load("GUIPrefabs/Map/MapGUI"));
            SetupExitButton();
            _camera.cullingMask = 1 << LayerMask.NameToLayer("GalaxyMap"); //galaxy map mask only
        }

        private void SetupExitButton() {
            GameObjectHelper.FindChild(_guiGameObject, "ExitBtn").GetComponent<Button>().onClick.AddListener(ExitBtn);
        }

        private void Exit() {
            GameController.IsPaused = false;
            GameController.CurrentShip.ShipObject.GetComponent<ShipCameraController>().TakeCameraControl();
            Destroy(_jumpRangeCircle);
            Destroy(_guiGameObject);
            Destroy(this);
        }

        private void ExitBtn() {
            _camera.cullingMask = GameController.DefaultGameMask;
            GameController.GUIController.SetShipGUIActive(true);
            Exit();
        }

        private void JumpExit() {
            Destroy(GameObject.Find("ProjectileHolder"));
            Exit();
        }

        private void CenterOnLocation() {
            _galaxyCameraController.SetCamera(_galaxyController.activeSystemController._solarSystem.Coordinate);
        }

        private void SolarSystemRaycast() {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit)) {
                var bodyHit = hit.collider.gameObject.GetComponent<SolarSystemController>();
                if (bodyHit) {
                    if (Input.GetMouseButtonDown(0)) {
                        SelectSystem(bodyHit);
                    }
                }

                Debug.DrawLine(hit.transform.position, _camera.transform.position);
            }
        }

        private void DrawJumpDistanceCircle() {
            float diameter = GameController.CurrentShip.ShipObject.GetComponent<ShipController>().JumpDriveController.GetMaxRange() * 2;
            _jumpRangeCircle = Instantiate((GameObject)Resources.Load("GUIPrefabs/Map/Circle"));
            _jumpRangeCircle.layer = LayerMask.NameToLayer("GalaxyMap");
            Vector2 currentSystemPosition = _galaxyController.activeSystemController._solarSystem.Coordinate;
            Vector3 position = new Vector3(currentSystemPosition.x, currentSystemPosition.y, GalaxyController.ZOffset + 1);
            _jumpRangeCircle.transform.position = position;
            _jumpRangeCircle.transform.localScale = new Vector3(diameter, diameter, 2);
        }

        private float GetJumpDistance() {
            return (_galaxyController.activeSystemController._solarSystem.Coordinate - _selectedSystem._solarSystem.Coordinate).magnitude;
        }

        private void SelectSystem(SolarSystemController selectedSystem) {
            _selectedSystem = selectedSystem;
            SetupJumpInfo();
        }

        private void SetupJumpInfo() {
            if (_jumpInfoCard) {
                DestroyImmediate(_jumpInfoCard);
            }

            _jumpInfoCard = Instantiate((GameObject)Resources.Load("GUIPrefabs/Map/JumpInfoCard"), GameObjectHelper.FindChild(_guiGameObject, "Info").transform);
            GameObjectHelper.SetGUITextValue(_jumpInfoCard, "SystemName", _selectedSystem._solarSystem.SystemName + " System");
            float jumpDistance = GetJumpDistance();
            GameObjectHelper.SetGUITextValue(_jumpInfoCard, "JumpDistanceValue", jumpDistance.ToString());
            GameObjectHelper.SetGUITextValue(_jumpInfoCard, "FuelConsumedValue", (_jumpDriveController.CalculateFuelEnergyUse(jumpDistance) / Fuel.MaxEnergy).ToString());

            string factionName = "None";
            string factionType = "None";
            if (_selectedSystem._solarSystem.OwnerFaction != null) {
                Faction faction = _selectedSystem._solarSystem.OwnerFaction;
                factionName = faction.GetFactionName();
                factionType = faction.factionType.ToString();
            }

            GameObjectHelper.SetGUITextValue(_jumpInfoCard, "FactionNameValue", factionName);
            GameObjectHelper.SetGUITextValue(_jumpInfoCard, "FactionTypeValue", factionType);
            SetupJumpBtn();
        }

        private void JumpToSelectedSystem() {
            FindObjectOfType<GameController>().ClearNPCs();
            Debug.Log("Jumping");
            _jumpDriveController.Jump(GetJumpDistance());
            GameController.ChangeSolarSystem(_selectedSystem, true);
            GameController.SetShipToSystemOrigin();
            JumpExit();
        }

        private void SetupJumpBtn() {
            GameObjectHelper.FindChild(_jumpInfoCard, "JumpBtn").GetComponent<Button>().onClick.AddListener(JumpBtnClick);
        }

        private void JumpBtnClick() {
            JumpDriveController.JumpStatus jumpStatus = _jumpDriveController.CanJump(_selectedSystem._solarSystem);

            if (jumpStatus == JumpDriveController.JumpStatus.InsufficientFuel) {
                GameObjectHelper.SetGUITextValue(_jumpInfoCard, "ErrorMsg", "Insufficient fuel");
            }
            else if (jumpStatus == JumpDriveController.JumpStatus.TooFar) {
                GameObjectHelper.SetGUITextValue(_jumpInfoCard, "ErrorMsg", "Too far");
            }
            else {
                JumpToSelectedSystem();
            }
        }
    }
}