using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Ships.ShipComponents.InternalComponents.JumpDrives;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code._Ships.ShipComponents.InternalComponents.Storage;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.GUI.SpaceStations.Services.Outfitting {
    public class SlotSelector : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
            private Transform SelectionMountTransform { get; set; }
            private Transform ObjectMountTransform { get; set; }
            private string SlotName { get; set; }
            public ShipComponentType _componentType;
            private OutfittingGUIController _outfittingGUIController;
            private bool _selected;
            private GameObject _hoverInfo;
            public ShipComponent CurrentShipComponent;
            public ShipComponent InitialShipComponent;

            public void Setup(Transform objectMountTransform, Transform selectionMountTransform, string slotName, ShipComponentType componentType, OutfittingGUIController outfittingGUIController, ShipComponent shipComponent) {
                SelectionMountTransform = selectionMountTransform;
                ObjectMountTransform = objectMountTransform;
                SlotName = slotName;
                _componentType = componentType;
                _outfittingGUIController = outfittingGUIController;
                CurrentShipComponent = shipComponent;
                InitialShipComponent = shipComponent;
            }
            
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
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.ThrusterCardPath, _outfittingGUIController._hoverInfo.transform);
                        _outfittingGUIController.SetupThrusterCard(_hoverInfo.transform, (Thruster)CurrentShipComponent);
                    }
                    else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(Weapon))) {
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.WeaponCardPath, _outfittingGUIController._hoverInfo.transform);
                        _outfittingGUIController.SetupWeaponCard(_hoverInfo.transform, (Weapon)CurrentShipComponent);
                    }
                    else if (CurrentShipComponent.GetType() == typeof(CargoBay)) {
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.CargoBayCardPath, _outfittingGUIController._hoverInfo.transform);
                        _outfittingGUIController.SetupCargoBayCard(_hoverInfo.transform, (CargoBay)CurrentShipComponent);
                    }
                    else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(PowerPlant))) {
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.PowerPlantCardPath, _outfittingGUIController._hoverInfo.transform);
                        _outfittingGUIController.SetupPowerPlantCard(_hoverInfo.transform, (PowerPlant)CurrentShipComponent);
                    }
                    else if (CurrentShipComponent.GetType().IsSubclassOf(typeof(Shield))) {
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.ShieldCardPath, _outfittingGUIController._hoverInfo.transform);
                        _outfittingGUIController.SetupShieldCard(_hoverInfo.transform, (Shield)CurrentShipComponent);
                    }
                    else if (CurrentShipComponent.GetType() == typeof(JumpDrive)) {
                        _hoverInfo = _outfittingGUIController.CreateComponentCard(OutfittingGUIController.JumpDriveCardPath, _outfittingGUIController._hoverInfo.transform);
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