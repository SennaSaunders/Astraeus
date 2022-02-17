using System;

namespace Code._Galaxy {
    [Serializable]
    public class GalaxyStats {
        public int systemCount;
        public int blackHole;
        public int starCount;
        public int planetCount;
        public int smallestSystem = Int32.MaxValue;
        public int numSmallestSystem;
        public int largestSystem;
        public int numLargestSystem;
    }
}