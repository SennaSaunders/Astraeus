using System.Collections.Generic;
using System.Linq;
using Code._CelestialObjects;
using UnityEngine;

namespace Code._Galaxy._SolarSystem {
    public class SolarSystem {
        public Vector2 Coordinate { get; }
        public List<Body> Bodies { get; }
        public Body Primary { get; }
        
        public SolarSystem(Vector2 coordinate, Body primary, List<Body> bodies) {
            Coordinate = coordinate;
            Bodies = bodies;
            Primary = primary;

        }

        public static float GetFurthestDistance(List<Body> bodies) {
            //get body attached to primaries get furthest child and it's furthest etc.
            Body primary = bodies.Find(b => b.Primary == null);
            List<Body> attachedToPrimary = bodies.FindAll(b => b.Primary == primary);
            bool stillChildren = true;

            float distance = 0;
            while (stillChildren && attachedToPrimary.Count > 0) {
                Body furthest = attachedToPrimary.OrderByDescending(b => b.Coordinate.x).ToList()[0];
                distance += furthest.Coordinate.x;
                
                attachedToPrimary = furthest.Children;
                if (attachedToPrimary.Count == 0) {
                    stillChildren = false;
                }
            }
            
            return distance;
        }
        
    }
}