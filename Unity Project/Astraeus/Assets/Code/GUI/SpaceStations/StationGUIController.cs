using System;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using Code.GUI.SpaceStations.Services;
using ICSharpCode.NRefactory.Ast;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations {
    public class StationGUIController : MonoBehaviour {
        private IStation _station;
        private GameController _gameController;

        private string _stationGUIBasePath = "GUIPrefabs/Station/";
        private string _serviceButtonPathSpecifier = "ServiceButton"; 
        private string _stationGUIPathSpecifier = "StationGUI";
        public GameObject stationGUI;

        public void Setup(IStation station, GameController gameController) {
            _station = station;
            _gameController = gameController;
            LoadGUI();
        }

        public void LoadGUI() {
            stationGUI = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_stationGUIBasePath+_stationGUIPathSpecifier));
            GameController.CurrentShip.Active = false;
            SetupButtons();
        }

        private Transform GetScrollContainer() {
            return stationGUI.transform.Find("MainContainer/Menu/Scroll View/Viewport/Content");
        }

        private void SetupButtons() {
            CreateServiceButtons();
            SetupExitBtn();
        }
        
        private void SetupExitBtn() {
            Button btn = GameObject.Find("ExitBtn").GetComponent<Button>();
            btn.onClick.AddListener(ExitBtnCLick);
        }

        private void ExitBtnCLick() {
            Debug.Log("Clicked - Exit Station Btn");
            GameController.CurrentShip.Active = true;
            Destroy(stationGUI);
            Destroy(this);
        }
        
        private void CreateServiceButtons() {
            foreach (StationService stationService in _station.StationServices) {
                //instantiate button
                GameObject buttonPrefab = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_stationGUIBasePath + _serviceButtonPathSpecifier), GetScrollContainer());

                UnityAction buttonMethod = null;
                if(stationService.GetType()==typeof(OutfittingService)) {
                    buttonMethod = OutfittingBtnClick;
                } else if (stationService.GetType() == typeof(RefuelService)) {
                    buttonMethod = RefuelBtnClick;
                }else if (stationService.GetType() == typeof(RepairService)) {
                    buttonMethod = RepairBtnClick;
                }else if (stationService.GetType() == typeof(ShipyardService)) {
                    buttonMethod = ShipyardBtnClick;
                }else if (stationService.GetType() == typeof(TradeService)) {
                    buttonMethod = TradeBtnClick;
                }
                Button btn = buttonPrefab.GetComponent<Button>(); 
                btn.onClick.AddListener(buttonMethod);

                TextMeshProUGUI text = buttonPrefab.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                text.text = stationService.ServiceName;
            }
        }

        private StationService FindStationService<T>() {
            return _station.StationServices.Find(s => s.GetType() == typeof(T));
        }
        
        private void OutfittingBtnClick() {
            Debug.Log("Outfitting Button Clicked");
            OutfittingGUIController outfittingGUIController = gameObject.AddComponent<OutfittingGUIController>();
            OutfittingService outfittingService = (OutfittingService)FindStationService<OutfittingService>();
            outfittingGUIController.StartOutfitting(outfittingService, this, _gameController);
        }

        private void RefuelBtnClick() {
            Debug.Log("Refuel Button Clicked");
            RefuelGUIController refuelGUIController = gameObject.AddComponent<RefuelGUIController>();
            RefuelService refuelService = (RefuelService)FindStationService<RefuelService>();
            refuelGUIController.StartRefuelGUI(refuelService, this);
        }

        private void RepairBtnClick() {
            Debug.Log("Refuel Button Clicked");
            RepairGUIController repairGUIController = gameObject.AddComponent<RepairGUIController>();
            RepairService repairService = (RepairService)FindStationService<RepairService>();
            repairGUIController.StartRepairGUI(repairService, this);
        }

        private void ShipyardBtnClick() {
            Debug.Log("Shipyard Button Clicked");
            ShipyardGUIController shipyardGUIController = gameObject.AddComponent<ShipyardGUIController>();
            ShipyardService shipyardService = (ShipyardService)FindStationService<ShipyardService>();
            shipyardGUIController.StartShipyardGUI(shipyardService, this);
        }

        private void TradeBtnClick() {
            Debug.Log("Trade Button Clicked");
            TradeGUIController tradeGUIController = gameObject.AddComponent<TradeGUIController>();
            TradeService tradeService = (TradeService)FindStationService<TradeService>();
            tradeGUIController.StartTradeGUI(tradeService, this);
        }
    }
}