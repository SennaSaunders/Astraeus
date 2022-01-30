using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Utility;
using UnityEngine;

namespace Code._Ships.Hulls {
    //ship blueprint for the components allowed on a particular hull 
    public abstract class Hull : MonoBehaviour {
        private PrefabHandler _prefabHandler;
        protected const string BaseHullPath = "Ships/Hulls/";
        public GameObject HullObject { get; private set; }
        public List<(ShipComponentType componentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)> InternalComponents;
        public List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> WeaponComponents;
        public List<(ShipComponentType componentType, ShipComponentTier maxSize, Thruster concreteComponent, string parentTransformName,bool needsBracket)> ThrusterComponents;
        public float HullMass;
        public Vector3 outfittingPosition;
        public Quaternion outfittingRotation = Quaternion.Euler(-30, -50, 30);

        protected abstract string GetHullFullPath();

        protected void SetupHull(float hullMass) {
            _prefabHandler = gameObject.AddComponent<PrefabHandler>();
            HullObject = _prefabHandler.loadPrefab(GetHullFullPath());
            SetWeaponComponents();
            SetThrusterComponents();
            SetInternalComponents();
            HullMass = hullMass;
        }

        public abstract void SetThrusterComponents();
        public abstract void SetWeaponComponents();
        public abstract void SetInternalComponents();
    }
}