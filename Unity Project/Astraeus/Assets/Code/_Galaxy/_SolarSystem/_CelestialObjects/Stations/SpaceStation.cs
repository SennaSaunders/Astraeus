using System.Collections.Generic;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationIndustries;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations {
    public class SpaceStation : Body, IStation {
        private string spaceStationPath = "SpaceStations/SpaceStation";
        public List<StationService> StationServices { get; set; }
        public List<StationIndustry> StationIndustries { get; set; }

        public void SetStationServices(List<StationService> stationServices) {
            StationServices = stationServices;
        }

        public void SetStationIndustries(List<StationIndustry> stationIndustries) {
            StationIndustries = stationIndustries;
        }


        public override GameObject GetSystemObject() {
            return GameController._prefabHandler.instantiateObject(GameController._prefabHandler.loadPrefab(spaceStationPath));
        }

        public SpaceStation(Body primary) : base(primary, BodyTier.T0) {
        }
    }
}