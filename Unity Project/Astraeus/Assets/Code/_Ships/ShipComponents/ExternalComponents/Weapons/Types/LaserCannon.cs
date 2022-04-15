using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Weapons.Types {
    public class LaserCannon : Weapon {
        private static float minTierFireDelay = .3f;
        private static float maxTierFireDelay = 1f;
        private static float baseDamage = 5;
        private static float basePowerDraw = 5;
        private static float projectileSpeed = 100;
        private static float travelTime = 10;
        private static int baseMass = 100;
        private static float minTierRotationSpeed = 5;
        private static float maxTierRotationSpeed = 3;

        public LaserCannon(ShipComponentTier componentSize) : base("Laser Cannon",componentSize, minTierFireDelay, maxTierFireDelay, baseDamage, projectileSpeed, travelTime, basePowerDraw, minTierRotationSpeed, maxTierRotationSpeed, baseMass) {
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "LaserCannon";
        }
        
        public override string GetProjectilePath() {
            return base.GetProjectilePath() + "LaserProjectile";
        }

        public override void SetColourChannelObjectMap() {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "TurretBase"},new Color(.2f,.2f,.2f)),
                (new List<string>() { "WeaponBarrel" }, new Color(.7f, .2f, .2f)),
                (new List<string>() { "TurretSpindle", "TurretBracket" }, new Color(.2f, .2f, .2f)),
                (new List<string>() { "WeaponBase" }, new Color(.7f, .2f, .2f))
            };
        }
    }
}