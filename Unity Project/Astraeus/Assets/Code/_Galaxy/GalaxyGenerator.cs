using System;
using System.Collections.Generic;
using System.Linq;
using Code._CelestialObjects;
using Code._CelestialObjects.BlackHole;
using Code._CelestialObjects.Planet;
using Code._CelestialObjects.Star;
using Code._Galaxy._SolarSystem;
using Code.TextureGen;
using UnityEngine;
using Random = System.Random;
using StarType = Code._CelestialObjects.Star.Star.StarType;
using BodyTier = Code._CelestialObjects.Body.BodyTier;

namespace Code._Galaxy {
    public class GalaxyGenerator : MonoBehaviour {
        public static Random Rng;
        public GalaxyGeneratorInput seed = new GalaxyGeneratorInput(1337);
        public GalaxyGeneratorInput maxSystems = new GalaxyGeneratorInput(1,2000,500);
        public GalaxyGeneratorInput minBodiesPerSystem = new GalaxyGeneratorInput(1,5,1);
        public GalaxyGeneratorInput maxBodiesPerSystem = new GalaxyGeneratorInput(5,20,5);
        public GalaxyGeneratorInput width = new GalaxyGeneratorInput(50,2000,1000);
        public GalaxyGeneratorInput height = new GalaxyGeneratorInput(50,2000,1000);
        public GalaxyGeneratorInput systemExclusionDiameter = new GalaxyGeneratorInput(5,20,5);

        public GalaxyStats stats;
        

        private bool IsRareRoll(int rarePercentageChance) {
            int maxRoll = 100;
            int rareRoll = maxRoll - rarePercentageChance;
            return Rng.Next(maxRoll) > rareRoll;
        }

        //Generates a galaxy
        public Galaxy GenGalaxy() {
            stats = new GalaxyStats();
            Rng = new Random(seed.GetValue());
            List<Vector2> systemCoordinates = PickDistributedPoints(maxSystems.GetValue(), width.GetValue(), height.GetValue(), systemExclusionDiameter.GetValue());
            List<SolarSystem> solarSystems = new List<SolarSystem>();
            //setup systems
            for (int i = 0; i < systemCoordinates.Count; i++) {
                CelestialBody systemPrimary;
                Vector2 systemCoordinate = systemCoordinates[i];
                int blackHoleChance = 2;
                if (IsRareRoll(blackHoleChance)) {
                    //black hole
                    systemPrimary = new BlackHole(null, new Vector2(0, 0));
                    stats.blackHole += 1;
                }
                else {
                    //star
                    systemPrimary = new Star(null, new Vector2(0, 0), PickRandomStarType());
                    stats.starCount += 1;
                }

                //pick random body count
                int numOfBodies = Rng.Next(minBodiesPerSystem.GetValue(), maxBodiesPerSystem.GetValue() + 1);
                solarSystems.Add(GenSolarSystem(systemPrimary, systemCoordinate, numOfBodies));
            }

            stats.numSmallestSystem = solarSystems.FindAll(s => s.Bodies.Count == stats.smallestSystem).ToList().Count;
            stats.numLargestSystem = solarSystems.FindAll(s => s.Bodies.Count == stats.largestSystem).ToList().Count;
            stats.systemCount = solarSystems.Count;
            return new Galaxy(solarSystems);
        }

        private static BodyTier PickRandomPlanetTier(BodyTier parent) {
            int maxTierFromParent = (int)parent - 1;
            int max = maxTierFromParent < (int)Planet.MAXPlanetTier ? maxTierFromParent : (int)Planet.MAXPlanetTier;
            return (BodyTier)Rng.Next((int)Planet.MINPlanetTier, max);
        }

        private static StarType PickRandomStarType() {
            int max = Enum.GetNames(typeof(StarType)).Length;
            StarType starType = (StarType)Rng.Next(max);
            return starType;
        }

        private static StarType PickRandomStarType(StarType type) {
            int max = (int)type;
            StarType starType = (StarType)Rng.Next(max + 1);
            return starType;
        }

        private Planet GenPlanet(Body primary, BodyTier tier) {
             int notRockyChance = 10;
             int earthChance = 30;
            
             PlanetGen planetGen;
             int size = 512;
             int planetTextureSeed = Rng.Next();
             if (IsRareRoll(notRockyChance)) {
                 if (IsRareRoll(earthChance)) { //earth-like
                     planetGen = new EarthWorldGen(planetTextureSeed, size);
                 }
                 else { //water world
                     planetGen = new WaterWorldGen(planetTextureSeed, size);
                 }
             }
             else { //rocky planet
                 planetGen = new RockyWorldGen(planetTextureSeed, size);
             }

             Planet planet = new Planet(primary, tier, planetGen);

            return planet;
        }

        private SolarSystem GenSolarSystem(CelestialBody primary, Vector2 systemCoordinate, int numOfBodies) {
            List<Body> celestialBodies = new List<Body> { primary };
            for (int i = celestialBodies.Count; i < numOfBodies; i++) {
                //find eligible primaries
                List<Body> potentialPrimaries = celestialBodies.FindAll(b => b.Tier > BodyTier.T2);
                int currentPrimaryIndex = Rng.Next(potentialPrimaries.Count);
                Body currentPrimary = potentialPrimaries[currentPrimaryIndex];
                //if currentPrimary == star then roll to pick another star
                int extraStarChance = 4;
                CelestialBody newBody;
                //stars - extraStarChance % chance
                if (currentPrimary.Primary == null && IsRareRoll(extraStarChance)) {
                    newBody = currentPrimary.GetType() == typeof(Star) ? new Star(currentPrimary, PickRandomStarType(((Star)currentPrimary).Type)) : new Star(currentPrimary, PickRandomStarType());
                    stats.starCount += 1;
                }
                //planets
                else {
                    //gen planet texture
                    newBody = GenPlanet(currentPrimary, PickRandomPlanetTier(currentPrimary.Tier));
                    stats.planetCount += 1;
                }

                celestialBodies.Add(newBody);
            }

            int systemSize = celestialBodies.Count;
            if (systemSize > stats.largestSystem) {
                stats.largestSystem = systemSize;
            }

            if (systemSize < stats.smallestSystem) {
                stats.smallestSystem = systemSize;
            }

            return new SolarSystem(systemCoordinate, primary, SetupRotations(SetupPositions(celestialBodies)));
        }

        private List<Body> SetupPositions(List<Body> bodies) {
            List<Body> bodiesCopy = bodies.ToList();
            List<List<Body>> samePrimaryBodiesLists = new List<List<Body>>();

            while (bodiesCopy.Count > 0) {
                List<Body> samePrimaryBodies = bodiesCopy.FindAll(b => b.Primary == bodiesCopy[0].Primary).ToList();
                foreach (Body body in samePrimaryBodies) {
                    bodiesCopy.Remove(body);
                }

                samePrimaryBodiesLists.Add(samePrimaryBodies);
            }

            for (int i = samePrimaryBodiesLists.Count - 1; i >= 0; i--) {
                for (int j = 0; j < samePrimaryBodiesLists[i].Count; j++) {
                    Body currentBody = samePrimaryBodiesLists[i][j];
                    Vector2 c;
                    float distance = currentBody.Tier.BaseDistance();
                    float clearChildrenDistance = GetSumMaxChildDistances(currentBody);
                    float clearPreviousBodyDistance = 0;
                    float clearPreviousBodyChildrenDistance = 0;
                    if (currentBody.Primary != null) { //if not system primary
                        float primaryBaseDistance = currentBody.Primary.Tier.BaseDistance();
                        if (j != 0) { //if not first child of primary
                            Body previousBody = samePrimaryBodiesLists[i][j - 1];
                            clearPreviousBodyDistance = previousBody.Coordinate.x;
                            clearPreviousBodyChildrenDistance = GetSumMaxChildDistances(previousBody);
                        }

                        distance += primaryBaseDistance + clearChildrenDistance + clearPreviousBodyDistance + clearPreviousBodyChildrenDistance;
                        c = new Vector2(distance, 0);
                    }
                    else {
                        c = new Vector2(0, 0);
                    }

                    currentBody.Coordinate = c;
                }
            }

            return bodies;
        }

        private static float GetSumMaxChildDistances(Body body) {
            bool stillChildren = true;
            float distance = 0;
            while (stillChildren) {
                if (body.Children.Count > 0) {
                    body = body.Children.OrderByDescending(c => c.Coordinate.x).ToList()[0];
                    distance += body.Coordinate.x;
                }
                else {
                    stillChildren = false;
                }
            }

            return distance;
        }

        private static List<Body> SetupRotations(List<Body> bodies) {
            //orbital period  - relative to distance from primary? random?
            //what units to use - seconds, minutes, hours?
            //rotation base set randomly
            foreach (Body body in bodies) {
                body.RotationBase = (float)Rng.NextDouble();
                body.RotationCurrent = body.RotationBase;
            }

            return bodies;
        }

        private class Tile {
            private readonly float _x1, _x2;
            private readonly float _y1, _y2;
            public readonly int XIndex, YIndex;

            public Tile(int xIndex, int yIndex, float x1, float x2, float y1, float y2) {
                XIndex = xIndex;
                YIndex = yIndex;
                _x1 = x1;
                _x2 = x2;
                _y1 = y1;
                _y2 = y2;
            }

            public Vector2 GetRandomPoint() {
                float x = (float)Rng.NextDouble() * (_x2 - _x1) + _x1;
                float y = (float)Rng.NextDouble() * (_y2 - _y1) + _y1;
                return new Vector2(x, y);
            }
        }

        private static Tile[,] GetTiles(float areaWidth, float areaHeight, float maxPointDiameter) {
            //get num of tiles needed
            int numXTiles = (int)Math.Ceiling(areaWidth / maxPointDiameter);
            int numYTiles = (int)Math.Ceiling(areaHeight / maxPointDiameter);

            Tile[,] grid = new Tile[numXTiles, numYTiles];

            float y1 = 0;
            for (int y = 0; y < numYTiles; y++) {
                float x1 = 0;
                float y2 = y1 + maxPointDiameter; //set y2
                for (int x = 0; x < numXTiles; x++) {
                    float x2 = x1 + maxPointDiameter; //set x2
                    grid[x, y] = new Tile(x, y, x1, x2, y1, y2); //assign tile's edges
                    x1 = x2 < areaWidth ? x2 : areaWidth; //increment x1 ensuring it doesn't go out of bounds
                }

                y1 = y2 < areaHeight ? y2 : areaHeight; //increment y1 ensuring it doesn't go out of bounds
            }

            return grid;
        }

        private static List<(int, int)> GetSurroundingIndexes(int x, int y) {
            List<(int, int)> surroundingIndexes = new List<(int, int)> {
                (x - 1, y - 1), //top left
                (x, y - 1), //top middle
                (x + 1, y - 1), //top right
                (x - 1, y), //middle left
                (x + 1, y), //middle right
                (x - 1, y + 1), //bottom left
                (x, y + 1), //bottom middle
                (x + 1, y + 1) //bottom right
            };

            return surroundingIndexes;
        }

        private bool InsideGridBounds((int x, int y) pos, int maxX, int maxY) {
            bool xValid = pos.x >= 0 && pos.x < maxX;
            bool yValid = pos.y >= 0 && pos.y < maxY;
            return xValid && yValid;
        }

        private List<Vector2> PickDistributedPoints(int maxPoints, float areaWidth, float areaHeight, float maxPointDiameter) {
            Tile[,] tileArray = GetTiles(areaWidth, areaHeight, maxPointDiameter);
            List<Tile> tileList = tileArray.Cast<Tile>().ToList();
            List<Vector2> coordinatesList = new List<Vector2>();

            while (coordinatesList.Count < maxPoints && tileList.Count > 0) {
                Tile pickedTile = tileList[Rng.Next(tileList.Count)]; //pick random tile
                tileList.Remove(pickedTile); //removes the selected tile from the potential tiles
                List<(int x, int y)> surroundingTiles = GetSurroundingIndexes(pickedTile.XIndex, pickedTile.YIndex); //get surrounding tiles
                foreach (var tile in surroundingTiles.Where(tile => InsideGridBounds(tile, tileArray.GetLength(0), tileArray.GetLength(1)))) {
                    tileList.Remove(tileArray[tile.x, tile.y]);
                }

                coordinatesList.Add(pickedTile.GetRandomPoint());
            }

            return coordinatesList;
        }
    }
}