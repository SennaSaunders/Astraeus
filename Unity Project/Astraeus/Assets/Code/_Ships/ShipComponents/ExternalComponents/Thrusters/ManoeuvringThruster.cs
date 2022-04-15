using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters {
    //for turning
    public class ManoeuvringThruster : Thruster{
        public ManoeuvringThruster(ShipComponentTier componentSize) : base("Manoeuvring Thruster",ShipComponentType.ManoeuvringThruster, componentSize, 50, 25000, 5) {
        }
        
        public override string GetFullPath() {
            return base.GetFullPath() + "ManoeuvringThruster";
        }

        public override void SetColourChannelObjectMap() {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "ManoeuvringThruster" }, new Color(.2f, .2f, .2f))
            };
        }
    }
}