using System.Collections.Generic;
using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects {
    public abstract class Body {
        protected Body(Body primary, Vector2 coordinate, BodyTier tier, Color mapColour) {
            SetPrimary(primary);
            Coordinate = coordinate;
            Tier = tier;
            MapColour = mapColour;
        }

        protected Body(Body primary, BodyTier tier, Color mapColour) {
            SetPrimary(primary);
            Tier = tier;
            MapColour = mapColour;
        }
        
        public BodyTier Tier { get; }
        public Body Primary { get; private set; }
        public List<Body> Children { get; } = new List<Body>();
        public Color MapColour;
        public Vector2 Coordinate { get; set; }
        public float RotationStart { get; set; }
        public float RotationCurrent { get; set; }
        
        private void SetPrimary(Body primary) {
            Primary = primary;
            if (primary != null) {
                Primary.AddChild(this);
            }
        }

        private void AddChild(Body body) {
            Children.Add(body);     
        }

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

        public abstract GameObject GetSystemObject();

        public virtual GameObject GetMiniMapObject() {
            return GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }

        public void RotateBody(float timeDelta) { //do i need this? i dont think so i think the controller should do it
            // (timeDelta /_orbitalPeriod) * 360 * gameSpeed
        }
    }

    public static class BodyTierExtension {
        public static float BaseDistance(this Body.BodyTier tier) {
            return tier.SystemScale()*4;
        }

        public static float SystemScale(this Body.BodyTier tier) {
            if (tier == Body.BodyTier.T9) return 1500;
            if (tier == Body.BodyTier.T8) return 1100;
            if (tier == Body.BodyTier.T7) return 700;
            if (tier == Body.BodyTier.T6) return 350;
            if (tier == Body.BodyTier.T5) return 300;
            if (tier == Body.BodyTier.T4) return 250;
            if (tier == Body.BodyTier.T3) return 200;
            if (tier == Body.BodyTier.T2) return 150;
            if (tier == Body.BodyTier.T1) return 100;
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

        public static int TextureSize(this Body.BodyTier tier) {
            if (tier == Body.BodyTier.T9) return 550;
            if (tier == Body.BodyTier.T8) return 500;
            if (tier == Body.BodyTier.T7) return 450;
            if (tier == Body.BodyTier.T6) return 400;
            if (tier == Body.BodyTier.T5) return 350;
            if (tier == Body.BodyTier.T4) return 300;
            if (tier == Body.BodyTier.T3) return 250;
            if (tier == Body.BodyTier.T2) return 150;
            if (tier == Body.BodyTier.T1) return 100;
            else return 1;
        }

        public static int InteractDistance(this Body.BodyTier tier) {
            if (tier == Body.BodyTier.T9) return 400;
            if (tier == Body.BodyTier.T8) return 300;
            if (tier == Body.BodyTier.T7) return 250;
            if (tier == Body.BodyTier.T6) return 210;
            if (tier == Body.BodyTier.T5) return 180;
            if (tier == Body.BodyTier.T4) return 150;
            if (tier == Body.BodyTier.T3) return 120;
            if (tier == Body.BodyTier.T2) return 110;
            if (tier == Body.BodyTier.T1) return 100;
            else return 80;
        }
    }
}