using System.Collections.Generic;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;
using Code._Galaxy._SolarSystem._CelestialObjects.Star;
using Code._Galaxy.GalaxyComponents;
using Code.TextureGen;
using UnityEngine;

namespace Code._Galaxy._SolarSystem {
    public class SolarSystem {
        public Sector Sector { get; set; }
        public Vector2 Coordinate { get; }
        public List<Body> Bodies { get; set; }
        public Body Primary { get; }

        public string SystemName;

        public Faction OwnerFaction { get; set; }

        public SolarSystemStats SystemStats;
        
        public SolarSystem(Vector2 coordinate, Body primary, List<Body> bodies, string systemName) {
            Coordinate = coordinate;
            Bodies = bodies;
            Primary = primary;
            SetSystemStats(Bodies);
            SystemName = systemName;
        }

        

        public void GenerateSolarSystemPlanetColours() {
            for (int i =0;i<Bodies.Count; i++) {
                Body currentBody = Bodies[i];
                if (currentBody.GetType() == typeof(Planet)) {
                    ((Planet)currentBody).GeneratePlanetColours();
                }
            }
        }
        
        public void GenerateSolarSystemTextures() {
            for (int i =0;i<Bodies.Count; i++) {
                Body currentBody = Bodies[i];
                if (currentBody.GetType() == typeof(Planet)) {
                    ((Planet)currentBody).GeneratePlanetTexture();
                }
            }
        }
        
        private void SetSystemStats(List<Body> bodies) {
            SolarSystemStats systemStats = new SolarSystemStats();
            foreach (Body body in bodies) {
                if (body.GetType().IsSubclassOf(typeof(CelestialBody))) {
                    if (body.GetType() == typeof(BlackHole)) {
                        systemStats.blackHoleCount++;
                    }else if (body.GetType() == typeof(Star)) {
                        systemStats.starCount++;
                    }else if (body.GetType() == typeof(Planet)) {
                        systemStats.planetCount++;
                        if (((Planet)body).PlanetGen.GetType() == typeof(EarthWorldGen)) {
                            systemStats.earthWorldCount++;
                        } else if (((Planet)body).PlanetGen.GetType() == typeof(WaterWorldGen)) {
                            systemStats.waterWorldCount++;
                        }
                        else if (((Planet)body).PlanetGen.GetType() == typeof(RockyWorldGen)) {
                            systemStats.rockyWorldCount++;
                        }
                    }

                    systemStats.celestialBodyCount++;
                }
            }

            SystemStats = systemStats;
        }

        // public static float GetFurthestDistance(List<Body> bodies) {
        //     //get body attached to primaries get furthest child and it's furthest etc.
        //     Body primary = bodies.Find(b => b.Primary == null);
        //     List<Body> attachedToPrimary = bodies.FindAll(b => b.Primary == primary);
        //     bool stillChildren = true;
        //
        //     float distance = 0;
        //     while (stillChildren && attachedToPrimary.Count > 0) {
        //         Body furthest = attachedToPrimary.OrderByDescending(b => b.Coordinate.x).ToList()[0];
        //         distance += furthest.Coordinate.x;
        //         
        //         attachedToPrimary = furthest.Children;
        //         if (attachedToPrimary.Count == 0) {
        //             stillChildren = false;
        //         }
        //     }
        //     
        //     return distance;
        // }
    }
}