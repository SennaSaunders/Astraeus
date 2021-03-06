using System.Collections.Generic;
using System.Linq;
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

        public List<(Transform mountTransform, Transform selectionTransform, ShipComponentTier maxSize, ShipComponentType componentType, Weapon weapon)> WeaponComponents = new List<(Transform mountTransform, Transform selectionTransform, ShipComponentTier maxSize, ShipComponentType componentType, Weapon weapon)>();
        public List<(Transform mountTransform, Transform selectionTransform, ShipComponentTier maxSize, ShipComponentType componentType, MainThruster thruster)> MainThrusterComponents = new List<(Transform mountTransform, Transform selectionTransform, ShipComponentTier maxSize, ShipComponentType componentType, MainThruster thruster)>();
        public (List<Transform> mountTransforms, Transform selectionTransform, ShipComponentTier maxSize, ShipComponentType componentType, ManoeuvringThruster thruster) ManoeuvringThrusterComponents;
        public List<(Transform mountTransform, Transform selectionTransform, ShipComponentTier maxSize, ShipComponentType Internal, InternalComponent internalComponent)> InternalComponents = new List<(Transform mountTransform, Transform selectionTransform, ShipComponentTier maxSize, ShipComponentType componentType, InternalComponent component)>();

        public GameObject CreateShip(Transform parent, Color markerColour) {
            CreateHull();
            CreateMainThrusterComponents();
            CreateManoeuvringThrusterComponents();
            CreateWeaponComponents();
            CreateInternalComponents();
            SetDefaultShipRotation();
            AddShipMarker(markerColour);
            ManagedShip.ShipObject.transform.SetParent(parent);
            return ManagedShip.ShipObject;
        }

        public GameObject CreateShip(Transform parent) {
            CreateHull();
            CreateMainThrusterComponents();
            CreateManoeuvringThrusterComponents();
            CreateWeaponComponents();
            CreateInternalComponents();
            SetDefaultShipRotation();
            ManagedShip.ShipObject.transform.SetParent(parent);
            return ManagedShip.ShipObject;
        }

        private void AddShipMarker(Color colour) {
            GameObject shipMarker = Instantiate((GameObject)Resources.Load("Icons/ShipMarkerIcon"), ManagedShip.ShipObject.transform, true);
            shipMarker.GetComponent<SpriteRenderer>().color = colour;
            shipMarker.name = "ShipMarker";
            shipMarker.layer = LayerMask.NameToLayer("LocalMap");
            shipMarker.transform.localScale = new Vector3(Ship.ShipMarkerSize, Ship.ShipMarkerSize, Ship.ShipMarkerSize);
            shipMarker.transform.localPosition = new Vector3(0, 0, -400); //places ship marker above other map objects
        }

        public void SetMappedMaterials(List<(GameObject mesh, int channelIdx)> meshObjects, List<(List<string> objectName, Color colour)> colourChannelObjectMap) {
            foreach ((GameObject mesh, int channelIdx) meshObject in meshObjects) {
                SetMaterial(meshObject.mesh, colourChannelObjectMap[meshObject.channelIdx].colour);
            }
        }

        private void SetMaterial(GameObject mesh, Color colour) {
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.color = colour;
            MeshRenderer meshRenderer = mesh.GetComponent<MeshRenderer>();
            meshRenderer.material = material;
        }

        private List<(GameObject mesh, int channelIdx)> MapHullMeshToChannel(List<GameObject> meshes, Hull hull) {
            List<(GameObject mesh, int channelIdx)> meshChannelMap = new List<(GameObject mesh, int channelIdx)>();
            foreach (GameObject mesh in meshes) {
                var channelIdx = hull.ColourChannelObjectMap.FindIndex(c => c.objectName.Contains(mesh.name));
                meshChannelMap.Add((mesh, channelIdx));
            }

            return meshChannelMap;
        }

        private List<(GameObject mesh, int channelIdx)> MapExternalMeshToChannel(List<GameObject> meshes, ExternalComponent externalComponent) {
            List<(GameObject mesh, int channelIdx)> meshChannelMap = new List<(GameObject mesh, int channelIdx)>();
            foreach (GameObject mesh in meshes) {
                string meshName = mesh.name;
                string clone = "(Clone)";
                if (mesh.name.EndsWith(clone)) {
                    meshName = meshName.Substring(0, meshName.Length - clone.Length);
                }

                var channelIdx = externalComponent.ColourChannelObjectMap.FindIndex(c => c.objectName.Contains(meshName));
                meshChannelMap.Add((mesh, channelIdx));
            }

            return meshChannelMap;
        }

        private void CreateHull() {
            ManagedShip.ShipObject = Instantiate((GameObject)Resources.Load(ManagedShip.ShipHull.GetHullFullPath()));
            ManagedShip.ShipHull.MeshObjects = MapHullMeshToChannel(GetMeshObjects(ManagedShip.ShipObject), ManagedShip.ShipHull);
            SetMappedMaterials(ManagedShip.ShipHull.MeshObjects, ManagedShip.ShipHull.ColourChannelObjectMap);
        }

        public List<GameObject> GetMeshObjects(GameObject meshObject) {
            List<GameObject> meshObjects = new List<GameObject>();
            MeshRenderer meshRenderer = meshObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null) {
                meshObjects.Add(meshObject);
            }

            for (int i = 0; i < meshObject.transform.childCount; i++) {
                meshObjects.AddRange(GetMeshObjects(meshObject.transform.GetChild(i).gameObject));
            }

            return meshObjects;
        }

        private void CreateWeaponComponents() {
            ShipComponentType componentType = ShipComponentType.Weapon;
            List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> componentSlots = ManagedShip.ShipHull.WeaponComponents.Where(c => c.componentType == componentType).ToList();

            foreach (var slot in componentSlots) {
                SetWeaponComponent(slot.parentTransformName, slot.concreteComponent);
            }
        }

        private void CreateMainThrusterComponents() {
            ShipComponentType componentType = ShipComponentType.MainThruster;
            List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName, bool needsBracket)> componentSlots = ManagedShip.ShipHull.MainThrusterComponents.Where(c => c.componentType == componentType).ToList();

            foreach (var slot in componentSlots) {
                SetMainThrusterComponent(slot.parentTransformName, slot.concreteComponent);
            }
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
            ManagedShip.ShipObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        private void CreateExternalShipComponent(Transform parent, ExternalComponent component, float scale) {
            //removes old game object
            if (parent.childCount > 0) { //clear already assigned components
                for (int i = parent.childCount; i > 0; i--) {
                    Destroy(parent.GetChild(i - 1).gameObject);
                }
            }

            //creates new gameobject
            if (component != null) {
                string path = component.GetFullPath();

                GameObject newComponentObject = Instantiate((GameObject)Resources.Load(path), parent);
                newComponentObject.transform.localScale = new Vector3(scale, scale, scale);
                component.InstantiatedGameObject = newComponentObject;
                component.MeshObjects = MapExternalMeshToChannel(GetMeshObjects(newComponentObject), component);
                SetMappedMaterials(component.MeshObjects, component.ColourChannelObjectMap);
            }
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

            if (mainThruster == null || mainThruster.ComponentSize <= slot.maxSize) {//if valid assignment
                if (MainThrusterComponents.Select(tc => tc.mountTransform).Contains(slotTransform)) {//if the transform belongs to main thrusters
                    MainThrusterComponents.Remove(MainThrusterComponents.Find(tc => tc.mountTransform == slotTransform));//remove the slot
                }

                MainThrusterComponents.Add((slotTransform, selectionTransform, slot.maxSize, ShipComponentType.MainThruster, mainThruster));//recreate the slot with the new component
                if (slotTransform != null) {
                    Transform holderTransform = slotTransform.Find("ComponentHolder");
                    if (holderTransform == null) {
                        GameObject componentHolder = new GameObject("ComponentHolder");
                        componentHolder.transform.SetParent(slotTransform, false);
                        holderTransform = componentHolder.transform;
                    }

                    int slotIndex = ManagedShip.ShipHull.MainThrusterComponents.IndexOf(slot);
                    slot.concreteComponent = mainThruster;
                    ManagedShip.ShipHull.MainThrusterComponents[slotIndex] = slot;

                    if (mainThruster != null) {
                        if (mainThruster.ComponentType == slot.componentType && mainThruster.ComponentSize <= slot.maxSize) {
                            if (slot.needsBracket) {
                                if (holderTransform.childCount > 0) { //clear already assigned components
                                    for (int i = holderTransform.childCount; i > 0; i--) {
                                        Destroy(holderTransform.GetChild(i - 1).gameObject);
                                    }
                                }

                                GameObject bracket = Instantiate((GameObject)Resources.Load("Ships/Thrusters/ThrusterBracket"), holderTransform);
                                List<GameObject> bracketMeshes = GetMeshObjects(bracket);
                                foreach (GameObject bracketMesh in bracketMeshes) {
                                    SetMaterial(bracketMesh, Color.black);
                                }

                                float scale = ShipComponent.GetTierMultipliedValue(1, slot.concreteComponent.ComponentSize);
                                bracket.transform.localScale = new Vector3(scale, scale, scale);
                                Transform bracketMountTransform = bracket.transform.Find("ThrusterBracket").transform.Find("ThrusterMount").transform;

                                CreateExternalShipComponent(bracketMountTransform, slot.concreteComponent, 1);
                            }
                            else {
                                CreateExternalShipComponent(holderTransform, slot.concreteComponent, ShipComponent.GetTierMultipliedValue(1, slot.concreteComponent.ComponentSize));
                            }

                            return true;
                        }
                    }
                    else { //removes component
                        CreateExternalShipComponent(holderTransform, null, 0);
                        return true;
                    }
                }
            }


            return false;
        }

        public bool SetManoeuvringThrusterComponent(ManoeuvringThruster manoeuvringThruster) {
            var slot = ManagedShip.ShipHull.ManoeuvringThrusterComponents;

            Transform selectionTransform = MapPrefabTransformStringToTransformObject(ManagedShip.ShipHull.ManoeuvringThrusterComponents.selectionTransformName);
            List<Transform> objectTransforms = new List<Transform>();
            if (manoeuvringThruster == null || manoeuvringThruster.ComponentSize <= slot.maxSize) {
                for (int i = 0; i < slot.parentTransformNames.Count; i++) {
                    string thrusterTransformName = ManagedShip.ShipHull.ManoeuvringThrusterComponents.parentTransformNames[i];
                    objectTransforms.Add(MapPrefabTransformStringToTransformObject(thrusterTransformName));
                }

                ManoeuvringThrusterComponents = (objectTransforms, selectionTransform, slot.maxSize, ShipComponentType.ManoeuvringThruster, manoeuvringThruster);
                List<Transform> holderTransforms = new List<Transform>();
                foreach (var manoeuvringThrusterTransform in ManoeuvringThrusterComponents.mountTransforms) {
                    Transform holderTransform = manoeuvringThrusterTransform.Find("Component Holder");
                    if (holderTransform == null) {
                        GameObject componentHolder = new GameObject("Component Holder");
                        componentHolder.transform.SetParent(manoeuvringThrusterTransform, false);
                        holderTransform = componentHolder.transform;
                    }

                    holderTransforms.Add(holderTransform);
                }

                if (manoeuvringThruster != null) {
                    if (manoeuvringThruster.ComponentSize <= slot.maxSize) {
                        slot.concreteComponent = manoeuvringThruster;
                        ManagedShip.ShipHull.ManoeuvringThrusterComponents.concreteComponent = manoeuvringThruster;
                        foreach (var holderTransform in holderTransforms) {
                            CreateExternalShipComponent(holderTransform, slot.concreteComponent, ShipComponent.GetTierMultipliedValue(1, slot.concreteComponent.ComponentSize));
                        }

                        return true;
                    }
                }
                else { //removes component
                    slot.concreteComponent = null;
                    ManagedShip.ShipHull.ManoeuvringThrusterComponents.concreteComponent = null;
                    foreach (var holderTransform in holderTransforms) {
                        CreateExternalShipComponent(holderTransform, null, 0);
                    }
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
            if (weapon == null || weapon.ComponentSize <= slot.maxSize) {
                if (!WeaponComponents.Select(wc => wc.mountTransform).Contains(slotTransform)) {
                    WeaponComponents.Add((slotTransform, selectionTransform, slot.maxSize, ShipComponentType.Weapon, weapon));
                }
                if (slotTransform != null) {
                    Transform holderTransform = slotTransform.Find("ComponentHolder");
                    if (holderTransform == null) {
                        GameObject componentHolder = new GameObject("ComponentHolder");
                        componentHolder.transform.SetParent(slotTransform, false);
                        holderTransform = componentHolder.transform;
                    }

                    //assign new component
                    int slotIndex = ManagedShip.ShipHull.WeaponComponents.IndexOf(slot);
                    slot.concreteComponent = weapon;
                    ManagedShip.ShipHull.WeaponComponents[slotIndex] = slot;
                    if (weapon != null) {
                        if (weapon.ComponentType == slot.componentType && weapon.ComponentSize <= slot.maxSize) {
                            //instantiate
                            CreateExternalShipComponent(holderTransform, slot.concreteComponent, ShipComponent.GetTierMultipliedValue(1, slot.concreteComponent.ComponentSize));
                            return true;
                        }
                    }
                    else {
                        CreateExternalShipComponent(holderTransform, null, 0);
                        return true;
                    }
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
                InternalComponents.Add((selectionTransform, selectionTransform, slot.maxSize, ShipComponentType.Internal, internalComponent));
            }

            if (selectionTransform != null && internalComponent != null) {
                if (internalComponent.ComponentType == slot.componentType && internalComponent.ComponentSize <= slot.maxSize) {
                    //assign new component
                    int slotIndex = ManagedShip.ShipHull.InternalComponents.IndexOf(slot);
                    slot.concreteComponent = internalComponent;
                    ManagedShip.ShipHull.InternalComponents[slotIndex] = slot;
                    return true;
                }
            }
            else if (selectionTransform != null) {
                int slotIndex = ManagedShip.ShipHull.InternalComponents.IndexOf(slot);
                slot.concreteComponent = internalComponent;
                ManagedShip.ShipHull.InternalComponents[slotIndex] = slot;
                return true;
            }

            return false;
        }
    }
}