using System;
using System.Collections.Generic;
using System.Linq;
using Code._CelestialObjects;
using Code._CelestialObjects.BlackHole;
using Code._CelestialObjects.Planet;
using Code._CelestialObjects.Star;
using Code._Galaxy;
using Code._Galaxy._SolarSystem;
using Code.TextureGen;

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
            if (factionType == Faction.FactionTypeEnum.Agriculture) return GetAgricultureFactionSystemDesire(solarSystem);
            if (factionType == Faction.FactionTypeEnum.Commerce) return GetCommerceFactionSystemDesire(solarSystem);
            if (factionType == Faction.FactionTypeEnum.Industrial) return GetIndustrialSystemDesire(solarSystem);
            if (factionType == Faction.FactionTypeEnum.Military) return GetMilitaryFactionSystemDesire(solarSystem);
            if (factionType == Faction.FactionTypeEnum.Pirate) return GetPirateFactionSystemDesire(solarSystem);
            if (factionType == Faction.FactionTypeEnum.Technology) return GetTechnologyFactionSystemDesire(solarSystem);
            else return 0; //if generic factions are desired this can be altered to allow for them
        }

        private static List<Body> GetCelestialBodiesInSystem(SolarSystem solarSystem) {
            return solarSystem.Bodies.FindAll(b => b.GetType().IsSubclassOf(typeof(CelestialBody)));
        }

        private static int GetAgricultureFactionSystemDesire(SolarSystem system) {
            //assign high values to earth-likes & water worlds - these are the most prized
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(Planet)) {
                    Planet planet = (Planet)body;
                    if (planet.PlanetGen.GetType() == typeof(EarthWorldGen) || planet.PlanetGen.GetType() == typeof(WaterWorldGen)) {
                        desireValue += AgricultureFaction.OrganicWorldDesire * (int)planet.Tier;
                    }
                    else {
                        desireValue += (int)body.Tier;
                    }
                }
                else if (body.GetType() == typeof(BlackHole)) {
                    desireValue += AgricultureFaction.BlackHoleDesire * (int)body.Tier;
                }
            }

            return desireValue;
        }

        private static int GetCommerceFactionSystemDesire(SolarSystem system) {
            //assign higher values to larger planets - so the more large planets the better
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(Planet)) {
                    desireValue += (int)body.Tier * (int)body.Tier;
                }
                else if (body.GetType() == typeof(BlackHole)) {
                    desireValue += CommerceFaction.BlackHoleDesire * (int)body.Tier;
                }
            }

            return desireValue;
        }

        private static int GetIndustrialSystemDesire(SolarSystem system) {
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(Planet)) {
                    Planet planet = (Planet)body;
                    if (planet.PlanetGen.GetType() == typeof(WaterWorldGen)) {
                        desireValue += IndustrialFaction.WaterWorldDesire * (int)planet.Tier;
                    }
                    else if (planet.PlanetGen.GetType() == typeof(RockyWorldGen)) {
                        desireValue += IndustrialFaction.RockyWorldDesire * (int)planet.Tier;
                    }
                    else {
                        desireValue += (int)body.Tier * (int)body.Tier;
                    }
                }
                else if (body.GetType() == typeof(BlackHole)) {
                    desireValue += IndustrialFaction.BlackHoleDesire * (int)body.Tier;
                }
            }

            return desireValue;
        }

        private static int GetMilitaryFactionSystemDesire(SolarSystem system) {
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(Planet)) { //ignore stars/black holes
                    Planet planet = (Planet)body;
                    if (planet.PlanetGen.GetType() == typeof(EarthWorldGen)) {
                        desireValue += MilitaryFaction.EarthWorldDesire * (int)planet.Tier;
                    }
                    else {
                        desireValue += (int)body.Tier;
                    }
                }
            }

            return desireValue;
        }

        private static int GetPirateFactionSystemDesire(SolarSystem system) {
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(Planet)) {
                    Planet planet = (Planet)body;
                    if (planet.PlanetGen.GetType() == typeof(EarthWorldGen)) {
                        desireValue += PirateFaction.EarthlikeDesire * (int)planet.Tier;
                    }
                    else {
                        desireValue += (int)planet.Tier;
                    }
                }
            }

            return desireValue;
        }

        private static int GetTechnologyFactionSystemDesire(SolarSystem system) {
            int desireValue = 0;
            foreach (Body body in GetCelestialBodiesInSystem(system)) {
                if (body.GetType() == typeof(BlackHole)) {
                    desireValue += TechnologyFaction.BlackHoleValue * (int)body.Tier;
                }
                else if (body.GetType() == typeof(Star) && body.Tier == Body.BodyTier.T9) {
                    desireValue += TechnologyFaction.StarT9Value * (int)body.Tier;
                }
                else if (body.GetType() == typeof(Star) && body.Tier == Body.BodyTier.T8) {
                    desireValue += TechnologyFaction.StarT8Value * (int)body.Tier;
                }
                else if (body.GetType() == typeof(Star)) {
                    desireValue += TechnologyFaction.StarT7Value * (int)body.Tier;
                }
                else if (body.GetType() == typeof(Planet)) {
                    Planet planet = (Planet)body;
                    if (planet.PlanetGen.GetType() == typeof(EarthWorldGen) || planet.PlanetGen.GetType() == typeof(WaterWorldGen)) {
                        desireValue += TechnologyFaction.OrganicWorldValue * (int)body.Tier;
                    }
                    else {
                        desireValue += (int)body.Tier;
                    }
                }
            }

            return desireValue;
        }
    }

    public class AgricultureFaction : Faction {
        public AgricultureFaction(SolarSystem homeSystem, List<SolarSystem> systems) : base(homeSystem, FactionTypeEnum.Agriculture, systems) {
        }

        public static int OrganicWorldDesire = 50;
        public static int BlackHoleDesire = -100;
    }

    public class CommerceFaction : Faction {
        public CommerceFaction(SolarSystem homeSystem, List<SolarSystem> systems) : base(homeSystem, FactionTypeEnum.Commerce, systems) {
        }

        public static int BlackHoleDesire = -100;
    }

    public class IndustrialFaction : Faction {
        public IndustrialFaction(SolarSystem homeSystem, List<SolarSystem> systems) : base(homeSystem, FactionTypeEnum.Industrial, systems) {
        }

        public static int WaterWorldDesire = 15;
        public static int RockyWorldDesire = 30;
        public static int BlackHoleDesire = -50;
    }

    public class MilitaryFaction : Faction {
        public MilitaryFaction(SolarSystem homeSystem, List<SolarSystem> systems) : base(homeSystem, FactionTypeEnum.Military, systems) {
        }

        public static int EarthWorldDesire = 20;
    }

    public class PirateFaction : Faction {
        public PirateFaction(SolarSystem homeSystem, List<SolarSystem> systems) : base(homeSystem, FactionTypeEnum.Pirate, systems) {
        }

        public static int EarthlikeDesire = -10;
    }

    public class TechnologyFaction : Faction {
        public TechnologyFaction(SolarSystem homeSystem, List<SolarSystem> systems) : base(homeSystem, FactionTypeEnum.Technology, systems) {
        }

        public static int BlackHoleValue = 30;
        public static int StarT9Value = 20;
        public static int StarT8Value = 15;
        public static int StarT7Value = 10;
        public static int OrganicWorldValue = 30;
    }
}