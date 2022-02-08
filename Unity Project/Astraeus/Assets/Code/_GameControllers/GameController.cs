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
        public static Ship _currentShip { get; set; }
        private GameObject playerShipContainer;
        
        public List<Ship> _npcShips = new List<Ship>();

        private static IStation _currentStation;

        private static ShipCreator _shipCreator;
        
        private UnityEngine.Camera _camera;


        private void Awake() {
            _camera = UnityEngine.Camera.main;
            gameObject.AddComponent<EventSystem>();
            gameObject.AddComponent<StandaloneInputModule>();
            _prefabHandler = gameObject.AddComponent<PrefabHandler>();
            playerShipContainer = new GameObject("Player Ship Container");
            SetupGameGUIController();
            _shipCreator = gameObject.AddComponent<ShipCreator>();
        }

        private void SetupShip() {
            _currentShip = _shipCreator.CreateDefaultShip(playerShipContainer);
            _currentShip.ShipObject.transform.SetParent(playerShipContainer.transform);
        }

        public void RefreshPlayerShip() {
            if (playerShipContainer.transform.childCount > 0) {
                for (int i = playerShipContainer.transform.childCount - 1; i >= 0; i--) {
                    Destroy(playerShipContainer.transform.GetChild(i).gameObject);
                }
            }
            
            _shipCreator._shipObjectHandler.ManagedShip = _currentShip;
            _shipCreator._shipObjectHandler.CreateShip(playerShipContainer.transform);
        }

        public void StartGame() {
            

            _guiController.SetupStationGUI(_currentStation);
            SetupShip();
            SetupCameraControllers();
            _galaxyController.DisplayGalaxy();
            SolarSystemController solarSystemController = _galaxyController.GetSolarSystemController(_currentSolarSystem);
            solarSystemController.DisplaySolarSystem();
        }

        private void SetupCameraControllers() {
            _camera.gameObject.AddComponent<GalaxyCameraController>();
            ShipCameraController shipCameraController = _currentShip.ShipObject.AddComponent<ShipCameraController>();
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

            ShipCreatorTest();
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
                newShip.ShipObject.transform.position = new Vector3(offset, 20, 0);
                _npcShips.Add(newShip);
                newShip = _shipCreator.CreateFactionShip(ShipCreator.ShipClass.Transport, ShipComponentTier.T5, .5f, faction, npcShipContainer);
                newShip.ShipObject.transform.SetParent(npcShipContainer.transform);
                newShip.ShipObject.transform.position = new Vector3(offset, 40, 0);
                _npcShips.Add(newShip);
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