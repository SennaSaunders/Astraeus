namespace Code._Ships.Thrusters {
    //for acceleration and deceleration
    public abstract class MainThruster : Thruster {
        protected MainThruster(string name, ShipComponentTier componentSize,int mass, float force,  float powerDraw) : base( name + " Thruster",ShipComponentType.MainThruster, componentSize, mass, force, powerDraw) {
        }
    }

    public class PrimitiveThruster : MainThruster {
        private static int _baseMass = 3000;
        private static float _baseForce = 5000;
        private static float _basePowerDraw = 500;

        public PrimitiveThruster(ShipComponentTier componentSize) : base("Primitive",componentSize, _baseMass, _baseForce, _basePowerDraw) {
        }
    }

    public class TechThruster : MainThruster {
        private static int _baseMass = 4000;
        private static float _baseForce = 10000;
        private static float _basePowerDraw = 1000;
        public TechThruster(ShipComponentTier componentSize) : base("Tech",componentSize, _baseMass, _baseForce, _basePowerDraw) {
        }
    }

    public class IndustrialThruster : MainThruster {
        private static int _baseMass = 5000;
        private static float _baseForce = 2000;
        private static float _basePowerDraw = 100;
        public IndustrialThruster(ShipComponentTier componentSize) : base("Industrial",componentSize, _baseMass, _baseForce, _basePowerDraw) {
        }
    }
}