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
using Code.Camera;
using Code.GUI.Loading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code._GameControllers {
    public class GameController : MonoBehaviour {
        public static GalaxyController GalaxyController;
        public static GameGUIController GUIController;
        public static PlayerProfile PlayerProfile = new PlayerProfile();
        public static SolarSystem CurrentSolarSystem;
        public static SpaceStation CurrentStation;
        public static bool IsPaused = true;
        public static Ship CurrentShip { get; set; }
        private GameObject _playerShipContainer;
        private PlayerShipController _playerShipController;
        public List<Ship> npcShips = new List<Ship>();
        public static ShipCreator ShipCreator;
        
        public static int MinExclusionDistance, GalaxyWidth, GalaxyHeight;
        public const int ShipZ = SolarSystemController.ZOffset - 100;
        public static LayerMask DefaultGameMask;
        

        private void Awake() {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("LocalMap"));
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Projectile"));
            gameObject.AddComponent<StandaloneInputModule>();
            _playerShipContainer = new GameObject("Player Ship Container");
            SetupGameGUIController();
            ShipCreator = gameObject.AddComponent<ShipCreator>();
            DefaultGameMask = SetDefaultGameMask();
        }

        private static int SetDefaultGameMask() {
            int mask = 1 << LayerMask.NameToLayer("Default");
            mask += 1 << LayerMask.NameToLayer("UI");
            mask += 1 << LayerMask.NameToLayer("Projectile");
            return mask;
        }

        public static void ChangeSolarSystem(SolarSystemController solarSystemController, bool shipGUI) {
            if (GalaxyController.activeSystemController != null) {
                GalaxyController.activeSystemController.Active = false;
            }
            GalaxyController.activeSystemController = solarSystemController;
            CurrentSolarSystem = solarSystemController._solarSystem;
            solarSystemController.DisplaySolarSystem(shipGUI);
        }

        private static void SetShipToStation() {
            if (CurrentStation != null) {
                
                Vector3 stationPos = GalaxyController.activeSystemController.GetBodyGameObject(CurrentStation).transform.position;
                CurrentShip.ShipObject.transform.position = new Vector3(stationPos.x, stationPos.y, ShipZ);
                CurrentShip.ShipObject.GetComponent<ShipController>().ThrusterController.Velocity = new Vector2();
            }
        }

        public static void SetShipToSystemOrigin() {
            CurrentShip.ShipObject.transform.position = new Vector3(0, 0, ShipZ);
        }

        private void SetupDefaultShip() {
            CurrentShip = ShipCreator.CreateDefaultShip(_playerShipContainer);
            PlayerProfile.Ships.Add(CurrentShip);
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
            ShipCreator.shipObjectHandler.ManagedShip = CurrentShip;
            ShipCreator.shipObjectHandler.CreateShip(_playerShipContainer.transform, Color.green);
            SetPlayerShipController();
            _playerShipController.Setup(CurrentShip);
            SetShipToStation();
            SetupShipCamera();
        }

        public static void StartGame() {
            GUIController.SetupStationGUI(CurrentStation);
            SetShipToStation();
        }

        public List<Faction> GetFactions() {
            return GalaxyController.GetFactions();
        }

        public SolarSystem GetCurrentSolarSystem() {
            return CurrentSolarSystem;
        }

        private void SetupShipCamera() {
            ShipCameraController shipCameraController = CurrentShip.ShipObject.AddComponent<ShipCameraController>();
            shipCameraController.TakeCameraControl();
        }

        public void Setup(Galaxy galaxy, int minExclusionDistance, int galaxyWidth, int galaxyHeight, LoadingScreenController loadingScreenController) {
            MinExclusionDistance = minExclusionDistance;
            GalaxyWidth = galaxyWidth;
            GalaxyHeight = galaxyHeight;
            GUIController.loadingScreenController = loadingScreenController;
            GalaxyController = FindObjectOfType<GalaxyController>();
            if (GalaxyController) {
                if (Application.isEditor) {
                    DestroyImmediate(GalaxyController);
                }
                else {
                    Destroy(GalaxyController);
                }
            }
            
            GalaxyController = gameObject.AddComponent<GalaxyController>();
            GalaxyController.SetGalaxy(galaxy);
            GalaxyController.DisplayGalaxy();
            ChangeSolarSystem(GalaxyController.GetSolarSystemController(GetStartingSystem()), false);
            CurrentStation = GetStartingStation(CurrentSolarSystem);
            GUIController.SetupShipGUI();
            SetupDefaultShip();
        }

        private void SetupGameGUIController() {
            GUIController = gameObject.AddComponent<GameGUIController>();
            GUIController.Setup(this);
        }

        public int GetNPCCount() {
            return npcShips.Count;
        }

        public void ClearNPCs() {
            foreach (Ship npcShip in npcShips) {
                Destroy(npcShip.ShipObject);
            }

            npcShips = new List<Ship>();
        }

        public void CreateNPC(Faction faction, ShipCreator.ShipClass shipClass, ShipComponentTier maxTier, float loadoutEfficiency, Vector2 spawnLocation) {
            string npcShipContainerName = "NPC Ships";
            GameObject npcShipContainer = GameObject.Find(npcShipContainerName);
            if (npcShipContainer == null) {
                npcShipContainer = new GameObject(npcShipContainerName);
            }

            Ship ship = ShipCreator.CreateFactionShip(shipClass, maxTier, loadoutEfficiency, faction, npcShipContainer);
            ship.ShipObject.transform.position = new Vector3(spawnLocation.x, spawnLocation.y, ShipZ);
            NPCShipController shipController = ship.ShipObject.AddComponent<NPCShipController>();
            shipController.Setup(ship);
            ShipCreator.FuelShip(ship);
            npcShips.Add(ship);
        }

        private SolarSystem GetStartingSystem() {
            //get a list of suitable starting locations
            List<Faction.FactionType> startingFactionTypes = new List<Faction.FactionType>() { Faction.FactionType.Agriculture, Faction.FactionType.Commerce, Faction.FactionType.Industrial };

            List<Faction> allFactions = GalaxyController.GetFactions();
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