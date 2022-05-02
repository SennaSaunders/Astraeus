using System;
using System.Collections.Generic;
using Code._Ships.Hulls.Types.Cargo;
using Code._Ships.Hulls.Types.Fighter;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class ShipyardService : StationService {
        protected override void SetGUIStrings() {
            ServiceName = "Shipyard";
            GUIPath = "GUIPrefabs/Station/Services/Shipyard/ShipyardGUI";
        }
        
        public List<Type> GetShipHulls() {
            List<Type> hullTypes = new List<Type> {
                typeof(SmallCargoHull),
                typeof(TriHauler),
                typeof(SmallFighterHull),
                typeof(SleekFighter),
                typeof(HeavyFighter)
            };
            return hullTypes;
        }
    }
}