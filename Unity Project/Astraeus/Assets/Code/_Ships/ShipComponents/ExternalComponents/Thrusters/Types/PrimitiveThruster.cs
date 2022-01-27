namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types {
    public class PrimitiveThruster : MainThruster {
        private static int _baseMass = 3000;
        private static float _baseForce = 5000;
        private static float _basePowerDraw = 500;

        public PrimitiveThruster(ShipComponentTier componentSize) : base("Primitive",componentSize, _baseMass, _baseForce, _basePowerDraw) {
        }

        public override string GetFullPath() {
            return base.GetFullPath() + "PrimitiveThruster";
        }
    }
}