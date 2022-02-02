using System.Collections.Generic;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationIndustries;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations {
    public class GroundStation : IStation {
        public GroundStation(List<StationService> stationServices, List<StationIndustry> stationIndustries) {
        }

        public List<StationService> StationServices { get; set; }
        public List<StationIndustry> StationIndustries { get; set; }
        public void SetStationServices(List<StationService> stationServices) {
            StationServices = stationServices;
        }

        public void SetStationIndustries(List<StationIndustry> stationIndustries) {
            StationIndustries = stationIndustries;
        }
    }
}