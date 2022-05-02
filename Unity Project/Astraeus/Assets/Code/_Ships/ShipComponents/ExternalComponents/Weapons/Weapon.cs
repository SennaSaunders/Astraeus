namespace Code._Ships.ShipComponents.ExternalComponents.Weapons {
    public abstract class Weapon : ExternalComponent {
        public float FireDelay;          //normalised between min/max tier values
        public float Damage;            //linear increase with tier
        public float ProjectileSpeed;   //fixed per weapon type
        public float MaxTravelTime;     //linear increase with tier
        public float PowerDraw;         //linear increase with tier
        public float RotationSpeed;
        
        
        //ammo type
        //public AmmoType ammoType //needs to be initialised in specific weapon types - contains a class
        
        protected Weapon(string componentName, ShipComponentTier componentSize, float minTierFireRate, float maxTierFireRate, float baseDamage, float projectileSpeed, float travelTime, float basePowerDraw, float minTierRotationSpeed, float maxTierRotationSpeed, int baseMass, int basePrice) : base(componentName, ShipComponentType.Weapon, componentSize, baseMass,basePrice) {
            FireDelay = GetTierNormalizedStat(minTierFireRate, maxTierFireRate, componentSize);
            Damage = GetTierMultipliedValue(baseDamage, componentSize);
            ProjectileSpeed = projectileSpeed;
            MaxTravelTime = GetTierMultipliedValue(travelTime, componentSize);
            PowerDraw = GetTierMultipliedValue(basePowerDraw, componentSize);
            RotationSpeed = GetTierNormalizedStat(minTierRotationSpeed, maxTierRotationSpeed, componentSize);
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "Weapons/";
        }

        public virtual string GetProjectilePath() {
            return base.GetFullPath() + "Weapons/" + "Projectiles/";
        }
    }
}