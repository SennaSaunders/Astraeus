using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Weapons.Types {
    public class Laser : Weapon {
        private static float minTierFireDelay = .3f;
        private static float maxTierFireDelay = .6f;
        private static float baseDamage = 35;
        private static float basePowerDraw = 30;
        private static float projectileSpeed = 500;
        private static float travelTime = 2;
        private static int baseMass = 200;
        private static float minTierRotationSpeed = 5;
        private static float maxTierRotationSpeed = 3;

        public Laser(ShipComponentTier componentSize) : base("Laser", componentSize, minTierFireDelay, maxTierFireDelay, baseDamage, projectileSpeed, travelTime, basePowerDraw, minTierRotationSpeed, maxTierRotationSpeed, baseMass, 1500) {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "WeaponBarrel", "WeaponBase" }, new Color(.7f, .2f, .2f)),
                (new List<string>() { "TurretSpindle", "TurretBracket" }, new Color(.2f, .2f, .2f)),
                (new List<string>() { "TurretBase" }, new Color(.2f, .2f, .2f))
            };
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "LaserCannon";
        }

        public override string GetProjectilePath() {
            return base.GetProjectilePath() + "LaserProjectile";
        }
    }
}