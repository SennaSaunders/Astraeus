using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._Utility;
using Code.GUI.SpaceStations.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations {
    public class StationGUIController : MonoBehaviour {
        private IStation _station;

        private string _stationGUIBasePath = "GUIPrefabs/Station/";
        private string _serviceButtonPathSpecifier = "ServiceButton"; 
        private string _stationGUIPathSpecifier = "StationGUI";
        private PrefabHandler _prefabHandler;
        private GameObject _stationGUI;

        public void Setup(IStation station) {
            _station = station;
            SetupHandler();
            SetupGUI();
        }

        private void SetupHandler() {
            _prefabHandler = gameObject.AddComponent<PrefabHandler>();
        }

        private void SetupGUI() {
            _stationGUI = _prefabHandler.instantiateObject(_prefabHandler.loadPrefab(_stationGUIBasePath+_stationGUIPathSpecifier));
            CreateServiceButtons();
        }

        private Transform GetScrollContainer() {
            return _stationGUI.transform.Find("MainContainer/Menu/Scroll View/Viewport/Content");
        }
        
        private void CreateServiceButtons() {
            foreach (StationService stationService in _station.StationServices) {
                //instantiate button
                GameObject buttonPrefab = _prefabHandler.instantiateObject(_prefabHandler.loadPrefab(_stationGUIBasePath + _serviceButtonPathSpecifier), GetScrollContainer());
                
                if(stationService.GetType()==typeof(OutfittingService)) {
                    Button btn = buttonPrefab.GetComponent<Button>(); 
                    btn.onClick.AddListener(OutfittingBtnClick);
                }

                TextMeshProUGUI text = buttonPrefab.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                text.text = stationService.serviceName;
            }
            ;
        }

        private void OutfittingBtnClick() {
            Debug.Log("Outfitting Button Clicked");
            OutfittingGUIController outfittingGUIController = gameObject.AddComponent<OutfittingGUIController>();
            OutfittingService outfittingService = (OutfittingService)_station.StationServices.Find(s => s.GetType() == typeof(OutfittingService));
            outfittingGUIController.SetupOutfittingService(outfittingService);
            Destroy(_stationGUI);
            Destroy(this);
        }
    }
}