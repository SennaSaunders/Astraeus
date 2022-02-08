using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code.GUI.SpaceStations;
using UnityEngine;

namespace Code._GameControllers {
    public class GameGUIController : MonoBehaviour {
        private GameController _gameController;
        
        public void SetupGameController(GameController gameController) {
            _gameController = gameController;
        }
        
        public void SetupShipGUI() {
            
        }
        
        public void SetupStationGUI(IStation station) {
            StationGUIController stationGUIController = gameObject.AddComponent<StationGUIController>();
            stationGUIController.Setup(station, _gameController);
        }
    }
}