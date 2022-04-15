using System.Collections.Generic;
using System.Linq;
using Code._Cargo;
using Code._Cargo.ProductTypes.Ships;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using Code._Ships.Controllers;
using Code._Ships.ShipComponents.InternalComponents.Storage;
using Code._Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class RefuelGUIController : MonoBehaviour {
        private RefuelService _refuelService;
        private StationGUIController _stationGUIController;
        private GameObject _guiGameObject;
        private CargoController _cargoController;
        private Slider _slider;

        private GameObject notEnoughCreditsMsg;
        private float creditMsgTime = 3;
        private float creditMsgCountdown = 0;

        public void StartRefuelGUI(RefuelService refuelService, StationGUIController stationGUIController) {
            _refuelService = refuelService;
            _stationGUIController = stationGUIController;
            SetupGUI();
        }
        
        private void Update() {
            if (notEnoughCreditsMsg != null) {
                if (notEnoughCreditsMsg.activeSelf) {
                    creditMsgCountdown -= Time.deltaTime;
                    if (creditMsgCountdown <= 0) {
                        notEnoughCreditsMsg.SetActive(false);
                    }
                }
            }
        }

        private void SetupGUI() {
            _stationGUIController.stationGUI.SetActive(false);
            _guiGameObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_refuelService.GUIPath));
            _cargoController = GameController.CurrentShip.ShipObject.GetComponent<ShipController>().CargoController;
            notEnoughCreditsMsg = GameObjectHelper.FindChild(_guiGameObject, "NotEnoughCredits");
            notEnoughCreditsMsg.SetActive(false);
            SetupHomeBtn();
            SetupSlider();
            SetupPurchaseBtn();
            UpdateCredits();
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
            _slider = GameObject.Find("FuelSlider").GetComponent<Slider>();
            _slider.onValueChanged.AddListener(delegate { SliderChange(); });
            UpdateGUIValues();
            SliderChange();
        }

        private void UpdateGUIValues() {
            int currentFuelUnits = _cargoController.GetCargoOfType(typeof(Fuel)).Count;
            _slider.minValue = 0;
            _slider.value = currentFuelUnits;
            _slider.maxValue = _cargoController.GetFreeCargoSpace() + currentFuelUnits;
            TextMeshProUGUI fullTxt = GameObject.Find("FullValue").GetComponentInChildren<TextMeshProUGUI>();
            fullTxt.text = _slider.maxValue.ToString();
        }

        private void SliderChange() {
            int currentFuelUnits = _cargoController.GetCargoOfType(typeof(Fuel)).Count;
            int total = (int)_slider.value;
            int change = total - currentFuelUnits;
            TextMeshProUGUI totalTxt = GameObject.Find("TotalValue").GetComponentInChildren<TextMeshProUGUI>();
            totalTxt.text = total.ToString();
            TextMeshProUGUI changeTxt = GameObject.Find("ChangeValue").GetComponentInChildren<TextMeshProUGUI>();
            changeTxt.text = change <= 0 ? change.ToString() : "+" + change;
            UpdateCredits();
        }

        private void UpdateCredits() {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CreditsCurrentValue", GameController.PlayerProfile._credits.ToString());
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CreditsChangeValue", GetChangeInFunds().ToString());
        }

        private int GetChangeInFunds() {
            int currentFuelUnits = _cargoController.GetCargoOfType(typeof(Fuel)).Count;
            int total = (int)_slider.value;
            int change = total - currentFuelUnits;
            return -change * Fuel.fuelPrice;
        }

        private void SetupPurchaseBtn() {
            Button purchaseBtn = GameObject.Find("PurchaseBtn").GetComponent<Button>();
            purchaseBtn.onClick.AddListener(PurchaseBtnClick);
        }

        private void PurchaseBtnClick() {
            int currentFuelUnits = _cargoController.GetCargoOfType(typeof(Fuel)).Count;
            int fuelChange = (int)_slider.value - currentFuelUnits;
            if (GameController.PlayerProfile.ChangeCredits(GetChangeInFunds())) {
                if (fuelChange > 0) {
                    List<Cargo> fuel = new List<Cargo>();
                    for (int i = 0; i < fuelChange; i++) {
                        fuel.Add(_refuelService.GetFuel());
                    }

                    _cargoController.AddCargo(fuel);

                    UpdateGUIValues();
                    SliderChange();
                }
                else if (fuelChange < 0) {
                    Debug.Log("Removing fuel");
                    List<Cargo> fuelToSell = new List<Cargo>();
                    fuelToSell.AddRange(_cargoController.GetCargoOfType(typeof(Fuel), fuelChange * -1).ToList());
                    _cargoController.RemoveCargo(fuelToSell);
                    UpdateGUIValues();
                    SliderChange();
                }
            }
            else {
                NotEnoughCredits();
            }
        }
        private void NotEnoughCredits() {
            notEnoughCreditsMsg.SetActive(true);
            creditMsgCountdown = creditMsgTime;
        }
    }
}