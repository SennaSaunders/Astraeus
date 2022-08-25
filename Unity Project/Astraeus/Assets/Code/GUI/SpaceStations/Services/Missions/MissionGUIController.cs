using System;
using System.Collections.Generic;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using Code._Utility;
using Code.Missions;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services.Missions {
    public class MissionGUIController : MonoBehaviour {
        private MissionService _missionService;
        public GameObject guiGameObject;
        private static readonly int NumTradeMissions = 10;
        private StationGUIController _stationGUIController;
        private GameObject _availableContentView;
        private GameObject _acceptedContentView;
        private List<TradeMissionGUIController> _acceptedTradeMissions = new List<TradeMissionGUIController>();
        private GameController _gameController;

        public void SetupGUI(MissionService missionService, StationGUIController stationGUIController) {
            _gameController = GameObjectHelper.GetGameController();
            _missionService = missionService;
            _stationGUIController = stationGUIController;
            _stationGUIController.stationGUI.SetActive(false);
            guiGameObject = (GameObject)Instantiate(Resources.Load(_missionService.GUIPath));
            _availableContentView = GameObjectHelper.FindChild(guiGameObject, "AvailableMissionsContent");
            _acceptedContentView = GameObjectHelper.FindChild(guiGameObject, "AcceptedMissionsContent");
            SetupExitBtn();
            GenerateAvailableMissions();
            DisplayAcceptedMissions();
            SetCreditsValue();
        }

        public void SetCreditsValue() {
            GameObjectHelper.SetGUITextValue(guiGameObject,"CreditsValue", _gameController.PlayerProfile._credits + "Cr");
        }

        private void SetupExitBtn() {
            Button button = GameObjectHelper.FindChild(guiGameObject, "ExitBtn").GetComponent<Button>();
            button.onClick.AddListener(Exit);
        }

        private void Exit() {
            _stationGUIController.stationGUI.SetActive(true);
            Destroy(guiGameObject);
            Destroy(this);
        }

        private void GenerateAvailableMissions() {
            for (int i = 0; i < NumTradeMissions; i++) {
                TradeMission mission = _missionService.GenTradeMission();
                TradeMissionGUIController tradeMissionGUIController = _availableContentView.AddComponent<TradeMissionGUIController>();
                tradeMissionGUIController.SetupAvailableGUI(_availableContentView, mission, this);
            }
        }

        private void DisplayAcceptedMissions() {
            foreach (Mission mission in _gameController.PlayerProfile.Missions) {
                AddAcceptedMission(mission);
            }
        }

        public void AddAcceptedMission(Mission mission) {
            if (mission.GetType() == typeof(TradeMission)) {
                TradeMissionGUIController tradeMissionGUIController = _acceptedContentView.AddComponent<TradeMissionGUIController>();
                _acceptedTradeMissions.Add(tradeMissionGUIController);
                tradeMissionGUIController.SetupAcceptedGUI(_acceptedContentView, (TradeMission)mission, this);
            }
        }

        public void RefreshAcceptedMissions() {
            foreach (TradeMissionGUIController tradeMissionGUIController in _acceptedTradeMissions) {
                tradeMissionGUIController.SetBaseGUIValues(true);
                tradeMissionGUIController.SetAcceptedValues();
            }
        }

        public void RemoveTradeMission(TradeMissionGUIController missionGUIController) {
            _acceptedTradeMissions.Remove(missionGUIController);
        }
    }
}