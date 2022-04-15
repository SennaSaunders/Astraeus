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
        private GameObject _guiGameObject;
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
        private GameObject _thrusterSubCategories;
        private GameObject _weaponSubCategories;
        private GameObject _internalSubCategories;

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

        private void SetupCamera() {
            _camera = UnityEngine.Camera.main;
            if (_camera != null) {
                _cameraController = _camera.gameObject.AddComponent<OutfittingCameraController>();
                _cameraController.TakeCameraControl();
                _cameraController.SetCameraPos();
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
            shipColourGUIController.SetupGUI(this, _guiGameObject);
        }


        private void ExitOutfitting() {
            _gameController.RefreshPlayerShip();
            _stationGUIController.stationGUI.SetActive(true);
            FindObjectOfType<ShipCameraController>().enabled = true;
            Destroy(_cameraController);
            Destroy(_guiGameObject);
            Destroy(this);
        }

        private void DisplayShip() {
            _shipObjectHandler.ManagedShip = GameController.CurrentShip; //needs to be changed to the player's current ship
            GameObject shipObject = _shipObjectHandler.CreateShip(GameObject.Find("ShipPanel").transform);
            shipObject.transform.position = _shipObjectHandler.ManagedShip.ShipHull.OutfittingPosition;
            shipObject.transform.rotation = _shipObjectHandler.ManagedShip.ShipHull.OutfittingRotation;
            AddDraggableToShip();
        }

        private void AddDraggableToShip() {
            GameObject shipPanel = GameObject.Find("ShipPanel");
            ShipRotatable shipRotatable = shipPanel.AddComponent<ShipRotatable>();
            shipRotatable.RotatableObject = _shipObjectHandler.ManagedShip.ShipObject;
        }


        private void SetupGUI() {
            _guiGameObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_outfittingService.GUIPath));
            SetShipObjectHandler();
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

            Button thrustersBtn = GameObject.Find("ThrustersBtn").GetComponentInChildren<Button>();
            thrustersBtn.onClick.AddListener(ThrustersBtnClick);
            thrustersBtn.Select(); //defaults to thruster upon opening outfitting
            ThrustersBtnClick();

            // Button manoeuvringThrustersBtn = GameObject.Find("ManoeuvringThrustersBtn").GetComponent<Button>();
            // manoeuvringThrustersBtn.onClick.AddListener(ManoeuvringThrusterBtnClick);

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
            List<(Transform, Transform, string)> slots = new List<(Transform, Transform, string)>() { (manoeuvringThrusterComponents.selectionTransform, manoeuvringThrusterComponents.selectionTransform, manoeuvringThrusterComponents.slotName) };
            CreateSelectionMarkers(slots);
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

        private void CargoBayBtnClick() {
            SubCatClick(DisplayCargoBayComponents, _shipObjectHandler.InternalComponents);
        }

        private void SubCatClick(Action displayFunction, List<(Transform, Transform, string)> slots) {
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
            DisplayComponents<LaserCannon>(DisplayWeaponCard);
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

        private (Transform, T) DisplayShipComponentCard<T>(Type componentType, ShipComponentTier tier, string cardPath) where T : ShipComponent {
            T shipComponent = (T)Activator.CreateInstance(componentType, tier);
            GameObject cardObject = CreateComponentCard(cardPath, componentType, tier);
            Transform cardComponents = cardObject.transform.Find("Details");
            return (cardComponents, shipComponent);
        }

        private void DisplayThrusterCard((Type thrusterType, ShipComponentTier tier) thruster) {
            (Transform cardComponents, Thruster thrusterInstance) = DisplayShipComponentCard<Thruster>(thruster.thrusterType, thruster.tier, _thrusterCardPath);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", thrusterInstance.ComponentName + " - " + thrusterInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Force", thrusterInstance.Force + " N");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "PowerDraw", thrusterInstance.PowerDraw + " GW/s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "PowerDraw", thrusterInstance.ComponentMass / 1000 + " T");
        }

        private void DisplayWeaponCard((Type weaponType, ShipComponentTier tier) weapon) {
            (Transform cardComponents, Weapon weaponInstance) = DisplayShipComponentCard<Weapon>(weapon.weaponType, weapon.tier, _weaponCardPath);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", weaponInstance.ComponentName + " - " + weaponInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Damage", weaponInstance.Damage + " DMG");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "RoF", 60 / weaponInstance.FireDelay + " RPM");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Mass", weaponInstance.ComponentMass / 1000 + " T");
        }

        private void DisplayPowerPlant((Type powerPlantType, ShipComponentTier tier) powerPlant) {
            (Transform cardComponents, PowerPlant powerPlantInstance) = DisplayShipComponentCard<PowerPlant>(powerPlant.powerPlantType, powerPlant.tier, _powerPlantCardPath);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", powerPlantInstance.ComponentName + " - " + powerPlantInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Capacity", powerPlantInstance.EnergyCapacity + " GW");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Recharge", powerPlantInstance.RechargeRate * 60 + " GW/s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Mass", powerPlantInstance.ComponentMass / 1000 + " T");
        }

        private void DisplayCargoBay((Type cargoBayType, ShipComponentTier tier) cargoBay) {
            (Transform cardComponents, CargoBay cargoBayInstance) = DisplayShipComponentCard<CargoBay>(cargoBay.cargoBayType, cargoBay.tier, _cargoBayCardPath);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", cargoBayInstance.ComponentName + " - " + cargoBayInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Capacity", cargoBayInstance.CargoVolume + " Units");
        }

        private void DisplayShieldCard((Type shieldType, ShipComponentTier tier) shield) {
            (Transform cardComponents, Shield shieldInstance) = DisplayShipComponentCard<Shield>(shield.shieldType, shield.tier, _shieldCardPath);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "ComponentName", shieldInstance.ComponentName + " - " + shieldInstance.ComponentSize);
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Strength", shieldInstance.StrengthCapacity + "GW");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "Recharge", shieldInstance.RechargeRate + "GW/s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "DamageDelay", shieldInstance.DamageRecoveryTime + "s");
            GameObjectHelper.SetGUITextValue(cardComponents.gameObject, "DepletionDelay", shieldInstance.DepletionRecoveryTime + "s");
        }

        private void CreateSelectionMarkers(List<(Transform mountTransform, Transform selectionTransform, string slotName)> shipComponents) {
            // List<(Transform ObjectMountTransform, Transform SelectionMountTransform, string SlotName)> currentSlots = new List<(Transform ObjectMountTransform, Transform SelectionMountTransform, string SlotName)>();
            bool sameSlots = true;
            if (shipComponents.Count == _selectionMarkers.Count) {
                for (int i = 0; i < shipComponents.Count; i++) {
                    SlotSelector selector = _selectionMarkers[i].GetComponent<SlotSelector>();
                    var selectionMarkerDetails = (selector.ObjectMountTransform, selector.SelectionMountTransform, SlotName: selector.SlotName);
                    sameSlots = shipComponents.Select(sc => sc.selectionTransform).ToList().Contains(selectionMarkerDetails.SelectionMountTransform);

                    if (!sameSlots) {
                        break;
                    }
                }
            }
            else {
                sameSlots = false;
            }

            //if same as current not destroy and create new
            if (!sameSlots) {
                foreach (GameObject selectionMarker in _selectionMarkers) {
                    Destroy(selectionMarker);
                }

                _selectionMarkers = new List<GameObject>();

                foreach ((Transform mountTransform, Transform selectionTransform, string slotName) selection in shipComponents) {
                    Transform canvas = _guiGameObject.transform.Find("Canvas");
                    GameObject marker = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_outfittingPath + "ShipComponentMarker"), canvas.Find("SelectionMarkers"));
                    _selectionMarkers.Add(marker);
                    SlotSelector slotSelector = marker.AddComponent<SlotSelector>();
                    slotSelector.Setup(selection.mountTransform, selection.selectionTransform, selection.slotName, this);

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

        private GameObject CreateComponentCard(string cardSpecifier, Type componentType, ShipComponentTier tier) {
            GameObject componentCard = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_outfittingPath + cardSpecifier), GetScrollViewContentContainer().transform);

            CardShipComponentModifier componentModifier = componentCard.AddComponent<CardShipComponentModifier>();

            componentModifier.Setup(componentType, tier, this);
            return componentCard;
        }


        private void ChangeSelectedShipWeapon(Weapon newComponent) {
            if (_selectedSlot != null) {
                bool success = _shipObjectHandler.SetWeaponComponent(_selectedSlot.ObjectMountTransform.name, newComponent);
                if (!success) {
                    Debug.Log("Couldn't attach this component");
                }
            }
            else {
                Debug.Log("Need to select a slot to attach to!");
            }
        }

        private void ChangeSelectedShipMainThruster(MainThruster newComponent) {
            if (_selectedSlot != null) {
                bool success = _shipObjectHandler.SetMainThrusterComponent(_selectedSlot.ObjectMountTransform.gameObject.name, newComponent);
                if (!success) {
                    Debug.Log("Couldn't attach this component");
                }
            }
            else {
                Debug.Log("Need to select a slot to attach to!");
            }
        }

        private void ChangeSelectedShipManoeuvringThruster(ManoeuvringThruster newComponent) {
            if (_selectedSlot != null) {
                bool success = _shipObjectHandler.SetManoeuvringThrusterComponent(newComponent);
                if (!success) {
                    Debug.Log("Couldn't attach this component");
                }
            }
            else {
                Debug.Log("Need to select a slot to attach to!");
            }
        }

        private void ChangeSelectedInternal(InternalComponent newComponent) {
            if (_selectedSlot != null) {
                bool success = _shipObjectHandler.SetInternalComponent(_selectedSlot.ObjectMountTransform.gameObject.name, newComponent);
                if (!success) {
                    Debug.Log("Couldn't attach this component");
                }
            }
            else {
                Debug.Log("Need to select a slot to attach to!");
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

        class SlotSelector : MonoBehaviour, IPointerClickHandler {
            public Transform SelectionMountTransform { get; private set; }
            public Transform ObjectMountTransform { get; private set; }
            public string SlotName { get; private set; }
            private OutfittingGUIController _outfittingGUIController;
            private bool _selected;

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

            public void Setup(Transform objectMountTransform, Transform selectionMountTransform, String slotName, OutfittingGUIController outfittingGUIController) {
                SelectionMountTransform = selectionMountTransform;
                ObjectMountTransform = objectMountTransform;
                this.SlotName = slotName;
                _outfittingGUIController = outfittingGUIController;
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
        }
    }
}