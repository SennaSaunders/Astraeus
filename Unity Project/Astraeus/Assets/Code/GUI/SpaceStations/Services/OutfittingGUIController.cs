using System;
using System.Collections.Generic;
using System.Linq;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._Ships;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Storage;
using Code._Utility;
using Code.GUI.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class OutfittingGUIController : MonoBehaviour {
        private OutfittingService _outfittingService;
        private GameObject _guiGameObject;
        private PrefabHandler _prefabHandler;
        private ShipObjectHandler _shipObjectHandler;
        private SlotSelector selectedSlot;
        private List<GameObject> _selectionMarkers = new List<GameObject>();

        private UnityEngine.Camera _camera;

        private string _outfittingGUIPath = "GUIPrefabs/SpaceStation/Services/Outfitting/";
        private string _cargoBayCardPath = "CargoBayCard";
        private string _weaponCardPath = "WeaponCard";
        private string _thrusterCardPath = "ThrusterCard";
        private string _powerPlantCardPath = "PowerPlantCard";

        private void Start() {
            _camera = UnityEngine.Camera.main;
            SetHandlers();
            SetGUIGameObject();
            SetupOutfittingService();
            DisplayShip();
            AddDraggableToShip();
            SetupComponentBtns();
        }

        private void SetHandlers() {
            _prefabHandler = gameObject.AddComponent<PrefabHandler>();
            _shipObjectHandler = gameObject.AddComponent<ShipObjectHandler>();
        }

        private void DisplayShip() {
            _shipObjectHandler.ManagedShip = _outfittingService.Ships[1]; //needs to be changed to the player's current ship
            _shipObjectHandler.CreateShip();

            GameObject shipObject = _shipObjectHandler.ShipObject;
            shipObject.transform.position = _shipObjectHandler.ManagedShip.ShipHull.outfittingPosition;
            shipObject.transform.rotation = _shipObjectHandler.ManagedShip.ShipHull.outfittingRotation;
        }

        private void SetupOutfittingService() {
            _outfittingService = gameObject.AddComponent<OutfittingService>();
            _outfittingService.SetAllAvailableComponents();
            _outfittingService.AddAllShips();
        }

        private void SetGUIGameObject() {
            _guiGameObject = GameObject.Find("OutfittingGUI");
        }

        private GameObject GetScrollViewContentContainer() {
            GameObject contentContainer = _guiGameObject.transform.Find("Canvas/MainPanel/ComponentsPanel/ComponentsInteractablePanel/ComponentScrollView/Viewport/ComponentContent").gameObject;
            return contentContainer;
        }

        private void SetupComponentBtns() {
            Button thrustersBtn = GameObject.Find("ThrustersBtn").GetComponentInChildren<Button>();
            thrustersBtn.onClick.AddListener(delegate { ThrusterBtnClick(); });
            thrustersBtn.Select();
            ThrusterBtnClick();
            Button weaponsBtn = GameObject.Find("WeaponsBtn").GetComponentInChildren<Button>();
            weaponsBtn.onClick.AddListener(delegate { WeaponBtnClick(); });
            Button internalBtn = GameObject.Find("InternalBtn").GetComponentInChildren<Button>();
            internalBtn.onClick.AddListener(delegate { InternalBtnClick(); });
        }

        private void AddDraggableToShip() {
            ShipRotatable shipRotatable = _guiGameObject.AddComponent<ShipRotatable>();
            shipRotatable.rotateableObject = _shipObjectHandler.ShipObject;
        }

        private void CreateSelectionMarkers(List<(Transform, Transform, string)> shipComponents) {
            foreach (GameObject selectionMarker in _selectionMarkers) {
                Destroy(selectionMarker);
            }

            _selectionMarkers = new List<GameObject>();

            foreach ((Transform mountTransform, Transform selectionTransform, string  slotName) selection in shipComponents) {
                Transform canvas = _guiGameObject.transform.Find("Canvas");
                GameObject marker = _prefabHandler.instantiateObject(_prefabHandler.loadPrefab(_outfittingGUIPath + "ShipComponentMarker"), canvas.Find("SelectionMarkers"));
                _selectionMarkers.Add(marker);
                SlotSelector slotSelector = marker.AddComponent<SlotSelector>();
                slotSelector.Setup(selection.mountTransform, selection.selectionTransform, this);

                GameObject markerText = _prefabHandler.instantiateObject(_prefabHandler.loadPrefab(_outfittingGUIPath + "ShipComponentMarkerName"), marker.transform);
                TextMeshProUGUI slotName = markerText.transform.GetComponentInChildren<TextMeshProUGUI>();
                slotName.text = selection.slotName;

            }
        }

        private void ThrusterBtnClick() {
            ClearScrollView();
            DisplayThrusterComponents();
            CreateSelectionMarkers(_shipObjectHandler.ThrusterComponents);
        }

        private void WeaponBtnClick() {
            ClearScrollView();
            DisplayWeaponComponents();
            CreateSelectionMarkers(_shipObjectHandler.WeaponComponents);
        }

        private void InternalBtnClick() {
            ClearScrollView();
            DisplayInternalComponents();
            CreateSelectionMarkers(_shipObjectHandler.InternalComponents);
        }

        private void ClearScrollView() {
            GameObject container = GetScrollViewContentContainer();

            for (int i = container.transform.childCount; i > 0; i--) {
                Destroy(container.transform.GetChild(i - 1).gameObject);
            }
        }

        private GameObject CreateComponentCard(string cardSpecifier, ShipComponent shipComponent) {
            GameObject componentCard = _prefabHandler.instantiateObject(_prefabHandler.loadPrefab(_outfittingGUIPath + cardSpecifier), GetScrollViewContentContainer().transform);

            CardShipComponentModifier componentModifier = componentCard.AddComponent<CardShipComponentModifier>();

            componentModifier.Setup(shipComponent, this);
            return componentCard;
        }

        private void DisplayThrusterComponents() {
            List<Thruster> thrusters = _outfittingService.AvailableComponents.Where(c => c.GetType().IsSubclassOf(typeof(Thruster))).Cast<Thruster>().ToList();

            foreach (Thruster thruster in thrusters) {
                DisplayThruster(thruster);
            }
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

        private void DisplayWeaponComponents() {
            List<Weapon> weapons = _outfittingService.AvailableComponents.Where(c => c.GetType().IsSubclassOf(typeof(_Ships.ShipComponents.ExternalComponents.Weapons.Weapon))).Cast<_Ships.ShipComponents.ExternalComponents.Weapons.Weapon>().ToList();

            foreach (Weapon weapon in weapons) {
                DisplayWeapon(weapon);
            }
        }

        private void DisplayWeapon(Weapon externalComponent) {
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

        private void DisplayInternalComponents() {
            List<PowerPlant> powerPlants = _outfittingService.AvailableComponents.Where(c => c.GetType().IsSubclassOf(typeof(PowerPlant))).Cast<PowerPlant>().ToList();

            foreach (PowerPlant powerPlant in powerPlants) {
                DisplayPowerPlant(powerPlant);
            }

            List<CargoBay> cargoBays = _outfittingService.AvailableComponents.Where(c => c.GetType() == typeof(CargoBay)).Cast<CargoBay>().ToList();

            foreach (CargoBay cargoBay in cargoBays) {
                DisplayCargoBay(cargoBay);
            }
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


        private void ChangeSelectedShipWeapon(Weapon newComponent) {
            if (selectedSlot != null) {
                bool success = _shipObjectHandler.SetWeaponComponent(selectedSlot._objectMountTransform.name, newComponent);
                if (!success) {
                    Debug.Log("Couldn't attach this component");
                }
            }
            else {
                Debug.Log("Need to select a slot to attach to!");
            }
        }

        private void ChangeSelectedShipMainThruster(MainThruster newComponent) {
            if (selectedSlot != null) {
                bool success = _shipObjectHandler.SetMainThrusterComponent(selectedSlot._objectMountTransform.gameObject.name, newComponent);
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
                }
            }
        }

        class SlotSelector : MonoBehaviour, IPointerClickHandler {
            public Transform _selectionMountTransform { get; private set; }
            public Transform _objectMountTransform { get; private set; }
            private OutfittingGUIController _outfittingGUIController;

            private void Update() {
                PositionSelf();
            }

            public void Setup(Transform objectMountTransform, Transform selectionMountTransform, OutfittingGUIController outfittingGUIController) {
                _selectionMountTransform = selectionMountTransform;
                _objectMountTransform = objectMountTransform;
                _outfittingGUIController = outfittingGUIController;
            }

            public void OnPointerClick(PointerEventData eventData) {
                _outfittingGUIController.selectedSlot = this;
                Debug.Log("Set selected slot to: " + _objectMountTransform.name);
            }

            private void PositionSelf() {
                Vector2 screenSpacePos = _outfittingGUIController._camera.WorldToScreenPoint(_selectionMountTransform.position);
                transform.rotation = Quaternion.LookRotation(-_outfittingGUIController._camera.transform.position);
                transform.position = screenSpacePos;
            }
        }
    }
}