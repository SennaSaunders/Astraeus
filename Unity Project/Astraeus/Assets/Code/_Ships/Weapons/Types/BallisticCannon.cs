namespace Code._Ships.Weapons.Types {
    public class BallisticCannon : Weapon {
        private static float minTierFireRate = 1.5f;
        private static float maxTierFireRate = .7f;
        private static float baseDamage = 10;
        private static float basePowerDraw = 5;
        private static float projectileSpeed = 30;
        private static float travelTime = 5;
        private static int baseMass = 100;

        public BallisticCannon(ShipComponentTier componentSize) : base("Ballistic Cannon", componentSize, minTierFireRate, maxTierFireRate, baseDamage, projectileSpeed, travelTime, basePowerDraw, baseMass) {
        }
    }
}