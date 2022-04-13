using System.Collections.Generic;
using System.Linq;
using Code._Cargo;
using Code._Cargo.ProductTypes.Ships;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using Code._Ships.Controllers;
using Code._Ships.ShipComponents.InternalComponents.Storage;
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

        public void StartRefuelGUI(RefuelService refuelService, StationGUIController stationGUIController) {
            _refuelService = refuelService;
            _stationGUIController = stationGUIController;
            SetupGUI();
        }

        private void SetupGUI() {
            _stationGUIController.stationGUI.SetActive(false);
            _guiGameObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_refuelService.GUIPath));
            _cargoController = GameController.CurrentShip.ShipObject.GetComponent<ShipController>().CargoController;
            SetupHomeBtn();
            SetupSlider();
            SetupPurchaseBtn();
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
            int currentFuelUnits = _cargoController.GetCargoOfType<Fuel>().Count;
            _slider.minValue = 0;
            _slider.value = currentFuelUnits;
            _slider.maxValue = _cargoController.GetFreeCargoSpace() + currentFuelUnits;
            TextMeshProUGUI fullTxt = GameObject.Find("FullValue").GetComponentInChildren<TextMeshProUGUI>();
            fullTxt.text = _slider.maxValue.ToString();
        }

        private void SliderChange() {
            int currentFuelUnits = _cargoController.GetCargoOfType<Fuel>().Count;
            int total = (int)_slider.value;
            int change = total - currentFuelUnits;
            TextMeshProUGUI totalTxt = GameObject.Find("TotalValue").GetComponentInChildren<TextMeshProUGUI>();
            totalTxt.text = total.ToString();
            TextMeshProUGUI changeTxt = GameObject.Find("ChangeValue").GetComponentInChildren<TextMeshProUGUI>();
            changeTxt.text = change <= 0 ? change.ToString() : "+" + change;
        }

        private void SetupPurchaseBtn() {
            Button purchaseBtn = GameObject.Find("PurchaseBtn").GetComponent<Button>();
            purchaseBtn.onClick.AddListener(PurchaseBtnClick);
        }

        private void PurchaseBtnClick() {
            int currentFuelUnits = _cargoController.GetCargoOfType<Fuel>().Count;
            int fuelChange = (int)_slider.value - currentFuelUnits;
            if (fuelChange > 0) {
                List<Cargo> fuel = new List<Cargo>();
                for (int i = 0; i < fuelChange; i++) {
                    fuel.Add(_refuelService.GetFuel());
                }

                bool addedFuel = _cargoController.AddCargo(fuel);
                if (addedFuel) {
                    Debug.Log("Successful purchase");
                    UpdateGUIValues();
                    SliderChange();
                }
                else {
                    Debug.Log("Failed to refuel");
                }
            }
            else if(fuelChange<0) {
                Debug.Log("Removing fuel");
                List<Cargo> fuelToSell = new List<Cargo>(); 
                fuelToSell.AddRange(_cargoController.GetCargoOfType<Fuel>(fuelChange*-1).ToList());
                _cargoController.RemoveCargo(fuelToSell);
                UpdateGUIValues();
                SliderChange();
            }
        }
    }
}