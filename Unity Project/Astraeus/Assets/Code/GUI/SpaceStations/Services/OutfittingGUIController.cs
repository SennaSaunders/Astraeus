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
        private string _outfittingGUIPath = "OutfittingGUI";
        
        
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
            _outfittingService = outfittingService;
            _stationGUIController = stationGUIController;
            _gameController = gameController;
            _stationGUIController.stationGUI.SetActive(false);
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
            
            _shipObjectHandler.CreateShip(GameObject.Find("ShipPanel").transform);

            GameObject shipObject = _shipObjectHandler.ManagedShip.ShipObject;
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
            _guiGameObject = GameController._prefabHandler.instantiateObject(GameController._prefabHandler.loadPrefab(_outfittingPath + _outfittingGUIPath));
            SetShipObjectHandler();
        }

        private Transform GetScrollViewContentContainer() {
            return  GameObject.Find("ComponentContent").transform;
        }

        private void SetupBtns() {
            SetupComponentBtns();
            SetupHomeBtn();
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
            thrustersBtn.Select();//defaults to thruster upon opening outfitting
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
            ClearScrollView();
            DisplayManoeuvringThrusterComponents();
            var manoeuvringThrusterComponents = _shipObjectHandler.ManoeuvringThrusterComponents;
            List<(Transform, Transform, string)> slots = new List<(Transform, Transform, string)>(){ (manoeuvringThrusterComponents.selectionTransform, manoeuvringThrusterComponents.selectionTransform, manoeuvringThrusterComponents.slotName)};
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
        private void DisplayComponents<T>(Action<T> displayFunc) {
            List<T> components = _outfittingService.GetComponentsOfType<T>();
            foreach (T component in components) {
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
            DisplayComponents<ManoeuvringThruster>(DisplayThruster);
        }

        private void DisplayMainThrusterComponents() {
            DisplayComponents<MainThruster>(DisplayThruster);
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

        private void DisplayThruster(Thruster thruster) {
            GameObject thrusterCard = CreateComponentCard(_thrusterCardPath, thruster);

            Transform cardComponents = thrusterCard.transform.Find("Details");

            TextMeshProUGUI title = cardComponents.Find("ComponentName").GetComponent<TextMeshProUGUI>();
            title.text = thruster.ComponentName + " - " + thruster.ComponentSize;

            TextMeshProUGUI force = cardComponents.Find("Force").GetComponent<TextMeshProUGUI>();
            force.text = thruster.Force + " N";

            TextMeshProUGUI powerDraw = cardComponents.Find("PowerDraw").GetComponent<TextMeshProUGUI>();
            powerDraw.text = thruster.PowerDraw + " GW/s";

            TextMeshProUGUI mass = cardComponents.Find("Mass").GetComponent<TextMeshProUGUI>();
            mass.text = (thruster.ComponentMass / 1000) + " T";
        }

        private void DisplayWeaponCard(Weapon externalComponent) {
            GameObject weaponCard = CreateComponentCard(_weaponCardPath, externalComponent);

            Transform cardComponents = weaponCard.transform.Find("Details");

            TextMeshProUGUI title = cardComponents.Find("ComponentName").GetComponent<TextMeshProUGUI>();
            title.text = externalComponent.ComponentName + " - " + externalComponent.ComponentSize;

            TextMeshProUGUI damage = cardComponents.Find("Damage").GetComponent<TextMeshProUGUI>();
            damage.text = externalComponent.Damage + " DMG";

            TextMeshProUGUI rateOfFire = cardComponents.Find("RoF").GetComponent<TextMeshProUGUI>();
            rateOfFire.text = externalComponent.FireRate * 60 + " RPM";

            TextMeshProUGUI mass = cardComponents.Find("Mass").GetComponent<TextMeshProUGUI>();
            mass.text = (externalComponent.ComponentMass / 1000) + " T";
        }

        private void DisplayPowerPlant(PowerPlant powerPlant) {
            GameObject powerPlantCard = CreateComponentCard(_powerPlantCardPath, powerPlant);

            Transform cardComponents = powerPlantCard.transform.Find("Details");

            TextMeshProUGUI title = cardComponents.Find("ComponentName").GetComponent<TextMeshProUGUI>();
            title.text = powerPlant.ComponentName + " - " + powerPlant.ComponentSize;

            TextMeshProUGUI capacity = cardComponents.Find("Capacity").GetComponent<TextMeshProUGUI>();
            capacity.text = powerPlant.EnergyCapacity + " GW";

            TextMeshProUGUI rechargeRate = cardComponents.Find("Recharge").GetComponent<TextMeshProUGUI>();
            rechargeRate.text = powerPlant.RechargeRate * 60 + " GW/s";

            TextMeshProUGUI mass = cardComponents.Find("Mass").GetComponent<TextMeshProUGUI>();
            mass.text = (powerPlant.ComponentMass / 1000) + " T";
        }

        private void DisplayCargoBay(CargoBay cargoBay) {
            GameObject cargoBayCard = CreateComponentCard(_cargoBayCardPath, cargoBay);

            Transform cardComponents = cargoBayCard.transform.Find("Details");

            TextMeshProUGUI title = cardComponents.Find("ComponentName").GetComponent<TextMeshProUGUI>();
            title.text = cargoBay.ComponentName + " - " + cargoBay.ComponentSize;

            TextMeshProUGUI capacity = cardComponents.Find("Capacity").GetComponent<TextMeshProUGUI>();
            capacity.text = cargoBay.CargoVolume + " Units";
        }

        private void DisplayShieldCard(Shield shield) {
            GameObject shieldCard = CreateComponentCard(_shieldCardPath, shield);
            Transform cardComponents = shieldCard.transform.Find("Details");
            
            TextMeshProUGUI title = cardComponents.Find("ComponentName").GetComponent<TextMeshProUGUI>();
            title.text = shield.ComponentName + " - " + shield.ComponentSize;
            
            TextMeshProUGUI strength = cardComponents.Find("Strength").GetComponent<TextMeshProUGUI>();
            strength.text = shield.StrengthCapacity + "GW";
            
            TextMeshProUGUI recharge = cardComponents.Find("Recharge").GetComponent<TextMeshProUGUI>();
            recharge.text = shield.RechargeRate + "GW/s";
            
            TextMeshProUGUI damageDelay = cardComponents.Find("DamageDelay").GetComponent<TextMeshProUGUI>();
            damageDelay.text = shield.DamageRecoveryTime + "s";
            
            TextMeshProUGUI depletionDelay = cardComponents.Find("DepletionDelay").GetComponent<TextMeshProUGUI>();
            depletionDelay.text = shield.DepletionRecoveryTime + "s";
        }
        
        private void CreateSelectionMarkers(List<(Transform, Transform, string)> shipComponents) {
            foreach (GameObject selectionMarker in _selectionMarkers) {
                Destroy(selectionMarker);
            }

            _selectionMarkers = new List<GameObject>();

            foreach ((Transform mountTransform, Transform selectionTransform, string  slotName) selection in shipComponents) {
                Transform canvas = _guiGameObject.transform.Find("Canvas");
                GameObject marker = GameController._prefabHandler.instantiateObject(GameController._prefabHandler.loadPrefab(_outfittingPath + "ShipComponentMarker"), canvas.Find("SelectionMarkers"));
                _selectionMarkers.Add(marker);
                SlotSelector slotSelector = marker.AddComponent<SlotSelector>();
                slotSelector.Setup(selection.mountTransform, selection.selectionTransform, this);

                GameObject markerText = GameController._prefabHandler.instantiateObject(GameController._prefabHandler.loadPrefab(_outfittingPath + "ShipComponentMarkerName"), marker.transform);
                TextMeshProUGUI slotName = markerText.transform.GetComponentInChildren<TextMeshProUGUI>();
                slotName.text = selection.slotName;

            }
        }

        private void ClearScrollView() {
            Transform container = GetScrollViewContentContainer();

            for (int i = container.childCount; i > 0; i--) {
                Destroy(container.GetChild(i - 1).gameObject);
            }
        }

        private GameObject CreateComponentCard(string cardSpecifier, ShipComponent shipComponent) {
            GameObject componentCard = GameController._prefabHandler.instantiateObject(GameController._prefabHandler.loadPrefab(_outfittingPath + cardSpecifier), GetScrollViewContentContainer().transform);

            CardShipComponentModifier componentModifier = componentCard.AddComponent<CardShipComponentModifier>();

            componentModifier.Setup(shipComponent, this);
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
            private ShipComponent _shipComponent;
            private OutfittingGUIController _outfittingGUIController;

            public void Setup(ShipComponent shipComponent, OutfittingGUIController outfittingGUIController) {
                _shipComponent = shipComponent;
                _outfittingGUIController = outfittingGUIController;
            }

            public void OnPointerClick(PointerEventData eventData) {
                Debug.Log("Trying to assign: " + _shipComponent.ComponentName + " - " + _shipComponent.ComponentSize);
                if (_shipComponent.GetType().IsSubclassOf(typeof(Weapon))) {
                    _outfittingGUIController.ChangeSelectedShipWeapon((Weapon)_shipComponent);
                }
                else if (_shipComponent.GetType().IsSubclassOf(typeof(MainThruster))) {
                    _outfittingGUIController.ChangeSelectedShipMainThruster((MainThruster)_shipComponent);
                } else if (_shipComponent.GetType() == typeof(ManoeuvringThruster)) {
                    _outfittingGUIController.ChangeSelectedShipManoeuvringThruster((ManoeuvringThruster)_shipComponent);
                }
                else if (_shipComponent.GetType().IsSubclassOf(typeof(InternalComponent))) {
                    _outfittingGUIController.ChangeSelectedInternal((InternalComponent)_shipComponent);
                }
            }
        }

        class SlotSelector : MonoBehaviour, IPointerClickHandler {
            public Transform SelectionMountTransform { get; private set; }
            public Transform ObjectMountTransform { get; private set; }
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
                image.color = new Color(255/255f,185/255f,0);
            }
            
            private void ChangeColourSelected() {
                var image = gameObject.GetComponent<SVGImage>();
                image.color = new Color(53/255f,157/255f,255/255f);
            }

            public void Setup(Transform objectMountTransform, Transform selectionMountTransform, OutfittingGUIController outfittingGUIController) {
                SelectionMountTransform = selectionMountTransform;
                ObjectMountTransform = objectMountTransform;
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