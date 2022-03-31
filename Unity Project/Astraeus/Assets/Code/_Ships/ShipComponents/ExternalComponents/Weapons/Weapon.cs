namespace Code._Ships.ShipComponents.ExternalComponents.Weapons {
    public abstract class Weapon : ExternalComponent {
        protected const string ComponentTypePath = "Weapons/";
        public float FireRate;//weapon dependant & normalised between min/max tier values
        public float Damage;//linear increase with tier
        public float ProjectileSpeed;//fixed per weapon type
        public float MaxTravelTime;//linear increase with tier
        public float PowerDraw;//linear increase with tier
        
        //ammo type
        //public AmmoType ammoType //needs to be initialised in specific weapon types - contains a class
        
        protected Weapon(string componentName, ShipComponentTier componentSize, float minTierFireRate, float maxTierFireRate, float baseDamage, float projectileSpeed, float travelTime, float basePowerDraw, int baseMass) : base(componentName, ShipComponentType.Weapon, componentSize, baseMass) {
            FireRate = GetTierNormalizedStat(minTierFireRate, maxTierFireRate, componentSize);
            Damage = GetTierMultipliedValue(baseDamage, componentSize);
            ProjectileSpeed = projectileSpeed;
            MaxTravelTime = GetTierMultipliedValue(travelTime, componentSize);
            PowerDraw = GetTierMultipliedValue(basePowerDraw, componentSize);
        }

        public override string GetFullPath() {
            return base.GetFullPath() + ComponentTypePath;
        }
    }
}