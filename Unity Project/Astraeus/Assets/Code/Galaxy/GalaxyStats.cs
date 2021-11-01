using System;

namespace Code.Galaxy {
    [Serializable]
    public class GalaxyStats {
        public int systemCount = 0;
        public int blackHole = 0;
        public int starCount = 0;
        public int planetCount = 0;
        public int smallestSystem = Int32.MaxValue;
        public int numSmallestSystem = 0;
        public int largestSystem = 0;
        public int numLargestSystem = 0;
    }
}