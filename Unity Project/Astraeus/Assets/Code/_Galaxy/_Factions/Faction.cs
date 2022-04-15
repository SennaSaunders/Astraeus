using System;
using System.Collections.Generic;
using System.Linq;
using Code._Galaxy._Factions.FactionTypes;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;
using Code._Galaxy._SolarSystem._CelestialObjects.Star;
using Code._Galaxy.GalaxyComponents;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;

namespace Code._Galaxy._Factions {
    public abstract class Faction {
        public enum FactionType {
            Agriculture,
            Commerce,
            Industrial,
            Military,
            Pirate,
            Technology
        }

        protected Faction(SolarSystem homeSystem, FactionType factionType) {
            HomeSystem = homeSystem;
            Systems = new List<SolarSystem>();
            AddSolarSystem(homeSystem);
            this.factionType = factionType;

            GroupName = factionType.GetFactionGroupNameList()[GalaxyGenerator.Rng.Next(this.factionType.GetFactionGroupNameList().Count)];
        }

        public FactionType factionType { get; }
        public SolarSystem HomeSystem { get; }
        public List<SolarSystem> Systems { get; }
        public List<Sector> Sectors { get; } = new List<Sector>();
        private string GroupName;

        public string GetFactionName() {
            return HomeSystem.SystemName + " " + GroupName;
        }


        public void AddSector(Sector sector) {
            if (!Sectors.Contains(sector)) {
                Sectors.Add(sector);
                sector.Factions.Add(this);
            }
        }

        public void AddSolarSystem(SolarSystem solarSystem) {
            Systems.Add(solarSystem);
            solarSystem.OwnerFaction = this;
            AddSector(solarSystem.Sector);
        }

        // public abstract List<(Type type, int desire)> GetCelestialBodyDesireValues();

        public static int GetCelestialBodyDesireValue(List<(Type type, Body.BodyTier tier, int desire)> desireValues, CelestialBody body) {
            if (body.GetType() == typeof(Planet)) {
                Planet planet = (Planet)body;
                foreach ((Type type, Body.BodyTier tier, int desire) typeDesire in desireValues) {
                    if (planet.PlanetGen.GetType() == typeDesire.type) {
                        return typeDesire.desire * (int)body.Tier;
                    }
                }
            }
            else if (body.GetType() == typeof(Star) && desireValues.Select(dv => dv.tier).Contains(body.Tier)) {
                foreach ((Type type, Body.BodyTier tier, int desire) typeDesire in desireValues) {
                    if (body.Tier == typeDesire.tier && body.GetType() == typeDesire.type) {
                        return typeDesire.desire * (int)body.Tier;
                    }
                }
            }
            else {
                foreach ((Type type, Body.BodyTier tier, int desire) typeDesire in desireValues) {
                    if (body.GetType() == typeDesire.type) {
                        return typeDesire.desire * (int)body.Tier;
                    }
                }
            }

            return (int)body.Tier;
        }

        protected static List<Body> GetCelestialBodiesInSystem(SolarSystem solarSystem) {
            return solarSystem.Bodies.FindAll(b => b.GetType().IsSubclassOf(typeof(CelestialBody)));
        }

        // attempts to grow the faction by 1 system
        public bool GrowFaction(Galaxy galaxy, bool encroachSectors) {
            if (GalaxyGenerator.Rng.NextDouble() < factionType.GetFactionGrowthChance()) {
                List<(int desire, SolarSystem newSystem)> potentialNewSystems = new List<(int desire, SolarSystem newSystem)>();

                foreach (Sector sector in Sectors) {
                    foreach (SolarSystem solarSystem in sector.Systems) {
                        bool inThisFaction = Systems.Contains(solarSystem);

                        //block colonisation into already colonised systems
                        List<List<SolarSystem>> otherFactionsSystems = galaxy.Factions.Select(f => f.Systems).ToList();
                        bool inOtherFaction = false;
                        foreach (List<SolarSystem> factionSystems in otherFactionsSystems) {
                            if (factionSystems.Contains(solarSystem)) {
                                inOtherFaction = true;
                            }
                        }

                        if (!inThisFaction && !inOtherFaction) {
                            potentialNewSystems.Add((factionType.GetPreCalcSystemDesire(solarSystem), solarSystem));
                        }
                    }
                }

                //get surrounding sectors

                List<Sector> sectorsToSearchFrom = new List<Sector>();
                sectorsToSearchFrom.AddRange(Sectors); //searches from inhabited sectors first

                List<(int desire, Sector sector)> potentialNewSectors = new List<(int desire, Sector sector)>();
                List<Sector> searchedSectors = new List<Sector>();

                int searchDistance = factionType.GetFactionSearchDistance();

                int maxX = galaxy.Sectors.Select(s => s.SectorTile).Max(t => t.XIndex);
                int maxY = galaxy.Sectors.Select(s => s.SectorTile).Max(t => t.YIndex);

                //search for surrounding sectors
                for (int currentSectorSearchDistance = 0; currentSectorSearchDistance < searchDistance; currentSectorSearchDistance++) {
                    List<Sector> newAdjacentSectors = new List<Sector>();

                    while (sectorsToSearchFrom.Count > 0) {
                        Sector searchSector = sectorsToSearchFrom[0];
                        searchedSectors.Add(searchSector);
                        sectorsToSearchFrom.Remove(searchSector);
                        List<(int x, int y)> adjacentSectorIndexes = searchSector.SectorTile.GetSurroundingIndexes().Where(index => Tile.InsideMapTiles(index, maxX, maxY)).ToList();

                        foreach ((int x, int y) sectorIndex in adjacentSectorIndexes) {
                            Sector potentialNewSector = galaxy.Sectors.Find(s => s.SectorTile.XIndex == sectorIndex.x && s.SectorTile.YIndex == sectorIndex.y);

                            bool alreadySearched = searchedSectors.Contains(potentialNewSector);
                            bool alreadySearching = sectorsToSearchFrom.Contains(potentialNewSector);
                            bool alreadyFound = newAdjacentSectors.Contains(potentialNewSector);

                            if (!alreadySearched && !alreadySearching && !alreadyFound) { //if sector hasn't been searched or isn't already in newAdjacentSectors
                                newAdjacentSectors.Add(potentialNewSector);
                            }
                        }
                    }

                    sectorsToSearchFrom.AddRange(newAdjacentSectors);
                    foreach (Sector newAdjacentSector in newAdjacentSectors) {
                        potentialNewSectors.Add((factionType.GetPreCalcSectorDesire(newAdjacentSector), newAdjacentSector));
                    }
                }

                //remove sectors if there are no systems
                potentialNewSectors = potentialNewSectors.FindAll(s => s.sector.Systems.Count > 0);

                //remove sectors if occupied and not allowed to encroach
                if (!encroachSectors) {
                    potentialNewSectors = potentialNewSectors.FindAll(s => s.sector.Factions.Count == 0);
                }

                //add systems in valid sectors to potentials
                foreach ((int desire, Sector sector) potentialNewSector in potentialNewSectors) {
                    foreach (SolarSystem solarSystem in potentialNewSector.sector.Systems) {
                        potentialNewSystems.Add((factionType.GetPreCalcSystemDesire(solarSystem), solarSystem));
                    }
                }

                // Need to modify the desire by the distance to the nearest system/sector

                List<(float closestDistance, int desire, SolarSystem solarSystem)> newSystems = new List<(float closestDistance, int desire, SolarSystem solarSystem)>();

                foreach ((int desire, SolarSystem newSystem) potentialNewSystem in potentialNewSystems) {
                    float distanceToClosestSystem = float.MaxValue;
                    foreach (SolarSystem solarSystem in Systems) {
                        Tile ownedSystemTile = solarSystem.Sector.SectorTile;
                        Tile newSystemTile = potentialNewSystem.newSystem.Sector.SectorTile;
                        float distance = ownedSystemTile.GetDistanceToTile(newSystemTile);
                        distanceToClosestSystem = distanceToClosestSystem > distance ? distance : distanceToClosestSystem;
                    }

                    newSystems.Add((distanceToClosestSystem, potentialNewSystem.desire, potentialNewSystem.newSystem));
                }

                List<(float closestDistance, int desire, Sector sector)> newSectors = new List<(float closestDistance, int desire, Sector sector)>();

                foreach ((int desire, Sector sector) potentialNewSector in potentialNewSectors) {
                    float distanceToClosestSector = float.MaxValue;
                    foreach (Sector sector in Sectors) {
                        Tile ownedSectorTile = sector.SectorTile;
                        Tile newSectorTile = potentialNewSector.sector.SectorTile;
                        float distance = ownedSectorTile.GetDistanceToTile(newSectorTile);
                        distanceToClosestSector = distanceToClosestSector > distance ? distance : distanceToClosestSector;
                    }

                    newSectors.Add((distanceToClosestSector, potentialNewSector.desire, potentialNewSector.sector));
                }

                List<(float weightedDesire, Sector sector)> weightedNewSectors = new List<(float weightedDesire, Sector sector)>();
                foreach ((float closestDistance, int desire, Sector sector) newSector in newSectors) {
                    float weightedDesire;
                    if (newSector.closestDistance == 0) {
                        int desireModifier = 2;
                        if (newSector.desire < 0) {
                            weightedDesire = (float)newSector.desire / desireModifier;
                        }
                        else {
                            weightedDesire = newSector.desire * desireModifier;
                        }
                    }
                    else {
                        weightedDesire = newSector.desire / newSector.closestDistance;
                    }

                    weightedNewSectors.Add((weightedDesire, newSector.sector));
                }

                List<(float weightedDesire, SolarSystem solarSystem)> weightedNewSystems = new List<(float weightedDesire, SolarSystem solarSystem)>();
                foreach ((float closestDistance, int desire, SolarSystem solarSystem) newSystem in newSystems) {
                    float weightedDesire;
                    if (newSystem.closestDistance == 0) {
                        int desireModifier = 2;
                        if (newSystem.desire < 0) {
                            weightedDesire = (float)newSystem.desire / desireModifier;
                        }
                        else {
                            weightedDesire = newSystem.desire * desireModifier;
                        }
                    }
                    else {
                        weightedDesire = newSystem.desire / newSystem.closestDistance;
                    }

                    weightedNewSystems.Add((weightedDesire, newSystem.solarSystem));
                }

                //choose the most desirable system or sector
                weightedNewSectors = weightedNewSectors.OrderByDescending(s => s.weightedDesire).ToList();
                weightedNewSystems = weightedNewSystems.OrderByDescending(s => s.weightedDesire).ToList();

                (float weightedDesire, Sector sector) bestSector = (0, null);
                if (weightedNewSectors.Count > 0) {
                    bestSector = weightedNewSectors[0];
                }

                (float weightedDesire, SolarSystem solarSystem) bestSystem = (0, null);
                if (weightedNewSystems.Count > 0) {
                    bestSystem = weightedNewSystems[0];
                }

                if (bestSector.sector != null) {
                    //check the best sector against system
                    if (bestSector.weightedDesire > bestSystem.weightedDesire) {
                        //choose the best system in the sector

                        int maxDesire = bestSector.sector.Systems.Max(s => factionType.GetPreCalcSystemDesire(s));
                        SolarSystem bestInSector = bestSector.sector.Systems.Find(s => factionType.GetPreCalcSystemDesire(s) == maxDesire);
                        AddSolarSystem(bestInSector);
                        return true;
                    }

                    AddSolarSystem(bestSystem.solarSystem);
                    return true;
                }

                if (bestSystem.solarSystem != null) {
                    AddSolarSystem(bestSystem.solarSystem);
                    return true;
                }
            }

            return false;
        }

        public abstract List<(Type weaponType, int spawnWeighting)> GetAllowedWeapons();
        public abstract List<(Type mainThrusterType, int spawnWeighting)> GetAllowedMainThrusters();
        public abstract List<(Type powerPlantType, int spawnWeighting)> GetAllowedPowerPlants();
        public abstract List<(Type shieldType, int spawnWeighting)> GetAllowedShields();
        public abstract List<(Type productType, float productionMult, float priceMult)> GetProductionMultipliers();
    }

    public static class FactionTypeExtension {
        private static List<(Faction.FactionType factionType, List<(int desire, SolarSystem system)> systemDesire)> _factionSystemDesires = new List<(Faction.FactionType, List<(int desire, SolarSystem system)>)>();
        private static List<(Faction.FactionType factionType, List<(int desire, Sector sector)> sectorDesire)> _factionSectorDesires = new List<(Faction.FactionType, List<(int desire, Sector sector)>)>();

        public static int GetFactionSearchDistance(this Faction.FactionType factionType) {
            if (factionType == Faction.FactionType.Agriculture) return 2;
            if (factionType == Faction.FactionType.Commerce) return 2;
            if (factionType == Faction.FactionType.Industrial) return 2;
            if (factionType == Faction.FactionType.Military) return 3;
            if (factionType == Faction.FactionType.Pirate) return 1;
            if (factionType == Faction.FactionType.Technology) return 4;
            else return 0;
        }

        public static void PreCalcDesireValues(List<Sector> sectors) {
            _factionSectorDesires = new List<(Faction.FactionType factionType, List<(int desire, Sector sector)> sectorDesire)>();
            _factionSystemDesires = new List<(Faction.FactionType factionType, List<(int desire, SolarSystem system)> systemDesire)>();
            int numFactionTypes = Enum.GetValues(typeof(Faction.FactionType)).Length;

            //could make multithreaded and calc a faction on each thread - need to ensure list stays in correct order - pre instantiate it?
            for (int i = 0; i < numFactionTypes; i++) { //for each faction type
                Faction.FactionType factionType = (Faction.FactionType)i;

                List<(int desire, SolarSystem system)> systemDesires = new List<(int desire, SolarSystem system)>();
                List<(int desire, Sector sector)> sectorDesires = new List<(int desire, Sector sector)>();

                for (int j = 0; j < sectors.Count; j++) { //for each sector in galaxy
                    Sector sector = sectors[j];
                    if (sector.Systems.Count > 0) { //stops adding empty sectors to the list
                        int sectorDesire = 0;
                        for (int k = 0; k < sectors[j].Systems.Count; k++) { //for each system in sector
                            SolarSystem system = sector.Systems[k];

                            int systemDesire = factionType.FactionSystemDesire(system);
                            sectorDesire += systemDesire;

                            systemDesires.Add((systemDesire, system));
                        }

                        sectorDesires.Add((sectorDesire, sector));
                    }
                }

                systemDesires = systemDesires.OrderByDescending(s => s.desire).ToList();
                sectorDesires = sectorDesires.OrderByDescending(s => s.desire).ToList();
                _factionSystemDesires.Add((factionType, systemDesires));
                _factionSectorDesires.Add((factionType, sectorDesires));
            }
        }

        public static Faction GetFactionObjectFromType(this Faction.FactionType factionType, SolarSystem homeWorld) {
            if (factionType == Faction.FactionType.Agriculture) return new AgricultureFaction(homeWorld);
            if (factionType == Faction.FactionType.Commerce) return new CommerceFaction(homeWorld);
            if (factionType == Faction.FactionType.Industrial) return new IndustrialFaction(homeWorld);
            if (factionType == Faction.FactionType.Military) return new MilitaryFaction(homeWorld);
            if (factionType == Faction.FactionType.Pirate) return new PirateFaction(homeWorld);
            if (factionType == Faction.FactionType.Technology) return new TechnologyFaction(homeWorld);
            else return null;
        }

        public static List<(int desire, Sector sector)> GetFactionSectorPreferencesList(this Faction.FactionType factionType) {
            return _factionSectorDesires.Find(f => f.factionType == factionType).sectorDesire;
        }

        public static List<(int desire, SolarSystem solarSystem)> GetFactionSystemPreferencesList(this Faction.FactionType factionType) {
            return _factionSystemDesires.Find(f => f.factionType == factionType).systemDesire;
        }

        public static List<(int desire, Sector sector)> GetPercentileFactionSectorPreferencesList(this Faction.FactionType factionType, float percentage) {
            List<(int desire, Sector sector)> allFactionPreferences = factionType.GetFactionSectorPreferencesList();
            return allFactionPreferences.GetRange(0, (int)Math.Floor(allFactionPreferences.Count * percentage));
        }

        public static List<(int desire, SolarSystem system)> GetPercentileFactionSystemPreferencesList(this Faction.FactionType factionType, float percentage) {
            List<(int desire, SolarSystem solarSystem)> allFactionPreferences = factionType.GetFactionSystemPreferencesList();
            return allFactionPreferences.GetRange(0, (int)Math.Floor(allFactionPreferences.Count * percentage));
        }

        public static List<string> GetFactionGroupNameList(this Faction.FactionType factionType) {
            if (factionType == Faction.FactionType.Agriculture)
                return new List<string> {
                    "Growers",
                    "Farmers",
                    "Producers",
                    "Harvesters",
                    "Tillers",
                    "Cultivators"
                };
            if (factionType == Faction.FactionType.Commerce)
                return new List<string> {
                    "Trading Company",
                    "Traders",
                    "Merchants",
                    "Guild",
                    "Corporation",
                    "Enterprise",
                    "Firm"
                };
            if (factionType == Faction.FactionType.Industrial)
                return new List<string> {
                    "Industrialists",
                    "Builders",
                    "Artificers",
                    "Scrappers",
                    "Oligarchy",
                    "Union",
                    "Mechanics",
                    "Technicians"
                };
            if (factionType == Faction.FactionType.Military)
                return new List<string> {
                    "Empire",
                    "Hegemony",
                    "Brigade",
                    "Trust",
                    "Alliance",
                    "Confederacy",
                    "Force",
                    "Axis",
                    "Federation",
                    "League"
                };
            if (factionType == Faction.FactionType.Pirate)
                return new List<string> {
                    "Horde",
                    "Gang",
                    "Cartel",
                    "Crew",
                    "Schism",
                    "Clan",
                    "Syndicate",
                    "Mafia",
                    "Cabal",
                    "Mob"
                };
            if (factionType == Faction.FactionType.Technology)
                return new List<string> {
                    "Collective",
                    "Association",
                    "Cooperative",
                    "Institute",
                    "Foundation",
                    "Establishment",
                    "Researchers",
                    "Empiricists"
                };
            else return new List<string> { "Faction" };
        }

        public static int GetFactionRatio(this Faction.FactionType factionType) {
            if (factionType == Faction.FactionType.Agriculture) return 8;
            if (factionType == Faction.FactionType.Commerce) return 7;
            if (factionType == Faction.FactionType.Industrial) return 5;
            if (factionType == Faction.FactionType.Military) return 4;
            if (factionType == Faction.FactionType.Pirate) return 8;
            if (factionType == Faction.FactionType.Technology) return 2;
            else return 0; //if generic unfocused factions are desired this can be increased to allow for them
        }

        public static int GetFactionSprawlRatio(this Faction.FactionType factionType) {
            if (factionType == Faction.FactionType.Agriculture) return 5;
            if (factionType == Faction.FactionType.Commerce) return 5;
            if (factionType == Faction.FactionType.Industrial) return 5;
            if (factionType == Faction.FactionType.Military) return 10;
            if (factionType == Faction.FactionType.Pirate) return 2;
            if (factionType == Faction.FactionType.Technology) return 3;
            else return 0; //if generic unfocused factions are desired this can be increased to allow for them
        }

        public static float GetFactionGrowthChance(this Faction.FactionType factionType) {
            if (factionType == Faction.FactionType.Agriculture) return 0.5f;
            if (factionType == Faction.FactionType.Commerce) return 0.6f;
            if (factionType == Faction.FactionType.Industrial) return 0.5f;
            if (factionType == Faction.FactionType.Military) return 0.75f;
            if (factionType == Faction.FactionType.Pirate) return 0.3f;
            if (factionType == Faction.FactionType.Technology) return 0.4f;
            else return 0;
        }

        public static int GetPreCalcSectorDesire(this Faction.FactionType factionType, Sector sector) {
            (int desire, Sector sector) sectorDesire = _factionSectorDesires.Find(f => f.factionType == factionType).sectorDesire.Find(s => s.sector == sector);

            return sectorDesire.desire;
        }

        public static int GetPreCalcSystemDesire(this Faction.FactionType factionType, SolarSystem system) {
            (int desire, SolarSystem system) systemDesire = _factionSystemDesires.Find(f => f.factionType == factionType).systemDesire.Find(s => s.system == system);
            return systemDesire.desire;
        }

        //this is private as it is only used in precalculating the desire values for systems/sectors
        public static int FactionSystemDesire(this Faction.FactionType factionType, SolarSystem solarSystem) {
            int desireSum = 0;
            foreach (Body body in solarSystem.Bodies) {
                desireSum += factionType.CelestialBodyDesire((CelestialBody)body);
            }

            return desireSum; //if generic factions are desired this can be altered to allow for them
        }

        public static int CelestialBodyDesire(this Faction.FactionType factionType, CelestialBody celestialBody) {
            if (factionType == Faction.FactionType.Agriculture) return AgricultureFaction.GetAgricultureFactionCelestialBodyDesire(celestialBody);
            if (factionType == Faction.FactionType.Commerce) return CommerceFaction.GetCommerceFactionCelestialBodyDesire(celestialBody);
            if (factionType == Faction.FactionType.Industrial) return IndustrialFaction.GetIndustrialCelestialBodyDesire(celestialBody);
            if (factionType == Faction.FactionType.Military) return MilitaryFaction.GetMilitaryFactionCelestialBodyDesire(celestialBody);
            if (factionType == Faction.FactionType.Pirate) return PirateFaction.GetPirateFactionCelestialBodyDesire(celestialBody);
            if (factionType == Faction.FactionType.Technology) return TechnologyFaction.GetTechnologyFactionCelestialBodyDesire(celestialBody);
            else return 0; //if generic factions are desired this can be altered to allow for them
        }
    }
}