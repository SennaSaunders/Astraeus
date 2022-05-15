using System;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._GameControllers;

namespace Code.Missions {
    public abstract class Mission {
        protected static Random r = new Random();
        public SpaceStation MissionPickupLocation { get; }
        public Faction MissionGiver { get; }
        public int RewardCredits { get; protected set; }

        protected Mission(SpaceStation missionPickupLocation, Faction missionGiver) {
            MissionPickupLocation = missionPickupLocation;
            MissionGiver = missionGiver;
        }

        public void GiveReward() {
            GameController.PlayerProfile.ChangeCredits(RewardCredits);
        }
    }
}