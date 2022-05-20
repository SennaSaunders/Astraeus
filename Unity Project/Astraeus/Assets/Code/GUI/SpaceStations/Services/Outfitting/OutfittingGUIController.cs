using System;
using System.Collections.Generic;
using System.Linq;
using Code._Cargo.ProductTypes.Ships;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using Code._Ships;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Ships.ShipComponents.InternalComponents.JumpDrives;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code._Ships.ShipComponents.InternalComponents.Storage;
using Code._Utility;
using Code.Camera;
using Code.GUI.ShipGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services.Outfitting {
    public class OutfittingGUIController : MonoBehaviour {
        private GameObject _previousGUI;
        private OutfittingService _outfittingService;
        private GameObject _guiGameObject, _shipObject, _thrusterSubCategories, _weaponSubCategories, _internalSubCategories, _feedbackMsg,_buyGUI;
        internal GameObject HoverInfo;
        public ShipObjectHandler shipObjectHandler;

        internal SlotSelector SelectedSlot;
        private List<SlotSelector> _mainThrusterSelectionMarkers = new List<SlotSelector>();
        private List<SlotSelector> _manThrusterSelectionMarkers = new List<SlotSelector>();
        private List<SlotSelector> _weaponSelectionMarkers = new List<SlotSelector>();
        private List<SlotSelector> _internalSelectionMarkers = new List<SlotSelector>();
        private List<GameObject> _tempDeactivatedSelectionMarkers = new List<GameObject>();
        private List<Button> _mainCategoryButtons = new List<Button>();
        private List<Button> _subCategoryButtons = new List<Button>();
        private Button _defaultThrusterButton, _defaultWeaponButton, _defaultInternalButton;
        
        public static Color BrightYellow = new Color(255 / 255f, 185 / 255f, 0);
        public static Color DarkYellow = new Color(150 / 255f, 100 / 255f, 0);
        public static Color BrightBlue = new Color(53 / 255f, 157 / 255f, 255 / 255f);
        public static Color MidBlue = new Color(20 / 255f, 100 / 255f, 150 / 255f);

        internal UnityEngine.Camera Camera;
        private OutfittingCameraController _cameraController;

        private const string OutfittingPath = "GUIPrefabs/Station/Services/Outfitting/";
        internal const string ThrusterCardPath = "ThrusterCard";
        internal const string WeaponCardPath = "WeaponCard";
        internal const string PowerPlantCardPath = "PowerPlantCard";
        internal const string ShieldCardPath = "ShieldCard";
        internal const string CargoBayCardPath = "CargoBayCard";
        internal const string JumpDriveCardPath = "JumpDriveCard";
        private const string BuyGUIPath = "BuyGUI";
        private const string BuyComponentCardPath = "BuyComponentCard";


        private const float MsgTimeout = 3;
        private float _currentFeedbackTime;
        private Ship _ship;

        private void SetShipObjectHandler() {
            shipObjectHandler = _guiGameObject.AddComponent<ShipObjectHandler>();
        }

        public void StartOutfitting(OutfittingService outfittingService, GameObject previousGUI, Ship ship) {
            CameraUtility.SolidSkybox(); //makes skybox black so the GUI looks cleaner 
            _outfittingService = outfittingService;
            _previousGUI = previousGUI;
            _previousGUI.SetActive(false);
            _ship = ship;

            SetupCamera();
            SetupGUI();
            SetCredits();
            SetCreditsChange();
            SetShipStats();
            SetComponentWarnings();

            DisplayShip();
            SetupBtns();
        }

        public void SetComponentWarnings() {
            GameObject warningsHolder = GameObjectHelper.FindChild(_guiGameObject, "Warnings");
            for (int i = warningsHolder.transform.childCount; i > 0; i--) {
                DestroyImmediate(warningsHolder.transform.GetChild(i - 1).gameObject);
            }
            //power plant
            if (_ship.ShipHull.InternalComponents.Select(mt => mt.concreteComponent).Where(mt => mt != null && mt.GetType().IsSubclassOf(typeof(PowerPlant))).ToList().Count == 0) {
                CreateWarningMsg("No Power Plant");
            }
            //cargo
            if (_ship.ShipHull.InternalComponents.Select(mt => mt.concreteComponent).Where(mt => mt != null && mt.GetType()==typeof(CargoBay)).ToList().Count == 0) {
                CreateWarningMsg("No Cargo Bay");
            }
            //main thrusters
            if (_ship.ShipHull.MainThrusterComponents.Select(mt => mt.concreteComponent).Where(mt => mt != null).ToList().Count == 0) {
                CreateWarningMsg("No Main Thruster");
            }
            //manoeuvring thrusters
            if (_ship.ShipHull.ManoeuvringThrusterComponents.concreteComponent==null) {
                CreateWarningMsg("No Manoeuvring Thruster");
            }
            //jump drive
            if (_ship.ShipHull.InternalComponents.Select(mt => mt.concreteComponent).Where(mt => mt != null && mt.GetType()==typeof(JumpDrive)).ToList().Count == 0) {
                CreateWarningMsg("No Jump Drive");
            }
        }

        private void CreateWarningMsg(string msg) {
            GameObject warningsHolder = GameObjectHelper.FindChild(_guiGameObject, "Warnings");
            GameObject newWarning = Instantiate((GameObject)Resources.Load(OutfittingPath + "Warning"), warningsHolder.transform);
            GameObjectHelper.SetGUITextValue(newWarning, "WarningMsg", msg);
            var warningsRectTransform = newWarning.GetComponent<RectTransform>(); 
            float height = warningsRectTransform.rect.height;
            float offset = -warningsHolder.transform.childCount * height;
            warningsRectTransform.localPosition = new Vector3(0, offset, 0);
        }

        public void SetShipStats() {
            SetThrusterStats();
            SetPowerStats();
            SetWeaponStats();
            SetShieldStats();
            SetJumpStats();
            SetCargoStats();
        }

        private void SetThrusterStats() {
            float mass = ShipStats.GetShipMass(_ship, false);
            //main
            //accel
            GameObjectHelper.SetGUITextValue(_guiGameObject, "MainAccelValue", (ShipStats.GetMainThrusterForce(_ship) / mass).ToString("0.0") + "m/s²");
            //power
            GameObjectHelper.SetGUITextValue(_guiGameObject, "MainPowerValue", ShipStats.GetMainThrusterPowerPerSecond(_ship).ToString("0.0") + "/s");
            
            //manoeuvring
            //accel
            GameObjectHelper.SetGUITextValue(_guiGameObject, "ManAccelValue", (ShipStats.GetManoeuvringThrusterForce(_ship) / mass).ToString("0.0") + "m/s²");
            //power
            GameObjectHelper.SetGUITextValue(_guiGameObject, "ManPowerValue", ShipStats.GetManoeuvringThrusterPowerPerSecond(_ship).ToString("0.0") + "/s");
        }

        private void SetPowerStats() {
            //capacity
            float powerCapacity = ShipStats.GetPowerCapacity(_ship);
            GameObjectHelper.SetGUITextValue(_guiGameObject, "PowerCapacityValue", powerCapacity.ToString("0.0"));
            //recharge
            GameObjectHelper.SetGUITextValue(_guiGameObject, "PowerRechargeValue", ShipStats.GetPowerRecharge(_ship).ToString("0.0"));
            //min drain time
            float powerPerSecond = ShipStats.GetMainThrusterPowerPerSecond(_ship);
            powerPerSecond += ShipStats.GetManoeuvringThrusterPowerPerSecond(_ship);
            powerPerSecond += ShipStats.GetShieldPowerPerSecond(_ship);
            powerPerSecond += ShipStats.GetWeaponPowerPerSecond(_ship);

            GameObjectHelper.SetGUITextValue(_guiGameObject, "PowerDrainMinTimeValue", powerPerSecond<=0? "N/A":(powerCapacity / powerPerSecond).ToString("0.0") + "s");
        }

        private void SetWeaponStats() {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "DPSValue", ShipStats.GetDPS(_ship).ToString("0.0"));
        }

        private void SetShieldStats() {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "ShieldCapacityValue", ShipStats.GetShieldCapacity(_ship).ToString("0.0"));
            GameObjectHelper.SetGUITextValue(_guiGameObject, "ShieldRechargeValue", ShipStats.GetShieldRechargeRate(_ship).ToString("0.0"));
            GameObjectHelper.SetGUITextValue(_guiGameObject, "ShieldRechargeEnergyValue", ShipStats.GetShieldPowerPerSecond(_ship).ToString("0.0"));
        }

        private void SetJumpStats() {
            float jumpRange = ShipStats.GetJumpRange(_ship);
            float maxEnergy = (JumpDrive.EnergyPerLY * jumpRange) / Fuel.MaxEnergy;
            //range
            GameObjectHelper.SetGUITextValue(_guiGameObject, "JumpRangeValue", jumpRange.ToString("0.0"));
            //max fuel consumption
            GameObjectHelper.SetGUITextValue(_guiGameObject, "JumpFuelValue", maxEnergy.ToString("0.0"));
        }

        private void SetCargoStats() {
            //cargo
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CapacityValue", ShipStats.GetMaxCargoSpace(_ship).ToString());
        }


        private void SetCredits() {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CreditsValue", GameController.PlayerProfile._credits.ToString());
        }

        public void SetCreditsChange() {
            int cost = 0;
            cost += GetComponentCosts(_weaponSelectionMarkers);
            cost += GetComponentCosts(_mainThrusterSelectionMarkers);
            cost += GetComponentCosts(_manThrusterSelectionMarkers);
            cost += GetComponentCosts(_internalSelectionMarkers);
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CreditChangeValue", cost.ToString());
        }

        private int GetPriceChange(ShipComponent oldComponent, ShipComponent newComponent) {
            int priceChange;
            if (oldComponent == null) {
                priceChange = newComponent.ComponentPrice;
            }
            else if (newComponent == null) {
                priceChange = -oldComponent.ComponentPrice;
            }
            else {
                priceChange = newComponent.ComponentPrice - oldComponent.ComponentPrice;
            }

            return priceChange;
        }

        private int SetupBuyComponentCard(ShipComponent oldComponent, ShipComponent newComponent, GameObject contentView) {
            GameObject buyCard = (GameObject)Instantiate(Resources.Load(OutfittingPath + BuyComponentCardPath), contentView.transform);
            GameObjectHelper.SetGUITextValue(buyCard, "OldComponentName", oldComponent == null ? "Empty" : oldComponent.ComponentName + " - " + oldComponent.ComponentSize);
            GameObjectHelper.SetGUITextValue(buyCard, "OldComponentPrice", oldComponent == null ? "N/A" : oldComponent.ComponentPrice + "Cr");
            GameObjectHelper.SetGUITextValue(buyCard, "NewComponentName", newComponent == null ? "Empty" : newComponent.ComponentName + " - " + newComponent.ComponentSize);
            GameObjectHelper.SetGUITextValue(buyCard, "NewComponentPrice", newComponent == null ? "N/A" : newComponent.ComponentPrice + "Cr");

            int priceChange = GetPriceChange(oldComponent, newComponent);
            GameObjectHelper.SetGUITextValue(buyCard, "PriceChangeValue", priceChange > 0 ? "+" + priceChange : priceChange.ToString());
            return priceChange;
        }

        private List<SlotSelector> GetAllSlotSelectors() {
            List<SlotSelector> selectionMarkers = new List<SlotSelector>();
            selectionMarkers.AddRange(_internalSelectionMarkers);
            selectionMarkers.AddRange(_mainThrusterSelectionMarkers);
            selectionMarkers.AddRange(_manThrusterSelectionMarkers);
            selectionMarkers.AddRange(_weaponSelectionMarkers);
            return selectionMarkers;
        }

        private void ResetLoadout() {
            List<SlotSelector> selectionMarkers = GetAllSlotSelectors();
            foreach (SlotSelector slotSelector in selectionMarkers) {
                slotSelector.ResetSlot();
            }
        }

        private void HideSelectionMarkers() {
            List<SlotSelector> selectionMarkers = GetAllSlotSelectors();

            foreach (SlotSelector selector in selectionMarkers) {
                if (selector.gameObject.activeSelf) {
                    _tempDeactivatedSelectionMarkers.Add(selector.gameObject);
                    selector.gameObject.SetActive(false);
                }
            }
        }

        private void ShowSelectionMarkers() {
            foreach (GameObject selectionMarker in _tempDeactivatedSelectionMarkers) {
                selectionMarker.SetActive(true);
            }
        }

        private void CloseBuyGUI() {
            ShowSelectionMarkers();
            Destroy(_buyGUI);
        }

        private void SetupBuyGUI() {
            HideSelectionMarkers();
            _buyGUI = (GameObject)Instantiate(Resources.Load(OutfittingPath + BuyGUIPath), GameObjectHelper.FindChild(_guiGameObject, "MainPanel").transform);
            GameObjectHelper.FindChild(_buyGUI, "CloseBtn").GetComponent<Button>().onClick.AddListener(CloseBuyGUI);
            GameObject contentView = GameObjectHelper.FindChild(_buyGUI, "Content");
            int priceChange = 0;
            priceChange += SetComponentBuyCards(_weaponSelectionMarkers, contentView);
            priceChange += SetComponentBuyCards(_mainThrusterSelectionMarkers, contentView);
            priceChange += SetComponentBuyCards(_manThrusterSelectionMarkers, contentView);
            priceChange += SetComponentBuyCards(_internalSelectionMarkers, contentView);

            GameObjectHelper.SetGUITextValue(_buyGUI, "CreditsValue", GameController.PlayerProfile._credits + "Cr");
            GameObjectHelper.SetGUITextValue(_buyGUI, "PriceValue", priceChange + "Cr");
            GameObjectHelper.FindChild(_buyGUI, "BuyBtn").GetComponent<Button>().onClick.AddListener(delegate { ConfirmPurchase(priceChange); });
        }

        private void ConfirmPurchase(int price) {
            if (GameController.PlayerProfile.AddCredits(-price)) {
                MsgActive("Purchase confirmed",Color.green);
                Purchased();
                CloseBuyGUI();
                SetCredits();
                SetCreditsChange();
                SetComponentWarnings();
            }
            else {
                MsgActive("Insufficient credits",Color.red);
            }
        }

        private void Purchased() {
            SetSlotInitialToCurrent(_mainThrusterSelectionMarkers);
            SetSlotInitialToCurrent(_manThrusterSelectionMarkers);
            SetSlotInitialToCurrent(_weaponSelectionMarkers);
            SetSlotInitialToCurrent(_internalSelectionMarkers);
        }

        private void SetSlotInitialToCurrent(List<SlotSelector> slotSelectors) {
            foreach (SlotSelector slotSelector in slotSelectors) {
                slotSelector.InitialShipComponent = slotSelector.CurrentShipComponent;
            }
        }

        private int GetComponentCosts(List<SlotSelector> selectionMarkers) {
            int priceChange = 0;
            foreach (SlotSelector selectionMarker in selectionMarkers) {
                if (selectionMarker.InitialShipComponent != null && selectionMarker.CurrentShipComponent != null) {
                    if (selectionMarker.InitialShipComponent.GetType() != selectionMarker.CurrentShipComponent.GetType() || selectionMarker.InitialShipComponent.ComponentSize != selectionMarker.CurrentShipComponent.ComponentSize) {
                        priceChange += GetPriceChange(selectionMarker.InitialShipComponent, selectionMarker.CurrentShipComponent);
                    }
                }
                else if (selectionMarker.InitialShipComponent != null || selectionMarker.CurrentShipComponent != null) {
                    priceChange += GetPriceChange(selectionMarker.InitialShipComponent, selectionMarker.CurrentShipComponent);
                }
            }

            return priceChange;
        }

        private int SetComponentBuyCards(List<SlotSelector> selectionMarkers, GameObject contentView) {
            int priceChange = 0;
            foreach (SlotSelector selectionMarker in selectionMarkers) {
                if (selectionMarker.InitialShipComponent != null && selectionMarker.CurrentShipComponent != null) {
                    if (selectionMarker.InitialShipComponent.GetType() != selectionMarker.CurrentShipComponent.GetType() || selectionMarker.InitialShipComponent.ComponentSize != selectionMarker.CurrentShipComponent.ComponentSize) {
                        priceChange += SetupBuyComponentCard(selectionMarker.InitialShipComponent, selectionMarker.CurrentShipComponent, contentView);
                    }
                }
                else if (selectionMarker.InitialShipComponent != null || selectionMarker.CurrentShipComponent != null) {
                    priceChange += SetupBuyComponentCard(selectionMarker.InitialShipComponent, selectionMarker.CurrentShipComponent, contentView);
                }
            }

            return priceChange;
        }

        private void Update() {
            FeedbackMsgCheck();
        }

        private void FeedbackMsgCheck() {
            if (_feedbackMsg.activeSelf) {
                _currentFeedbackTime -= Time.deltaTime;
                if (_currentFeedbackTime <= 0) {
                    _feedbackMsg.SetActive(false);
                }
            }
        }

        private void MsgActive(string msg, Color colour) {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "ErrorMsg", msg, colour);
            _currentFeedbackTime = MsgTimeout;
            _feedbackMsg.SetActive(true);
        }

        private void SuccessMsg() {
            MsgActive("Component assigned successfully", Color.green);
        }

        private void UnselectedSlotError() {
            MsgActive("Select a slot on the ship first", Color.red);
        }

        private void SlotSizeError() {
            MsgActive("Component is too big for the selected slot",Color.red);
        }

        public void CargoBayFilledError() {
            MsgActive("Remove fuel/cargo before changing",Color.red);
        }

        private void SetupCamera() {
            Camera = UnityEngine.Camera.main;
            if (Camera != null) {
                _cameraController = Camera.gameObject.AddComponent<OutfittingCameraController>();
                _cameraController.TakeCameraControl();
                _cameraController.SetCameraPos();
                Camera.farClipPlane = 2500;
            }
        }

        private void SetupExitBtn() {
            Button exitBtn = GameObject.Find("ExitBtn").GetComponent<Button>();
            exitBtn.onClick.AddListener(Exit);
        }

        private void SetupColourBtn() {
            Button colourBtn = GameObject.Find("ColourBtn").GetComponent<Button>();
            colourBtn.onClick.AddListener(ColourBtnClick);
        }

        private void ColourBtnClick() {
            ShipColourGUIController shipColourGUIController = gameObject.AddComponent<ShipColourGUIController>();
            shipColourGUIController.SetupGUI(shipObjectHandler, _guiGameObject, _shipObject);
        }

        private bool ShipLoadoutConfirmed() {
            List<SlotSelector> selectionMarkers = GetAllSlotSelectors();
            foreach (SlotSelector selectionMarker in selectionMarkers) {
                if ((selectionMarker.InitialShipComponent == null && selectionMarker.CurrentShipComponent == null)) { //if both unset then true
                    return true;
                }

                if (selectionMarker.InitialShipComponent == null || selectionMarker.CurrentShipComponent == null) { //if one unset return false
                    return false;
                } //otherwise check component & if not the same type and size return false

                if (selectionMarker.InitialShipComponent.GetType() != selectionMarker.CurrentShipComponent.GetType() || selectionMarker.InitialShipComponent.ComponentSize != selectionMarker.CurrentShipComponent.ComponentSize) {
                    return false;
                }
            }

            return true;
        }

        private void Exit() {
            if (ShipLoadoutConfirmed()) {
                _previousGUI.SetActive(true);
                Destroy(_cameraController);
                Destroy(_guiGameObject);
                Destroy(this);
            }
            else {
                MsgActive("Need to confirm loadout",Color.red);
                if (_buyGUI == null) {
                    SetupBuyGUI();
                }
            }
        }

        private void DisplayShip() {
            shipObjectHandler.ManagedShip = _ship;
            _shipObject = shipObjectHandler.CreateShip(GameObject.Find("ShipPanel").transform, Color.black);
            _shipObject.transform.position = shipObjectHandler.ManagedShip.ShipHull.OutfittingPosition;
            _shipObject.transform.rotation = shipObjectHandler.ManagedShip.ShipHull.OutfittingRotation;
            AddDraggableToShipPanel();
        }

        private void AddDraggableToShipPanel() {
            GameObject shipPanel = GameObject.Find("ShipPanel");
            ShipRotatable shipRotatable = shipPanel.AddComponent<ShipRotatable>();
            shipRotatable.RotatableObject = shipObjectHandler.ManagedShip.ShipObject;
        }

        private void SetupGUI() {
            _guiGameObject = Instantiate((GameObject)Resources.Load(_outfittingService.GUIPath));
            SetShipObjectHandler();
            _feedbackMsg = GameObjectHelper.FindChild(_guiGameObject, "ErrorMsg");
            HoverInfo = GameObjectHelper.FindChild(_guiGameObject, "HoverInfo");
        }

        private Transform GetScrollViewContentContainer() {
            return GameObject.Find("ComponentContent").transform;
        }

        private void SetupBtns() {
            SetupComponentBtns();
            SetupExitBtn();
            SetupColourBtn();
            SetupBuyBtn();
            SetupResetBtn();
        }

        private void SetupResetBtn() {
            GameObjectHelper.FindChild(_guiGameObject, "ResetBtn").GetComponent<Button>().onClick.AddListener(ResetLoadout);
        }

        private void SetupBuyBtn() {
            GameObjectHelper.FindChild(_guiGameObject, "BuyBtn").GetComponent<Button>().onClick.AddListener(SetupBuyGUI);
        }

        private void SetupComponentBtns() {
            //sub categories
            _thrusterSubCategories = GameObjectHelper.FindChild(_guiGameObject, "ThrusterSubCategories");

            Button mainThrusterBtn = GameObjectHelper.FindChild(_thrusterSubCategories, "MainThrustersBtn").GetComponent<Button>();
            _defaultThrusterButton = mainThrusterBtn;
            mainThrusterBtn.onClick.AddListener(delegate { MainThrusterBtnClick(mainThrusterBtn); });
            _subCategoryButtons.Add(mainThrusterBtn);

            Button manoeuvringBtn = GameObjectHelper.FindChild(_thrusterSubCategories, "ManoeuvringThrustersBtn").GetComponent<Button>();
            manoeuvringBtn.onClick.AddListener(delegate { ManoeuvringThrusterBtnClick(manoeuvringBtn); });
            _subCategoryButtons.Add(manoeuvringBtn);

            _weaponSubCategories = GameObjectHelper.FindChild(_guiGameObject, "WeaponSubCategories");
            Button ballisticBtn = GameObjectHelper.FindChild(_weaponSubCategories, "BallisticBtn").GetComponent<Button>();
            _defaultWeaponButton = ballisticBtn;
            ballisticBtn.onClick.AddListener(delegate { BallisticBtnClick(ballisticBtn); });
            _subCategoryButtons.Add(ballisticBtn);
            
            Button laserBtn = GameObjectHelper.FindChild(_weaponSubCategories, "LaserBtn").GetComponent<Button>();
            laserBtn.onClick.AddListener(delegate { LaserBtnClick(laserBtn); });
            _subCategoryButtons.Add(laserBtn);

            Button railgunBtn = GameObjectHelper.FindChild(_weaponSubCategories, "RailgunBtn").GetComponent<Button>();
            railgunBtn.onClick.AddListener(delegate { RailgunBtnClick(railgunBtn); });
            _subCategoryButtons.Add(railgunBtn);

            _internalSubCategories = GameObjectHelper.FindChild(_guiGameObject, "InternalSubCategories");
            Button powerPlantButton = GameObjectHelper.FindChild(_internalSubCategories, "PowerPlantBtn").GetComponent<Button>();
            _defaultInternalButton = powerPlantButton;
            powerPlantButton.onClick.AddListener(delegate { PowerPlantBtnClick(powerPlantButton); });
            _subCategoryButtons.Add(powerPlantButton);

            Button shieldsBtn = GameObjectHelper.FindChild(_internalSubCategories, "ShieldsBtn").GetComponent<Button>();
            shieldsBtn.onClick.AddListener(delegate { ShieldsBtnClick(shieldsBtn); });
            _subCategoryButtons.Add(shieldsBtn);

            Button cargoBtn = GameObjectHelper.FindChild(_internalSubCategories, "CargoBtn").GetComponent<Button>();
            cargoBtn.onClick.AddListener(delegate { CargoBayBtnClick(cargoBtn); });
            _subCategoryButtons.Add(cargoBtn);

            Button jumpDriveBtn = GameObjectHelper.FindChild(_internalSubCategories, "JumpDriveBtn").GetComponent<Button>();
            jumpDriveBtn.onClick.AddListener(delegate { JumpDriveBtnCLick(jumpDriveBtn); });
            _subCategoryButtons.Add(jumpDriveBtn);
            
            
            // main categories
            Button thrustersBtn = GameObjectHelper.FindChild(_guiGameObject, "ThrustersBtn").GetComponentInChildren<Button>();
            thrustersBtn.onClick.AddListener(delegate { ThrustersBtnClick(thrustersBtn); });
            _mainCategoryButtons.Add(thrustersBtn);

            Button weaponsBtn = GameObjectHelper.FindChild(_guiGameObject, "WeaponsBtn").GetComponentInChildren<Button>();
            weaponsBtn.onClick.AddListener(delegate { WeaponBtnClick(weaponsBtn); });
            _mainCategoryButtons.Add(weaponsBtn);

            Button internalBtn = GameObjectHelper.FindChild(_guiGameObject, "InternalBtn").GetComponentInChildren<Button>();
            internalBtn.onClick.AddListener(delegate { InternalBtnClick(internalBtn); });
            _mainCategoryButtons.Add(internalBtn);
            ThrustersBtnClick(thrustersBtn);
        }

        private void SetBtnColours(List<Button> buttons, Button clickedButton) {
            foreach (Button button in buttons) {
                ColorBlock buttonColors = button.colors;
                buttonColors.normalColor = BrightYellow;
                buttonColors.selectedColor = BrightYellow;
                buttonColors.highlightedColor = DarkYellow;
                buttonColors.pressedColor = DarkYellow;
                button.colors = buttonColors;
            }

            ColorBlock clickedButtonColors = clickedButton.colors;
            clickedButtonColors.normalColor = BrightBlue;
            clickedButtonColors.selectedColor = BrightBlue;
            clickedButtonColors.highlightedColor = MidBlue;
            clickedButtonColors.pressedColor = MidBlue;
            clickedButton.colors = clickedButtonColors;
        }

        // main categories
        private void ThrustersBtnClick(Button button) {
            _thrusterSubCategories.SetActive(true);
            _weaponSubCategories.SetActive(false);
            _internalSubCategories.SetActive(false);
            MainThrusterBtnClick(_defaultThrusterButton);
            SelectionMarkersSetActive(_internalSelectionMarkers, false);
            SelectionMarkersSetActive(_weaponSelectionMarkers, false);
            SetBtnColours(_mainCategoryButtons, button);
        }

        private void WeaponBtnClick(Button button) {
            _thrusterSubCategories.SetActive(false);
            _weaponSubCategories.SetActive(true);
            _internalSubCategories.SetActive(false);
            BallisticBtnClick(_defaultWeaponButton);
            SelectionMarkersSetActive(_internalSelectionMarkers, false);
            SelectionMarkersSetActive(_weaponSelectionMarkers, true);
            SelectionMarkersSetActive(_mainThrusterSelectionMarkers, false);
            SelectionMarkersSetActive(_manThrusterSelectionMarkers, false);
            SetBtnColours(_mainCategoryButtons, button);
        }

        private void InternalBtnClick(Button button) {
            _thrusterSubCategories.SetActive(false);
            _weaponSubCategories.SetActive(false);
            _internalSubCategories.SetActive(true);
            PowerPlantBtnClick(_defaultInternalButton);
            SelectionMarkersSetActive(_internalSelectionMarkers, true);
            SelectionMarkersSetActive(_weaponSelectionMarkers, false);
            SelectionMarkersSetActive(_mainThrusterSelectionMarkers, false);
            SelectionMarkersSetActive(_manThrusterSelectionMarkers, false);
            SetBtnColours(_mainCategoryButtons, button);
        }

        //sub categories
        private void MainThrusterBtnClick(Button button) {
            _mainThrusterSelectionMarkers = SubCatClick(DisplayMainThrusterComponents, shipObjectHandler.MainThrusterComponents, _mainThrusterSelectionMarkers);
            SelectionMarkersSetActive(_mainThrusterSelectionMarkers, true);
            SelectionMarkersSetActive(_manThrusterSelectionMarkers, false);
            SetBtnColours(_subCategoryButtons, button);
        }

        private void ManoeuvringThrusterBtnClick(Button button) {
            // Manoeuvring thrusters have a different type slot tuple so this method can't use sub cat click 
            ClearScrollView();
            DisplayManoeuvringThrusterComponents();
            var manoeuvringThrusterComponents = shipObjectHandler.ManoeuvringThrusterComponents;
            var slots = new List<(Transform, Transform, ShipComponentTier, ShipComponentType, ManoeuvringThruster)>() { (manoeuvringThrusterComponents.selectionTransform, manoeuvringThrusterComponents.selectionTransform, manoeuvringThrusterComponents.maxSize, manoeuvringThrusterComponents.componentType, manoeuvringThrusterComponents.thruster) };
            _manThrusterSelectionMarkers = GetSelectionMarkers(slots, _manThrusterSelectionMarkers);
            SelectionMarkersSetActive(_mainThrusterSelectionMarkers, false);
            SelectionMarkersSetActive(_manThrusterSelectionMarkers, true);
            SetBtnColours(_subCategoryButtons, button);
        }

        private void BallisticBtnClick(Button button) {
            _weaponSelectionMarkers = SubCatClick(DisplayBallisticComponents, shipObjectHandler.WeaponComponents, _weaponSelectionMarkers);
            SetBtnColours(_subCategoryButtons, button);
        }

        private void LaserBtnClick(Button button) {
            _weaponSelectionMarkers = SubCatClick(DisplayLaserComponents, shipObjectHandler.WeaponComponents, _weaponSelectionMarkers);
            SetBtnColours(_subCategoryButtons, button);
        }

        private void RailgunBtnClick(Button button) {
            _weaponSelectionMarkers = SubCatClick(DisplayRailgunComponents, shipObjectHandler.WeaponComponents, _weaponSelectionMarkers);
            SetBtnColours(_subCategoryButtons, button);
        }

        private void PowerPlantBtnClick(Button button) {
            _internalSelectionMarkers = SubCatClick(DisplayPowerPlantComponents, shipObjectHandler.InternalComponents, _internalSelectionMarkers);
            SetBtnColours(_subCategoryButtons, button);
        }

        private void ShieldsBtnClick(Button button) {
            _internalSelectionMarkers = SubCatClick(DisplayShieldsComponents, shipObjectHandler.InternalComponents, _internalSelectionMarkers);
            SetBtnColours(_subCategoryButtons, button);
        }

        private void JumpDriveBtnCLick(Button button) {
            _internalSelectionMarkers = SubCatClick(DisplayJumpDriveComponents, shipObjectHandler.InternalComponents, _internalSelectionMarkers);
            SetBtnColours(_subCategoryButtons, button);
        }

        private void CargoBayBtnClick(Button button) {
            _internalSelectionMarkers = SubCatClick(DisplayCargoBayComponents, shipObjectHandler.InternalComponents, _internalSelectionMarkers);
            SetBtnColours(_subCategoryButtons, button);
        }

        private List<SlotSelector> SubCatClick<T>(Action displayFunction, List<(Transform mountTransform, Transform selectionTransform, ShipComponentTier slotSize, ShipComponentType componentType, T component)> slots, List<SlotSelector> selectionMarkers) where T : ShipComponent {
            ClearScrollView();
            displayFunction.Invoke();
            return GetSelectionMarkers(slots, selectionMarkers);
        }

        //display methods
        private void DisplayComponents<T>(Action<(Type type, ShipComponentTier)> displayFunc) where T : ShipComponent {
            var components = _outfittingService.GetComponentsOfType(typeof(T));
            //add tier in here
            foreach (var component in components) {
                displayFunc(component);
            }
        }

        private void DisplayBallisticComponents() {
            DisplayComponents<BallisticCannon>(DisplayWeaponCard);
        }

        private void DisplayLaserComponents() {
            DisplayComponents<Laser>(DisplayWeaponCard);
        }

        private void DisplayRailgunComponents() {
            DisplayComponents<Railgun>(DisplayWeaponCard);
        }

        private void DisplayManoeuvringThrusterComponents() {
            DisplayComponents<ManoeuvringThruster>(DisplayThrusterCard);
        }

        private void DisplayMainThrusterComponents() {
            DisplayComponents<MainThruster>(DisplayThrusterCard);
        }

        private void DisplayShieldsComponents() {
            DisplayComponents<Shield>(DisplayShieldCard);
        }

        private void DisplayCargoBayComponents() {
            DisplayComponents<CargoBay>(DisplayCargoBay);
        }

        private void DisplayPowerPlantComponents() {
            DisplayComponents<PowerPlant>(DisplayPowerPlant);
        }

        private void DisplayJumpDriveComponents() {
            DisplayComponents<JumpDrive>(DisplayJumpDrive);
        }

        private (Transform, T) DisplayShipComponentCard<T>(Type componentType, ShipComponentTier tier, string cardPath) where T : ShipComponent {
            T shipComponent = (T)Activator.CreateInstance(componentType, tier);
            GameObject cardObject = CreateComponentCardModifier(cardPath, componentType, tier);
            Transform cardComponents = cardObject.transform.Find("Details");
            return (cardComponents, shipComponent);
        }

        private void DisplayThrusterCard((Type thrusterType, ShipComponentTier tier) thruster) {
            (Transform cardComponents, Thruster thrusterInstance) = DisplayShipComponentCard<Thruster>(thruster.thrusterType, thruster.tier, ThrusterCardPath);
            SetupThrusterCard(cardComponents, thrusterInstance);
        }

        internal void SetupThrusterCard(Transform cardComponents, Thruster thrusterInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", thrusterInstance.ComponentName + " - " + thrusterInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Force", thrusterInstance.Force + " N");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "PowerDraw", thrusterInstance.PowerDraw + " GW/s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "PowerDraw", thrusterInstance.ComponentMass / 1000 + " T");
        }

        private void DisplayWeaponCard((Type weaponType, ShipComponentTier tier) weapon) {
            (Transform cardComponents, Weapon weaponInstance) = DisplayShipComponentCard<Weapon>(weapon.weaponType, weapon.tier, WeaponCardPath);
            SetupWeaponCard(cardComponents, weaponInstance);
        }

        internal void SetupWeaponCard(Transform cardComponents, Weapon weaponInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", weaponInstance.ComponentName + " - " + weaponInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Damage", weaponInstance.Damage + " DMG");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "RoF", (60 / weaponInstance.FireDelay).ToString("0.0") + " RPM");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Mass", weaponInstance.ComponentMass / 1000 + " T");
        }

        private void DisplayPowerPlant((Type powerPlantType, ShipComponentTier tier) powerPlant) {
            (Transform cardComponents, PowerPlant powerPlantInstance) = DisplayShipComponentCard<PowerPlant>(powerPlant.powerPlantType, powerPlant.tier, PowerPlantCardPath);
            SetupPowerPlantCard(cardComponents, powerPlantInstance);
        }

        private void DisplayJumpDrive((Type jumpDriveType, ShipComponentTier tier) jumpDrive) {
            (Transform cardComponents, JumpDrive jumpDriveInstance) = DisplayShipComponentCard<JumpDrive>(jumpDrive.jumpDriveType, jumpDrive.tier, JumpDriveCardPath);
            SetupJumpDriveCard(cardComponents, jumpDriveInstance);
        }

        internal void SetupPowerPlantCard(Transform cardComponents, PowerPlant powerPlantInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", powerPlantInstance.ComponentName + " - " + powerPlantInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Capacity", powerPlantInstance.EnergyCapacity + " GW");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Recharge", powerPlantInstance.RechargeRate * 60 + " GW/s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Mass", powerPlantInstance.ComponentMass / 1000 + " T");
        }

        private void DisplayCargoBay((Type cargoBayType, ShipComponentTier tier) cargoBay) {
            (Transform cardComponents, CargoBay cargoBayInstance) = DisplayShipComponentCard<CargoBay>(cargoBay.cargoBayType, cargoBay.tier, CargoBayCardPath);
            SetupCargoBayCard(cardComponents, cargoBayInstance);
        }

        internal void SetupCargoBayCard(Transform cardComponents, CargoBay cargoBayInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", cargoBayInstance.ComponentName + " - " + cargoBayInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Capacity", cargoBayInstance.CargoVolume + " Units");
        }

        private void DisplayShieldCard((Type shieldType, ShipComponentTier tier) shield) {
            (Transform cardComponents, Shield shieldInstance) = DisplayShipComponentCard<Shield>(shield.shieldType, shield.tier, ShieldCardPath);
            SetupShieldCard(cardComponents, shieldInstance);
        }

        internal void SetupShieldCard(Transform cardComponents, Shield shieldInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", shieldInstance.ComponentName + " - " + shieldInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Strength", shieldInstance.StrengthCapacity + "GW");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Recharge", shieldInstance.RechargeRate + "GW/s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "DamageDelay", shieldInstance.DamageRecoveryTime + "s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "DepletionDelay", shieldInstance.DepletionRecoveryTime + "s");
        }

        internal void SetupJumpDriveCard(Transform cardComponents, JumpDrive jumpDriveInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", jumpDriveInstance.ComponentName + " - " + jumpDriveInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Range", jumpDriveInstance.JumpRange + "LY");
        }

        private void SelectionMarkersSetActive(List<SlotSelector> selectionMarkers, bool active) {
            foreach (SlotSelector selectionMarker in selectionMarkers) {
                selectionMarker.gameObject.SetActive(active);
            }
        }

        private List<SlotSelector> GetSelectionMarkers<T>(List<(Transform mountTransform, Transform selectionTransform, ShipComponentTier slotSize, ShipComponentType componentType, T component)> shipComponents, List<SlotSelector> selectionMarkers) where T : ShipComponent {
            if (selectionMarkers == null || selectionMarkers.Count == 0) {
                selectionMarkers = new List<SlotSelector>();

                foreach ((Transform mountTransform, Transform selectionTransform, ShipComponentTier slotSize, ShipComponentType componentType, T component) selection in shipComponents) {
                    Transform canvas = _guiGameObject.transform.Find("Canvas");
                    GameObject marker = Instantiate((GameObject)Resources.Load(OutfittingPath + "ShipComponentMarker"), canvas.Find("SelectionMarkers"));
                    // GameObject markerText = Instantiate((GameObject)Resources.Load(OutfittingPath + "ShipComponentMarkerName"), marker.transform);
                    GameObjectHelper.SetGUITextValue(marker, "SlotTier", selection.slotSize.ToString());
                    SlotSelector slotSelector = marker.AddComponent<SlotSelector>();
                    slotSelector.Setup(selection.mountTransform, selection.selectionTransform, selection.componentType, this, selection.component);
                    selectionMarkers.Add(slotSelector);
                }
            }

            return selectionMarkers;
        }

        private void ClearScrollView() {
            Transform container = GetScrollViewContentContainer();

            for (int i = container.childCount; i > 0; i--) {
                Destroy(container.GetChild(i - 1).gameObject);
            }
        }

        public GameObject CreateComponentCard(string cardSpecifier, Transform parent) {
            return Instantiate((GameObject)Resources.Load(OutfittingPath + cardSpecifier), parent);
        }

        private GameObject CreateComponentCardModifier(string cardSpecifier, Type componentType, ShipComponentTier tier) {
            GameObject componentCard = CreateComponentCard(cardSpecifier, GetScrollViewContentContainer());

            CardShipComponentModifier componentModifier = componentCard.AddComponent<CardShipComponentModifier>();

            componentModifier.Setup(componentType, tier, this);
            return componentCard;
        }

        private void ChangeSelectedSlotComponent(ShipComponent newComponent) {
            if (SelectedSlot != null && SelectedSlot._componentType == newComponent.ComponentType) {
                SlotSelector.AssignmentStatus success = SelectedSlot.ChangeComponent(newComponent);
                if (success == SlotSelector.AssignmentStatus.SlotSizeError) {
                    SlotSizeError();
                }
                else if (success == SlotSelector.AssignmentStatus.CargoFilledError) {
                    CargoBayFilledError();
                }
                else {
                    SuccessMsg();
                    SelectedSlot.CurrentShipComponent = newComponent;
                    SelectedSlot.ChangeSlotText();
                    SetCreditsChange();
                    SetComponentWarnings();
                    SetShipStats();
                }
            }
            else {
                UnselectedSlotError();
            }
        }

        class CardShipComponentModifier : MonoBehaviour, IPointerClickHandler {
            private Type _componentType;
            private ShipComponentTier _tier;
            private OutfittingGUIController _outfittingGUIController;

            public void Setup(Type componentType, ShipComponentTier tier, OutfittingGUIController outfittingGUIController) {
                _componentType = componentType;
                _tier = tier;
                _outfittingGUIController = outfittingGUIController;
            }

            public void OnPointerClick(PointerEventData eventData) {
                ShipComponent shipComponent = (ShipComponent)Activator.CreateInstance(_componentType, _tier);
                Debug.Log("Trying to assign: " + shipComponent.ComponentName + " - " + shipComponent.ComponentSize);
                if (shipComponent.GetType().IsSubclassOf(typeof(Weapon))) {
                    _outfittingGUIController.ChangeSelectedSlotComponent((Weapon)shipComponent);
                }
                else if (shipComponent.GetType().IsSubclassOf(typeof(MainThruster))) {
                    _outfittingGUIController.ChangeSelectedSlotComponent((MainThruster)shipComponent);
                }
                else if (shipComponent.GetType() == typeof(ManoeuvringThruster)) {
                    _outfittingGUIController.ChangeSelectedSlotComponent((ManoeuvringThruster)shipComponent);
                }
                else if (shipComponent.GetType().IsSubclassOf(typeof(InternalComponent))) {
                    _outfittingGUIController.ChangeSelectedSlotComponent((InternalComponent)shipComponent);
                }
            }
        }
    }
}