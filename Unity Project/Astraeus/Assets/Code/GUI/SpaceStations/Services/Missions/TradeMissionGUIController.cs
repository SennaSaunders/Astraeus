using Code._Galaxy._SolarSystem;
using Code._GameControllers;
using Code._Utility;
using Code.GUI.Map;
using Code.Missions;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services.Missions {
    public class TradeMissionGUIController : MonoBehaviour {
        private TradeMission _tradeMission;
        private string path = "GUIPrefabs/Station/Services/Missions/TradeMissionGUI";
        private GameObject _guiGameObject;
        private MissionGUIController _missionGUIController;
        private Slider _slider;
        private GameController _gameController;

        private enum DeliveryLocationEnum {
            Pickup,
            Destination,
            Other
        }

        private DeliveryLocationEnum LocationCheck() {
            if (_gameController.CurrentStation == _tradeMission.Destination) {
                return DeliveryLocationEnum.Destination;
            }

            if (_gameController.CurrentStation == _tradeMission.MissionPickupLocation) {
                return DeliveryLocationEnum.Pickup;
            }

            return DeliveryLocationEnum.Other;
        }

        private void SetupBaseGUI(GameObject parent, TradeMission mission) {
            _gameController = GameObjectHelper.GetGameController();
            _tradeMission = mission;
            _guiGameObject = (GameObject)Instantiate(Resources.Load(path), parent.transform);
            GameObjectHelper.FindChild(_guiGameObject, "MissionCompletePanel").SetActive(false);
            SetBaseGUIValues(false);
        }

        public void SetBaseGUIValues(bool refresh) {
            if (!refresh) {
                GameObjectHelper.SetGUITextValue(_guiGameObject, "MissionTitle", GetMissionName());
                GameObjectHelper.SetGUITextValue(_guiGameObject, "RewardValue", _tradeMission.RewardCredits + "Cr");
                GameObjectHelper.SetGUITextValue(_guiGameObject, "Description", GetMissionDescription());
                GameObjectHelper.SetGUITextValue(_guiGameObject, "FactionValue", _tradeMission.MissionGiver.GetFactionName());
                GameObjectHelper.SetGUITextValue(_guiGameObject, "PickupValue", _tradeMission.MissionPickupLocation.SolarSystem.SystemName + " Station");
                GameObjectHelper.SetGUITextValue(_guiGameObject, "DestinationValue", _tradeMission.Destination.SolarSystem.SystemName + " Station");
                
                Vector2 currentSystemPos = _gameController.GalaxyController.activeSystemController.SolarSystem.Coordinate;
                string pickupDistString = (currentSystemPos - _tradeMission.MissionPickupLocation.SolarSystem.Coordinate).magnitude.ToString("0.0"); 
                GameObjectHelper.SetGUITextValue(_guiGameObject, "PickupDistanceValue", pickupDistString + " LY");
                string destDistString = (currentSystemPos - _tradeMission.Destination.SolarSystem.Coordinate).magnitude.ToString("0.0");
                GameObjectHelper.SetGUITextValue(_guiGameObject, "DestinationDistanceValue", destDistString + " LY");
                Button pickupMapBtn = GameObjectHelper.FindChild(_guiGameObject, "PickupMapBtn").GetComponent<Button>();
                pickupMapBtn.onClick.AddListener(delegate { MapBtn(_tradeMission.MissionPickupLocation.SolarSystem); });
                Button destinationMapBtn = GameObjectHelper.FindChild(_guiGameObject, "DestinationMapBtn").GetComponent<Button>();
                destinationMapBtn.onClick.AddListener(delegate { MapBtn(_tradeMission.Destination.SolarSystem); });
            }
            
            GameObjectHelper.SetGUITextValue(_guiGameObject, "QuotaValue", _tradeMission.CargoDelivered + "/" + _tradeMission.CargoQuota);
        }

        private void MapBtn(SolarSystem solarSystem) {
            GameObject mapHolder = new GameObject("Galaxy Map Holder");
            GalaxyMapGUIController galaxyMapGUIController = mapHolder.AddComponent<GalaxyMapGUIController>();
            galaxyMapGUIController.Setup(_missionGUIController.guiGameObject, solarSystem, mapHolder);
        }

        public void SetupAvailableGUI(GameObject parent, TradeMission mission, MissionGUIController missionGUIController) {
            SetupBaseGUI(parent, mission);
            SetupAvailableElements(mission);
            _missionGUIController = missionGUIController;
        }
        
        private void SetupAvailableElements(Mission mission) {
            //turn off deliver/pickup panel
            GameObjectHelper.FindChild(_guiGameObject, "CargoPanel").SetActive(false);
            //setup accept mission button
            Button button = GameObjectHelper.FindChild(_guiGameObject, "AcceptBtn").GetComponent<Button>();
            button.onClick.AddListener(delegate { AcceptMission(mission); });
        }

        private void AcceptMission(Mission mission) {
            _gameController.PlayerProfile.Missions.Add(mission);
            _missionGUIController.AddAcceptedMission(mission);
            Destroy(_guiGameObject);
            Destroy(this);
        }

        public void SetupAcceptedGUI(GameObject parent, TradeMission mission,MissionGUIController missionGUIController) {
            _missionGUIController = missionGUIController;
            SetupBaseGUI(parent, mission);
            SetAcceptedValues();
            
        }

        public void SetAcceptedValues() {
            switch (LocationCheck()) {
                case DeliveryLocationEnum.Destination:
                    SetupDeliveryElements();
                    break;
                case DeliveryLocationEnum.Pickup:
                    SetupPickupElements();
                    break;
                default:
                    SetupViewOnly();
                    break;
            }
        }

        private void SetupDeliveryElements() {
            //turn off accepted panel
            GameObjectHelper.FindChild(_guiGameObject, "AcceptPanel").SetActive(false);
            //setup slider
            int min = 0;
            int cargoCount = _gameController.PlayerShipController.CargoController.GetCargoOfType(_tradeMission.Cargo.GetType()).Count;
            int cargoLeft = _tradeMission.CargoQuota - _tradeMission.CargoDelivered;
            int max = cargoCount < cargoLeft ? cargoCount : cargoLeft;
            _slider = SetupSlider(min, max);
            GameObjectHelper.SetGUITextValue(_guiGameObject, "MaxCargo", max.ToString());
            Button button = GameObjectHelper.FindChild(_guiGameObject, "CargoBtn").GetComponent<Button>();
            button.onClick.AddListener(DeliverBtn);
        }

        private void DeliverBtn() {
            bool delivered = _tradeMission.AttemptDelivery((int)_slider.value);
            if (_tradeMission.CargoQuota == _tradeMission.CargoDelivered) {
                MissionComplete();
            }
            _missionGUIController.RefreshAcceptedMissions();
        }

        private void MissionComplete() {
            _tradeMission.GiveReward();
            GameObjectHelper.FindChild(_guiGameObject, "MissionCompletePanel").SetActive(true);
            GameObjectHelper.FindChild(_guiGameObject, "MainPanel").SetActive(false);
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CompleteRewardMsg", "Earned: "+ _tradeMission.RewardCredits+"Cr");
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CreditBalanceMsg", "Credits: "+ _gameController.PlayerProfile._credits+"Cr");
            _missionGUIController.SetCreditsValue();
            _gameController.PlayerProfile.Missions.Remove(_tradeMission);
            _missionGUIController.RemoveTradeMission(this);
            Destroy(_guiGameObject, 3);
            Destroy(this,3);
        }
        
        private void SetupPickupElements() {
            //turn off accepted panel
            GameObjectHelper.FindChild(_guiGameObject, "AcceptPanel").SetActive(false);
            //setup slider
            int min = 0;
            int cargoSpace = _gameController.PlayerShipController.CargoController.GetFreeCargoSpace();
            int cargoLeft = _tradeMission.CargoQuota - _tradeMission.SuppliedCargo;
            int max = cargoSpace < cargoLeft ? cargoSpace : cargoLeft;
            _slider = SetupSlider(min, max);
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CargoNum", "Pickup:");
            GameObjectHelper.SetGUITextValue(_guiGameObject, "MaxCargo", max.ToString());
            GameObject buttonObject = GameObjectHelper.FindChild(_guiGameObject, "CargoBtn"); 
            Button button = buttonObject.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(PickupCargo);
            GameObjectHelper.SetGUITextValue(buttonObject, "BtnText", "Pickup");
        }

        private void PickupCargo() {
            _tradeMission.PickupCargo((int)_slider.value);
            _missionGUIController.RefreshAcceptedMissions();
        }

        private void SetupViewOnly() {
            GameObjectHelper.FindChild(_guiGameObject, "AcceptPanel").SetActive(false);
            GameObjectHelper.FindChild(_guiGameObject, "CargoPanel").SetActive(false);
        }

        private Slider SetupSlider(int min, int max) {
            Slider slider = GameObjectHelper.FindChild(_guiGameObject, "CargoSlider").GetComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.onValueChanged.AddListener(delegate { SliderChange((int)slider.value); });
            slider.value = max;
            
            return slider;
        }

        private void SliderChange(int value) {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CargoValue", value.ToString());
        }

        private string GetMissionName() {
            return "Delivery to " + _tradeMission.Destination.SolarSystem.SystemName + " Station";
        }

        private string GetMissionDescription() {
            return "Deliver " + _tradeMission.CargoQuota + " units of " + _tradeMission.Cargo.Name + " to " + _tradeMission.Destination.SolarSystem.SystemName + " Station";
        }
    }
}