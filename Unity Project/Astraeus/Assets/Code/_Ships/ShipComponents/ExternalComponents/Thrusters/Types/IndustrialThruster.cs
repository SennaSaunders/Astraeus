namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types {
    public class IndustrialThruster : MainThruster {
        private static int _baseMass = 5000;
        private static float _baseForce = 2000;
        private static float _basePowerDraw = 100;
        public IndustrialThruster(ShipComponentTier componentSize) : base("Industrial",componentSize, _baseMass, _baseForce, _basePowerDraw) {
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "IndustrialThruster";
        }
    }
}