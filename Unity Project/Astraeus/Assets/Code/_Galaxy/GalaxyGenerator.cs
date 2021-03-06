using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;
using Code._Galaxy._SolarSystem._CelestialObjects.Star;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._Galaxy.GalaxyComponents;
using Code.TextureGen;
using UnityEngine;
using Random = System.Random;
using StarType = Code._Galaxy._SolarSystem._CelestialObjects.Star.Star.StarType;
using BodyTier = Code._Galaxy._SolarSystem._CelestialObjects.Body.BodyTier;

namespace Code._Galaxy {
    public class GalaxyGenerator : MonoBehaviour {
        public static Random Rng = new Random();
        public GalaxyGeneratorInput seed = new GalaxyGeneratorInput(1337);
        public GalaxyGeneratorInput maxSystems = new GalaxyGeneratorInput(200, 2000, 200);
        public GalaxyGeneratorInput minBodiesPerSystem = new GalaxyGeneratorInput(1, 5, 1);
        public GalaxyGeneratorInput maxBodiesPerSystem = new GalaxyGeneratorInput(5, 20, 5);
        public GalaxyGeneratorInput width = new GalaxyGeneratorInput(300, 2000, 300);
        public GalaxyGeneratorInput height = new GalaxyGeneratorInput(300, 2000, 300);
        public GalaxyGeneratorInput systemExclusionDistance = new GalaxyGeneratorInput(5, 20, 5);
        private List<string> potentialSystemNames;

        public GalaxyStats stats;


        private bool IsRare(int rarePercentageChance) {
            int maxRoll = 100;
            int rareRoll = maxRoll - rarePercentageChance;
            return Rng.Next(maxRoll) > rareRoll;
        }

        //has to be called before galaxy generation as getting Application.dataPath has to be on the main thread
        public void SetPotentialSystemNames() {
            TextAsset namesTxt = (TextAsset)Resources.Load("SystemNames/System Names");
            var lines = namesTxt.text.Split('\n');
            potentialSystemNames = new List<string>();
            foreach (string line in lines) {
                if (!line.StartsWith("*")) {
                    potentialSystemNames.Add(line.TrimEnd('\n', '\r'));
                }
            }
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
                if (IsRare(blackHoleChance)) {
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
            GenSpaceStations(galaxy.Factions);

            for (int i = 0; i < galaxy.Systems.Count; i++) { //sets the position of all planets and space stations
                galaxy.Systems[i].Bodies = SetupRotations(SetupPositions(galaxy.Systems[i].Bodies));
            }

            return galaxy;
        }

        private void GenSpaceStations(List<Faction> factions) {
            //for each faction for each solar system get the most desirable planet and add a space station to it
            foreach (Faction faction in factions) {
                foreach (SolarSystem solarSystem in faction.Systems) {
                    Body bestBody = null;
                    int highestDesire = Int32.MinValue;
                    foreach (Body body in solarSystem.Bodies) {
                        int currentDesire = faction.factionType.CelestialBodyDesire((CelestialBody)body);
                        if (currentDesire > highestDesire) {
                            highestDesire = currentDesire;
                            bestBody = body;
                        }
                    }

                    SpaceStation spaceStation = new SpaceStation(bestBody, solarSystem);
                    //need to decided how to choose whether there is an outfitting service and a shipyard
                    //only refuel and repair should be everywhere
                    //for the moment just instantiate the outfitter with the faction specific parts

                    spaceStation.StationServices = new List<StationService>() {
                        new RefuelService(),
                        new RepairService(),
                        new ShipyardService(),
                        new OutfittingService(faction),
                        new TradeService(faction, solarSystem),
                        new MissionService(faction, spaceStation)
                    };
                    solarSystem.Bodies.Add(spaceStation);
                }
            }
        }

        private void GenGalaxyFactions(Galaxy galaxy) {
            List<Sector> sectors = galaxy.Sectors;
            List<Sector> sectorsWithBodies = sectors.FindAll(s => s.Systems.Count > 0);

            //find the ratio of factions to sectors
            int numFactionTypes = Enum.GetValues(typeof(Faction.FactionType)).Length;
            int highestFactionSprawl = 0;
            int totalFactionSprawl = 0;

            for (int i = 0; i < numFactionTypes; i++) {
                Faction.FactionType factionType = (Faction.FactionType)i;
                int factionSprawl = factionType.GetFactionSprawlRatio();
                highestFactionSprawl = factionSprawl > highestFactionSprawl ? factionSprawl : highestFactionSprawl;
                totalFactionSprawl += factionType.GetFactionRatio() * factionSprawl;
            }

            //then set the max number of factions for each
            float maxPopulationDensity = .9f;
            float factionRatioMultiplier = (sectorsWithBodies.Count * maxPopulationDensity) / totalFactionSprawl;

            List<(Faction.FactionType type, int maxFactionsOfType)> maxFactionAmounts = new List<(Faction.FactionType type, int maxFactionsOfType)>();
            for (int i = 0; i < numFactionTypes; i++) {
                Faction.FactionType factionType = (Faction.FactionType)i;
                maxFactionAmounts.Add((factionType, (int)Math.Ceiling(factionRatioMultiplier * factionType.GetFactionRatio())));
            }

            FactionTypeExtension.PreCalcDesireValues(sectors); //Pre calculate the preferences for each sector/system for each faction

            //choose a home world for each faction
            //get the top sector in the list and add it to a chosen sector list
            //iterate through faction types first before coming back to

            List<Faction> factions = new List<Faction>();
            List<Sector> pickedSectors = new List<Sector>();
            int largestTypeNum = maxFactionAmounts.OrderByDescending(a => a.maxFactionsOfType).ToList()[0].maxFactionsOfType;

            for (int factionNum = 0; factionNum < largestTypeNum; factionNum++) {
                for (int factionTypeIndex = 0; factionTypeIndex < numFactionTypes; factionTypeIndex++) {
                    int maxFactionsOfType = maxFactionAmounts.Find(f => f.type == (Faction.FactionType)factionTypeIndex).maxFactionsOfType;
                    if (factionNum < maxFactionsOfType) {
                        Faction.FactionType factionType = (Faction.FactionType)factionTypeIndex;
                        //section of the top results and pick from those - so that all the best sectors aren't always chosen
                        float percentile = .1f; //get the top 10%
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
            for (int sprawlNum = 0; sprawlNum < highestFactionSprawl; sprawlNum++) {
                for (int factionIndex = 0; factionIndex < factions.Count; factionIndex++) {
                    Faction faction = factions[factionIndex];

                    if (sprawlNum < faction.factionType.GetFactionSprawlRatio()) { //if this faction is still allowed to grow
                        bool factionGrew = faction.GrowFaction(galaxy, false);
                        if (!factionGrew) {
                            Debug.Log("Faction failed to expand");
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
            int max = Enum.GetValues(typeof(StarType)).Length + 1;
            StarType starType = (StarType)Rng.Next(min, max);
            return starType;
        }

        private Planet GenPlanet(Body primary, BodyTier tier) {
            int notRockyChance = 20;
            int earthChance = 30;

            PlanetGen planetGen;
            int size = tier.TextureSize();
            int planetTextureSeed = Rng.Next();
            if (IsRare(notRockyChance)) {
                if (IsRare(earthChance)) { //earth-like
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
                if (currentPrimary.Primary == null && IsRare(extraStarChance)) {
                    newBody = currentPrimary.GetType() == typeof(Star) ? new Star(currentPrimary, PickRandomStarType(((Star)currentPrimary).StarClass)) : new Star(currentPrimary, PickRandomStarType());
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

            string systemName = potentialSystemNames[Rng.Next(potentialSystemNames.Count)];
            potentialSystemNames.Remove(systemName);

            return new SolarSystem(systemCoordinate, primary, celestialBodies, systemName);
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
                    Vector2 bodyCoordinate;
                    if (currentBody.GetType() == typeof(SpaceStation)) {
                        bodyCoordinate = new Vector2(currentBody.Primary.Tier.SystemScale(), 0);
                    }
                    else {
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
                            bodyCoordinate = new Vector2(distance, 0);
                        }
                        else {
                            bodyCoordinate = new Vector2(0, 0);
                        }
                    }

                    currentBody.Coordinate = bodyCoordinate;
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
            foreach (Body body in bodies) {
                body.RotationStart = (float)Rng.NextDouble();
                body.RotationCurrent = body.RotationStart;
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