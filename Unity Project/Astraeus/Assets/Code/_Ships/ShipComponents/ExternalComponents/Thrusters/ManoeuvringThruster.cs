namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters {
    //for turning
    public class ManoeuvringThruster : Thruster{
        public ManoeuvringThruster(ShipComponentTier componentSize) : base("Manoeuvring Thruster",ShipComponentType.ManoeuvringThruster, componentSize, 50, 25000, 100) {
        }
        
        public override string GetFullPath() {
            return base.GetFullPath() + "ManoeuvringThruster";
        }
    }
}