namespace Code._Ships.Weapons {
    public abstract class Weapon : ShipComponent {
        public float FireRate;//weapon dependant & normalised between min/max tier values
        public float Damage;//linear increase with tier
        public float ProjectileSpeed;//fixed per weapon type
        public float MaxTravelTime;//linear increase with tier
        public float PowerDraw;//linear increase with tier
        
        //ammo type
        //public AmmoType ammoType //needs to be initialised in specific weapon types - contains a class
        
        protected Weapon(string name, ShipComponentTier componentSize, float minTierFireRate, float maxTierFireRate, float baseDamage, float projectileSpeed, float travelTime, float basePowerDraw, int baseMass) : base(name, ShipComponentType.Weapon, componentSize, baseMass) {
            FireRate = GetTierNormalizedStat(minTierFireRate, maxTierFireRate, componentSize);
            Damage = GetTierMultipliedStat(baseDamage, componentSize);
            ProjectileSpeed = projectileSpeed;
            MaxTravelTime = GetTierMultipliedStat(travelTime, componentSize);
            PowerDraw = GetTierMultipliedStat(basePowerDraw, componentSize);
        }
        
        //fire
        public void Fire() {
            
        }
    }
}