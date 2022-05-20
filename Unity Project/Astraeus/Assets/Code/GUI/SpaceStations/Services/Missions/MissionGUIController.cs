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
        private static int numTradeMissions = 10;
        private StationGUIController _stationGUIController;
        GameObject availableContentView;
        GameObject acceptedContentView;
        private List<TradeMissionGUIController> _acceptedTradeMissions = new List<TradeMissionGUIController>();

        public void SetupGUI(MissionService missionService, StationGUIController stationGUIController) {
            _missionService = missionService;
            _stationGUIController = stationGUIController;
            _stationGUIController.stationGUI.SetActive(false);
            guiGameObject = (GameObject)Instantiate(Resources.Load(_missionService.GUIPath));
            availableContentView = GameObjectHelper.FindChild(guiGameObject, "AvailableMissionsContent");
            acceptedContentView = GameObjectHelper.FindChild(guiGameObject, "AcceptedMissionsContent");
            SetupExitBtn();
            GenerateAvailableMissions();
            DisplayAcceptedMissions();
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
            for (int i = 0; i < numTradeMissions; i++) {
                TradeMission mission = _missionService.GenTradeMission();
                GameObject contentView = GameObjectHelper.FindChild(guiGameObject, "AvailableMissionsContent");
                TradeMissionGUIController tradeMissionGUIController = contentView.AddComponent<TradeMissionGUIController>();
                tradeMissionGUIController.SetupAvailableGUI(contentView, mission, this);
            }
        }

        private void DisplayAcceptedMissions() {
            foreach (Mission mission in GameController.PlayerProfile.Missions) {
                AddAcceptedMission(mission);
            }
        }

        public void AddAcceptedMission(Mission mission) {
            if (mission.GetType() == typeof(TradeMission)) {
                TradeMissionGUIController tradeMissionGUIController = acceptedContentView.AddComponent<TradeMissionGUIController>();
                _acceptedTradeMissions.Add(tradeMissionGUIController);
                tradeMissionGUIController.SetupAcceptedGUI(acceptedContentView, (TradeMission)mission, this);
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