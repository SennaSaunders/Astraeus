using System.Collections.Generic;
using Code._Ships.Hulls;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class ShipyardService : StationService {
        private List<Hull> hulls;

        void SetShipHulls() {
            hulls = new List<Hull>();
            hulls.Add(gameObject.AddComponent<MedCargoHull>());
            hulls.Add(gameObject.AddComponent<SmallFighterHull>());
        }
    }
}