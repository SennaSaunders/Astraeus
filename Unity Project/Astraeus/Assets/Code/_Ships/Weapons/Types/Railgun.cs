namespace Code._Ships.Weapons.Types {
    public class Railgun :Weapon{
        private static float minTierFireRate = .2f;
        private static float maxTierFireRate = .15f;
        private static float baseDamage = 30;
        private static float basePowerDraw = 30;
        private static float projectileSpeed = 55;
        private static float travelTime = 5;
        private static int baseMass = 200;

        public Railgun(ShipComponentTier componentSize) : base("Railgun",componentSize, minTierFireRate, maxTierFireRate, baseDamage, projectileSpeed, travelTime, basePowerDraw, baseMass) {
        }
    }
}