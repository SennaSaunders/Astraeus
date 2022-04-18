using Code._Cargo.ProductTypes.Ships;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class RefuelService : StationService {
        protected override void SetGUIStrings() {
            ServiceName = "Refuel";
            GUIPath = "GUIPrefabs/Station/Services/Refuel/RefuelGUI";
        }

        public Fuel GetFuel() {
            return new Fuel();
        }
    }
}