using System.Collections.Generic;
using System.Linq;
using Code._Cargo.ProductTypes.Ships;
using Code._Galaxy;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._GameControllers;
using Code._Ships.Controllers;
using Code._Ships.ShipComponents.InternalComponents.JumpDrives;
using Code._Utility;
using Code.Camera;
using Code.Missions;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.Map {
    public class GalaxyMapGUIController : MonoBehaviour {
        private GalaxyController _galaxyController;
        private UnityEngine.Camera _camera;
        private SolarSystemController _selectedSystem;
        private GalaxyCameraController _galaxyCameraController;
        private GameObject _guiGameObject, _jumpInfoCard, _jumpRangeCircle, _previousGUI,_guiHolder, _circleHolder;
        private JumpDriveController _jumpDriveController;
        private List<(SolarSystem pickup, List<TradeMission> missions)> _solarSystemTradePickup;
        private List<(SolarSystem destination, List<TradeMission> missions)> _solarSystemTradeDestination;
        private List<GameObject> _missionCircles = new List<GameObject>();

        public void Setup(GameObject previousGUI, SolarSystem solarSystem, GameObject holder) {
            _previousGUI = previousGUI;
            _guiHolder = holder;
            _circleHolder = new GameObject("Circle Holder");
            if (_previousGUI != null) {
                _previousGUI.SetActive(false);
            }
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
            CenterOnLocation(solarSystem);
            DrawJumpDistanceCircle();
            DrawMissionCircles();
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
            GameController.CurrentShip.ShipObject.GetComponent<ShipCameraController>().TakeCameraControl();
            Destroy(_jumpRangeCircle);
            Destroy(_circleHolder);
            Destroy(_guiGameObject);
            Destroy(_guiHolder);
            
            Destroy(this);
        }

        private void ExitBtn() {
            _camera.cullingMask = GameController.DefaultGameMask;
            if (_previousGUI == null) {
                GameController.GUIController.SetShipGUIActive(true);
                GameController.IsPaused = false;
            }
            else {
                _previousGUI.SetActive(true);
            }
            
            Exit();
        }

        private void JumpExit() {
            GameController.CurrentShip.ShipObject.GetComponent<ShipController>().ThrusterController.Velocity = new Vector2();
            Destroy(GameObject.Find("ProjectileHolder"));
            Exit();
        }

        private void CenterOnLocation(SolarSystem solarSystem) {
            _galaxyCameraController.SetCamera(solarSystem.Coordinate);
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
            Vector2 currentSystemPosition = _galaxyController.activeSystemController.SolarSystem.Coordinate;
            string circlePath = "GUIPrefabs/Map/Circle";
            DrawCircle(diameter, currentSystemPosition, Color.white, circlePath);
        }
        
        private void DrawMissionCircles() {
            //trade missions
            _solarSystemTradePickup = new List<(SolarSystem pickup, List<TradeMission> missions)>();
            _solarSystemTradeDestination = new List<(SolarSystem pickup, List<TradeMission> missions)>();
            foreach (TradeMission mission in GameController.PlayerProfile.Missions.FindAll(m => m.GetType()==typeof(TradeMission)).ToList().Cast<TradeMission>()) {
                (SolarSystem pickup, List<TradeMission> missions) pickupMap = _solarSystemTradePickup.Find(s => s.pickup ==mission.MissionPickupLocation.SolarSystem);
                (SolarSystem destination, List<TradeMission> missions) destinationMap = _solarSystemTradeDestination.Find(s => s.destination ==mission.Destination.SolarSystem);
                if (pickupMap.pickup==null) {
                    _solarSystemTradePickup.Add((mission.MissionPickupLocation.SolarSystem, new List<TradeMission>(){mission}));
                }
                else {
                    pickupMap.missions.Add(mission);
                }

                if (destinationMap.destination == null) {
                    _solarSystemTradeDestination.Add((mission.Destination.SolarSystem, new List<TradeMission>(){mission}));
                }
                else {
                    destinationMap.missions.Add(mission);
                }
            }

            string circlePath = "GUIPrefabs/Map/ThickCircle";

            foreach ((SolarSystem pickup, List<TradeMission> missions) tradePickups in _solarSystemTradePickup) {
                float pickupDiameter = 2;
                DrawCircle(pickupDiameter, tradePickups.pickup.Coordinate, Color.green, circlePath);
            }

            foreach ((SolarSystem destination, List<TradeMission> missions) tradeDestinations in _solarSystemTradeDestination) {
                float destinationDiameter = 2.3f;
                DrawCircle(destinationDiameter, tradeDestinations.destination.Coordinate, Color.blue, circlePath);
            }
        }

        private void DrawCircle(float diameter, Vector2 coordinate, Color colour, string circlePath) {
            GameObject circle = Instantiate((GameObject)Resources.Load(circlePath), _circleHolder.transform);
            circle.layer = LayerMask.NameToLayer("GalaxyMap");
            Vector3 position = new Vector3(coordinate.x, coordinate.y, GalaxyController.ZOffset);
            circle.transform.position = position;
            circle.transform.localScale = new Vector3(diameter, diameter, 2);
            circle.GetComponent<SpriteRenderer>().color = colour;
        }

        private float GetJumpDistance() {
            return (_galaxyController.activeSystemController.SolarSystem.Coordinate - _selectedSystem.SolarSystem.Coordinate).magnitude;
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
            GameObjectHelper.SetGUITextValue(_jumpInfoCard, "SystemName", _selectedSystem.SolarSystem.SystemName + " System");
            float jumpDistance = GetJumpDistance();
            GameObjectHelper.SetGUITextValue(_jumpInfoCard, "JumpDistanceValue", jumpDistance.ToString());
            GameObjectHelper.SetGUITextValue(_jumpInfoCard, "FuelConsumedValue", (_jumpDriveController.CalculateFuelEnergyUse(jumpDistance) / Fuel.MaxEnergy).ToString());

            string factionName = "None";
            string factionType = "None";
            if (_selectedSystem.SolarSystem.OwnerFaction != null) {
                Faction faction = _selectedSystem.SolarSystem.OwnerFaction;
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
            GameObject jumpBtn = GameObjectHelper.FindChild(_jumpInfoCard, "JumpBtn");
            if (_previousGUI == null) {
                jumpBtn.GetComponent<Button>().onClick.AddListener(JumpBtnClick);
            }
            else {
                jumpBtn.SetActive(false);
            }
        }

        private void JumpBtnClick() {
            JumpDriveController.JumpStatus jumpStatus = _jumpDriveController.CanJump(_selectedSystem.SolarSystem);

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