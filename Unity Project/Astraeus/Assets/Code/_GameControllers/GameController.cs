using System.Collections.Generic;
using Code._Galaxy;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._Ships;
using Code._Ships.ShipComponents;
using UnityEngine;

namespace Code._GameControllers {
    public class GameController : MonoBehaviour {
        public enum GameFocus {
            Map,
            SolarSystem,
            Paused
        }
        
        private static GalaxyController _galaxyController;
        private static GameGUIController _guiController;
        private static SolarSystem _currentSolarSystem;
        public static Ship _currentShip { get; set; }
        private static IStation _currentStation;
        private static ShipCreator _shipCreator;
        
        public static GameFocus GameStateFocus;

        private void Awake() {
            SetupGameGUIController();
            
        }

        private void SetupShip() {
            _shipCreator = gameObject.AddComponent<ShipCreator>();
            _currentShip = _shipCreator.CreateDefaultShip();
        }

        public void StartGame() {
            _guiController.SetupStationGUI(_currentStation);
            SetupShip();
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

            
            //ShipCreatorTest();
        }

        private void SetupGameGUIController() {
            _guiController = gameObject.AddComponent<GameGUIController>();
            _guiController.SetupGameController(this);
        } 

        public void ShipCreatorTest() {
            List<Faction> factions = _galaxyController.GetFactions();

            int offset = 0;
            int offsetChange = 20;
            foreach (Faction faction in factions) {
                _shipCreator.CreateFactionShip(ShipCreator.ShipClass.Fighter, ShipComponentTier.T5, .5f, faction);
                _shipCreator._shipObjectHandler.ShipObject.transform.position = new Vector3(offset, 0, 0);
                _shipCreator.CreateFactionShip(ShipCreator.ShipClass.Transport, ShipComponentTier.T5, .5f, faction);
                _shipCreator._shipObjectHandler.ShipObject.transform.position = new Vector3(offset, 0, 20);
                offset += offsetChange;
            }
            
        }

        public static void ShowGalaxy() {
            _galaxyController.DisplayGalaxy();
        }

        private SolarSystem GetStartingSystem() {
            //get a list of suitable starting locations
            List<Faction.FactionType> startingFactionTypes = new List<Faction.FactionType>() { Faction.FactionType.Agriculture, Faction.FactionType.Commerce, Faction.FactionType.Industrial };

            List<Faction> allFactions = _galaxyController.GetFactions();
            List<Faction> eligibleFactions = new List<Faction>();

            foreach (Faction faction in allFactions) {//get factions of startingFactionTypes
                bool eligibleFactionType = false;
                foreach (Faction.FactionType factionType in startingFactionTypes) {
                    eligibleFactionType = factionType == faction.factionType ? true : false;
                    if (eligibleFactionType) {
                        eligibleFactions.Add(faction);
                        break;
                    }
                }
            }

            if (eligibleFactions.Count > 0) {
                int chosenFactionIndex = GalaxyGenerator.Rng.Next(eligibleFactions.Count);//get a random eligible faction - using galaxy generator RNG so it's fixed by the galaxy generating seed
            
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
        //start of game stuff
        //pick starting system
        //pick space station in system
        //spawn player in system
    }
}