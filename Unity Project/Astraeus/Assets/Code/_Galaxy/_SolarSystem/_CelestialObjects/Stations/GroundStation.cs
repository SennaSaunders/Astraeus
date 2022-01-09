﻿using System.Collections.Generic;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationIndustries;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations {
    public class GroundStation : Station {
        public GroundStation(List<StationService> stationServices, List<StationIndustry> stationIndustries) : base(stationServices, stationIndustries) {
        }
    }
}