using UnityEngine;

namespace Code._CelestialObjects {
    public abstract class CelestialBody : Body {
        protected CelestialBody(Body primary, Vector2 coordinate, BodyTier tier) : base(primary, coordinate, tier) {
        }

        protected CelestialBody(Body primary, BodyTier tier) : base(primary, tier) {
        }
        
        public override GameObject GetSystemObject() {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            float scale = Tier.SystemScale();
            obj.transform.localScale = new Vector3(scale, scale, scale);
            return obj;
        }

        public GameObject GetMapObject() {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            float scale = Tier.MapScale();
            obj.transform.localScale = new Vector3(scale, scale, scale);
            return obj;
        }
    }
}