using System;
using System.Collections.Generic;
using System.Linq;
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
using Code.GUI.Utility;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class OutfittingGUIController : MonoBehaviour {
        private StationGUIController _stationGUIController;
        private OutfittingService _outfittingService;
        private GameObject _guiGameObject,_shipObject,_thrusterSubCategories,_weaponSubCategories, _internalSubCategories, _errorMsg, _hoverInfo;
        private ShipObjectHandler _shipObjectHandler;
        
        private SlotSelector _selectedSlot;
        private List<GameObject> _selectionMarkers = new List<GameObject>();

        private UnityEngine.Camera _camera;
        private OutfittingCameraController _cameraController;

        private string _outfittingPath = "GUIPrefabs/Station/Services/Outfitting/";
        private string _thrusterCardPath = "ThrusterCard";
        private string _weaponCardPath = "WeaponCard";
        private string _powerPlantCardPath = "PowerPlantCard";
        private string _shieldCardPath = "ShieldCard";
        private string _cargoBayCardPath = "CargoBayCard";
        private string _jumpDriveCardPath = "JumpDriveCard";
        
        
        private float errorTimeout = 3;
        private float currentErrorTime = 0;

        private GameController _gameController;


        private void SetShipObjectHandler() {
            _shipObjectHandler = _guiGameObject.AddComponent<ShipObjectHandler>();
        }

        public void StartOutfitting(OutfittingService outfittingService, StationGUIController stationGUIController, GameController gameController) {
            // _outfittingService = new OutfittingService();
            // _outfittingService.SetAllAvailableComponents();
            _outfittingService = outfittingService;
            _stationGUIController = stationGUIController;
            _stationGUIController.stationGUI.SetActive(false);
            _gameController = gameController;
            
            SetupCamera();
            SetupGUI();
            
            DisplayShip();
            SetupBtns();
        }

        private void Update() {
            ErrorMsgCheck();
        }

        private void ErrorMsgCheck() {
            if (_errorMsg.activeSelf == true) {
                currentErrorTime -= Time.deltaTime;
                if (currentErrorTime <= 0) {
                    _errorMsg.SetActive(false);
                }
            }
        }

        private void ErrorMsgActive(string errorMsg) {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "ErrorMsg", errorMsg);
            currentErrorTime = errorTimeout;
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


        private void ExitOutfitting() {
            _camera.farClipPlane = 3000;
            _gameController.RefreshPlayerShip();
            _stationGUIController.stationGUI.SetActive(true);
            FindObjectOfType<ShipCameraController>().enabled = true;
            Destroy(_cameraController);
            Destroy(_guiGameObject);
            Destroy(this);
        }

        private void DisplayShip() {
            _shipObjectHandler.ManagedShip = GameController.CurrentShip;
            _shipObject = _shipObjectHandler.CreateShip(GameObject.Find("ShipPanel").transform);
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
            _guiGameObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_outfittingService.GUIPath));
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
        }

        private void SetupComponentBtns() {
            _thrusterSubCategories = GameObject.Find("ThrusterSubCategories");
            _thrusterSubCategories.gameObject.transform.Find("MainThrustersBtn").GetComponent<Button>().onClick.AddListener(MainThrusterBtnClick);
            _thrusterSubCategories.gameObject.transform.Find("ManoeuvringThrustersBtn").GetComponent<Button>().onClick.AddListener(ManoeuvringThrusterBtnClick);

            _weaponSubCategories = GameObject.Find("WeaponSubCategories");
            _weaponSubCategories.gameObject.transform.Find("BallisticBtn").GetComponent<Button>().onClick.AddListener(BallisticBtnClick);
            _weaponSubCategories.gameObject.transform.Find("LaserBtn").GetComponent<Button>().onClick.AddListener(LaserBtnClick);
            _weaponSubCategories.gameObject.transform.Find("RailgunBtn").GetComponent<Button>().onClick.AddListener(RailgunBtnClick);

            _internalSubCategories = GameObject.Find("InternalSubCategories");
            _internalSubCategories.gameObject.transform.Find("PowerPlantBtn").GetComponent<Button>().onClick.AddListener(PowerPlantBtnClick);
            _internalSubCategories.gameObject.transform.Find("ShieldsBtn").GetComponent<Button>().onClick.AddListener(ShieldsBtnClick);
            _internalSubCategories.gameObject.transform.Find("CargoBtn").GetComponent<Button>().onClick.AddListener(CargoBayBtnClick);
            _internalSubCategories.gameObject.transform.Find("JumpDriveBtn").GetComponent<Button>().onClick.AddListener(JumpDriveBtnCLick);

            Button thrustersBtn = GameObject.Find("ThrustersBtn").GetComponentInChildren<Button>();
            thrustersBtn.onClick.AddListener(ThrustersBtnClick);
            thrustersBtn.Select(); //defaults to thruster upon opening outfitting
            ThrustersBtnClick();

            Button weaponsBtn = GameObject.Find("WeaponsBtn").GetComponentInChildren<Button>();
            weaponsBtn.onClick.AddListener(WeaponBtnClick);
            Button internalBtn = GameObject.Find("InternalBtn").GetComponentInChildren<Button>();
            internalBtn.onClick.AddListener(InternalBtnClick);
        }

        // main categories
        private void ThrustersBtnClick() {
            _thrusterSubCategories.SetActive(true);
            _weaponSubCategories.SetActive(false);
            _internalSubCategories.SetActive(false);
            MainThrusterBtnClick();
        }

        private void WeaponBtnClick() {
            _thrusterSubCategories.SetActive(false);
            _weaponSubCategories.SetActive(true);
            _internalSubCategories.SetActive(false);
            BallisticBtnClick();
        }

        private void InternalBtnClick() {
            _thrusterSubCategories.SetActive(false);
            _weaponSubCategories.SetActive(false);
            _internalSubCategories.SetActive(true);
            PowerPlantBtnClick();
        }

        //sub categories
        private void MainThrusterBtnClick() {
            SubCatClick(DisplayMainThrusterComponents, _shipObjectHandler.MainThrusterComponents);
        }

        private void ManoeuvringThrusterBtnClick() {
            // Manoeuvring thrusters have a different slot variable so this method can't use sub cat click 
            ClearScrollView();
            DisplayManoeuvringThrusterComponents();
            var manoeuvringThrusterComponents = _shipObjectHandler.ManoeuvringThrusterComponents;
            List<(Transform, Transform, string, ManoeuvringThruster)> slots = new List<(Transform, Transform, string, ManoeuvringThruster)>() { (manoeuvringThrusterComponents.selectionTransform, manoeuvringThrusterComponents.selectionTransform, manoeuvringThrusterComponents.slotName, manoeuvringThrusterComponents.thruster) };
            CreateSelectionMarkers<ManoeuvringThruster>(slots);
        }

        private void BallisticBtnClick() {
            SubCatClick(DisplayBallisticComponents, _shipObjectHandler.WeaponComponents);
        }

        private void LaserBtnClick() {
            SubCatClick(DisplayLaserComponents, _shipObjectHandler.WeaponComponents);
        }

        private void RailgunBtnClick() {
            SubCatClick(DisplayRailgunComponents, _shipObjectHandler.WeaponComponents);
        }

        private void PowerPlantBtnClick() {
            SubCatClick(DisplayPowerPlantComponents, _shipObjectHandler.InternalComponents);
        }

        private void ShieldsBtnClick() {
            SubCatClick(DisplayShieldsComponents, _shipObjectHandler.InternalComponents);
        }

        private void JumpDriveBtnCLick() {
            SubCatClick(DisplayJumpDriveComponents, _shipObjectHandler.InternalComponents);
        }

        private void CargoBayBtnClick() {
            SubCatClick(DisplayCargoBayComponents, _shipObjectHandler.InternalComponents);
        }

        private void SubCatClick<T>(Action displayFunction, List<(Transform mountTransform, Transform selectionTransform, string slotName, T component)> slots) where T : ShipComponent {
            ClearScrollView();
            displayFunction.Invoke();
            CreateSelectionMarkers(slots);
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
            (Transform cardComponents, Thruster thrusterInstance) = DisplayShipComponentCard<Thruster>(thruster.thrusterType, thruster.tier, _thrusterCardPath);
            SetupThrusterCard(cardComponents, thrusterInstance);
        }

        private void SetupThrusterCard(Transform cardComponents, Thruster thrusterInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", thrusterInstance.ComponentName + " - " + thrusterInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Force", thrusterInstance.Force + " N");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "PowerDraw", thrusterInstance.PowerDraw + " GW/s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "PowerDraw", thrusterInstance.ComponentMass / 1000 + " T");
        }

        private void DisplayWeaponCard((Type weaponType, ShipComponentTier tier) weapon) {
            (Transform cardComponents, Weapon weaponInstance) = DisplayShipComponentCard<Weapon>(weapon.weaponType, weapon.tier, _weaponCardPath);
            SetupWeaponCard(cardComponents, weaponInstance);
        }

        private void SetupWeaponCard(Transform cardComponents, Weapon weaponInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", weaponInstance.ComponentName + " - " + weaponInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Damage", weaponInstance.Damage + " DMG");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "RoF", 60 / weaponInstance.FireDelay + " RPM");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Mass", weaponInstance.ComponentMass / 1000 + " T");
        }

        private void DisplayPowerPlant((Type powerPlantType, ShipComponentTier tier) powerPlant) {
            (Transform cardComponents, PowerPlant powerPlantInstance) = DisplayShipComponentCard<PowerPlant>(powerPlant.powerPlantType, powerPlant.tier, _powerPlantCardPath);
            SetupPowerPlantCard(cardComponents, powerPlantInstance);
        }

        private void DisplayJumpDrive((Type jumpDriveType, ShipComponentTier tier) jumpDrive) {
            (Transform cardComponents, JumpDrive jumpDriveInstance) = DisplayShipComponentCard<JumpDrive>(jumpDrive.jumpDriveType, jumpDrive.tier, _jumpDriveCardPath);
            SetupJumpDriveCard(cardComponents, jumpDriveInstance);
        }

        private void SetupPowerPlantCard(Transform cardComponents, PowerPlant powerPlantInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", powerPlantInstance.ComponentName + " - " + powerPlantInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Capacity", powerPlantInstance.EnergyCapacity + " GW");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Recharge", powerPlantInstance.RechargeRate * 60 + " GW/s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Mass", powerPlantInstance.ComponentMass / 1000 + " T");
        }

        private void DisplayCargoBay((Type cargoBayType, ShipComponentTier tier) cargoBay) {
            (Transform cardComponents, CargoBay cargoBayInstance) = DisplayShipComponentCard<CargoBay>(cargoBay.cargoBayType, cargoBay.tier, _cargoBayCardPath);
            SetupCargoBayCard(cardComponents, cargoBayInstance);
        }

        private void SetupCargoBayCard(Transform cardComponents, CargoBay cargoBayInstance) {
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", cargoBayInstance.ComponentName + " - " + cargoBayInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Capacity", cargoBayInstance.CargoVolume + " Units");
        }

        private void DisplayShieldCard((Type shieldType, ShipComponentTier tier) shield) {
            (Transform cardComponents, Shield shieldInstance) = DisplayShipComponentCard<Shield>(shield.shieldType, shield.tier, _shieldCardPath);
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

        private void CreateSelectionMarkers<T>(List<(Transform mountTransform, Transform selectionTransform, string slotName, T component)> shipComponents) where T : ShipComponent {
            // List<(Transform ObjectMountTransform, Transform SelectionMountTransform, string SlotName)> currentSlots = new List<(Transform ObjectMountTransform, Transform SelectionMountTransform, string SlotName)>();
            bool sameSlots = true;
            if (shipComponents.Count == _selectionMarkers.Count) {
                for (int i = 0; i < shipComponents.Count; i++) {
                    SlotSelector selector = _selectionMarkers[i].GetComponent<SlotSelector>();
                    var selectionMarkerDetails = (selector.ObjectMountTransform, selector.SelectionMountTransform, selector.SlotName);
                    sameSlots = shipComponents.Select(sc => sc.selectionTransform).ToList().Contains(selectionMarkerDetails.SelectionMountTransform);

                    if (!sameSlots) {
                        break;
                    }
                }
            }
            else {
                sameSlots = false;
            }

            //if not same as current slots
            if (!sameSlots) {
                foreach (GameObject selectionMarker in _selectionMarkers) {
                    Destroy(selectionMarker);
                }

                _selectionMarkers = new List<GameObject>();

                foreach ((Transform mountTransform, Transform selectionTransform, string slotName, ShipComponent component) selection in shipComponents) {
                    Transform canvas = _guiGameObject.transform.Find("Canvas");
                    GameObject marker = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_outfittingPath + "ShipComponentMarker"), canvas.Find("SelectionMarkers"));
                    _selectionMarkers.Add(marker);
                    SlotSelector slotSelector = marker.AddComponent<SlotSelector>();
                    slotSelector.Setup(selection.mountTransform, selection.selectionTransform, selection.slotName, this, selection.component);

                    GameObject markerText = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_outfittingPath + "ShipComponentMarkerName"), marker.transform);
                    TextMeshProUGUI slotName = markerText.transform.GetComponentInChildren<TextMeshProUGUI>();
                    slotName.text = selection.slotName;
                }
            }
        }

        private void ClearScrollView() {
            Transform container = GetScrollViewContentContainer();

            for (int i = container.childCount; i > 0; i--) {
                Destroy(container.GetChild(i - 1).gameObject);
            }
        }

        private GameObject CreateComponentCard(string cardSpecifier, Transform parent) {
            return GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_outfittingPath + cardSpecifier), parent);
        }

        private GameObject CreateComponentCardModifier(string cardSpecifier, Type componentType, ShipComponentTier tier) {
            GameObject componentCard = CreateComponentCard(cardSpecifier, GetScrollViewContentContainer());

            CardShipComponentModifier componentModifier = componentCard.AddComponent<CardShipComponentModifier>();

            componentModifier.Setup(componentType, tier, this);
            return componentCard;
        }


        private void ChangeSelectedShipWeapon(Weapon newComponent) {
            if (_selectedSlot != null) {
                bool success = _shipObjectHandler.SetWeaponComponent(_selectedSlot.ObjectMountTransform.name, newComponent);
                if (!success) {
                    SlotSizeError();
                }
                else {
                    _selectedSlot._shipComponent = newComponent;
                }
            }
            else {
                UnselectedSlotError();
            }
        }

        private void ChangeSelectedShipMainThruster(MainThruster newComponent) {
            if (_selectedSlot != null) {
                bool success = _shipObjectHandler.SetMainThrusterComponent(_selectedSlot.ObjectMountTransform.gameObject.name, newComponent);
                if (!success) {
                    SlotSizeError();
                }
                else {
                    _selectedSlot._shipComponent = newComponent;
                }
            }
            else {
                UnselectedSlotError();
            }
        }

        private void ChangeSelectedShipManoeuvringThruster(ManoeuvringThruster newComponent) {
            if (_selectedSlot != null) {
                bool success = _shipObjectHandler.SetManoeuvringThrusterComponent(newComponent);
                if (!success) {
                    SlotSizeError();
                }
                else {
                    _selectedSlot._shipComponent = newComponent;
                }
            }
            else {
                UnselectedSlotError();
            }
        }

        private void ChangeSelectedInternal(InternalComponent newComponent) {
            if (_selectedSlot != null) {
                bool success = _shipObjectHandler.SetInternalComponent(_selectedSlot.ObjectMountTransform.gameObject.name, newComponent);
                if (!success) {
                    SlotSizeError();
                }
                else {
                    _selectedSlot._shipComponent = newComponent;
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
                    _outfittingGUIController.ChangeSelectedShipWeapon((Weapon)shipComponent);
                }
                else if (shipComponent.GetType().IsSubclassOf(typeof(MainThruster))) {
                    _outfittingGUIController.ChangeSelectedShipMainThruster((MainThruster)shipComponent);
                }
                else if (shipComponent.GetType() == typeof(ManoeuvringThruster)) {
                    _outfittingGUIController.ChangeSelectedShipManoeuvringThruster((ManoeuvringThruster)shipComponent);
                }
                else if (shipComponent.GetType().IsSubclassOf(typeof(InternalComponent))) {
                    _outfittingGUIController.ChangeSelectedInternal((InternalComponent)shipComponent);
                }
            }
        }

        class SlotSelector : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
            public Transform SelectionMountTransform { get; private set; }
            public Transform ObjectMountTransform { get; private set; }
            public string SlotName { get; private set; }
            private OutfittingGUIController _outfittingGUIController;
            private bool _selected;
            private GameObject _hoverInfo;
            public ShipComponent _shipComponent;

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
                _shipComponent = shipComponent;
            }

            public void OnPointerClick(PointerEventData eventData) {
                _outfittingGUIController._selectedSlot = this;
                _selected = true;
                ChangeColourSelected();
                Debug.Log("Set selected slot to: " + ObjectMountTransform.name);
            }

            private void PositionSelf() {
                Vector2 screenSpacePos = _outfittingGUIController._camera.WorldToScreenPoint(SelectionMountTransform.position);
                Transform selectorTransform = transform;
                selectorTransform.rotation = Quaternion.LookRotation(-_outfittingGUIController._camera.transform.position);
                selectorTransform.position = screenSpacePos;
            }

            public void OnPointerEnter(PointerEventData eventData) {
                Debug.Log("Hovering: " + _shipComponent.ComponentName);

                if (_shipComponent.GetType().IsSubclassOf(typeof(Thruster))) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(_outfittingGUIController._thrusterCardPath, _outfittingGUIController._hoverInfo.transform);
                    _outfittingGUIController.SetupThrusterCard(_hoverInfo.transform, (Thruster)_shipComponent);
                }
                else if (_shipComponent.GetType().IsSubclassOf(typeof(Weapon))) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(_outfittingGUIController._weaponCardPath, _outfittingGUIController._hoverInfo.transform);
                    _outfittingGUIController.SetupWeaponCard(_hoverInfo.transform, (Weapon)_shipComponent);
                }
                else if (_shipComponent.GetType() == typeof(CargoBay)) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(_outfittingGUIController._cargoBayCardPath, _outfittingGUIController._hoverInfo.transform);
                    _outfittingGUIController.SetupCargoBayCard(_hoverInfo.transform, (CargoBay)_shipComponent);
                }
                else if (_shipComponent.GetType().IsSubclassOf(typeof(PowerPlant))) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(_outfittingGUIController._powerPlantCardPath, _outfittingGUIController._hoverInfo.transform);
                    _outfittingGUIController.SetupPowerPlantCard(_hoverInfo.transform, (PowerPlant)_shipComponent);
                }
                else if (_shipComponent.GetType().IsSubclassOf(typeof(Shield))) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(_outfittingGUIController._shieldCardPath, _outfittingGUIController._hoverInfo.transform);
                    _outfittingGUIController.SetupShieldCard(_hoverInfo.transform, (Shield)_shipComponent);
                }
                else if (_shipComponent.GetType() == typeof(JumpDrive)) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(_outfittingGUIController._jumpDriveCardPath, _outfittingGUIController._hoverInfo.transform);
                    _outfittingGUIController.SetupJumpDriveCard(_hoverInfo.transform, (JumpDrive)_shipComponent);
                }

                RectTransform rectTransform =_hoverInfo.GetComponent<RectTransform>(); 
                rectTransform.localPosition = new Vector3();
                rectTransform.pivot = new Vector2(.5f, 0);
            }

            public void OnPointerExit(PointerEventData eventData) {
                Destroy(_hoverInfo);
            }
        }
    }
}