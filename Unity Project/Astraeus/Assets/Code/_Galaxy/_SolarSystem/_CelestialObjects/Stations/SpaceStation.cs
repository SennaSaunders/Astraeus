using System.Collections.Generic;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationIndustries;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations {
    public class SpaceStation : Body, IStation {
        private string spaceStationPath = "Bodies/SpaceStations/SpaceStation";
        public SolarSystem SolarSystem { get; private set; }
        public List<StationService> StationServices { get; set; }
        public List<StationIndustry> StationIndustries { get; set; }

        public void SetStationServices(List<StationService> stationServices) {
            StationServices = stationServices;
        }

        public void SetStationIndustries(List<StationIndustry> stationIndustries) {
            StationIndustries = stationIndustries;
        }


        public override GameObject GetSystemObject() {
            return (GameObject)Resources.Load(spaceStationPath);
        }

        public override GameObject GetMiniMapObject() {
            return GameObject.CreatePrimitive(PrimitiveType.Cube);
        }

        public SpaceStation(Body primary, SolarSystem solarSystem) : base(primary, BodyTier.T0, new Color(.7f,.7f,.7f)) {
            SolarSystem = solarSystem;
        }
    }
}