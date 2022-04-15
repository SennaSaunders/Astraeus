using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types {
    public class IndustrialThruster : MainThruster {
        private static int _baseMass = 5000;
        private static float _baseForce = 50000;
        private static float _basePowerDraw = 10;
        public IndustrialThruster(ShipComponentTier componentSize) : base("Industrial",componentSize, _baseMass, _baseForce, _basePowerDraw) {
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "IndustrialThruster";
        }

        public override void SetColourChannelObjectMap() {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "IndustrialBody" }, new Color(.7f, .4f, .1f)), 
                (new List<string>() { "Exhaust", "Panels" }, new Color(.2f, .3f, .4f))
            };
        }
    }
}