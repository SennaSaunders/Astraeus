namespace Code._Ships.ShipComponents.ExternalComponents.Weapons.Types {
    public class BallisticCannon : Weapon {
        private static float minTierFireDelay = .75f;
        private static float maxTierFireDelay = 1.5f;
        private static float baseDamage = 10;
        private static float basePowerDraw = 2;
        private static float projectileSpeed = 50;
        private static float travelTime = 10;
        private static int baseMass = 100;
        private static float minTierRotationSpeed = 5;
        private static float maxTierRotationSpeed = 3;

        public BallisticCannon(ShipComponentTier componentSize) : base("Ballistic Cannon", componentSize, minTierFireDelay, maxTierFireDelay, baseDamage, projectileSpeed, travelTime, basePowerDraw, minTierRotationSpeed, maxTierRotationSpeed,baseMass) {
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "BallisticCannon";
        }

        public override string GetProjectilePath() {
            return base.GetProjectilePath() + "PhysicalProjectile";
        }
    }
}