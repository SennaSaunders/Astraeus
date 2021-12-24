﻿using System.Collections.Generic;
using Code._CelestialObjects.Stations.StationIndustries;
using Code._CelestialObjects.Stations.StationServices;

namespace Code._CelestialObjects.Stations {
    public class SpaceStation : Station{
        public SpaceStation(List<StationService> stationServices, List<StationIndustry> stationIndustries) : base(stationServices, stationIndustries) {
        }
    }
}