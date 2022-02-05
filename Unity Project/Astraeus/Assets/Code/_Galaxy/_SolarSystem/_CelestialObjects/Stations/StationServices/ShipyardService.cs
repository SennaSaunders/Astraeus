using System.Collections.Generic;
using Code._Ships.Hulls;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class ShipyardService : StationService {
        private List<Hull> hulls;

        protected override void SetGUIStrings() {
            serviceName = "Shipyard";
        }
        void SetShipHulls() {
            hulls = new List<Hull>();
            hulls.Add(new MedCargoHull());
            hulls.Add(new SmallFighterHull());
        }

        
    }
}