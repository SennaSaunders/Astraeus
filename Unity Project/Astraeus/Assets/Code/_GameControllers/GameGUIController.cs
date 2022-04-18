using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code.GUI.SpaceStations;
using UnityEngine;

namespace Code._GameControllers {
    public class GameGUIController : MonoBehaviour {
        private GameController _gameController;
        private ShipGUIController _shipGUIController;
        private MapGUIController _mapGUIController;
        private StationGUIController _stationGUIController;

        public void SetupGameController(GameController gameController) {
            _gameController = gameController;
        }

        public void SetupShipGUI() {
            _shipGUIController = gameObject.AddComponent<ShipGUIController>();
        }

        public void SetupStationGUI(IStation station) {
            if (!gameObject.GetComponent<StationGUIController>()) {
                GameController._currentStation = station;
                ToggleShipGUI();
                _stationGUIController = gameObject.AddComponent<StationGUIController>();
                _stationGUIController.Setup(station, _gameController);
            }
        }

        public void SetupGalaxyMap() {
            GameController.isPaused = true;
            _mapGUIController = gameObject.AddComponent<MapGUIController>();
        }

        public void ToggleShipGUI() {
            bool isActive = _shipGUIController.guiGameObject.activeSelf;
            _shipGUIController.guiGameObject.SetActive(!isActive);
        }

        public bool InStation() {
            return _shipGUIController != null;
        }
    }
}