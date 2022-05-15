using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._Ships;

namespace Code.Missions {
    public class BountyHuntMission : Mission {
        public BountyHuntMission(SpaceStation missionPickupLocation, Faction missionGiver) : base(missionPickupLocation, missionGiver) {
        }

        public Ship TargetShip;

        private void CreateTargetShip() {
            
        }
    }
}