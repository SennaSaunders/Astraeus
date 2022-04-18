using Code._Cargo.ProductTypes.Ships;
using Code._Galaxy;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._Ships.Controllers;
using Code._Ships.ShipComponents.InternalComponents.JumpDrives;
using Code._Utility;
using Code.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Code._GameControllers {
    public class MapGUIController : MonoBehaviour {
        private GalaxyController _galaxyController;
        private UnityEngine.Camera _camera;
        private SolarSystemController _selectedSystem;
        private GalaxyCameraController _galaxyCameraController;
        private GameObject _guiGameObject, _jumpInfoCard, _jumpRangeCircle;
        private JumpDriveController _jumpDriveController;
        private void Start() {
            GameController.GUIController.ToggleShipGUI();
            _galaxyController = GameController._galaxyController;
            _jumpDriveController = GameController.CurrentShip.ShipObject.GetComponent<ShipController>().JumpDriveController;
            _camera = UnityEngine.Camera.main;
            _galaxyCameraController = _camera.gameObject.AddComponent<GalaxyCameraController>();
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
            _guiGameObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab("GUIPrefabs/Map/MapGUI"));
            SetupExitButton();
            
        }

        private void SetupExitButton() {
            GameObjectHelper.FindChild(_guiGameObject, "ExitBtn").GetComponent<Button>().onClick.AddListener(Exit);
        }

        private void Exit() {
            GameController.isPaused = false;
            GameController.GUIController.ToggleShipGUI();
            GameController.CurrentShip.ShipObject.GetComponent<ShipCameraController>().TakeCameraControl();
            Destroy(_jumpRangeCircle);
            Destroy(_guiGameObject);
            Destroy(this);
        }

        private void JumpExit() {
            //if station gui is up then delete it
            GameController.GUIController.ToggleShipGUI();
            GameController.CurrentShip.ShipObject.GetComponent<ShipCameraController>().TakeCameraControl();
            GameController.isPaused = false;
            Destroy(GameObject.Find("ProjectileHolder"));
            Destroy(_jumpRangeCircle);
            Destroy(_guiGameObject);
            Destroy(this);
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
            _jumpRangeCircle = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab("GUIPrefabs/Map/Circle"));
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

            _jumpInfoCard = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab("GUIPrefabs/Map/JumpInfoCard"), GameObjectHelper.FindChild(_guiGameObject, "JumpInfo").transform);
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
            Debug.Log("Jumping");
            _jumpDriveController.Jump(GetJumpDistance());
            _galaxyController.activeSystemController.Active = false;
            _selectedSystem.Active = true;
            _galaxyController.activeSystemController = _selectedSystem;
            GameController.CurrentSolarSystem = _galaxyController.activeSystemController._solarSystem;
            _galaxyController.activeSystemController.DisplaySolarSystem();
            GameController.CurrentShip.ShipObject.transform.position = new Vector3(0, 0, GameController.ShipZ);//jump to star
            JumpExit();
        }

        private void SetupJumpBtn() {
            GameObjectHelper.FindChild(_jumpInfoCard, "JumpBtn").GetComponent<Button>().onClick.AddListener(JumpBtnClick);
        }

        private void JumpBtnClick() {
            
            JumpDriveController.JumpStatus jumpStatus = _jumpDriveController.CanJump(_selectedSystem._solarSystem);
            
            if (jumpStatus == JumpDriveController.JumpStatus.InsufficientFuel) {
                GameObjectHelper.SetGUITextValue(_jumpInfoCard, "ErrorMsg", "Insufficient fuel");
            } else if (jumpStatus == JumpDriveController.JumpStatus.TooFar) {
                GameObjectHelper.SetGUITextValue(_jumpInfoCard, "ErrorMsg", "Too far");
            }
            else {
                JumpToSelectedSystem();
            }
            
        }
    }
}