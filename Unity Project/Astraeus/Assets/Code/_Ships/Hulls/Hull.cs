using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Utility;
using UnityEngine;

namespace Code._Ships.Hulls {
    //ship blueprint for the components allowed on a particular hull 
    public abstract class Hull : MonoBehaviour {
        private PrefabHandler _prefabHandler;
        protected const string BaseHullPath = "Ships/Hulls/";
        public GameObject hullObject { get; private set; }
        public List<(ShipComponentType componentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)> InternalComponents;
        public List<(ShipComponentType componentType, ShipComponentTier maxSize, ExternalComponent concreteComponent, string parentTransformName)> ExternalComponents;
        public float HullMass;
        public Vector3 outfittingPosition;
        public Quaternion outfittingRotation;

        protected abstract string GetHullFullPath();

        protected void SetupHull(float hullMass) {
            _prefabHandler = gameObject.AddComponent<PrefabHandler>();
            hullObject = _prefabHandler.loadPrefab(GetHullFullPath());
            SetExternalComponents();
            SetInternalComponents();
            HullMass = hullMass;
        }
        
        public abstract void SetExternalComponents();
        public abstract void SetInternalComponents();
    }
}