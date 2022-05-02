using System.Collections.Generic;
using Code._Ships;

namespace Code._PlayerProfile {
    public class PlayerProfile {
        public long _credits { get; private set; } = 1000000;
        public List<Ship> Ships = new List<Ship>();

        public bool ChangeCredits(int changeAmount) {
            long creditsAfterChange = _credits + changeAmount;
            if (creditsAfterChange < 0) {//can't have negative credits
                return false;
            }
            _credits = creditsAfterChange;
            return true;
        }
        
        
    }
}