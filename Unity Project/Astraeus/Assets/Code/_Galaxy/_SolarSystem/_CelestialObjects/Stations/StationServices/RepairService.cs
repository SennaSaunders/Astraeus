namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class RepairService : StationService {
        protected override void SetGUIStrings() {
            ServiceName = "Repair";
            GUIPath = "GUIPrefabs/Station/Services/Repair/RepairGUI";
        }
    }
}