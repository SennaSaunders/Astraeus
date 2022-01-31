using System.Collections.Generic;
using System.Linq;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Utility;
using UnityEngine;

namespace Code._Ships {
    public class ShipObjectHandler : MonoBehaviour {
        private PrefabHandler _prefabHandler;
        public Ship ManagedShip { get; set; }
        public GameObject ShipObject { get; private set; }

        public List<(Transform mountTransform, Transform selectionTransform, string slotName)> WeaponComponents = new List<(Transform mountTransform, Transform selectionTransform, string slotName)>();
        public List<(Transform mountTransform, Transform selectionTransform, string slotName)> ThrusterComponents = new List<(Transform slotTransform, Transform selectionTransform, string slotName)>();
        public List<(Transform mountTransform, Transform selectionTransform, string slotName)> InternalComponents = new List<(Transform mountTransform, Transform selectionTransform, string slotName)>();

        private void Awake() {
            _prefabHandler = gameObject.AddComponent<PrefabHandler>();
        }

        public void CreateShip() {
            CreateHull();
            CreateWeaponComponents();
            CreateMainThrusterComponents();
            CreateInternalComponents();
        }

        private void CreateInternalComponents() {
            ShipComponentType componentType = ShipComponentType.Internal;
            List<(ShipComponentType componentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)> componentSlots = ManagedShip.ShipHull.InternalComponents.Where(c => c.componentType == componentType).ToList();

            foreach (var slot in componentSlots) {
                SetInternalComponent(slot.parentTransformName, slot.concreteComponent);
            }
            
        }

        private void CreateHull() {
            ShipObject = _prefabHandler.instantiateObject(ManagedShip.ShipHull.HullObject);
        }
        
        
        private void CreateExternalShipComponent(Transform parent, ExternalComponent component, float scale) {
            if (parent.childCount > 0) {//clear already assigned components
                for (int i = parent.childCount; i > 0; i--) {
                    Destroy(parent.GetChild(i-1).gameObject);
                }
            }

            string path = component.GetFullPath();

            GameObject newComponentObject = _prefabHandler.instantiateObject(_prefabHandler.loadPrefab(path), parent);
            newComponentObject.transform.localScale = new Vector3(scale, scale, scale);
        }
        
        private Transform GetSelectionTransform(Transform parent) {
            List<Transform> selectionTransforms = new List<Transform>();
            for (int i = 0; i < parent.childCount; i++) {
                if (parent.GetChild(i).name != "ComponentHolder") {
                    selectionTransforms.Add(parent.GetChild(i));
                }
            }

            if (selectionTransforms.Count == 0) {
                return parent;
            }

            return selectionTransforms[0];
        }
        
        private void CreateWeaponComponents() {
            ShipComponentType componentType = ShipComponentType.Weapon;
            List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> componentSlots = ManagedShip.ShipHull.WeaponComponents.Where(c => c.componentType == componentType).ToList();
            
            foreach (var slot in componentSlots) {
                SetWeaponComponent(slot.parentTransformName, slot.concreteComponent);
            }
        }

        public bool SetWeaponComponent(string parentName, Weapon weapon) {
            //get ShipComponent slot
            (ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName) slot = ManagedShip.ShipHull.WeaponComponents.Find(w => w.parentTransformName == parentName);
            

            //get object transform
            Transform slotTransform = MapPrefabTransformStringToTransformObject(parentName);
            Transform selectionTransform = GetSelectionTransform(slotTransform);
            if (WeaponComponents.Select(wc => wc.mountTransform).Contains(slotTransform)) {
                WeaponComponents.Remove(WeaponComponents.Find(wc => wc.mountTransform == slotTransform));
            }
            WeaponComponents.Add((slotTransform,selectionTransform, "Weapon - " + slot.maxSize));

            if (slotTransform != null&&weapon!=null) {
                if (weapon.ComponentType == slot.componentType && weapon.ComponentSize <= slot.maxSize) {
                     Transform holderTransform = slotTransform.Find("ComponentHolder");
                    if (holderTransform == null) {
                        GameObject componentHolder = new GameObject("ComponentHolder");
                        componentHolder.transform.SetParent(slotTransform, false);
                        holderTransform = componentHolder.transform;
                    }

                    //assign & instantiate new component
                    int slotIndex = ManagedShip.ShipHull.WeaponComponents.IndexOf(slot);
                    slot.concreteComponent = weapon;
                    ManagedShip.ShipHull.WeaponComponents[slotIndex] = slot;
                    
                    CreateExternalShipComponent(holderTransform, slot.concreteComponent, ShipComponent.GetTierMultipliedValue(1, slot.concreteComponent.ComponentSize));
                    
                    return true;
                }
            }

            return false;
        }

        private void CreateMainThrusterComponents() {
            ShipComponentType componentType = ShipComponentType.MainThruster;
            List<(ShipComponentType componentType, ShipComponentTier maxSize, Thruster concreteComponent, string parentTransformName, bool needsBracket)> componentSlots = ManagedShip.ShipHull.ThrusterComponents.Where(c => c.componentType == componentType).ToList();
            
            foreach (var slot in componentSlots) {
                SetMainThrusterComponent(slot.parentTransformName, (MainThruster)slot.concreteComponent);
            }
        }
        
        public bool SetMainThrusterComponent(string parentName, MainThruster mainThruster) {
            //get ShipComponent slot
            (ShipComponentType componentType, ShipComponentTier maxSize, Thruster concreteComponent, string parentTransformName, bool needsBracket) slot = ManagedShip.ShipHull.ThrusterComponents.Find(ec => ec.parentTransformName == parentName);

            //get object transform
            Transform slotTransform = MapPrefabTransformStringToTransformObject(parentName);
            Transform selectionTransform = GetSelectionTransform(slotTransform);
            if (ThrusterComponents.Select(tc => tc.mountTransform).Contains(slotTransform)) {
                ThrusterComponents.Remove(ThrusterComponents.Find(tc => tc.mountTransform == slotTransform));
            }
            ThrusterComponents.Add((slotTransform, selectionTransform, "Main Thruster - " + slot.maxSize));

            if (slotTransform != null&&mainThruster!=null) {
                if (mainThruster.ComponentType == slot.componentType && mainThruster.ComponentSize <= slot.maxSize) {
                    Transform holderTransform = slotTransform.Find("ComponentHolder");
                    if (holderTransform == null) {
                        GameObject componentHolder = new GameObject("ComponentHolder");
                        componentHolder.transform.SetParent(slotTransform, false);
                        holderTransform = componentHolder.transform;
                    }

                    int slotIndex = ManagedShip.ShipHull.ThrusterComponents.IndexOf(slot);
                    slot.concreteComponent = mainThruster;
                    ManagedShip.ShipHull.ThrusterComponents[slotIndex] = slot;

                    if (slot.needsBracket) {
                        if (holderTransform.childCount > 0) {//clear already assigned components
                            for (int i = holderTransform.childCount; i > 0; i--) {
                                Destroy(holderTransform.GetChild(i-1).gameObject);
                            }
                        }
                        
                        GameObject bracket = _prefabHandler.instantiateObject(_prefabHandler.loadPrefab("Ships/Thrusters/ThrusterBracket"), holderTransform);
                        float scale = ShipComponent.GetTierMultipliedValue(1, slot.concreteComponent.ComponentSize);
                        bracket.transform.localScale = new Vector3(scale,scale,scale);
                        Transform bracketMountTransform = bracket.transform.Find("ThrusterBracket").transform.Find("ThrusterMount").transform;
                        
                        CreateExternalShipComponent(bracketMountTransform, slot.concreteComponent, 1);

                        //instantiate bracket
                        //get bracket hook
                        //use hook transform as parent for thruster
                    }
                    else {
                        CreateExternalShipComponent(holderTransform, slot.concreteComponent, ShipComponent.GetTierMultipliedValue(1, slot.concreteComponent.ComponentSize));
                    }
                    return true;
                }
            }

            return false;
        }
        
        public bool SetInternalComponent(string parentName, InternalComponent internalComponent) {
            //get ShipComponent slot
            (ShipComponentType componentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName) slot = ManagedShip.ShipHull.InternalComponents.Find(w => w.parentTransformName == parentName);

            //get object transform
            Transform selectionTransform = MapPrefabTransformStringToTransformObject(parentName);
            
            if (InternalComponents.Select(wc => wc.mountTransform).Contains(selectionTransform)) {
                InternalComponents.Remove(WeaponComponents.Find(wc => wc.mountTransform == selectionTransform));
            }
            
            InternalComponents.Add((selectionTransform,selectionTransform, "Internal - " + slot.maxSize));

            if (selectionTransform != null&&internalComponent!=null) {
                if (internalComponent.ComponentType == slot.componentType && internalComponent.ComponentSize <= slot.maxSize) {
                    //assign new component
                    int slotIndex = ManagedShip.ShipHull.InternalComponents.IndexOf(slot);
                    slot.concreteComponent = internalComponent;
                    ManagedShip.ShipHull.InternalComponents[slotIndex] = slot;
                    return true;
                }
            }

            return false;
        }

        private Transform MapPrefabTransformStringToTransformObject(string parentTransformNames) {
            List<Transform> allShipTransforms = ShipObject.GetComponentsInChildren<Transform>().ToList();
            Transform parentTransform = allShipTransforms.Find(t => t.name == parentTransformNames);

            return parentTransform;
        }
    }
}