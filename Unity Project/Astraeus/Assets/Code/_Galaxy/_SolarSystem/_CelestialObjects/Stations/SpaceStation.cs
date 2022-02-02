using System.Collections.Generic;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationIndustries;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations {
    public class SpaceStation : Body, IStation{
        public List<StationService> StationServices { get; set; }
        public List<StationIndustry> StationIndustries { get; set; }

        public void SetStationServices(List<StationService> stationServices) {
            StationServices = stationServices;
        }

        public void SetStationIndustries(List<StationIndustry> stationIndustries) {
            StationIndustries = stationIndustries;
        }


        public override GameObject GetSystemObject() {
            throw new System.NotImplementedException();
        }

        public SpaceStation(Body primary, BodyTier tier) : base(primary, tier) {
        }
    }
}