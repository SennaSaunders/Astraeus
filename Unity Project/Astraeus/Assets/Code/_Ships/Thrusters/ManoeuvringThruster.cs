namespace Code._Ships.Thrusters {
    //for turning
    public class ManoeuvringThruster : Thruster{
        public ManoeuvringThruster(int componentSize,int mass, float force, float powerDraw) : base(ShipComponentType.ManoeuvringThruster, componentSize, mass, force, powerDraw) {
        }
    }
}