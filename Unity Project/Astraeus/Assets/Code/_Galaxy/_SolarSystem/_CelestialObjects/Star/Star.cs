using UnityEngine;

namespace Code._CelestialObjects.Star {
    public class Star : CelestialBody {
        public enum StarType {
            O,
            B,
            A,
            F,
            G,
            K,
            M
        }
        public StarType Type { get;}
        //prefab
        public Star(Body primary, Vector2 coordinate, StarType type) : base(primary, coordinate, TierFromType(type)) {
            Type = type;
        }
        
        public Star(Body primary, StarType type) : base(primary, TierFromType(type)) {
            Type = type;
        }

        private static BodyTier TierFromType(StarType type) {
            return type switch {
                StarType.O => BodyTier.T9,
                StarType.B => BodyTier.T8,
                _ => BodyTier.T7
            };
        }
    }
}