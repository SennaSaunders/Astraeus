namespace Code._Ships.Thrusters {
    //for acceleration and deceleration
    public class MainThruster : Thruster {
        public MainThruster(int componentSize, int mass, float force, float powerDraw) : base(ShipComponentType.MainThruster, componentSize, mass, force, powerDraw) {
        }
    }
}