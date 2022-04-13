using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class RepairGUIController :MonoBehaviour {
        private StationGUIController _stationGUIController;
        private RepairService _repairService;
        private GameObject _guiGameObject;
        
        public void StartRepairGUI(RepairService repairService, StationGUIController stationGUIController) {
            _repairService = repairService;
            _stationGUIController = stationGUIController;
            SetupGUI();
        }
        
        private void SetupGUI() {
            _stationGUIController.stationGUI.SetActive(false);
            _guiGameObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_repairService.GUIPath));
            SetupHomeBtn();
        }

        private void SetupHomeBtn() {
            Button homeBtn = GameObject.Find("HomeBtn").GetComponent<Button>();
            homeBtn.onClick.AddListener(Exit);
        }

        private void Exit() {
            _stationGUIController.stationGUI.SetActive(true);
            Destroy(_guiGameObject);
            Destroy(this);
        }

        private void SetupSlider() {
            
        }
    }
}