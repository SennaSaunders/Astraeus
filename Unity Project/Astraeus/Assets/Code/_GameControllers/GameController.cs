using System.Collections.Generic;
using Code._Galaxy;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._Ships;
using Code._Ships.ShipComponents;
using Code._Utility;
using Code.Camera;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code._GameControllers {
    public class GameController : MonoBehaviour {
        public static PrefabHandler _prefabHandler;
        private static GalaxyController _galaxyController;
        private static GameGUIController _guiController;
        private static SolarSystem _currentSolarSystem;
        public const int ShipZ = SolarSystemController.ZOffset-10;
        public static Ship CurrentShip { get; set; }
        private GameObject _playerShipContainer;
        private PlayerShipController _playerShipController;
        
        public List<Ship> npcShips = new List<Ship>();

        private static IStation _currentStation;
        private Vector3 _playerPosition;

        private static ShipCreator _shipCreator;
        
        private UnityEngine.Camera _camera;


        private void Awake() {
            _camera = UnityEngine.Camera.main;
            gameObject.AddComponent<EventSystem>();
            gameObject.AddComponent<StandaloneInputModule>();
            _prefabHandler = gameObject.AddComponent<PrefabHandler>();
            _playerShipContainer = new GameObject("Player Ship Container");
            SetupGameGUIController();
            _shipCreator = gameObject.AddComponent<ShipCreator>();
        }

        private void SetShipPosition() {
            if (_currentStation != null) {
                Vector3 stationPos = _galaxyController.GetSolarSystemController(_currentSolarSystem).GetBodyGameObject((Body)_currentStation).transform.position;
                CurrentShip.ShipObject.transform.position = new Vector3(stationPos.x, stationPos.y, ShipZ);
            }
        }

        private void SetupDefaultShip() {
            CurrentShip = _shipCreator.CreateDefaultShip(_playerShipContainer);
            CurrentShip.ShipObject.transform.SetParent(_playerShipContainer.transform);
            SetPlayerShipController();
            _playerShipController.Setup(CurrentShip);
            SetShipPosition();
            SetupShipCamera();
        }

        private void SetPlayerShipController() {
            _playerShipController = CurrentShip.ShipObject.AddComponent<PlayerShipController>();
        }

        public void RefreshPlayerShip() {
            if (_playerShipContainer.transform.childCount > 0) {
                for (int i = _playerShipContainer.transform.childCount - 1; i >= 0; i--) {
                    Destroy(_playerShipContainer.transform.GetChild(i).gameObject);
                }
            }
            
            _shipCreator._shipObjectHandler.ManagedShip = CurrentShip;
            _shipCreator._shipObjectHandler.CreateShip(_playerShipContainer.transform);
            SetPlayerShipController();
            _playerShipController.Setup(CurrentShip);
            SetShipPosition();
            SetupShipCamera();
        }

        public void StartGame() {
            _guiController.SetupStationGUI(_currentStation);
            _galaxyController.DisplayGalaxy();
            SolarSystemController solarSystemController = _galaxyController.GetSolarSystemController(_currentSolarSystem);
            solarSystemController.DisplaySolarSystem();
            SetupDefaultShip();
        }


        private void SetupShipCamera() {
            ShipCameraController shipCameraController = CurrentShip.ShipObject.AddComponent<ShipCameraController>();
            shipCameraController.TakeCameraControl();
        }

        public void SetupGalaxyController(Galaxy galaxy) {
            _galaxyController = FindObjectOfType<GalaxyController>();
            if (Application.isEditor) {
                DestroyImmediate(_galaxyController);
            }
            else {
                Destroy(_galaxyController);
            }

            _galaxyController = gameObject.AddComponent<GalaxyController>();
            _galaxyController.SetGalaxy(galaxy);
            _currentSolarSystem = GetStartingSystem();
            _currentStation = GetStartingStation(_currentSolarSystem);

            // ShipCreatorTest();
        }

        private void SetupGameGUIController() {
            _guiController = gameObject.AddComponent<GameGUIController>();
            _guiController.SetupGameController(this);
        }

        public void ShipCreatorTest() {
            List<Faction> factions = _galaxyController.GetFactions();

            int offset = 0;
            int offsetChange = 20;
            GameObject npcShipContainer = new GameObject("NPC Ships");
            foreach (Faction faction in factions) {
                Ship newShip = _shipCreator.CreateFactionShip(ShipCreator.ShipClass.Fighter, ShipComponentTier.T5, .5f, faction, npcShipContainer);
                newShip.ShipObject.transform.SetParent(npcShipContainer.transform);
                newShip.ShipObject.transform.position = new Vector3(offset, 20, ShipZ);
                npcShips.Add(newShip);
                newShip = _shipCreator.CreateFactionShip(ShipCreator.ShipClass.Transport, ShipComponentTier.T5, .5f, faction, npcShipContainer);
                newShip.ShipObject.transform.SetParent(npcShipContainer.transform);
                newShip.ShipObject.transform.position = new Vector3(offset, 40, ShipZ);
                npcShips.Add(newShip);
                offset += offsetChange;
            }
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