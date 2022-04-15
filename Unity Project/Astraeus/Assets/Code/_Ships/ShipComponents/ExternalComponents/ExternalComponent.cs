using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents {
    public abstract class ExternalComponent : ShipComponent {
        protected const string BaseComponentPath = "Ships/";
        public GameObject InstantiatedGameObject;
        public List<GameObject> MeshObjects;
        public List<(List<string> objectName, Color colour)> ColourChannelObjectMap;
        protected ExternalComponent(string componentName, ShipComponentType componentType, ShipComponentTier componentSize, float baseMass) : base(componentName, componentType, componentSize, baseMass) {
            SetColourChannelObjectMap();
        }

        public virtual string GetFullPath() {
            return BaseComponentPath;
        }

        public abstract void SetColourChannelObjectMap();
    }
}