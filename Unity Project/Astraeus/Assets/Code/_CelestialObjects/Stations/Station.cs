﻿using System.Collections.Generic;
using Code._CelestialObjects.Stations.StationIndustries;
using Code._CelestialObjects.Stations.StationServices;

namespace Code._CelestialObjects.Stations {
    public abstract class Station {
        protected Station(List<StationService> stationServices, List<StationIndustry> stationIndustries) {
            StationServices = stationServices;
            StationIndustries = stationIndustries;
        }

        public List<StationService> StationServices { get; }
        public List<StationIndustry> StationIndustries{ get; }
    }
}