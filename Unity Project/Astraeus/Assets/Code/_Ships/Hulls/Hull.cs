using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.Hulls {
    //ship blueprint for the components allowed on a particular hull 
    public abstract class Hull : MonoBehaviour {
        public string BaseHullPath = "Ships/Hulls/";
        public Vector3 outfittingPosition;
        public Quaternion outfittingRotation;
        protected abstract string GetHullPath();
        
        protected void SetupHull(List<( ShipComponentType, int maxSize, int maxNum)> internalComponents, float hullMass) {
            SetHullPrefab(GetHullPath());
            SetExternalComponents();
            InternalComponents = internalComponents;
            HullMass = hullMass;
        }

        private void SetHullPrefab(string hullPath) {
            string fullHullPath = BaseHullPath+ hullPath;
            hullObject = (GameObject)Resources.Load(fullHullPath);
        }

        public GameObject hullObject;
        public List<(ShipComponentType, int maxSize, int maxNum)> InternalComponents;
        public List<(ShipComponentType, int maxSize, Transform parent)> ExternalComponents;
        public float HullMass;

        public abstract void SetExternalComponents();
    }
}