using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class TradeGUIController :MonoBehaviour{
        private TradeService _tradeService;
        private StationGUIController _stationGUIController;
        private GameObject _guiGameObject;

        public void StartTradeGUI(TradeService tradeService, StationGUIController stationGUIController) {
            _tradeService = tradeService;
            _stationGUIController = stationGUIController;
            SetupGUI();
        }

        public void SetupGUI() {
            _stationGUIController.stationGUI.SetActive(false);
            _guiGameObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_tradeService.GUIPath));
            SetupHomeBtn();
        }

        public void SetupHomeBtn() {
            Button homeBtn = GameObject.Find("HomeBtn").GetComponent<Button>();
            homeBtn.onClick.AddListener(Exit);
        }

        public void Exit() {
            _stationGUIController.stationGUI.SetActive(true);
            Destroy(_guiGameObject);
            Destroy(this);
        }
    }
}