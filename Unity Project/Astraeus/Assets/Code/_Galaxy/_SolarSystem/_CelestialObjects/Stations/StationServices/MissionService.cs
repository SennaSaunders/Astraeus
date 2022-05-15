using System;
using Code._Galaxy._Factions;
using Code.Missions;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices {
    public class MissionService : StationService{
        private Faction _faction;
        private SpaceStation _spaceStation;
        public MissionService(Faction faction, SpaceStation spaceStation) {
            _faction = faction;
            _spaceStation = spaceStation;
        }

        protected override void SetGUIStrings() {
            ServiceName = "Missions";
            GUIPath = "GUIPrefabs/Station/Services/Missions/MissionGUI";
        }

        public TradeMission GenTradeMission() {
            return new TradeMission(_spaceStation, _faction);
        }
    }
}