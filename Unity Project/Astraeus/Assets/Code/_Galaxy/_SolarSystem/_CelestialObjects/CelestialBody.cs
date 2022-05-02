using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects {
    public abstract class CelestialBody : Body {
        protected CelestialBody(Body primary, Vector2 coordinate, BodyTier tier, Color mapColour) : base(primary, coordinate, tier, mapColour) {//constructor for CelestialBody as a system primary
        }

        protected CelestialBody(Body primary, BodyTier tier, Color mapColour) : base(primary, tier, mapColour) {//constructor for CelestialBody as a satellite
        }

        public GameObject GetMapObject() {
            return GetSystemObject();
        }
    }
}