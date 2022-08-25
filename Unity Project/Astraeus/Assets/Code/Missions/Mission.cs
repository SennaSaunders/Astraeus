using System;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._GameControllers;
using Code._Utility;

namespace Code.Missions {
    public abstract class Mission {
        protected GameController _gameController;
        public SpaceStation MissionPickupLocation { get; }
        public Faction MissionGiver { get; }
        public int RewardCredits { get; protected set; }

        protected Mission(SpaceStation missionPickupLocation, Faction missionGiver) {
            _gameController = GameObjectHelper.GetGameController();
            MissionPickupLocation = missionPickupLocation;
            MissionGiver = missionGiver;
            _gameController = GameObjectHelper.GetGameController();
        }

        public void GiveReward() {
            _gameController.PlayerProfile.AddCredits(RewardCredits);
        }
    }
}