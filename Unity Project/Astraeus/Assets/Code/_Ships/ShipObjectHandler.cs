using System;
using System.Collections.Generic;
using System.Linq;
using Code._GameControllers;
using Code._Ships.Hulls;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Utility;
using UnityEngine;

namespace Code._Ships {
    public class ShipObjectHandler : MonoBehaviour {
        public Ship ManagedShip { get; set; }

        public List<(Transform mountTransform, Transform selectionTransform, string slotName)> WeaponComponents = new List<(Transform mountTransform, Transform selectionTransform, string slotName)>();
        public List<(Transform mountTransform, Transform selectionTransform, string slotName)> MainThrusterComponents = new List<(Transform mountTransform, Transform selectionTransform, string slotName)>();
        public (List<Transform> mountTransforms, Transform selectionTransform, string slotName) ManoeuvringThrusterComponents;
        public List<(Transform mountTransform, Transform selectionTransform, string slotName)> InternalComponents = new List<(Transform mountTransform, Transform selectionTransform, string slotName)>();
        private static readonly int Metallic = Shader.PropertyToID("Metallic");
        private static readonly int Smoothness = Shader.PropertyToID("Smoothness");
        private static readonly int Property = Shader.PropertyToID("Smoothness Scale");


        public GameObject CreateShip(Transform parent) {
            CreateHull();
            CreateMainThrusterComponents();
            CreateManoeuvringThrusterComponents();
            CreateWeaponComponents();
            CreateInternalComponents();
            SetDefaultShipRotation();
            ManagedShip.ShipObject.transform.SetParent(parent);
            SetDefaultMaterials();
            return ManagedShip.ShipObject;
        }

        public void SetDefaultMaterials() {
            List<(Type objectType, List<(GameObject mesh, Color colour)> meshColour)> meshObjectMaps = GetHullMeshObjects(ManagedShip.ShipHull);

            foreach ((Type objectType, List<(GameObject mesh, Color colour)> meshColour) meshObjectMap in meshObjectMaps) {
                foreach ((GameObject mesh, Color colour) meshColour in meshObjectMap.meshColour) {
                    Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    material.color = meshColour.colour;
                    MeshRenderer meshRenderer = meshColour.mesh.GetComponent<MeshRenderer>();
                    meshRenderer.material = material;
                }
                 
            }
        }

        public void SetMaterials(List<(Type objectType, List<(GameObject mesh, Color colour)> meshColour)> meshObjectMaps) {
            foreach ((Type objectType, List<(GameObject mesh, Color colour)> meshColour) meshObjectMap in meshObjectMaps) {
                foreach ((GameObject mesh, Color colour) meshColour in meshObjectMap.meshColour) {
                    Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    material.color = meshColour.colour;
                    MeshRenderer meshRenderer = meshColour.mesh.GetComponent<MeshRenderer>();
                    meshRenderer.material = material;
                }
            }
        }

        public List<(Type objectType, List<(GameObject mesh, Color colour)> meshColour)> GetHullMeshObjects(Hull hull) {
            List<(Type objectType, List<(GameObject mesh, Color colour)> meshColour)> meshTypeObjects = new List<(Type objectType, List<(GameObject mesh, Color colour)> meshColour)>();
            hull.SetColourChannelObjectMap();
            foreach (GameObject meshObject in hull.MeshObjects) {
                Color colour = hull.ColourChannelObjectMap.Find(channel => channel.objectName.Contains(meshObject.name)).colour;
                if (meshTypeObjects.Select(mto => mto.objectType).Contains(hull.GetType())) {
                    (Type objectType, List<(GameObject mesh, Color colour)> meshColour) temp = meshTypeObjects.Find(mto => mto.objectType == hull.GetType());
                    var typeIndex = meshTypeObjects.IndexOf(temp);
                    temp.meshColour.Add((meshObject, colour));
                    meshTypeObjects[typeIndex] = temp;
                }
                else {
                    Type hullType = hull.GetType();
                    meshTypeObjects.Add((hullType, new List<(GameObject mesh, Color colour)>(){ (meshObject, colour)}));
                }
            }
            return meshTypeObjects;
        }

        private List<(Type objectType, List<(GameObject mesh, Color colour)> meshColour)> MapExternalComponents(List<ExternalComponent> externalComponents) {
            List<(Type objectType, List<(GameObject mesh, Color colour)> meshColour)> meshTypeObjects = new List<(Type objectType, List<(GameObject mesh, Color colour)> meshColour)>();
            foreach (var component in externalComponents) {
                if (component != null) {
                    component.SetColourChannelObjectMap();
                    foreach (GameObject meshObject in component.MeshObjects) {
                        Color colour = component.ColourChannelObjectMap.Find(channel => channel.objectName.Contains(meshObject.name)).colour;
                        if (meshTypeObjects.Select(mto => mto.objectType).Contains(component.GetType())) {
                            (Type objectType, List<(GameObject mesh, Color colour)> meshColour) temp = meshTypeObjects.Find(mto => mto.objectType == component.GetType());
                            var typeIndex = meshTypeObjects.IndexOf(temp);
                            temp.meshColour.Add((meshObject, colour));
                            meshTypeObjects[typeIndex] = temp;
                        }
                        else {
                            Type componentType = component.GetType();
                            meshTypeObjects.Add((componentType , new List<(GameObject mesh, Color colour)>(){ (meshObject, colour)}));
                        }
                    }
                }
            }

            return meshTypeObjects;
        }

        private void CreateHull() {
            ManagedShip.ShipObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(ManagedShip.ShipHull.GetHullFullPath()));
            ManagedShip.ShipHull.MeshObjects = GameController._prefabHandler.GetMeshObjects(ManagedShip.ShipObject);
            Rigidbody hullRigidbody = ManagedShip.ShipObject.AddComponent<Rigidbody>();
            hullRigidbody.isKinematic = false;
            hullRigidbody.useGravity = false;
            hullRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        
        private void CreateWeaponComponents() {
            ShipComponentType componentType = ShipComponentType.Weapon;
            List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> componentSlots = ManagedShip.ShipHull.WeaponComponents.Where(c => c.componentType == componentType).ToList();
            
            foreach (var slot in componentSlots) {
                SetWeaponComponent(slot.parentTransformName, slot.concreteComponent);
            }
            SetMaterials(MapExternalComponents(componentSlots.Select(cs=>cs.concreteComponent).Cast<ExternalComponent>().ToList()));
        }
        
        private void CreateMainThrusterComponents() {
            ShipComponentType componentType = ShipComponentType.MainThruster;
            List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName, bool needsBracket)> componentSlots = ManagedShip.ShipHull.MainThrusterComponents.Where(c => c.componentType == componentType).ToList();
            
            foreach (var slot in componentSlots) {
                SetMainThrusterComponent(slot.parentTransformName, slot.concreteComponent);
            }
            SetMaterials(MapExternalComponents(componentSlots.Select(cs=>cs.concreteComponent).Cast<ExternalComponent>().ToList()));
        }
        
        private void CreateManoeuvringThrusterComponents() {
            var slot = ManagedShip.ShipHull.ManoeuvringThrusterComponents;
            SetManoeuvringThrusterComponent(slot.concreteComponent);
        }

        private void CreateInternalComponents() {
            ShipComponentType componentType = ShipComponentType.Internal;
            List<(ShipComponentType componentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)> componentSlots = ManagedShip.ShipHull.InternalComponents.Where(c => c.componentType == componentType).ToList();

            foreach (var slot in componentSlots) {
                SetInternalComponent(slot.parentTransformName, slot.concreteComponent);
            }
            
        }

        private void SetDefaultShipRotation() {
            ManagedShip.ShipObject.transform.localRotation = Quaternion.Euler(0,0,0);
        }

        private void CreateExternalShipComponent(Transform parent, ExternalComponent component, float scale) {
            if (parent.childCount > 0) {//clear already assigned components
                for (int i = parent.childCount; i > 0; i--) {
                    Destroy(parent.GetChild(i-1).gameObject);
                }
            }

            string path = component.GetFullPath();

            GameObject newComponentObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(path), parent);
            newComponentObject.transform.localScale = new Vector3(scale, scale, scale);
            component.InstantiatedGameObject = newComponentObject;
            component.MeshObjects = GameController._prefabHandler.GetMeshObjects(newComponentObject);
            SetMaterials(MapExternalComponents(new List<ExternalComponent>(){component}));
        }

        private Transform MapPrefabTransformStringToTransformObject(string parentTransformNames) {
            return GameObjectHelper.FindChild(ManagedShip.ShipObject, parentTransformNames).transform;
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

        public bool SetMainThrusterComponent(string parentName, MainThruster mainThruster) {
            //get ShipComponent slot
            var slot = ManagedShip.ShipHull.MainThrusterComponents.Find(ec => ec.parentTransformName == parentName);

            //get object transform
            Transform slotTransform = MapPrefabTransformStringToTransformObject(parentName);
            Transform selectionTransform = GetSelectionTransform(slotTransform);
            if (MainThrusterComponents.Select(tc => tc.mountTransform).Contains(slotTransform)) {
                MainThrusterComponents.Remove(MainThrusterComponents.Find(tc => tc.mountTransform == slotTransform));
            }
            MainThrusterComponents.Add((slotTransform, selectionTransform, "Main Thruster - " + slot.maxSize));

            if (slotTransform != null&&mainThruster!=null) {
                if (mainThruster.ComponentType == slot.componentType && mainThruster.ComponentSize <= slot.maxSize) {
                    Transform holderTransform = slotTransform.Find("ComponentHolder");
                    if (holderTransform == null) {
                        GameObject componentHolder = new GameObject("ComponentHolder");
                        componentHolder.transform.SetParent(slotTransform, false);
                        holderTransform = componentHolder.transform;
                    }

                    int slotIndex = ManagedShip.ShipHull.MainThrusterComponents.IndexOf(slot);
                    slot.concreteComponent = mainThruster;
                    ManagedShip.ShipHull.MainThrusterComponents[slotIndex] = slot;

                    if (slot.needsBracket) {
                        if (holderTransform.childCount > 0) {//clear already assigned components
                            for (int i = holderTransform.childCount; i > 0; i--) {
                                Destroy(holderTransform.GetChild(i-1).gameObject);
                            }
                        }
                        
                        GameObject bracket = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab("Ships/Thrusters/ThrusterBracket"), holderTransform);
                        float scale = ShipComponent.GetTierMultipliedValue(1, slot.concreteComponent.ComponentSize);
                        bracket.transform.localScale = new Vector3(scale,scale,scale);
                        Transform bracketMountTransform = bracket.transform.Find("ThrusterBracket").transform.Find("ThrusterMount").transform;
                        
                        CreateExternalShipComponent(bracketMountTransform, slot.concreteComponent, 1);
                    }
                    else {
                        CreateExternalShipComponent(holderTransform, slot.concreteComponent, ShipComponent.GetTierMultipliedValue(1, slot.concreteComponent.ComponentSize));
                    }
                    return true;
                }
            }

            return false;
        }

        public bool SetManoeuvringThrusterComponent(ManoeuvringThruster manoeuvringThruster) {
            var slot = ManagedShip.ShipHull.ManoeuvringThrusterComponents;

            Transform selectionTransform = MapPrefabTransformStringToTransformObject(ManagedShip.ShipHull.ManoeuvringThrusterComponents.selectionTransformName);
            List<Transform> objectTransforms = new List<Transform>();
            for (int i = 0; i < slot.thrusters.Count;i++) {
                var thruster = ManagedShip.ShipHull.ManoeuvringThrusterComponents.thrusters[i];
                objectTransforms.Add(MapPrefabTransformStringToTransformObject(thruster.parentTransformName));
                thruster.centerOffset = objectTransforms[i].localPosition.y;
                ManagedShip.ShipHull.ManoeuvringThrusterComponents.thrusters[i] = thruster;
            }
             
            ManoeuvringThrusterComponents = (objectTransforms, selectionTransform, "Manoeuvring Thruster - " + slot.maxSize);

            if (manoeuvringThruster != null) {
                if (manoeuvringThruster.ComponentSize <= slot.maxSize) {
                    slot.concreteComponent = manoeuvringThruster;
                    
                    foreach (var manoeuvringThrusterTransform in ManoeuvringThrusterComponents.mountTransforms) {
                        Transform holderTransform = manoeuvringThrusterTransform.Find("Component Holder");
                        if (holderTransform == null) {
                            GameObject componentHolder = new GameObject("Component Holder");
                            componentHolder.transform.SetParent(manoeuvringThrusterTransform, false);
                            holderTransform = componentHolder.transform;
                        }

                        if (holderTransform.childCount > 0) {
                            for (int i = holderTransform.childCount; i > 0; i--) {
                                Destroy(holderTransform.GetChild(i-1).gameObject);
                            }
                        }
                        
                        CreateExternalShipComponent(holderTransform, slot.concreteComponent, ShipComponent.GetTierMultipliedValue(1, slot.concreteComponent.ComponentSize));
                    }
                    ManagedShip.ShipHull.ManoeuvringThrusterComponents.concreteComponent = manoeuvringThruster;

                    return true;
                }
                
            }
            
            return false;
        }
        
        public bool SetWeaponComponent(string parentName, Weapon weapon) {
            //get ShipComponent slot
            var slot = ManagedShip.ShipHull.WeaponComponents.Find(w => w.parentTransformName == parentName);

            //get object transform
            Transform slotTransform = MapPrefabTransformStringToTransformObject(parentName);
            Transform selectionTransform = GetSelectionTransform(slotTransform);
            if (!WeaponComponents.Select(wc => wc.mountTransform).Contains(slotTransform)) {
                WeaponComponents.Add((slotTransform,selectionTransform, "Weapon - " + slot.maxSize));
            }

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
        
        public bool SetInternalComponent(string parentName, InternalComponent internalComponent) {
            //get ShipComponent slot
            (ShipComponentType componentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName) slot = ManagedShip.ShipHull.InternalComponents.Find(w => w.parentTransformName == parentName);

            //get object transform
            Transform selectionTransform = MapPrefabTransformStringToTransformObject(parentName);

            if (!InternalComponents.Select(ic => ic.mountTransform).Contains(selectionTransform)) {
                InternalComponents.Add((selectionTransform,selectionTransform, "Internal - " + slot.maxSize));
                // InternalComponents.Remove(WeaponComponents.Find(wc => wc.mountTransform == selectionTransform));
            }
            
            

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

        
    }
}