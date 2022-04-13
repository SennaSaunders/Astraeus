namespace Code._Ships.ShipComponents.ExternalComponents.Weapons.Types {
    public class Railgun :Weapon{
        private static float minTierFireDelay = 2;
        private static float maxTierFireDelay = 5;
        private static float baseDamage = 30;
        private static float basePowerDraw = 15;
        private static float projectileSpeed = 150;
        private static float travelTime = 15;
        private static int baseMass = 200;
        private static float minTierRotationSpeed = 3;
        private static float maxTierRotationSpeed = 1;

        public Railgun(ShipComponentTier componentSize) : base("Railgun",componentSize, minTierFireDelay, maxTierFireDelay, baseDamage, projectileSpeed, travelTime, basePowerDraw, minTierRotationSpeed, maxTierRotationSpeed,baseMass) {
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "Railgun";
        }
        
        public override string GetProjectilePath() {
            return base.GetProjectilePath() + "PhysicalProjectile";
        }
    }
}