namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters {
    public abstract class Thruster : ExternalComponent {
        protected const string ComponentTypePath = "Thrusters/";
        public float Force;
        public float PowerDraw;

        public override string GetFullPath() {
            return base.GetFullPath() + ComponentTypePath;
        }
        protected Thruster(string componentName, ShipComponentType componentType, ShipComponentTier componentSize, int baseMass, float baseForce, float basePowerDraw) : base(componentName,componentType, componentSize, baseMass) {
            Force = GetTierMultipliedValue(baseForce, componentSize);
            PowerDraw = GetTierMultipliedValue(basePowerDraw, componentSize);
        }
        
        
    }
}