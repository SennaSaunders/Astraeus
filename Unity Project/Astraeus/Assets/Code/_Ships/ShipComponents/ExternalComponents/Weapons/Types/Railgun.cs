using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Weapons.Types {
    public class Railgun :Weapon{
        private static float minTierFireDelay = 2;
        private static float maxTierFireDelay = 5;
        private static float baseDamage = 200;
        private static float basePowerDraw = 100;
        private static float projectileSpeed = 500;
        private static float travelTime = 3;
        private static int baseMass = 200;
        private static float minTierRotationSpeed = 3;
        private static float maxTierRotationSpeed = 1;

        public Railgun(ShipComponentTier componentSize) : base("Railgun",componentSize, minTierFireDelay, maxTierFireDelay, baseDamage, projectileSpeed, travelTime, basePowerDraw, minTierRotationSpeed, maxTierRotationSpeed,baseMass,3000) {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "Barrel" }, new Color(.2f, .2f, .2f)),
                (new List<string>() { "GunBack", "StrutBottom", "StrutTopLeft", "StrutTopRight" }, new Color(.4f, .1f, .5f)),
                (new List<string>() { "TurretSpindle", "GunBase" }, new Color(.2f, .2f, .2f)),
                (new List<string>() { "ChargingSlit" }, new Color(.3f, .5f, 1f)),
                (new List<string>() { "TurretBase" }, new Color(.2f, .2f, .2f)),
            };
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "Railgun";
        }
        
        public override string GetProjectilePath() {
            return base.GetProjectilePath() + "PhysicalProjectile";
        }
    }
}