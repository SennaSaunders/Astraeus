using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents {
    public abstract class ExternalComponent : ShipComponent {
        protected const string BaseComponentPath = "Ships/";

        [JsonIgnore]
        public GameObject InstantiatedGameObject;
        [JsonIgnore]
        public List<(GameObject mesh, int channelIdx)> MeshObjects;
        [JsonIgnore]
        public List<(List<string> objectName, Color colour)> ColourChannelObjectMap;

        protected ExternalComponent(string componentName, ShipComponentType componentType, ShipComponentTier componentSize, float baseMass, int basePrice) : base(componentName, componentType, componentSize, baseMass,basePrice) {
        }

        public virtual string GetFullPath() {
            return BaseComponentPath;
        }
    }
}