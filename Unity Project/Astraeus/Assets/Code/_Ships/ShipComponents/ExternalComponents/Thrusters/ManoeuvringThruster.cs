namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters {
    //for turning
    public class ManoeuvringThruster : Thruster{
        public ManoeuvringThruster(string name, ShipComponentTier componentSize,int mass, float force, float powerDraw) : base( name+ " Manoeuvring Thruster",ShipComponentType.ManoeuvringThruster, componentSize, mass, force, powerDraw) {
        }
    }
}