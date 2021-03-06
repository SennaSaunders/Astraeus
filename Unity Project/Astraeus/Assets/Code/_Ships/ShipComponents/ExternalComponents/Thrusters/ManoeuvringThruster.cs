using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters {
    public class ManoeuvringThruster : Thruster {//for turning and strafing
        public ManoeuvringThruster(ShipComponentTier componentSize) : base("Manoeuvring Thruster", ShipComponentType.ManoeuvringThruster, componentSize, 50, 25000, 5, 500) {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() { (new List<string>() { "ManoeuvringThruster" }, new Color(.2f, .2f, .2f)) };
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "ManoeuvringThruster";
        }
    }
}