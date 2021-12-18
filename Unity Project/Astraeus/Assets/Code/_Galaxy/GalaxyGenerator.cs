using System;
using System.Collections.Generic;
using System.Linq;
using Code._CelestialObjects;
using Code._CelestialObjects.BlackHole;
using Code._CelestialObjects.Planet;
using Code._CelestialObjects.Star;
using Code._Factions;
using Code._Galaxy._SolarSystem;
using Code._Galaxy.GalaxyComponents;
using Code.TextureGen;
using UnityEngine;
using Random = System.Random;
using StarType = Code._CelestialObjects.Star.Star.StarType;
using BodyTier = Code._CelestialObjects.Body.BodyTier;

namespace Code._Galaxy {
    public class GalaxyGenerator : MonoBehaviour {
        public static Random Rng = new Random();
        public GalaxyGeneratorInput seed = new GalaxyGeneratorInput(1337);
        public GalaxyGeneratorInput maxSystems = new GalaxyGeneratorInput(1, 2000, 500);
        public GalaxyGeneratorInput minBodiesPerSystem = new GalaxyGeneratorInput(1, 5, 1);
        public GalaxyGeneratorInput maxBodiesPerSystem = new GalaxyGeneratorInput(5, 20, 5);
        public GalaxyGeneratorInput width = new GalaxyGeneratorInput(50, 2000, 1000);
        public GalaxyGeneratorInput height = new GalaxyGeneratorInput(50, 2000, 1000);
        public GalaxyGeneratorInput systemExclusionDistance = new GalaxyGeneratorInput(5, 20, 5);

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
            List<Vector2> systemCoordinates = PickDistributedPoints(maxSystems.GetValue(), width.GetValue(), height.GetValue(), systemExclusionDistance.GetValue());
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

            Galaxy galaxy = new Galaxy(solarSystems, GetSectors(solarSystems));

            GenGalaxyFactions(galaxy);

            return galaxy;
        }

        private void GenGalaxyFactions(Galaxy galaxy) {
            List<Sector> sectors = galaxy.Sectors;
            List<Sector> sectorsWithBodies = sectors.FindAll(s => s.Systems.Count > 0);

            //find the ratio of factions to sectors
            int numFactionTypes = Enum.GetValues(typeof(Faction.FactionTypeEnum)).Length;
            int maxFactionSprawl = 0;
            int maxTotalFactionSprawl = 0;

            for (int i = 0; i < numFactionTypes; i++) {
                Faction.FactionTypeEnum factionType = (Faction.FactionTypeEnum)i;
                int factionSprawl = factionType.GetFactionSprawlRatio();
                maxFactionSprawl = factionSprawl > maxFactionSprawl ? factionSprawl : maxFactionSprawl;
                maxTotalFactionSprawl += factionType.GetFactionRatio() * factionSprawl;
            }

            //then set the max number of factions for each
            float maxPopulationDensity = .7f;
            float factionRatioMultiplier = (sectorsWithBodies.Count * maxPopulationDensity) / maxTotalFactionSprawl;

            List<(Faction.FactionTypeEnum type, int maxFactionsOfType)> maxFactionAmounts = new List<(Faction.FactionTypeEnum type, int maxFactionsOfType)>();
            for (int i = 0; i < numFactionTypes; i++) {
                Faction.FactionTypeEnum factionType = (Faction.FactionTypeEnum)i;
                maxFactionAmounts.Add((factionType, (int)Math.Ceiling(factionRatioMultiplier * factionType.GetFactionRatio())));
            }

            FactionTypeExtension.PreCalcDesireValues(sectors); //Pre calculate (for performance) the preferences for each sector/system for each faction

            //choose a home world for each faction
            //get the top sector in the list and add it to a chosen sector list
            //iterate through faction types first before coming back to

            List<Faction> factions = new List<Faction>();
            List<Sector> pickedSectors = new List<Sector>();
            int largestTypeNum = maxFactionAmounts.OrderByDescending(a => a.maxFactionsOfType).ToList()[0].maxFactionsOfType;

            for (int factionNum = 0; factionNum < largestTypeNum; factionNum++) {
                for (int factionTypeIndex = 0; factionTypeIndex < numFactionTypes; factionTypeIndex++) {
                    int maxFactionsOfType = maxFactionAmounts.Find(f => f.type == (Faction.FactionTypeEnum)factionTypeIndex).maxFactionsOfType;
                    if (factionNum < maxFactionsOfType) {
                        Faction.FactionTypeEnum factionType = (Faction.FactionTypeEnum)factionTypeIndex;
                        //section of the top results and pick from those - so that all the best sectors aren't always chosen
                        float percentile = .05f; //get the top 5%
                        List<(int desire, Sector sector)> topSectors = factionType.GetPercentileFactionSectorPreferencesList(percentile).FindAll(s => !pickedSectors.Contains(s.sector)); //gets all top sectors for a faction other than those already picked

                        if (topSectors.Count > 0) { //only creates a faction if there are valid sectors to choose from
                            (int desire, Sector sector) pickedSector = topSectors[Rng.Next(topSectors.Count)]; //get a random sector in the top list
                            pickedSectors.Add(pickedSector.sector);
                            SolarSystem preferredSolarSystemInSector = factionType.GetFactionSystemPreferencesList().Find(s => pickedSector.sector.Systems.Contains(s.solarSystem)).solarSystem;
                            if (preferredSolarSystemInSector != null) {
                                
                                Faction faction = factionType.GetFactionObjectFromType(preferredSolarSystemInSector);
                                factions.Add(faction);
                                faction.AddSector(pickedSector.sector);
                            }
                        }
                    }
                }
            }

            galaxy.Factions = factions;

            // Grow factions
            for (int sprawlRoll = 0; sprawlRoll < maxFactionSprawl; sprawlRoll++) {
                for (int factionIndex = 0; factionIndex < factions.Count; factionIndex++) {
                    Faction faction = factions[factionIndex];

                    if (sprawlRoll < faction.FactionType.GetFactionSprawlRatio()) { //if this faction is still allowed to grow
                        if (Rng.NextDouble() < faction.FactionType.GetFactionGrowthChance()) { //if this factions successfully rolls for a chance to grow
                            bool factionGrew = faction.GrowFaction(galaxy, false);
                        }
                    }
                }
            }
        }


        //organises planets into sectors for creating factions and controlling faction spread
        private List<Sector> GetSectors(List<SolarSystem> solarSystems) {
            float exclusionMultiplier = 5;
            float sectorSize = systemExclusionDistance.Value * exclusionMultiplier;
            Tile[,] sectorTileArray = GetTiles(width.Value, height.Value, sectorSize);
            List<Tile> sectorTileList = sectorTileArray.Cast<Tile>().ToList();
            List<Sector> sectors = new List<Sector>();

            foreach (Tile tile in sectorTileList) {
                sectors.Add(new Sector(tile));
            }

            foreach (Sector sector in sectors) {
                List<SolarSystem> systems = solarSystems.FindAll(s => sector.SectorTile.IsInsideTile(s.Coordinate));
                foreach (SolarSystem solarSystem in systems) {
                    solarSystem.Sector = sector;
                }
                sector.SetSolarSystems(systems);
            }

            return sectors;
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
            int min = (int)type;
            int max = Enum.GetValues(typeof(StarType)).Length+1;
            StarType starType = (StarType)Rng.Next(min, max);
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


        private static Tile[,] GetTiles(float areaWidth, float areaHeight, float tileSize) {
            //get num of tiles needed
            int numXTiles = (int)Math.Ceiling(areaWidth / tileSize);
            int numYTiles = (int)Math.Ceiling(areaHeight / tileSize);

            Tile[,] grid = new Tile[numXTiles, numYTiles];

            float y1 = 0;
            for (int y = 0; y < numYTiles; y++) {
                float x1 = 0;
                float y2 = y1 + tileSize; //set y2
                for (int x = 0; x < numXTiles; x++) {
                    float x2 = x1 + tileSize; //set x2
                    grid[x, y] = new Tile(x, y, x1, x2, y1, y2); //assign tile's edges
                    x1 = x2 < areaWidth ? x2 : areaWidth; //increment x1 ensuring it doesn't go out of bounds
                }

                y1 = y2 < areaHeight ? y2 : areaHeight; //increment y1 ensuring it doesn't go out of bounds
            }

            return grid;
        }


        private List<Vector2> PickDistributedPoints(int maxPoints, float areaWidth, float areaHeight, float exclusionDistance) {
            Tile[,] tileArray = GetTiles(areaWidth, areaHeight, exclusionDistance);
            List<Tile> tileList = tileArray.Cast<Tile>().ToList();
            List<Vector2> coordinatesList = new List<Vector2>();

            while (coordinatesList.Count < maxPoints && tileList.Count > 0) {
                Tile pickedTile = tileList[Rng.Next(tileList.Count)]; //pick random tile
                tileList.Remove(pickedTile); //removes the selected tile from the potential tiles
                List<(int x, int y)> surroundingTiles = pickedTile.GetSurroundingIndexes(); //get surrounding tiles
                foreach (var tile in surroundingTiles.Where(tile => Tile.InsideMapTiles(tile, tileArray.GetLength(0) - 1, tileArray.GetLength(1) - 1))) {
                    tileList.Remove(tileArray[tile.x, tile.y]);
                }

                coordinatesList.Add(pickedTile.GetRandomPoint());
            }

            return coordinatesList;
        }
    }
}