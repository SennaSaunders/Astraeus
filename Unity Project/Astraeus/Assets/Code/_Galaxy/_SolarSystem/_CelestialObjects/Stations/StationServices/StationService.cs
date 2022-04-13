namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public abstract class StationService {
        protected StationService() {
            Setup();
        }

        //need a link to the GUI for the service
        public string GUIPath { get; protected set; }
        public string ServiceName { get; protected set;}

        protected abstract void SetGUIStrings();

        private void Setup() {
            SetGUIStrings();
        }
    }
}