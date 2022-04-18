using System.Collections.Generic;
using Code._Galaxy;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._PlayerProfile;
using Code._Ships;
using Code._Ships.Controllers;
using Code._Ships.ShipComponents;
using Code._Utility;
using Code.Camera;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code._GameControllers {
    public class GameController : MonoBehaviour {
        public static PrefabHandler _prefabHandler;
        public static GalaxyController _galaxyController;
        public static GameGUIController GUIController;
        public static SolarSystem CurrentSolarSystem;
        public static int MinExclusionDistance, GalaxyWidth,GalaxyHeight;
        public const int ShipZ = SolarSystemController.ZOffset - 100;

        public static PlayerProfile PlayerProfile = new PlayerProfile();
        public static Ship CurrentShip { get; set; }
        private GameObject _playerShipContainer;
        private PlayerShipController _playerShipController;
        
        public List<Ship> npcShips = new List<Ship>();
        public static IStation _currentStation;
        private static ShipCreator _shipCreator;
        public static bool isPaused = true;


        private void Awake() {
            gameObject.AddComponent<StandaloneInputModule>();
            _prefabHandler = gameObject.AddComponent<PrefabHandler>();
            _playerShipContainer = new GameObject("Player Ship Container");
            SetupGameGUIController();
            _shipCreator = gameObject.AddComponent<ShipCreator>();
        }

        private void SetShipToStation() {
            if (_currentStation != null) {
                Vector3 stationPos = _galaxyController.GetSolarSystemController(CurrentSolarSystem).GetBodyGameObject((Body)_currentStation).transform.position;
                CurrentShip.ShipObject.transform.position = new Vector3(stationPos.x, stationPos.y, ShipZ);
            }
        }

        private void SetupDefaultShip() {
            CurrentShip = _shipCreator.CreateDefaultShip(_playerShipContainer);
            CurrentShip.ShipObject.transform.SetParent(_playerShipContainer.transform);
            SetPlayerShipController();
            _playerShipController.Setup(CurrentShip);
            SetupShipCamera();
        }

        private void SetPlayerShipController() {
            _playerShipController = CurrentShip.ShipObject.AddComponent<PlayerShipController>();
            _playerShipController.gameObject.tag = "Player";
        }

        public void RefreshPlayerShip() {
            if (_playerShipContainer.transform.childCount > 0) {
                for (int i = _playerShipContainer.transform.childCount - 1; i >= 0; i--) {
                    Destroy(_playerShipContainer.transform.GetChild(i).gameObject);
                }
            }
            
            _shipCreator.shipObjectHandler.ManagedShip = CurrentShip;
            _shipCreator.shipObjectHandler.CreateShip(_playerShipContainer.transform);
            SetPlayerShipController();
            _playerShipController.Setup(CurrentShip);
            SetShipToStation();
            SetupShipCamera();
        }

        public void StartGame() {
            SetupDefaultShip();
            _galaxyController.DisplayGalaxy();
            _galaxyController.activeSystemController = _galaxyController.GetSolarSystemController(CurrentSolarSystem);
            _galaxyController.activeSystemController.DisplaySolarSystem();
            GUIController.SetupShipGUI();
            GUIController.SetupStationGUI(_currentStation);
            SetShipToStation();
        }

        public List<Faction> GetFactions() {
            return _galaxyController.GetFactions();
        }

        public SolarSystem GetCurrentSolarSystem() {
            return CurrentSolarSystem;
        }

        private void SetupShipCamera() {
            ShipCameraController shipCameraController = CurrentShip.ShipObject.AddComponent<ShipCameraController>();
            shipCameraController.TakeCameraControl();
        }

        public void Setup(Galaxy galaxy, int minExclusionDistance, int galaxyWidth, int galaxyHeight) {
            MinExclusionDistance = minExclusionDistance;
            GalaxyWidth = galaxyWidth;
            GalaxyHeight = galaxyHeight;
            _galaxyController = FindObjectOfType<GalaxyController>();
            if (Application.isEditor) {
                DestroyImmediate(_galaxyController);
            }
            else {
                Destroy(_galaxyController);
            }

            _galaxyController = gameObject.AddComponent<GalaxyController>();
            _galaxyController.SetGalaxy(galaxy);
            CurrentSolarSystem = GetStartingSystem();
            _currentStation = GetStartingStation(CurrentSolarSystem);
        }

        private void SetupGameGUIController() {
            GUIController = gameObject.AddComponent<GameGUIController>();
            GUIController.SetupGameController(this);
        }

        public void CreateNPC(Faction faction, ShipCreator.ShipClass shipClass, ShipComponentTier maxTier, float loadoutEfficiency, Vector2 spawnLocation) {
            string npcShipContainerName = "NPC Ships";
            GameObject npcShipContainer = GameObject.Find(npcShipContainerName);
            if (npcShipContainer == null) {
                npcShipContainer = new GameObject(npcShipContainerName);
            }

            Ship ship = _shipCreator.CreateFactionShip(shipClass, maxTier, loadoutEfficiency, faction, npcShipContainer);
            ship.ShipObject.transform.position = new Vector3(spawnLocation.x, spawnLocation.y, ShipZ);
            NPCShipController shipController = ship.ShipObject.AddComponent<NPCShipController>();
            shipController.Setup(ship, ship.ShipObject.transform.position);
            npcShips.Add(ship);
        }

        private SolarSystem GetStartingSystem() {
            //get a list of suitable starting locations
            List<Faction.FactionType> startingFactionTypes = new List<Faction.FactionType>() { Faction.FactionType.Agriculture, Faction.FactionType.Commerce, Faction.FactionType.Industrial };

            List<Faction> allFactions = _galaxyController.GetFactions();
            List<Faction> eligibleFactions = new List<Faction>();

            foreach (Faction faction in allFactions) { //get factions of startingFactionTypes
                foreach (Faction.FactionType factionType in startingFactionTypes) {
                    bool eligibleFactionType = factionType == faction.factionType;
                    if (eligibleFactionType) {
                        eligibleFactions.Add(faction);
                        break;
                    }
                }
            }

            if (eligibleFactions.Count > 0) {
                int chosenFactionIndex = GalaxyGenerator.Rng.Next(eligibleFactions.Count); //get a random eligible faction - using galaxy generator RNG so it's fixed by the galaxy generating seed

                return eligibleFactions[chosenFactionIndex].HomeSystem;
            }

            return null;
        }

        private SpaceStation GetStartingStation(SolarSystem solarSystem) {
            List<Body> bodies = solarSystem.Bodies;
            List<SpaceStation> spaceStations = new List<SpaceStation>();

            foreach (Body body in bodies) {
                if (body.GetType() == typeof(SpaceStation)) {
                    spaceStations.Add((SpaceStation)body);
                }
            }

            if (spaceStations.Count > 0) {
                int chosenStationIndex = GalaxyGenerator.Rng.Next(spaceStations.Count);
                return spaceStations[chosenStationIndex];
            }

            return null;
        }
    }
}