using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using Code._Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class RepairGUIController : MonoBehaviour {
        private StationGUIController _stationGUIController;
        private RepairService _repairService;
        private GameObject _guiGameObject;
        private const float ErrorTime = 3;
        private const int PricePerUnit = 5;
        private float _currentErrorTime;

        private void Update() {
            if (_currentErrorTime > 0) {
                _currentErrorTime -= Time.deltaTime;
                if (_currentErrorTime <= 0) {
                    SetFeedbackMsg("", Color.black);
                }
            }
        }

        public void StartRepairGUI(RepairService repairService, StationGUIController stationGUIController) {
            _repairService = repairService;
            _stationGUIController = stationGUIController;
            SetupGUI();
        }

        private void SetupGUI() {
            _stationGUIController.stationGUI.SetActive(false);
            _guiGameObject = Instantiate((GameObject)Resources.Load(_repairService.GUIPath));
            SetupBtn("ExitBtn", Exit);
            SetupBtn("RepairBtn", RepairBtn);
            SetPricePerUnitText();
            Refresh();
        }

        private void SetupBtn(string buttonName, UnityAction function) {
            Button button = GameObjectHelper.FindChild(_guiGameObject, buttonName).GetComponent<Button>();
            button.onClick.AddListener(function);
        }

        private void Exit() {
            _stationGUIController.stationGUI.SetActive(true);
            Destroy(_guiGameObject);
            Destroy(this);
        }

        private void SetPricePerUnitText() {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "PricePerUnit", PricePerUnit.ToString()+"Cr/Unit");
        }

        private (float current, float max) GetHealthValues() {
            float currentHealth = GameController.CurrentShip.ShipHull.CurrentHullStrength;
            float maxHealth = GameController.CurrentShip.ShipHull.BaseHullStrength;
            return (currentHealth, maxHealth);
        }

        private void Refresh() {
            SetHealthText();
            SetRepairCostValueText();
            SetCredits();
        }

        private void SetCredits() {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CreditsValue", GameController.PlayerProfile._credits + "Cr");
        }

        private void SetHealthText() {
            (float current, float max) health = GetHealthValues();
            GameObjectHelper.SetGUITextValue(_guiGameObject, "HealthValue", health.current + "/" + health.max);
        }

        private int GetRepairCost() {
            (float current, float max) health = GetHealthValues();
            return (int)((health.max - health.current) * PricePerUnit);
        }

        private void SetRepairCostValueText() {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "PriceValue", GetRepairCost() + "Cr");
        }


        private void RepairBtn() {
            (float current, float max) health = GetHealthValues();
            if (health.current < health.max) {
                if (GameController.PlayerProfile.AddCredits(GetRepairCost())) {
                    GameController.CurrentShip.ShipHull.CurrentHullStrength = GameController.CurrentShip.ShipHull.BaseHullStrength;
                    GameController.CurrentShip.ShipHull.NotifyObservers();
                    SetFeedbackMsg("Ship Repaired", Color.green);
                    Refresh();
                }
                else {
                    SetFeedbackMsg("Not enough credits", Color.red);
                }
            }
            else {
                SetFeedbackMsg("Ship does not need repairs", Color.red);
            }
        }

        private void SetFeedbackMsg(string message, Color colour) {
            _currentErrorTime = ErrorTime;
            GameObjectHelper.SetGUITextValue(_guiGameObject, "Msg", message, colour);
        }
    }
}