using System.Collections.Generic;
using Code.CelestialObjects;
using UnityEngine;

namespace Code.Galaxy {
    public class SolarSystem {
        public Vector2 Coordinate { get; }
        public List<Body> Bodies { get; }

        public SolarSystem(Vector2 coordinate, List<Body> bodies) {
            Coordinate = coordinate;
            Bodies = bodies;
        }
    }
}