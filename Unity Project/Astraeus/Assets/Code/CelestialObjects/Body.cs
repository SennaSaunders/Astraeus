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
        public float _rotationBase { get; set; }
        public float _orbitalPeriod { get; set; }
        public float _rotationCurrent { get; set; }

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
            T9 //largest
        }

        public BodyTier Tier { get; }

        public abstract GameObject GetSystemObject();

        public void RotateBody(float timeDelta) { //do i need this? i dont think so i think the controller should do it
            // (timeDelta /_orbitalPeriod) * 360 * gameSpeed
        }
    }

    public static class BodyTierExtension {
        public static float BaseDistance(this Body.BodyTier tier) {
            if (tier == Body.BodyTier.T9) return 160;
            if (tier == Body.BodyTier.T8) return 130;
            if (tier == Body.BodyTier.T7) return 100;
            if (tier == Body.BodyTier.T6) return 90;
            if (tier == Body.BodyTier.T5) return 60;
            if (tier == Body.BodyTier.T4) return 50;
            if (tier == Body.BodyTier.T3) return 40;
            if (tier == Body.BodyTier.T2) return 30;
            if (tier == Body.BodyTier.T1) return 20;
            else return 5;
        }

        public static float SystemScale(this Body.BodyTier tier) {
            if (tier == Body.BodyTier.T9) return 45;
            if (tier == Body.BodyTier.T8) return 36;
            if (tier == Body.BodyTier.T7) return 27;
            if (tier == Body.BodyTier.T6) return 17;
            if (tier == Body.BodyTier.T5) return 12;
            if (tier == Body.BodyTier.T4) return 7.5f;
            if (tier == Body.BodyTier.T3) return 5;
            if (tier == Body.BodyTier.T2) return 2.5f;
            if (tier == Body.BodyTier.T1) return 1;
            else return 1;
        }

        public static float MapScale(this Body.BodyTier tier) {
            if (tier == Body.BodyTier.T9) return 2;
            if (tier == Body.BodyTier.T8) return 1.8f;
            if (tier == Body.BodyTier.T7) return 1.6f;
            if (tier == Body.BodyTier.T6) return 1.5f;
            if (tier == Body.BodyTier.T5) return 1.4f;
            if (tier == Body.BodyTier.T4) return 1.3f;
            if (tier == Body.BodyTier.T3) return 1.2f;
            if (tier == Body.BodyTier.T2) return 1.1f;
            if (tier == Body.BodyTier.T1) return 1;
            else return 1;
        }
    }
}