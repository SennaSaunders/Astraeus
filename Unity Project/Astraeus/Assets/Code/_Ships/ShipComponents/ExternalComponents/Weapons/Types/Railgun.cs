namespace Code._Ships.ShipComponents.ExternalComponents.Weapons.Types {
    public class Railgun :Weapon{
        private static float minTierFireRate = .2f;
        private static float maxTierFireRate = .15f;
        private static float baseDamage = 30;
        private static float basePowerDraw = 30;
        private static float projectileSpeed = 55;
        private static float travelTime = 5;
        private static int baseMass = 200;
        private static float minTierRotationSpeed = 3;
        private static float maxTierRotationSpeed = 1;

        public Railgun(ShipComponentTier componentSize) : base("Railgun",componentSize, minTierFireRate, maxTierFireRate, baseDamage, projectileSpeed, travelTime, basePowerDraw, minTierRotationSpeed, maxTierRotationSpeed,baseMass) {
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "Railgun";
        }
        
        public override string GetProjectilePath() {
            return base.GetProjectilePath() + "PhysicalProjectile";
        }
    }
}