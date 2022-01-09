namespace Code._Ships.Thrusters {
    public abstract class Thruster : ShipComponent {
        public float Force;
        public float PowerDraw;

        protected Thruster(string name, ShipComponentType componentType, ShipComponentTier componentSize, int baseMass, float baseForce, float basePowerDraw) : base(name,componentType, componentSize, baseMass) {
            Force = GetTierMultipliedStat(baseForce, componentSize);
            PowerDraw = GetTierMultipliedStat(basePowerDraw, componentSize);
        }
    }
}