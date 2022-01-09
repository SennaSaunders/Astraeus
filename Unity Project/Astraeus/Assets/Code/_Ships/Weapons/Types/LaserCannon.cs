namespace Code._Ships.Weapons.Types {
    public class LaserCannon : Weapon {
        private static float minTierFireRate = 2f;
        private static float maxTierFireRate = 3f;
        private static float baseDamage = 5;
        private static float basePowerDraw = 10;
        private static float projectileSpeed = 70;
        private static float travelTime = 3;
        private static int baseMass = 100;

        public LaserCannon(ShipComponentTier componentSize) : base("Laser Cannon",componentSize, minTierFireRate, maxTierFireRate, baseDamage, projectileSpeed, travelTime, basePowerDraw, baseMass) {
        }
    }
}