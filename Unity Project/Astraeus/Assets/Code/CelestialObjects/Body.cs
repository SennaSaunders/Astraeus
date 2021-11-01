using System.Collections.Generic;
using UnityEngine;

namespace Code.CelestialObjects {
    public abstract class Body {
        protected Body(Body primary, Vector2 coordinate, BodyTier tier) {
            SetPrimary(primary);
            Coordinate = coordinate;
            Tier = tier;
        }
        
        protected Body(Body primary, BodyTier tier) {
            SetPrimary(primary);
            Tier = tier;
        }

        public Body Primary { get; private set; }

        public List<Body> Children { get; } = new List<Body>();
        
        private void SetPrimary(Body primary) {
            Primary = primary;
            if (primary != null) {
                Primary.AddChild(this);
            }
        }

        private void AddChild(Body body) {
            Children.Add(body);
        }
        public Vector2 Coordinate { get; set; }
        private float _rotationBase;
        private float _orbitalPeriod;
        private float _rotationCurrent;
        
        public enum BodyTier {
            T0, //space stations
            T1, //smallest celestial body
            T2, 
            T3,
            T4, 
            T5, 
            T6, //largest planet
            T7, 
            T8,
            T9  //largest
        }

        public BodyTier Tier { get;  }

        public void RotateBody(float timeDelta) {
            // (timeDelta /_orbitalPeriod) * 360 * gameSpeed
        }
    }

    public static class BodyTierExtension {
        public static float BaseDistance(this Body.BodyTier tier) {
            if (tier == Body.BodyTier.T9) return 80;
            if (tier == Body.BodyTier.T8) return 65;
            if (tier == Body.BodyTier.T7) return 50;
            if (tier == Body.BodyTier.T6) return 45;
            if (tier == Body.BodyTier.T5) return 30;
            if (tier == Body.BodyTier.T4) return 25;
            if (tier == Body.BodyTier.T3) return 20;
            if (tier == Body.BodyTier.T2) return 15;
            if (tier == Body.BodyTier.T1) return 10;
            else return 5;
        }
    }
}