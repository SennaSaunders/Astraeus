using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Weapons.Types {
    public class BallisticCannon : Weapon {
        private static float minTierFireDelay = .6f;
        private static float maxTierFireDelay = 1.2f;
        private static float baseDamage = 50;
        private static float basePowerDraw = 15;
        private static float projectileSpeed = 300;
        private static float travelTime = 3;
        private static int baseMass = 250;
        private static float minTierRotationSpeed = 4;
        private static float maxTierRotationSpeed = 2;

        public BallisticCannon(ShipComponentTier componentSize) : base("Ballistic Cannon", componentSize, minTierFireDelay, maxTierFireDelay, baseDamage, projectileSpeed, travelTime, basePowerDraw, minTierRotationSpeed, maxTierRotationSpeed,baseMass,1000) {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "GunBarrel", "RecoilCompensator", "GunBack" }, new Color(.2f,.2f,.2f)),
                (new List<string>() { "TurretSpindle", "GunBase" }, new Color(.2f,.2f,.5f)),
                (new List<string>() { "TurretBase" }, Color.black)
            };
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "BallisticCannon";
        }

        public override string GetProjectilePath() {
            return base.GetProjectilePath() + "PhysicalProjectile";
        }
    }
}