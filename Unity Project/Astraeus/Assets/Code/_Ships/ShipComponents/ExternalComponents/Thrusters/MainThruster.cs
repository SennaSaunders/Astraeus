namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters {
    //for acceleration and deceleration
    public abstract class MainThruster : Thruster {
        protected MainThruster(string name, ShipComponentTier componentSize,int mass, float force,  float powerDraw) : base( name + " Thruster",ShipComponentType.MainThruster, componentSize, mass, force, powerDraw) {
        }
    }
}