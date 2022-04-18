using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.InternalComponents;
using Code.Camera;
using UnityEngine;

namespace Code._Ships.Hulls {
    //ship blueprint for the components allowed on a particular hull 
    public abstract class Hull {
        protected Hull(Vector3 outfittingPosition, float hullMass, float hullStrength) {
            OutfittingPosition = new Vector3(outfittingPosition.x, outfittingPosition.y, outfittingPosition.z + OutfittingCameraController.ZOffset);
            HullMass = hullMass;
            HullStrength = hullStrength;
            SetupHull();
        }

        public float HullStrength;
        protected const string BaseHullPath = "Ships/Hulls/";
        public List<(ShipComponentType componentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)> InternalComponents;
        public List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> WeaponComponents;
        public List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName,bool needsBracket)> MainThrusterComponents;
        public (ShipComponentType componentType, ShipComponentTier maxSize, ManoeuvringThruster concreteComponent, string selectionTransformName, List<(string parentTransformName, float centerOffset)> thrusters) ManoeuvringThrusterComponents;
        public List<List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName,bool needsBracket)>> TiedThrustersSets = new List<List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName,bool needsBracket)>>();  
        public float HullMass;
        public List<(GameObject mesh, int channelIdx)> MeshObjects;
        public List<(List<string> objectName, Color colour)> ColourChannelObjectMap;
        
        public Vector3 OutfittingPosition { get; set; }
        public Quaternion OutfittingRotation { get; } = Quaternion.Euler(50, 0, -30);

        public abstract string GetHullFullPath();

        private void SetupHull() {
            SetWeaponComponents();
            SetThrusterComponents();
            SetInternalComponents();
        }

        protected abstract void SetThrusterComponents();
        protected abstract void SetWeaponComponents();
        protected abstract void SetInternalComponents();
    }
}