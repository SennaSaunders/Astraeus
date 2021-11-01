using System.Collections.Generic;
using UnityEngine;

namespace Code.CelestialObjects {
    public class CelestialBody : Body {

        public CelestialBody(Body primary, Vector2 coordinate, BodyTier tier) : base(primary, coordinate, tier) {
        }
        public CelestialBody(Body primary, BodyTier tier) : base(primary, tier) {
        }
    }
}