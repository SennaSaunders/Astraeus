using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code.GUI.Loading;
using Code.GUI.Map;
using Code.GUI.ShipGUI;
using Code.GUI.SpaceStations;
using UnityEngine;

namespace Code._GameControllers {
    public class GameGUIController : MonoBehaviour {
        private GameController _gameController;
        public LoadingScreenController loadingScreenController;
        public ShipGUIController shipGUIController;
        private LocalMapGUIController _localMapGUIController;
        public StationGUIController stationGUIController;

        public void Setup(GameController gameController) {
            _gameController = gameController;
        }

        public void SetupShipGUI() {
            shipGUIController = gameObject.AddComponent<ShipGUIController>();
            SetShipGUIActive(false);
        }

        public void SetupLocalMapGUI() {
            GameController.IsPaused = true;
            _localMapGUIController = gameObject.AddComponent<LocalMapGUIController>();
            _localMapGUIController.SetupGUI();
        }

        public void SetupStationGUI(SpaceStation station) {
            if (stationGUIController != null) {
                Destroy(stationGUIController);
            }

            GameController.CurrentStation = station;
            SetShipGUIActive(false);
            stationGUIController = gameObject.AddComponent<StationGUIController>();
            stationGUIController.Setup(station, _gameController);
        }

        public void SetupGalaxyMap() {
            GameController.IsPaused = true;
            gameObject.AddComponent<GalaxyMapGUIController>();
        }

        public void SetShipGUIActive(bool on) {
            if (shipGUIController != null) {
                shipGUIController.guiGameObject.SetActive(on);
            }
        }
    }
}