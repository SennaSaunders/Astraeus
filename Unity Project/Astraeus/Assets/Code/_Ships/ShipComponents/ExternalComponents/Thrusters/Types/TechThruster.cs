using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types {
    public class TechThruster : MainThruster {
        private static int _baseMass = 4000;
        private static float _baseForce = 100000;
        private static float _basePowerDraw = 50;
        public TechThruster(ShipComponentTier componentSize) : base("Tech",componentSize, _baseMass, _baseForce, _basePowerDraw) {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "TechThruster" }, new Color(.7f, .4f, .2f))
            };
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "TechThruster";
        }
    }
}