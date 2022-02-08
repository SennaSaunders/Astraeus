using System.Collections.Generic;
using System.Linq;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using Code._Ships;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Storage;
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
        private SlotSelector selectedSlot;
        private List<GameObject> _selectionMarkers = new List<GameObject>();

        private UnityEngine.Camera _camera;

        
        private string _outfittingPath = "GUIPrefabs/Station/Services/Outfitting/";
        private string _outfittingGUIpath = "OutfittingGUI";
        private string _cargoBayCardPath = "CargoBayCard";
        private string _weaponCardPath = "WeaponCard";
        private string _thrusterCardPath = "ThrusterCard";
        private string _powerPlantCardPath = "PowerPlantCard";
        private GameController _gameController;

        private void SetShipObjectHandler() {
            _shipObjectHandler = _guiGameObject.AddComponent<ShipObjectHandler>();
        }
        
        public void StartOutfitting(OutfittingService outfittingService, StationGUIController stationGUIController, GameController gameController) {
            _outfittingService = outfittingService;
            _stationGUIController = stationGUIController;
            _gameController = gameController;
            _stationGUIController.stationGUI.SetActive(false);
            _camera = UnityEngine.Camera.main;
            
            SetupGUI();
            DisplayShip();
            SetupBtns();
        }

        private void SetupHomeBtn() {
            Button homeBtn = GameObject.Find("HomeBtn").GetComponent<Button>();
            homeBtn.onClick.AddListener(ExitOutfitting);
        }

        private void ExitOutfitting() {
            _gameController.RefreshPlayerShip();
            _stationGUIController.stationGUI.SetActive(true);
            Destroy(_guiGameObject);
            Destroy(this);
        }

        private void DisplayShip() {
            _shipObjectHandler.ManagedShip = GameController._currentShip; //needs to be changed to the player's current ship
            
            _shipObjectHandler.CreateShip(GameObject.Find("ShipPanel").transform);

            GameObject shipObject = _shipObjectHandler.ManagedShip.ShipObject;
            shipObject.transform.position = _shipObjectHandler.ManagedShip.ShipHull.OutfittingPosition;
            shipObject.transform.rotation = _shipObjectHandler.ManagedShip.ShipHull.OutfittingRotation;
            AddDraggableToShip();
        }

        

        private void SetupGUI() {
            _guiGameObject = GameController._prefabHandler.instantiateObject(GameController._prefabHandler.loadPrefab(_outfittingPath + _outfittingGUIpath));
            SetShipObjectHandler();
        }

        private Transform GetScrollViewContentContainer() {
            return  _guiGameObject.transform.Find("Canvas/MainPanel/ComponentsPanel/ComponentsInteractablePanel/ComponentScrollView/Viewport/ComponentContent");
        }

        private void SetupBtns() {
            SetupComponentBtns();
            SetupHomeBtn();
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
            GameObject shipPanel = GameObject.Find("ShipPanel");
            ShipRotatable shipRotatable = shipPanel.AddComponent<ShipRotatable>();
            shipRotatable.rotateableObject = _shipObjectHandler.ManagedShip.ShipObject;
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
        
        private void ChangeSelectedInternal(InternalComponent newComponent) {
            if (selectedSlot != null) {
                bool success = _shipObjectHandler.SetInternalComponent(selectedSlot._objectMountTransform.gameObject.name, newComponent);
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
                else if (_shipComponent.GetType().IsSubclassOf(typeof(InternalComponent))) {
                    _outfittingGUIController.ChangeSelectedInternal((InternalComponent)_shipComponent);
                }
            }
        }

        class SlotSelector : MonoBehaviour, IPointerClickHandler {
            public Transform _selectionMountTransform { get; private set; }
            public Transform _objectMountTransform { get; private set; }
            private OutfittingGUIController _outfittingGUIController;
            private bool selected = false;

            private void Awake() {
                ChangeColourNotSelected();
            }

            private void Update() {
                PositionSelf();
                CheckSelected();
            }

            private void CheckSelected() {
                if (selected) {
                    if (_outfittingGUIController.selectedSlot != this) {
                        selected = false;
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
                _selectionMountTransform = selectionMountTransform;
                _objectMountTransform = objectMountTransform;
                _outfittingGUIController = outfittingGUIController;
            }

            public void OnPointerClick(PointerEventData eventData) {
                _outfittingGUIController.selectedSlot = this;
                selected = true;
                ChangeColourSelected();
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