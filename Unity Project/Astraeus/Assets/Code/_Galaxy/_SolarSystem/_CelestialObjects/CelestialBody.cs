using Code._GameControllers;
using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects {
    public abstract class CelestialBody : Body {
        protected CelestialBody(Body primary, Vector2 coordinate, BodyTier tier) : base(primary, coordinate, tier) {//constructor for CelestialBody as a system primary
        }

        protected CelestialBody(Body primary, BodyTier tier) : base(primary, tier) {//constructor for CelestialBody as a satellite
        }
        
        public override GameObject GetSystemObject() {
            GameObject obj = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab("Sphere/WarpSphere"));
            float scale = Tier.SystemScale();
            obj.transform.localScale = new Vector3(scale, scale, scale);
            return obj;
        }

        public virtual GameObject GetMapObject() {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            float scale = Tier.MapScale();
            obj.transform.localScale = new Vector3(scale, scale, scale);
            return obj;
        }
    }
}