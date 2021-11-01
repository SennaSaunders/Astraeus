using System.Collections.Generic;
namespace Code.Galaxy {
    public class Galaxy {
        public List<SolarSystem> Systems { get; }

        public Galaxy(List<SolarSystem> systems) {
            Systems = systems;
        }
    }
}
