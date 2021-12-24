namespace Code._Ships.Thrusters {
    public class Thruster : ShipComponent {
        public float Force;
        public float PowerDraw;

        public Thruster(ShipComponentType componentType, int componentSize, int mass, float force, float powerDraw) : base(componentType, componentSize, mass) {
            Force = force;
            PowerDraw = powerDraw;
        }
    }
}