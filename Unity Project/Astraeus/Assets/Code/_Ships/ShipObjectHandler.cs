using System.Collections.Generic;
using System.Linq;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents;
using Code._Utility;
using UnityEngine;

namespace Code._Ships {
    public class ShipObjectHandler : MonoBehaviour {
        private PrefabHandler _prefabHandler;
        public Ship ManagedShip { get; set; }
        public GameObject ShipObject { get; private set; }

        private List<SphereCollider> _weaponSelectionColliders;
        private List<SphereCollider> _thrusterSelectionColliders;
        private List<SphereCollider> _internalSelectionColliders;

        private void Awake() {
            _prefabHandler = gameObject.AddComponent<PrefabHandler>();
        }

        public void CreateShip() {
            CreateHull();
            CreateExternalComponents(ShipComponentType.Weapon);
            CreateExternalComponents(ShipComponentType.MainThruster);
        }

        private void CreateHull() {
            ShipObject = _prefabHandler.instantiateObject(ManagedShip.ShipHull.hullObject);
        }

        private List<SphereCollider> CreateSelectionColliders(List<Transform> selectables) {
            List<SphereCollider> selectionColliders = new List<SphereCollider>();
            foreach (Transform selectable in selectables) {
                SphereCollider newCollider = selectable.gameObject.AddComponent<SphereCollider>();
                newCollider.radius = 1;
                selectionColliders.Add(newCollider);
            }

            return selectionColliders;
        }

        public void CreateInternalSelectionColliders() {
            ClearSelectionColliders();
        }

        public void CreateWeaponSelectionColliders() {
            ClearSelectionColliders();
            _weaponSelectionColliders = CreateExternalSelectionColliders(ShipComponentType.Weapon);
        }

        public void CreateThrusterSelectionColliders() {
            ClearSelectionColliders();
            _thrusterSelectionColliders = CreateExternalSelectionColliders(ShipComponentType.MainThruster);
        }

        public void ClearSelectionColliders() {
            List<List<SphereCollider>> colliderLists = new List<List<SphereCollider>>() { _weaponSelectionColliders, _thrusterSelectionColliders, _internalSelectionColliders };

            foreach (List<SphereCollider> colliderList in colliderLists) {
                if (colliderList != null) {
                    foreach (SphereCollider sphereCollider in colliderList) {
                        Destroy(sphereCollider);
                    }
                }
            }
        }

        private List<SphereCollider> CreateExternalSelectionColliders(ShipComponentType type) {
            List<string> slotTransformStrings = ManagedShip.ShipHull.ExternalComponents.Where(ec => ec.componentType == type).Select(w => w.parentTransformName).ToList();
            
            List<Transform> slotTransforms = new List<Transform>();

            foreach (string transformString in slotTransformStrings) {
                slotTransforms.Add(MapPrefabTransformStringToTransformObject(transformString));
            }

            return CreateSelectionColliders(slotTransforms);
        }

        private void CreateExternalComponents(ShipComponentType componentType) {
            List<(ShipComponentType componentType, ShipComponentTier maxSize, ExternalComponent concreteComponent, string parentTransformName)> componentSlots = ManagedShip.ShipHull.ExternalComponents.Where(c => c.componentType == componentType).ToList();
            
            foreach (var slot in componentSlots) {
                if (slot.concreteComponent != null) {
                    SetExternalComponent(slot.parentTransformName, slot.concreteComponent);
                }
            }
        }

        private Transform MapPrefabTransformStringToTransformObject(string parentTransformNames) {
            List<Transform> allShipTransforms = ShipObject.GetComponentsInChildren<Transform>().ToList();
            Transform parentTransform = allShipTransforms.Find(t => t.name == parentTransformNames);

            return parentTransform;
        }

        public bool SetExternalComponent(string parentName, ExternalComponent component) {
            //get ShipComponent slot
            (ShipComponentType componentType, ShipComponentTier maxSize, ExternalComponent concreteComponent, string parentTransformName) slot = ManagedShip.ShipHull.ExternalComponents.Find(ec => ec.parentTransformName == parentName);

            //get object transform
            Transform parent = MapPrefabTransformStringToTransformObject(parentName);

            if (parent != null) {
                if (component.ComponentType == slot.componentType && component.ComponentSize <= slot.maxSize) {
                    //destroy already attached objects
                    for (int i = parent.childCount-1; i >= 0; i--) {
                        Destroy(parent.GetChild(i).gameObject);
                    }
                    
                    //assign & instantiate new component
                    slot.concreteComponent = component;
                    string path = slot.concreteComponent.GetFullPath();
                    GameObject newComponentObject = _prefabHandler.instantiateObject(_prefabHandler.loadPrefab(path), parent);
                    float scale = ShipComponent.GetTierMultipliedValue(1, slot.concreteComponent.ComponentSize);
                    newComponentObject.transform.localScale = new Vector3(scale, scale, scale);

                    return true;
                }
            }

            return false;
        }
    }
}