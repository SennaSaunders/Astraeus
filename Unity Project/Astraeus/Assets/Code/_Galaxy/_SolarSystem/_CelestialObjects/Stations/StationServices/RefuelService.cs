using Code._Cargo.ProductTypes.Ships;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class RefuelService : StationService {
        public int FuelPrice { get; } = 10;//simulate price or have it fixed??
        protected override void SetGUIStrings() {
            ServiceName = "Refuel";
            GUIPath = "GUIPrefabs/Station/Services/Refuel/RefuelGUI";
        }

        public Fuel GetFuel() {
            return new Fuel(200);
        }
    }
}