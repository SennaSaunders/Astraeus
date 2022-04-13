using System.Collections.Generic;
using Code._Ships.Hulls;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class ShipyardService : StationService {
        private List<Hull> hulls;

        protected override void SetGUIStrings() {
            ServiceName = "Shipyard";
            GUIPath = "GUIPrefabs/Station/Services/Shipyard/ShipyardGUI";
        }
        void SetShipHulls() {
            hulls = new List<Hull>();
            hulls.Add(new SmallCargoHull());
            hulls.Add(new SmallFighterHull());
        }

        
    }
}