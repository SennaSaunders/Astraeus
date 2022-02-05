namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public abstract class StationService {
        protected StationService() {
            Setup();
        }

        //need a link to the GUI for the service
        public string guiString;
        public string serviceName;

        protected abstract void SetGUIStrings();

        private void Setup() {
            SetGUIStrings();
        }
    }
}