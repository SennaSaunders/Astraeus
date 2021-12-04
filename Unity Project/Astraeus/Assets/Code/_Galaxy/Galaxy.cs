using System.Collections.Generic;

namespace Code._Galaxy {
    public class Galaxy {
        public List<_SolarSystem.SolarSystem> Systems { get; }

        public Galaxy(List<_SolarSystem.SolarSystem> systems) {
            Systems = systems;
        }
    }
}
