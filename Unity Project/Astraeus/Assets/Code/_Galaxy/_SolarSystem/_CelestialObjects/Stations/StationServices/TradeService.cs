namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class TradeService :StationService{
        protected override void SetGUIStrings() {
            ServiceName = "Outfitting";
            GUIPath = "GUIPrefabs/Station/Services/Trade/TradeGUI";
        }
    }
}