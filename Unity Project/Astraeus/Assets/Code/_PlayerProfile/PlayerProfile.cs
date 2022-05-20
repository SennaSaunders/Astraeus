using System.Collections.Generic;
using Code._Ships;
using Code.Missions;

namespace Code._PlayerProfile {
    public class PlayerProfile {
        public long _credits { get; private set; } = 1000000;
        public List<Ship> Ships = new List<Ship>();
        public List<Mission> Missions = new List<Mission>();

        public bool AddCredits(int changeAmount) {
            long creditsAfterChange = _credits + changeAmount;
            if (creditsAfterChange < 0) {//can't have negative credits
                return false;
            }
            _credits = creditsAfterChange;
            return true;
        }
        
        
    }
}