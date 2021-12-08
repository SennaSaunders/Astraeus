using System;
using System.Collections.Generic;
using System.Linq;
using Code._CelestialObjects;
using Code._Factions.FactionTypes;
using Code._Galaxy;
using Code._Galaxy._SolarSystem;

namespace Code._Factions {
    public abstract class Faction {
        protected Faction(SolarSystem homeSystem, FactionTypeEnum factionType, List<SolarSystem> systems) {
            HomeSystem = homeSystem;
            FactionType = factionType;
            Systems = systems;
            GroupName = factionType.GetFactionGroupNameList()[GalaxyGenerator.Rng.Next(FactionType.GetFactionGroupNameList().Count)];
        }

        //will affect the type of goods mass produced in this factions systems
        //planet types should also be modifiers to this
        //such as earth likes producing more food
        public enum FactionTypeEnum {
            Agriculture,
            Commerce,
            Industrial,
            Military,
            Pirate,
            Technology
        }

        public SolarSystem HomeSystem { get; }
        public FactionTypeEnum FactionType { get; }
        public List<SolarSystem> Systems { get; }
        public string GroupName { get; }
        
        protected static List<Body> GetCelestialBodiesInSystem(SolarSystem solarSystem) {
            return solarSystem.Bodies.FindAll(b => b.GetType().IsSubclassOf(typeof(CelestialBody)));
        }
    }

    public static class FactionTypeExtension {
        private static List<(Faction.FactionTypeEnum factionType, List<(int desire, SolarSystem system)> systemDesire)> factionSystemDesires = new List<(Faction.FactionTypeEnum, List<(int desire, SolarSystem system)>)>();
        private static List<(Faction.FactionTypeEnum factionType, List<(int desire, Sector sector)> sectorDesire)> factionSectorDesires = new List<(Faction.FactionTypeEnum, List<(int desire, Sector sector)>)>();

        public static void PreCalcDesireValues(List<Sector> sectors) {
            factionSectorDesires = new List<(Faction.FactionTypeEnum factionType, List<(int desire, Sector sector)> sectorDesire)>();
            factionSystemDesires = new List<(Faction.FactionTypeEnum factionType, List<(int desire, SolarSystem system)> systemDesire)>();
            int numFactionTypes = Enum.GetValues(typeof(Faction.FactionTypeEnum)).Length;

            //could make multithreaded and calc a faction on each thread - need to ensure list stays in correct order - pre instantiate it?
            for (int i = 0; i < numFactionTypes; i++) { //for each faction type
                Faction.FactionTypeEnum factionType = (Faction.FactionTypeEnum)i;

                List<(int desire, SolarSystem system)> systemDesires = new List<(int desire, SolarSystem system)>();
                List<(int desire, Sector sector)> sectorDesires = new List<(int desire, Sector sector)>();

                for (int j = 0; j < sectors.Count; j++) { //for each sector in galaxy
                    Sector sector = sectors[j];

                    int sectorDesire = 0;
                    for (int k = 0; k < sectors[j].Systems.Count; k++) { //for each system in sector
                        SolarSystem system = sector.Systems[k];

                        int systemDesire = factionType.FactionSystemDesire(system);
                        sectorDesire += systemDesire;

                        systemDesires.Add((systemDesire, system));
                    }

                    sectorDesires.Add((sectorDesire, sector));
                }

                systemDesires = systemDesires.OrderByDescending(s => s.desire).ToList();
                sectorDesires = sectorDesires.OrderByDescending(s => s.desire).ToList();
                factionSystemDesires.Add((factionType, systemDesires));
                factionSectorDesires.Add((factionType, sectorDesires));
            }
        }

        public static List<string> GetFactionGroupNameList(this Faction.FactionTypeEnum factionType) {
            if (factionType == Faction.FactionTypeEnum.Agriculture)
                return new List<string> {
                    "Growers",
                    "Farmers",
                    "Producers",
                    "Harvesters",
                    "Tillers",
                    "Cultivators"
                };
            if (factionType == Faction.FactionTypeEnum.Commerce)
                return new List<string> {
                    "Trading Company",
                    "Traders",
                    "Merchants",
                    "Guild",
                    "Corporation",
                    "Enterprise",
                    "Firm"
                };
            if (factionType == Faction.FactionTypeEnum.Industrial)
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
            if (factionType == Faction.FactionTypeEnum.Military)
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
            if (factionType == Faction.FactionTypeEnum.Pirate)
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
            if (factionType == Faction.FactionTypeEnum.Technology)
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

        public static int FactionRatio(this Faction.FactionTypeEnum factionType) {
            if (factionType == Faction.FactionTypeEnum.Agriculture) return 8;
            if (factionType == Faction.FactionTypeEnum.Commerce) return 7;
            if (factionType == Faction.FactionTypeEnum.Industrial) return 5;
            if (factionType == Faction.FactionTypeEnum.Military) return 4;
            if (factionType == Faction.FactionTypeEnum.Pirate) return 8;
            if (factionType == Faction.FactionTypeEnum.Technology) return 2;
            else return 0; //if generic unfocused factions are desired this can be increased to allow for them
        }

        public static int FactionSprawlRatio(this Faction.FactionTypeEnum factionType) {
            if (factionType == Faction.FactionTypeEnum.Agriculture) return 4;
            if (factionType == Faction.FactionTypeEnum.Commerce) return 10;
            if (factionType == Faction.FactionTypeEnum.Industrial) return 7;
            if (factionType == Faction.FactionTypeEnum.Military) return 20;
            if (factionType == Faction.FactionTypeEnum.Pirate) return 2;
            if (factionType == Faction.FactionTypeEnum.Technology) return 3;
            else return 0; //if generic unfocused factions are desired this can be increased to allow for them
        }

        public static int GetFactionSectorDesire(this Faction.FactionTypeEnum factionType, Sector sector) {
            (int desire, Sector sector) sectorDesire = factionSectorDesires.Find(f => f.factionType == factionType).sectorDesire.Find(s => s.sector == sector);

            return sectorDesire.desire;
        }

        public static int GetFactionSystemDesire(this Faction.FactionTypeEnum factionType, SolarSystem system) {
            (int desire, SolarSystem system) systemDesire = factionSystemDesires.Find(f => f.factionType == factionType).systemDesire.Find(s => s.system == system);
            return systemDesire.desire;
        }

        private static int FactionSystemDesire(this Faction.FactionTypeEnum factionType, SolarSystem solarSystem) {
            if (factionType == Faction.FactionTypeEnum.Agriculture) return AgricultureFaction.GetAgricultureFactionSystemDesire(solarSystem);
            if (factionType == Faction.FactionTypeEnum.Commerce) return CommerceFaction.GetCommerceFactionSystemDesire(solarSystem);
            if (factionType == Faction.FactionTypeEnum.Industrial) return IndustrialFaction.GetIndustrialSystemDesire(solarSystem);
            if (factionType == Faction.FactionTypeEnum.Military) return MilitaryFaction.GetMilitaryFactionSystemDesire(solarSystem);
            if (factionType == Faction.FactionTypeEnum.Pirate) return PirateFaction.GetPirateFactionSystemDesire(solarSystem);
            if (factionType == Faction.FactionTypeEnum.Technology) return TechnologyFaction.GetTechnologyFactionSystemDesire(solarSystem);
            else return 0; //if generic factions are desired this can be altered to allow for them
        }
    }
}