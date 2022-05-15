using System.Collections.Generic;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using Code._Utility;
using Code.GUI.SpaceStations.Services;
using Code.GUI.SpaceStations.Services.Missions;
using Code.GUI.SpaceStations.Services.Outfitting;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations {
    public class StationGUIController : MonoBehaviour {
        private IStation Station { get; set; }
        private GameController _gameController;

        private readonly string _stationGUIBasePath = "GUIPrefabs/Station/";
        private readonly string _serviceButtonPathSpecifier = "ServiceButton"; 
        private readonly string _stationGUIPathSpecifier = "StationGUI";
        public GameObject stationGUI;

        public void Setup(IStation station, GameController gameController) {
            Station = station;
            _gameController = gameController;
            LoadGUI();
        }

        private void LoadGUI() {
            stationGUI = Instantiate((GameObject)Resources.Load(_stationGUIBasePath+_stationGUIPathSpecifier));
            GameController.IsPaused = true;
            GameController.GUIController.SetShipGUIActive(false);
            SetupStationInfo();
            SetupButtons();
        }

        private void SetupStationInfo() {
            SolarSystem solarSystem = GameController.CurrentSolarSystem;
            GameObjectHelper.SetGUITextValue(stationGUI, "StationName", solarSystem.SystemName + " Station");
            GameObjectHelper.SetGUITextValue(stationGUI, "FactionNameValue", solarSystem.OwnerFaction.GetFactionName());
            GameObjectHelper.SetGUITextValue(stationGUI, "FactionTypeValue", solarSystem.OwnerFaction.factionType.ToString());
            GameObjectHelper.SetGUITextValue(stationGUI, "FactionHomeSystemValue", solarSystem.OwnerFaction.HomeSystem.SystemName);
            GameObjectHelper.SetGUITextValue(stationGUI, "FactionStandingValue", "Implement Faction standing");
            GameObjectHelper.SetGUITextValue(stationGUI, "SystemStatusValue", "Implement System Status");
            GameObjectHelper.SetGUITextValue(stationGUI, "BodyCountValue", solarSystem.SystemStats.celestialBodyCount.ToString());
            
            string summaryText = "";
            List<(string bodyType, int count)> bodyCountMapping = new List<(string bodyType, int count)>() {
                ("Black Holes", solarSystem.SystemStats.blackHoleCount),
                ("Stars", solarSystem.SystemStats.starCount),
                ("Planets", solarSystem.SystemStats.planetCount)
            };
            
            List<(string bodyType, int count)> planetCountMapping = new List<(string bodyType, int count)>() {
                ("Earth-like", solarSystem.SystemStats.earthWorldCount),
                ("Water Worlds", solarSystem.SystemStats.waterWorldCount),
                ("Rocky", solarSystem.SystemStats.rockyWorldCount)
            };

            foreach ((string bodyType, int count) bodyCount in bodyCountMapping) {
                if (bodyCount.count > 0) {
                    summaryText += bodyCount.bodyType + ": " + bodyCount.count + "\n";
                }
            }

            summaryText += "\nPlanet Summary\n";
            foreach ((string bodyType, int count) bodyCount in planetCountMapping) {
                if (bodyCount.count > 0) {
                    summaryText += bodyCount.bodyType + ": " + bodyCount.count + "\n";
                }
            }
            
            GameObjectHelper.SetGUITextValue(stationGUI, "CelestialBodiesSummary", summaryText);
        }

        

        private Transform GetScrollContainer() {
            return stationGUI.transform.Find("MainContainer/Menu/Scroll View/Viewport/Content");
        }

        private void SetupButtons() {
            CreateServiceButtons();
            SetupExitBtn();
        }
        
        private void SetupExitBtn() {
            Button btn = GameObjectHelper.FindChild(stationGUI, "ExitBtn").GetComponent<Button>();
            btn.onClick.AddListener(Exit);
        }

        private void Exit() {
            GameController.IsPaused = false;
            GameController.GUIController.SetShipGUIActive(true);
            Destroy(stationGUI);
            Destroy(this);
        }
        
        private void CreateServiceButtons() {
            foreach (StationService stationService in Station.StationServices) {
                //instantiate button
                GameObject buttonPrefab = Instantiate((GameObject)Resources.Load(_stationGUIBasePath + _serviceButtonPathSpecifier), GetScrollContainer());

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
                }else if (stationService.GetType() == typeof(MissionService)) {
                    buttonMethod = MissionBtnClick;
                }
                Button btn = buttonPrefab.GetComponent<Button>(); 
                btn.onClick.AddListener(buttonMethod);

                TextMeshProUGUI text = buttonPrefab.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                text.text = stationService.ServiceName;
            }
        }

        

        public StationService FindStationService<T>() {
            return Station.StationServices.Find(s => s.GetType() == typeof(T));
        }
        
        private void OutfittingBtnClick() {
            OutfittingGUIController outfittingGUIController = gameObject.AddComponent<OutfittingGUIController>();
            OutfittingService outfittingService = (OutfittingService)FindStationService<OutfittingService>();
            outfittingGUIController.StartOutfitting(outfittingService, stationGUI, _gameController, GameController.CurrentShip);
        }

        private void RefuelBtnClick() {
            RefuelGUIController refuelGUIController = gameObject.AddComponent<RefuelGUIController>();
            RefuelService refuelService = (RefuelService)FindStationService<RefuelService>();
            refuelGUIController.StartRefuelGUI(refuelService, this);
        }

        private void RepairBtnClick() {
            RepairGUIController repairGUIController = gameObject.AddComponent<RepairGUIController>();
            RepairService repairService = (RepairService)FindStationService<RepairService>();
            repairGUIController.StartRepairGUI(repairService, this);
        }

        private void ShipyardBtnClick() {
            ShipyardGUIController shipyardGUIController = gameObject.AddComponent<ShipyardGUIController>();
            ShipyardService shipyardService = (ShipyardService)FindStationService<ShipyardService>();
            shipyardGUIController.StartShipyardGUI(shipyardService, this);
        }

        private void TradeBtnClick() {
            TradeGUIController tradeGUIController = gameObject.AddComponent<TradeGUIController>();
            TradeService tradeService = (TradeService)FindStationService<TradeService>();
            tradeGUIController.StartTradeGUI(tradeService, this);
        }
        private void MissionBtnClick() {
            MissionGUIController missionGUIController = gameObject.AddComponent<MissionGUIController>();
            MissionService missionService = (MissionService)FindStationService<MissionService>();
            missionGUIController.SetupGUI(missionService, this);
        }
    }
}