using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types {
    public class PrimitiveThruster : MainThruster {
        private static int _baseMass = 3000;
        private static float _baseForce = 75000;
        private static float _basePowerDraw = 20;

        public PrimitiveThruster(ShipComponentTier componentSize) : base("Primitive",componentSize, _baseMass, _baseForce, _basePowerDraw, 1500) {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "PrimitiveBody" }, new Color(.3f, .5f, 1f)), 
                (new List<string>() { "ThrusterCone", "ThrusterVent" }, new Color(.3f, .3f, .3f))
            };
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "PrimitiveThruster";
        }
    }
}