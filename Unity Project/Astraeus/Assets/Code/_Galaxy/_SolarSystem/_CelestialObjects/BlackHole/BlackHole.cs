using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects.BlackHole {
    public class BlackHole : CelestialBody {
        public BlackHole(Body primary, Vector2 coordinate) : base(primary, coordinate, BodyTier.T9) {
        }

    }
}