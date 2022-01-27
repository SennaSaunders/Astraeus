namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types {
    public class TechThruster : MainThruster {
        private static int _baseMass = 4000;
        private static float _baseForce = 10000;
        private static float _basePowerDraw = 1000;
        public TechThruster(ShipComponentTier componentSize) : base("Tech",componentSize, _baseMass, _baseForce, _basePowerDraw) {
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "TechThruster";
        }
    }
}