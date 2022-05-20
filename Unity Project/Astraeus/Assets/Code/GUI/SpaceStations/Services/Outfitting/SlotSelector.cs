using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Ships.ShipComponents.InternalComponents.JumpDrives;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code._Ships.ShipComponents.InternalComponents.Storage;
using Code._Utility;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.GUI.SpaceStations.Services.Outfitting {
    public class SlotSelector : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        private Transform SelectionMountTransform { get; set; }
        private Transform ObjectMountTransform { get; set; }
        public ShipComponentType _componentType;
        private OutfittingGUIController _outfittingGUIController;
        private bool _selected;
        private GameObject _hoverInfo;
        public ShipComponent CurrentShipComponent;
        public ShipComponent InitialShipComponent;

        public void Setup(Transform objectMountTransform, Transform selectionMountTransform, ShipComponentType componentType, OutfittingGUIController outfittingGUIController, ShipComponent shipComponent) {
            SelectionMountTransform = selectionMountTransform;
            ObjectMountTransform = objectMountTransform;
            _componentType = componentType;
            _outfittingGUIController = outfittingGUIController;
            CurrentShipComponent = shipComponent;
            ChangeSlotText();
            InitialShipComponent = shipComponent;
        }

        public enum AssignmentStatus {
            SlotSizeError,
            CargoFilledError,
            Success
        }

        public AssignmentStatus ChangeComponent(ShipComponent newComponent) {
            bool success = false;
            if (CurrentShipComponent != null) {
                if (CurrentShipComponent.GetType() == typeof(CargoBay)) {
                    if (((CargoBay)CurrentShipComponent).StoredCargo.Count > 0) {
                        return AssignmentStatus.CargoFilledError;
                    }
                }
            }

            if (newComponent.GetType().IsSubclassOf(typeof(Weapon))) {
                success = ChangeWeapon((Weapon)newComponent);
            }
            else if (newComponent.GetType().IsSubclassOf(typeof(MainThruster))) {
                success = ChangeMainThruster((MainThruster)newComponent);
            }
            else if (newComponent.GetType() == (typeof(ManoeuvringThruster))) {
                success = ChangeManThruster((ManoeuvringThruster)newComponent);
            }
            else if (newComponent.GetType().IsSubclassOf(typeof(InternalComponent))) {
                success = ChangeInternal((InternalComponent)newComponent);
            }

            return success ? AssignmentStatus.Success : AssignmentStatus.SlotSizeError;
        }

        private bool ChangeInternal(InternalComponent newComponent) {
            return _outfittingGUIController.shipObjectHandler.SetInternalComponent(ObjectMountTransform.gameObject.name, newComponent);
        }

        private bool ChangeWeapon(Weapon newComponent) {
            return _outfittingGUIController.shipObjectHandler.SetWeaponComponent(ObjectMountTransform.gameObject.name, newComponent);
        }

        private bool ChangeManThruster(ManoeuvringThruster newComponent) {
            return _outfittingGUIController.shipObjectHandler.SetManoeuvringThrusterComponent(newComponent);
        }

        private bool ChangeMainThruster(MainThruster newComponent) {
            return _outfittingGUIController.shipObjectHandler.SetMainThrusterComponent(ObjectMountTransform.gameObject.name, newComponent);
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
                if (_outfittingGUIController.SelectedSlot != this) {
                    _selected = false;
                    ChangeColourNotSelected();
                }
            }
        }

        public void ChangeSlotText() {
            string componentName = CurrentShipComponent != null ? CurrentShipComponent.ComponentName + " - " + CurrentShipComponent.ComponentSize : "Empty";
            GameObjectHelper.SetGUITextValue(gameObject, "ShipComponentMarkerName", componentName);
        }

        private void ChangeColourNotSelected() {
            var image = gameObject.GetComponent<SVGImage>();
            image.color = OutfittingGUIController.BrightYellow;
        }

        private void ChangeColourSelected() {
            var image = gameObject.GetComponent<SVGImage>();
            image.color = OutfittingGUIController.BrightBlue;
        }


        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Left) { //select slot
                _outfittingGUIController.SelectedSlot = this;
                _selected = true;
                ChangeColourSelected();
                Debug.Log("Set selected slot to: " + ObjectMountTransform.name);
            }
            else if (eventData.button == PointerEventData.InputButton.Right) { //clear slot
                if (CurrentShipComponent != null) {
                    if (CurrentShipComponent.GetType() == typeof(CargoBay)) {
                        if (((CargoBay)CurrentShipComponent).StoredCargo.Count > 0) {
                            _outfittingGUIController.CargoBayFilledError();
                        }
                        else {
                            RemoveComponent();
                            
                            _outfittingGUIController.SetShipStats();
                        }
                    }
                    else {
                        RemoveComponent();
                        _outfittingGUIController.SetShipStats();
                    }
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Middle) { //reset slot
                ResetSlot();
                _outfittingGUIController.SetShipStats();
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
            ChangeSlotText();
            
            _outfittingGUIController.SetCreditsChange();
            _outfittingGUIController.SetComponentWarnings();
            _outfittingGUIController.SetShipStats();
        }

        private void RemoveComponent() {
            if (CurrentShipComponent != null) {
                if (CurrentShipComponent.GetType().IsSubclassOf(typeof(Weapon))) {
                    _outfittingGUIController.shipObjectHandler.SetWeaponComponent(ObjectMountTransform.gameObject.name, null);
                }
                else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(MainThruster))) {
                    _outfittingGUIController.shipObjectHandler.SetMainThrusterComponent(ObjectMountTransform.gameObject.name, null);
                }
                else if (CurrentShipComponent.GetType() == (typeof(ManoeuvringThruster))) {
                    _outfittingGUIController.shipObjectHandler.SetManoeuvringThrusterComponent(null);
                }
                else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(InternalComponent))) {
                    _outfittingGUIController.shipObjectHandler.SetInternalComponent(ObjectMountTransform.gameObject.name, null);
                }

                CurrentShipComponent = null;
                ChangeSlotText();
            }

            _outfittingGUIController.SetCreditsChange();
            _outfittingGUIController.SetComponentWarnings();
        }

        private void PositionSelf() {
            Vector2 screenSpacePos = _outfittingGUIController.Camera.WorldToScreenPoint(SelectionMountTransform.position);
            Transform selectorTransform = transform;
            selectorTransform.rotation = Quaternion.LookRotation(-_outfittingGUIController.Camera.transform.position);
            selectorTransform.position = screenSpacePos;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (CurrentShipComponent != null) {
                Debug.Log("Hovering: " + CurrentShipComponent.ComponentName);

                if (CurrentShipComponent.GetType().IsSubclassOf(typeof(Thruster))) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.ThrusterCardPath, _outfittingGUIController.HoverInfo.transform);
                    _outfittingGUIController.SetupThrusterCard(_hoverInfo.transform, (Thruster)CurrentShipComponent);
                }
                else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(Weapon))) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.WeaponCardPath, _outfittingGUIController.HoverInfo.transform);
                    _outfittingGUIController.SetupWeaponCard(_hoverInfo.transform, (Weapon)CurrentShipComponent);
                }
                else if (CurrentShipComponent.GetType() == typeof(CargoBay)) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.CargoBayCardPath, _outfittingGUIController.HoverInfo.transform);
                    _outfittingGUIController.SetupCargoBayCard(_hoverInfo.transform, (CargoBay)CurrentShipComponent);
                }
                else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(PowerPlant))) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.PowerPlantCardPath, _outfittingGUIController.HoverInfo.transform);
                    _outfittingGUIController.SetupPowerPlantCard(_hoverInfo.transform, (PowerPlant)CurrentShipComponent);
                }
                else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(Shield))) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.ShieldCardPath, _outfittingGUIController.HoverInfo.transform);
                    _outfittingGUIController.SetupShieldCard(_hoverInfo.transform, (Shield)CurrentShipComponent);
                }
                else if (CurrentShipComponent.GetType() == typeof(JumpDrive)) {
                    _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.JumpDriveCardPath, _outfittingGUIController.HoverInfo.transform);
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