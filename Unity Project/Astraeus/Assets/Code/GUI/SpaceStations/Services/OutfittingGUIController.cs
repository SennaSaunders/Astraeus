using System;
using System.Collections.Generic;
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
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class OutfittingGUIController : MonoBehaviour {
        private GameObject _previousGUI;
        private OutfittingService _outfittingService;
        private GameObject _guiGameObject, _shipObject, _thrusterSubCategories, _weaponSubCategories, _internalSubCategories, _errorMsg, _hoverInfo, _buyGUI;
        private ShipObjectHandler _shipObjectHandler;

        private SlotSelector _selectedSlot;
        private List<SlotSelector> _mainThrusterSelectionMarkers = new List<SlotSelector>();
        private List<SlotSelector> _manThrusterSelectionMarkers = new List<SlotSelector>();
        private List<SlotSelector> _weaponSelectionMarkers = new List<SlotSelector>();
        private List<SlotSelector> _internalSelectionMarkers = new List<SlotSelector>();
        private List<GameObject> _tempDeactivatedSelectionMarkers = new List<GameObject>();

        private UnityEngine.Camera _camera;
        private OutfittingCameraController _cameraController;

        private const string OutfittingPath = "GUIPrefabs/Station/Services/Outfitting/";
        private const string ThrusterCardPath = "ThrusterCard";
        private const string WeaponCardPath = "WeaponCard";
        private const string PowerPlantCardPath = "PowerPlantCard";
        private const string ShieldCardPath = "ShieldCard";
        private const string CargoBayCardPath = "CargoBayCard";
        private const string JumpDriveCardPath = "JumpDriveCard";
        private const string BuyGUIPath = "BuyGUI";
        private const string BuyComponentCardPath = "BuyComponentCard";


        private const float ErrorTimeout = 3;
        private float _currentErrorTime;

        private GameController _gameController;
        private Ship _ship;


        private void SetShipObjectHandler() {
            _shipObjectHandler = _guiGameObject.AddComponent<ShipObjectHandler>();
        }

        public void StartOutfitting(OutfittingService outfittingService, GameObject previousGUI, GameController gameController, Ship ship) {
            CameraUtility.SolidSkybox(); //makes skybox black so the GUI looks cleaner 
            _outfittingService = outfittingService;
            _previousGUI = previousGUI;
            _previousGUI.SetActive(false);
            _gameController = gameController;
            _ship = ship;

            SetupCamera();
            SetupGUI();

            DisplayShip();
            SetupBtns();
        }

        private int SetupBuyComponentCard(ShipComponent oldComponent, ShipComponent newComponent, GameObject contentView) {
            GameObject buyCard = (GameObject)Instantiate(Resources.Load(OutfittingPath + BuyComponentCardPath), contentView.transform);
            GameObjectHelper.SetGUITextValue(buyCard, "OldComponentName", oldComponent == null ? "Empty" : oldComponent.ComponentName + " - " + oldComponent.ComponentSize);
            GameObjectHelper.SetGUITextValue(buyCard, "OldComponentPrice", oldComponent == null ? "N/A" : oldComponent.ComponentPrice + "Cr");
            GameObjectHelper.SetGUITextValue(buyCard, "NewComponentName", newComponent == null ? "Empty" : newComponent.ComponentName + " - " + newComponent.ComponentSize);
            GameObjectHelper.SetGUITextValue(buyCard, "NewComponentPrice", newComponent == null ? "N/A" : newComponent.ComponentPrice + "Cr");

            int priceChange = 0;
            if (oldComponent == null) {
                priceChange = newComponent.ComponentPrice;
            }
            else if (newComponent == null) {
                priceChange = -oldComponent.ComponentPrice;
            }
            else {
                priceChange = newComponent.ComponentPrice - oldComponent.ComponentPrice;
            }

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
            if (GameController.PlayerProfile.ChangeCredits(-price)) {
                ErrorMsgActive("Purchase confirmed");
                Purchased();
                CloseBuyGUI();
            }
            else {
                ErrorMsgActive("Insufficient credits");
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

        private int SetComponentBuyCards(List<SlotSelector> selectionMarkers, GameObject contentView) {
            int priceChange = 0;
            foreach (SlotSelector selectionMarker in selectionMarkers) {
                if (selectionMarker.InitialShipComponent != null && selectionMarker.CurrentShipComponent != null) {
                    if (selectionMarker.InitialShipComponent.GetType() != selectionMarker.CurrentShipComponent.GetType() || selectionMarker.InitialShipComponent.ComponentSize != selectionMarker.CurrentShipComponent.ComponentSize) {
                        priceChange += SetupBuyComponentCard(selectionMarker.InitialShipComponent, selectionMarker.CurrentShipComponent, contentView);
                    }
                } else if(selectionMarker.InitialShipComponent != null || selectionMarker.CurrentShipComponent != null) {
                    priceChange += SetupBuyComponentCard(selectionMarker.InitialShipComponent, selectionMarker.CurrentShipComponent, contentView);
                }
                
            }

            return priceChange;
        }


        private void Update() {
            ErrorMsgCheck();
        }

        private void ErrorMsgCheck() {
            if (_errorMsg.activeSelf == true) {
                _currentErrorTime -= Time.deltaTime;
                if (_currentErrorTime <= 0) {
                    _errorMsg.SetActive(false);
                }
            }
        }

        private void ErrorMsgActive(string errorMsg) {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "ErrorMsg", errorMsg);
            _currentErrorTime = ErrorTimeout;
            _errorMsg.SetActive(true);
        }

        private void UnselectedSlotError() {
            ErrorMsgActive("Select a slot on the ship first");
        }

        private void SlotSizeError() {
            ErrorMsgActive("Component is too big for the selected slot");
        }

        private void SetupCamera() {
            _camera = UnityEngine.Camera.main;
            if (_camera != null) {
                _cameraController = _camera.gameObject.AddComponent<OutfittingCameraController>();
                _cameraController.TakeCameraControl();
                _cameraController.SetCameraPos();
                _camera.farClipPlane = 2500;
            }
        }

        private void SetupHomeBtn() {
            Button homeBtn = GameObject.Find("HomeBtn").GetComponent<Button>();
            homeBtn.onClick.AddListener(ExitOutfitting);
        }

        private void SetupColourBtn() {
            Button colourBtn = GameObject.Find("ColourBtn").GetComponent<Button>();
            colourBtn.onClick.AddListener(ColourBtnClick);
        }

        private void ColourBtnClick() {
            ShipColourGUIController shipColourGUIController = gameObject.AddComponent<ShipColourGUIController>();
            shipColourGUIController.SetupGUI(_shipObjectHandler, _guiGameObject, _shipObject);
        }

        private bool ShipLoadoutConfirmed() {
            List<SlotSelector> selectionMarkers = GetAllSlotSelectors();
            foreach (SlotSelector selectionMarker in selectionMarkers) {
                if ((selectionMarker.InitialShipComponent == null && selectionMarker.CurrentShipComponent == null)) {//if both unset then true
                    return true;
                }
                if (selectionMarker.InitialShipComponent == null || selectionMarker.CurrentShipComponent == null) {//if one unset return false
                    return false;
                }//otherwise check component & if not the same type and size return false
                if (selectionMarker.InitialShipComponent.GetType() != selectionMarker.CurrentShipComponent.GetType() || selectionMarker.InitialShipComponent.ComponentSize != selectionMarker.CurrentShipComponent.ComponentSize) {
                    return false;
                }
            }
            return true;
        }

        private void ExitOutfitting() {
            if (ShipLoadoutConfirmed()) {
                CameraUtility.NormalSkybox();
                _camera.farClipPlane = 3000;
                _gameController.RefreshPlayerShip();
                _previousGUI.SetActive(true);
                FindObjectOfType<ShipCameraController>().enabled = true;
                Destroy(_cameraController);
                Destroy(_guiGameObject);
                Destroy(this);
            }
            else {
                ErrorMsgActive("Need to confirm loadout");
                if (_buyGUI == null) {
                    SetupBuyGUI();
                }
            }
            
        }

        private void DisplayShip() {
            _shipObjectHandler.ManagedShip = _ship;
            _shipObject = _shipObjectHandler.CreateShip(GameObject.Find("ShipPanel").transform, Color.black);
            _shipObject.transform.position = _shipObjectHandler.ManagedShip.ShipHull.OutfittingPosition;
            _shipObject.transform.rotation = _shipObjectHandler.ManagedShip.ShipHull.OutfittingRotation;
            AddDraggableToShipPanel();
        }

        private void AddDraggableToShipPanel() {
            GameObject shipPanel = GameObject.Find("ShipPanel");
            ShipRotatable shipRotatable = shipPanel.AddComponent<ShipRotatable>();
            shipRotatable.RotatableObject = _shipObjectHandler.ManagedShip.ShipObject;
        }

        private void SetupGUI() {
            _guiGameObject = Instantiate((GameObject)Resources.Load(_outfittingService.GUIPath));
            SetShipObjectHandler();
            _errorMsg = GameObjectHelper.FindChild(_guiGameObject, "ErrorMsg");
            _hoverInfo = GameObjectHelper.FindChild(_guiGameObject, "HoverInfo");
        }

        private Transform GetScrollViewContentContainer() {
            return GameObject.Find("ComponentContent").transform;
        }

        private void SetupBtns() {
            SetupComponentBtns();
            SetupHomeBtn();
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
            _thrusterSubCategories = GameObjectHelper.FindChild(_guiGameObject, "ThrusterSubCategories");
            GameObjectHelper.FindChild(_thrusterSubCategories, "MainThrustersBtn").GetComponent<Button>().onClick.AddListener(MainThrusterBtnClick);
            GameObjectHelper.FindChild(_thrusterSubCategories, "ManoeuvringThrustersBtn").GetComponent<Button>().onClick.AddListener(ManoeuvringThrusterBtnClick);

            _weaponSubCategories = GameObjectHelper.FindChild(_guiGameObject, "WeaponSubCategories");
            GameObjectHelper.FindChild(_weaponSubCategories, "BallisticBtn").GetComponent<Button>().onClick.AddListener(BallisticBtnClick);
            GameObjectHelper.FindChild(_weaponSubCategories, "LaserBtn").GetComponent<Button>().onClick.AddListener(LaserBtnClick);
            GameObjectHelper.FindChild(_weaponSubCategories, "RailgunBtn").GetComponent<Button>().onClick.AddListener(RailgunBtnClick);

            _internalSubCategories = GameObjectHelper.FindChild(_guiGameObject, "InternalSubCategories");
            GameObjectHelper.FindChild(_internalSubCategories, "PowerPlantBtn").GetComponent<Button>().onClick.AddListener(PowerPlantBtnClick);
            GameObjectHelper.FindChild(_internalSubCategories, "ShieldsBtn").GetComponent<Button>().onClick.AddListener(ShieldsBtnClick);
            GameObjectHelper.FindChild(_internalSubCategories, "CargoBtn").GetComponent<Button>().onClick.AddListener(CargoBayBtnClick);
            GameObjectHelper.FindChild(_internalSubCategories, "JumpDriveBtn").GetComponent<Button>().onClick.AddListener(JumpDriveBtnCLick);

            Button thrustersBtn = GameObjectHelper.FindChild(_guiGameObject, "ThrustersBtn").GetComponentInChildren<Button>();
            thrustersBtn.onClick.AddListener(ThrustersBtnClick);
            thrustersBtn.Select(); //defaults to thruster upon opening outfitting
            ThrustersBtnClick();

            Button weaponsBtn = GameObjectHelper.FindChild(_guiGameObject, "WeaponsBtn").GetComponentInChildren<Button>();
            weaponsBtn.onClick.AddListener(WeaponBtnClick);
            Button internalBtn = GameObjectHelper.FindChild(_guiGameObject, "InternalBtn").GetComponentInChildren<Button>();
            internalBtn.onClick.AddListener(InternalBtnClick);
        }

        // main categories
        private void ThrustersBtnClick() {
            _thrusterSubCategories.SetActive(true);
            _weaponSubCategories.SetActive(false);
            _internalSubCategories.SetActive(false);
            MainThrusterBtnClick();
            SelectionMarkersSetActive(_internalSelectionMarkers, false);
            SelectionMarkersSetActive(_weaponSelectionMarkers, false);
        }

        private void WeaponBtnClick() {
            _thrusterSubCategories.SetActive(false);
            _weaponSubCategories.SetActive(true);
            _internalSubCategories.SetActive(false);
            BallisticBtnClick();
            SelectionMarkersSetActive(_internalSelectionMarkers, false);
            SelectionMarkersSetActive(_weaponSelectionMarkers, true);
            SelectionMarkersSetActive(_mainThrusterSelectionMarkers, false);
            SelectionMarkersSetActive(_manThrusterSelectionMarkers, false);
        }

        private void InternalBtnClick() {
            _thrusterSubCategories.SetActive(false);
            _weaponSubCategories.SetActive(false);
            _internalSubCategories.SetActive(true);
            PowerPlantBtnClick();
            SelectionMarkersSetActive(_internalSelectionMarkers, true);
            SelectionMarkersSetActive(_weaponSelectionMarkers, false);
            SelectionMarkersSetActive(_mainThrusterSelectionMarkers, false);
            SelectionMarkersSetActive(_manThrusterSelectionMarkers, false);
        }

        //sub categories
        private void MainThrusterBtnClick() {
            _mainThrusterSelectionMarkers = SubCatClick(DisplayMainThrusterComponents, _shipObjectHandler.MainThrusterComponents, _mainThrusterSelectionMarkers);
            SelectionMarkersSetActive(_mainThrusterSelectionMarkers, true);
            SelectionMarkersSetActive(_manThrusterSelectionMarkers, false);
        }

        private void ManoeuvringThrusterBtnClick() {
            // Manoeuvring thrusters have a different type slot tuple so this method can't use sub cat click 
            ClearScrollView();
            DisplayManoeuvringThrusterComponents();
            var manoeuvringThrusterComponents = _shipObjectHandler.ManoeuvringThrusterComponents;
            List<(Transform, Transform, string, ManoeuvringThruster)> slots = new List<(Transform, Transform, string, ManoeuvringThruster)>() { (manoeuvringThrusterComponents.selectionTransform, manoeuvringThrusterComponents.selectionTransform, manoeuvringThrusterComponents.slotName, manoeuvringThrusterComponents.thruster) };
            _manThrusterSelectionMarkers = GetSelectionMarkers(slots, _manThrusterSelectionMarkers);
            SelectionMarkersSetActive(_mainThrusterSelectionMarkers, false);
            SelectionMarkersSetActive(_manThrusterSelectionMarkers, true);
        }

        private void BallisticBtnClick() {
            _weaponSelectionMarkers = SubCatClick(DisplayBallisticComponents, _shipObjectHandler.WeaponComponents, _weaponSelectionMarkers);
        }

        private void LaserBtnClick() {
            _weaponSelectionMarkers = SubCatClick(DisplayLaserComponents, _shipObjectHandler.WeaponComponents, _weaponSelectionMarkers);
        }

        private void RailgunBtnClick() {
            _weaponSelectionMarkers = SubCatClick(DisplayRailgunComponents, _shipObjectHandler.WeaponComponents, _weaponSelectionMarkers);
        }

        private void PowerPlantBtnClick() {
            _internalSelectionMarkers = SubCatClick(DisplayPowerPlantComponents, _shipObjectHandler.InternalComponents, _internalSelectionMarkers);
        }

        private void ShieldsBtnClick() {
            _internalSelectionMarkers = SubCatClick(DisplayShieldsComponents, _shipObjectHandler.InternalComponents, _internalSelectionMarkers);
        }

        private void JumpDriveBtnCLick() {
            _internalSelectionMarkers = SubCatClick(DisplayJumpDriveComponents, _shipObjectHandler.InternalComponents, _internalSelectionMarkers);
        }

        private void CargoBayBtnClick() {
            _internalSelectionMarkers = SubCatClick(DisplayCargoBayComponents, _shipObjectHandler.InternalComponents, _internalSelectionMarkers);
        }

        private List<SlotSelector> SubCatClick<T>(Action displayFunction, List<(Transform mountTransform, Transform selectionTransform, string slotName, T component)> slots, List<SlotSelector> selectionMarkers) where T : ShipComponent {
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

        private void SetupThrusterCard(Transform cardComponents, Thruster thrusterInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", thrusterInstance.ComponentName + " - " + thrusterInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Force", thrusterInstance.Force + " N");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "PowerDraw", thrusterInstance.PowerDraw + " GW/s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "PowerDraw", thrusterInstance.ComponentMass / 1000 + " T");
        }

        private void DisplayWeaponCard((Type weaponType, ShipComponentTier tier) weapon) {
            (Transform cardComponents, Weapon weaponInstance) = DisplayShipComponentCard<Weapon>(weapon.weaponType, weapon.tier, WeaponCardPath);
            SetupWeaponCard(cardComponents, weaponInstance);
        }

        private void SetupWeaponCard(Transform cardComponents, Weapon weaponInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", weaponInstance.ComponentName + " - " + weaponInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Damage", weaponInstance.Damage + " DMG");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "RoF", 60 / weaponInstance.FireDelay + " RPM");
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

        private void SetupPowerPlantCard(Transform cardComponents, PowerPlant powerPlantInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", powerPlantInstance.ComponentName + " - " + powerPlantInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Capacity", powerPlantInstance.EnergyCapacity + " GW");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Recharge", powerPlantInstance.RechargeRate * 60 + " GW/s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Mass", powerPlantInstance.ComponentMass / 1000 + " T");
        }

        private void DisplayCargoBay((Type cargoBayType, ShipComponentTier tier) cargoBay) {
            (Transform cardComponents, CargoBay cargoBayInstance) = DisplayShipComponentCard<CargoBay>(cargoBay.cargoBayType, cargoBay.tier, CargoBayCardPath);
            SetupCargoBayCard(cardComponents, cargoBayInstance);
        }

        private void SetupCargoBayCard(Transform cardComponents, CargoBay cargoBayInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", cargoBayInstance.ComponentName + " - " + cargoBayInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Capacity", cargoBayInstance.CargoVolume + " Units");
        }

        private void DisplayShieldCard((Type shieldType, ShipComponentTier tier) shield) {
            (Transform cardComponents, Shield shieldInstance) = DisplayShipComponentCard<Shield>(shield.shieldType, shield.tier, ShieldCardPath);
            SetupShieldCard(cardComponents, shieldInstance);
        }

        private void SetupShieldCard(Transform cardComponents, Shield shieldInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", shieldInstance.ComponentName + " - " + shieldInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Strength", shieldInstance.StrengthCapacity + "GW");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Recharge", shieldInstance.RechargeRate + "GW/s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "DamageDelay", shieldInstance.DamageRecoveryTime + "s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "DepletionDelay", shieldInstance.DepletionRecoveryTime + "s");
        }

        private void SetupJumpDriveCard(Transform cardComponents, JumpDrive jumpDriveInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", jumpDriveInstance.ComponentName + " - " + jumpDriveInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Range", jumpDriveInstance.JumpRange + "LY");
        }

        private void SelectionMarkersSetActive(List<SlotSelector> selectionMarkers, bool active) {
            foreach (SlotSelector selectionMarker in selectionMarkers) {
                selectionMarker.gameObject.SetActive(active);
            }
        }

        private List<SlotSelector> GetSelectionMarkers<T>(List<(Transform mountTransform, Transform selectionTransform, string slotName, T component)> shipComponents, List<SlotSelector> selectionMarkers) where T : ShipComponent {
            if (selectionMarkers == null || selectionMarkers.Count == 0) {
                selectionMarkers = new List<SlotSelector>();

                foreach ((Transform mountTransform, Transform selectionTransform, string slotName, ShipComponent component) selection in shipComponents) {
                    Transform canvas = _guiGameObject.transform.Find("Canvas");
                    GameObject marker = Instantiate((GameObject)Resources.Load(OutfittingPath + "ShipComponentMarker"), canvas.Find("SelectionMarkers"));

                    SlotSelector slotSelector = marker.AddComponent<SlotSelector>();
                    slotSelector.Setup(selection.mountTransform, selection.selectionTransform, selection.slotName, this, selection.component);
                    selectionMarkers.Add(slotSelector);
                    GameObject markerText = Instantiate((GameObject)Resources.Load(OutfittingPath + "ShipComponentMarkerName"), marker.transform);
                    TextMeshProUGUI slotName = markerText.transform.GetComponentInChildren<TextMeshProUGUI>();
                    slotName.text = selection.slotName;
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

        private GameObject CreateComponentCard(string cardSpecifier, Transform parent) {
            return Instantiate((GameObject)Resources.Load(OutfittingPath + cardSpecifier), parent);
        }

        private GameObject CreateComponentCardModifier(string cardSpecifier, Type componentType, ShipComponentTier tier) {
            GameObject componentCard = CreateComponentCard(cardSpecifier, GetScrollViewContentContainer());

            CardShipComponentModifier componentModifier = componentCard.AddComponent<CardShipComponentModifier>();

            componentModifier.Setup(componentType, tier, this);
            return componentCard;
        }

        private void ChangeSelectedSlotComponent(ShipComponent newComponent) {
            if (_selectedSlot != null) {
                bool success = _selectedSlot.ChangeComponent(newComponent);
                if (!success) {
                    SlotSizeError();
                }
                else {
                    _selectedSlot.CurrentShipComponent = newComponent;
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

        class SlotSelector : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
            private Transform SelectionMountTransform { get; set; }
            private Transform ObjectMountTransform { get; set; }
            private string SlotName { get; set; }
            private OutfittingGUIController _outfittingGUIController;
            private bool _selected;
            private GameObject _hoverInfo;
            public ShipComponent CurrentShipComponent;
            public ShipComponent InitialShipComponent;

            public bool ChangeComponent(ShipComponent newComponent) {
                bool success = false;
                if (newComponent.GetType().IsSubclassOf(typeof(Weapon))) {
                    success = ChangeWeapon((Weapon)newComponent);
                }else if (newComponent.GetType().IsSubclassOf(typeof(MainThruster))) {
                    success = ChangeMainThruster((MainThruster)newComponent);
                }
                else if (newComponent.GetType() == (typeof(ManoeuvringThruster))) {
                    success = ChangeManThruster((ManoeuvringThruster)newComponent);
                }
                else if (newComponent.GetType().IsSubclassOf(typeof(InternalComponent))) {
                    success = ChangeInternal((InternalComponent)newComponent);
                }

                return success;
            }

            private bool ChangeInternal(InternalComponent newComponent) {
                return _outfittingGUIController._shipObjectHandler.SetInternalComponent(ObjectMountTransform.gameObject.name, newComponent);
            }

            private bool ChangeWeapon(Weapon newComponent) {
                return _outfittingGUIController._shipObjectHandler.SetWeaponComponent(ObjectMountTransform.gameObject.name, newComponent);
            }

            private bool ChangeManThruster(ManoeuvringThruster newComponent) {
                return _outfittingGUIController._shipObjectHandler.SetManoeuvringThrusterComponent(newComponent);
            }

            private bool ChangeMainThruster(MainThruster newComponent) {
                return _outfittingGUIController._shipObjectHandler.SetMainThrusterComponent(ObjectMountTransform.gameObject.name, newComponent);
            }

            private void Awake() {
                ChangeColourNotSelected();
            }

            private void Update() {
                PositionSelf();
                CheckSelected();
            }

            private void CheckSelected() {
                if (_selected) {
                    if (_outfittingGUIController._selectedSlot != this) {
                        _selected = false;
                        ChangeColourNotSelected();
                    }
                }
            }

            private void ChangeColourNotSelected() {
                var image = gameObject.GetComponent<SVGImage>();
                image.color = new Color(255 / 255f, 185 / 255f, 0);
            }

            private void ChangeColourSelected() {
                var image = gameObject.GetComponent<SVGImage>();
                image.color = new Color(53 / 255f, 157 / 255f, 255 / 255f);
            }

            public void Setup(Transform objectMountTransform, Transform selectionMountTransform, string slotName, OutfittingGUIController outfittingGUIController, ShipComponent shipComponent) {
                SelectionMountTransform = selectionMountTransform;
                ObjectMountTransform = objectMountTransform;
                SlotName = slotName;
                _outfittingGUIController = outfittingGUIController;
                CurrentShipComponent = shipComponent;
                InitialShipComponent = shipComponent;
            }

            public void OnPointerClick(PointerEventData eventData) {
                if (eventData.button == PointerEventData.InputButton.Left) { //select slot
                    _outfittingGUIController._selectedSlot = this;
                    _selected = true;
                    ChangeColourSelected();
                    Debug.Log("Set selected slot to: " + ObjectMountTransform.name);
                }
                else if (eventData.button == PointerEventData.InputButton.Right) { //clear slot
                    RemoveComponent();
                }
                else if (eventData.button == PointerEventData.InputButton.Middle) { //reset slot
                    ResetSlot();
                }
            }

            public void ResetSlot() {
                if (InitialShipComponent == null) {
                    RemoveComponent();
                }
                else {
                    ChangeComponent(InitialShipComponent);
                }

                CurrentShipComponent = InitialShipComponent;
            }

            private void RemoveComponent() {
                if (CurrentShipComponent != null) {
                    if (CurrentShipComponent.GetType().IsSubclassOf(typeof(Weapon))) {
                        _outfittingGUIController._shipObjectHandler.SetWeaponComponent(ObjectMountTransform.gameObject.name, null);
                    }else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(MainThruster))) {
                        _outfittingGUIController._shipObjectHandler.SetMainThrusterComponent(ObjectMountTransform.gameObject.name, null);
                    }
                    else if (CurrentShipComponent.GetType() == (typeof(ManoeuvringThruster))) {
                        _outfittingGUIController._shipObjectHandler.SetManoeuvringThrusterComponent(null);
                    }
                    else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(InternalComponent))) {
                        _outfittingGUIController._shipObjectHandler.SetInternalComponent(ObjectMountTransform.gameObject.name, null);
                    }

                    CurrentShipComponent = null;
                }
            }

            private void PositionSelf() {
                Vector2 screenSpacePos = _outfittingGUIController._camera.WorldToScreenPoint(SelectionMountTransform.position);
                Transform selectorTransform = transform;
                selectorTransform.rotation = Quaternion.LookRotation(-_outfittingGUIController._camera.transform.position);
                selectorTransform.position = screenSpacePos;
            }

            public void OnPointerEnter(PointerEventData eventData) {
                if (CurrentShipComponent != null) {
                    Debug.Log("Hovering: " + CurrentShipComponent.ComponentName);

                    if (CurrentShipComponent.GetType().IsSubclassOf(typeof(Thruster))) {
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(ThrusterCardPath, _outfittingGUIController._hoverInfo.transform);
                        _outfittingGUIController.SetupThrusterCard(_hoverInfo.transform, (Thruster)CurrentShipComponent);
                    }
                    else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(Weapon))) {
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(WeaponCardPath, _outfittingGUIController._hoverInfo.transform);
                        _outfittingGUIController.SetupWeaponCard(_hoverInfo.transform, (Weapon)CurrentShipComponent);
                    }
                    else if (CurrentShipComponent.GetType() == typeof(CargoBay)) {
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(CargoBayCardPath, _outfittingGUIController._hoverInfo.transform);
                        _outfittingGUIController.SetupCargoBayCard(_hoverInfo.transform, (CargoBay)CurrentShipComponent);
                    }
                    else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(PowerPlant))) {
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(PowerPlantCardPath, _outfittingGUIController._hoverInfo.transform);
                        _outfittingGUIController.SetupPowerPlantCard(_hoverInfo.transform, (PowerPlant)CurrentShipComponent);
                    }
                    else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(Shield))) {
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(ShieldCardPath, _outfittingGUIController._hoverInfo.transform);
                        _outfittingGUIController.SetupShieldCard(_hoverInfo.transform, (Shield)CurrentShipComponent);
                    }
                    else if (CurrentShipComponent.GetType() == typeof(JumpDrive)) {
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(JumpDriveCardPath, _outfittingGUIController._hoverInfo.transform);
                        _outfittingGUIController.SetupJumpDriveCard(_hoverInfo.transform, (JumpDrive)CurrentShipComponent);
                    }

                    RectTransform rectTransform = _hoverInfo.GetComponent<RectTransform>();
                    rectTransform.localPosition = new Vector3();
                    rectTransform.pivot = new Vector2(.5f, 0);
                }
            }

            public void OnPointerExit(PointerEventData eventData) {
                Destroy(_hoverInfo);
            }
        }
    }
}