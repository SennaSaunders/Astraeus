using System.Collections;
using System.Collections.Generic;
using Code._CelestialObjects;
using Code._CelestialObjects.BlackHole;
using Code._CelestialObjects.Planet;
using Code._CelestialObjects.Star;
using Code._Factions;
using Code._Galaxy;
using Code._Galaxy._SolarSystem;
using Code.TextureGen;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
    public class FactionTest {
        private static List<Sector> _sectors;

        [SetUp]
        public static void SetupTest() {
            SetupSectors();
            FactionTypeExtension.PreCalcDesireValues(_sectors);
        }
        private static void SetupSectors() {
            List<Sector> sectors = new List<Sector>();
            Vector2 v = new Vector2();
            Star t7Star = new Star(null, v, Star.StarType.A);
            Star t8Star = new Star(null, v, Star.StarType.B);
            Star t9Star = new Star(null, v, Star.StarType.O);
            BlackHole blackHole = new BlackHole(null, v);
            Planet t1RockyPlanet = new Planet(null, Body.BodyTier.T1,new RockyWorldGen(0, 0));
            Planet t2RockyPlanet = new Planet(null, Body.BodyTier.T2,new RockyWorldGen(0, 0));
            Planet t3RockyPlanet = new Planet(null, Body.BodyTier.T3,new RockyWorldGen(0, 0));
            
            Planet t4EarthPlanet = new Planet(null, Body.BodyTier.T4,new EarthWorldGen(0, 0));
            Planet t5EarthPlanet = new Planet(null, Body.BodyTier.T5,new EarthWorldGen(0, 0));
            Planet t6EarthPlanet = new Planet(null, Body.BodyTier.T6,new EarthWorldGen(0, 0));
            
            Planet t1WaterPlanet = new Planet(null, Body.BodyTier.T1,new WaterWorldGen(0, 0));
            Planet t3WaterPlanet = new Planet(null, Body.BodyTier.T3,new WaterWorldGen(0, 0));
            Planet t5WaterPlanet = new Planet(null, Body.BodyTier.T5,new WaterWorldGen(0, 0));
            
            
            List<Body> bodies0 = new List<Body>();//t7 star - no planets
            bodies0.Add(t7Star);
            
            List<Body> bodies1 = new List<Body>();//t7 star - rocky planets
            bodies1.Add(t7Star);
            bodies1.Add(t1RockyPlanet);
            bodies1.Add(t2RockyPlanet);
            bodies1.Add(t3RockyPlanet);
            
            List<Body> bodies2 = new List<Body>();//t7 star - earthlike
            bodies2.Add(t7Star);
            bodies2.Add(t4EarthPlanet);
            bodies2.Add(t5EarthPlanet);
            bodies2.Add(t6EarthPlanet);
            
            List<Body> bodies3 = new List<Body>();//t8 star - rocky
            bodies3.Add(t8Star);
            bodies3.Add(t1RockyPlanet);
            bodies3.Add(t2RockyPlanet);
            bodies3.Add(t3RockyPlanet);
            
            List<Body> bodies4 = new List<Body>();//t8 star - earthlike + water worlds
            bodies4.Add(t8Star);
            bodies4.Add(t4EarthPlanet);
            bodies4.Add(t5WaterPlanet);
            
            List<Body> bodies5 = new List<Body>();//t9 star - rocky
            bodies5.Add(t9Star);
            bodies5.Add(t1RockyPlanet);
            bodies5.Add(t2RockyPlanet);
            bodies5.Add(t3RockyPlanet);
            
            List<Body> bodies6 = new List<Body>();//t9 star - earthlike
            bodies6.Add(t9Star);
            bodies6.Add(t4EarthPlanet);
            
            List<Body> bodies7 = new List<Body>();//t9 star - earthlike + waterworlds
            bodies7.Add(t9Star);
            bodies7.Add(t4EarthPlanet);
            bodies7.Add(t1WaterPlanet);
            bodies7.Add(t3WaterPlanet);
            
            List<Body> bodies8 = new List<Body>();//t9 star all worlds
            bodies8.Add(t9Star);
            bodies8.Add(t1RockyPlanet);
            bodies8.Add(t4EarthPlanet);
            bodies8.Add(t5WaterPlanet);
            
            List<Body> bodies9 = new List<Body>();//black hole - rocky
            bodies9.Add(blackHole);
            bodies9.Add(t1RockyPlanet);
            bodies9.Add(t2RockyPlanet);
            bodies9.Add(t3RockyPlanet);
            
            List<Body> bodies10 = new List<Body>();//black hole - earthlike
            bodies10.Add(blackHole);
            bodies10.Add(t4EarthPlanet);
            
            List<Body> bodies11 = new List<Body>();//black hole planet mix
            bodies11.Add(blackHole);
            bodies11.Add(t3RockyPlanet);
            bodies11.Add(t3WaterPlanet);
            bodies11.Add(t5EarthPlanet);
            
            List<Body> bodies12 = new List<Body>();////black hole, t9, t8, t7 star
            bodies12.Add(blackHole);
            bodies12.Add(t9Star);
            bodies12.Add(t8Star);
            bodies12.Add(t7Star);
            
            
            List<Body> bodies13 = new List<Body>();//black hole + t9, t8, t7 star rocky eathlike waterworld
            bodies13.Add(blackHole);
            bodies13.Add(t7Star);
            bodies13.Add(t8Star);
            bodies13.Add(t9Star);
            bodies13.Add(t3RockyPlanet);
            bodies13.Add(t4EarthPlanet);
            bodies13.Add(t5WaterPlanet);
            
            List<Body> bodies14 = new List<Body>();//planet mix
            bodies14.Add(t7Star);
            bodies14.Add(t1RockyPlanet);
            bodies14.Add(t2RockyPlanet);
            bodies14.Add(t4EarthPlanet);
            bodies14.Add(t6EarthPlanet);
            bodies14.Add(t3WaterPlanet);
            bodies14.Add(t5WaterPlanet);
            
            SolarSystem solarSystem0 = new SolarSystem(v, null, bodies0);
            SolarSystem solarSystem1 = new SolarSystem(v, null, bodies1);
            SolarSystem solarSystem2 = new SolarSystem(v, null, bodies2);
            SolarSystem solarSystem3 = new SolarSystem(v, null, bodies3);
            SolarSystem solarSystem4 = new SolarSystem(v, null, bodies4);
            SolarSystem solarSystem5 = new SolarSystem(v, null, bodies5);
            SolarSystem solarSystem6 = new SolarSystem(v, null, bodies6);
            SolarSystem solarSystem7 = new SolarSystem(v, null, bodies7);
            SolarSystem solarSystem8 = new SolarSystem(v, null, bodies8);
            SolarSystem solarSystem9 = new SolarSystem(v, null, bodies9);
            SolarSystem solarSystem10 = new SolarSystem(v, null, bodies10);
            SolarSystem solarSystem11 = new SolarSystem(v, null, bodies11);
            SolarSystem solarSystem12 = new SolarSystem(v, null, bodies12);
            SolarSystem solarSystem13 = new SolarSystem(v, null, bodies13);
            SolarSystem solarSystem14 = new SolarSystem(v, null, bodies14);

            List<SolarSystem> sector0Systems = new List<SolarSystem> { solarSystem0 };
            List<SolarSystem> sector1Systems = new List<SolarSystem> { solarSystem1 };
            List<SolarSystem> sector2Systems = new List<SolarSystem> { solarSystem2 };
            List<SolarSystem> sector3Systems = new List<SolarSystem> { solarSystem3};
            List<SolarSystem> sector4Systems = new List<SolarSystem> { solarSystem4};
            List<SolarSystem> sector5Systems = new List<SolarSystem> { solarSystem5};
            List<SolarSystem> sector6Systems = new List<SolarSystem> { solarSystem6};
            List<SolarSystem> sector7Systems = new List<SolarSystem> { solarSystem7};
            List<SolarSystem> sector8Systems = new List<SolarSystem> { solarSystem8};
            List<SolarSystem> sector9Systems = new List<SolarSystem> { solarSystem9};
            List<SolarSystem> sector10Systems = new List<SolarSystem> { solarSystem10};
            List<SolarSystem> sector11Systems = new List<SolarSystem> { solarSystem11};
            List<SolarSystem> sector12Systems = new List<SolarSystem> { solarSystem12};
            List<SolarSystem> sector13Systems = new List<SolarSystem> { solarSystem13};
            List<SolarSystem> sector14Systems = new List<SolarSystem> { solarSystem14};
            
            List<SolarSystem> sector15Systems = new List<SolarSystem> { solarSystem0,solarSystem1};
            List<SolarSystem> sector16Systems = new List<SolarSystem> { solarSystem2,solarSystem3, solarSystem4, solarSystem5};
            List<SolarSystem> sector17Systems = new List<SolarSystem> { solarSystem6, solarSystem7, solarSystem8, solarSystem9, solarSystem10};
            List<SolarSystem> sector18Systems = new List<SolarSystem> { solarSystem11, solarSystem12, solarSystem13, solarSystem14};
            

            Sector sector0 = new Sector(null);
            sector0.SetSolarSystems(sector0Systems);
            sectors.Add(sector0);
            
            Sector sector1 = new Sector(null);
            sector1.SetSolarSystems(sector1Systems);
            sectors.Add(sector1);
            
            Sector sector2 = new Sector(null);
            sector2.SetSolarSystems(sector2Systems);
            sectors.Add(sector2);
            
            Sector sector3= new Sector(null);
            sector3.SetSolarSystems(sector3Systems);
            sectors.Add(sector3);
            
            Sector sector4= new Sector(null);
            sector4.SetSolarSystems(sector4Systems);
            sectors.Add(sector4);
            
            Sector sector5= new Sector(null);
            sector5.SetSolarSystems(sector5Systems);
            sectors.Add(sector5);
            
            Sector sector6= new Sector(null);
            sector6.SetSolarSystems(sector6Systems);
            sectors.Add(sector6);
            
            Sector sector7= new Sector(null);
            sector7.SetSolarSystems(sector7Systems);
            sectors.Add(sector7);
            
            Sector sector8= new Sector(null);
            sector8.SetSolarSystems(sector8Systems);
            sectors.Add(sector8);
            
            Sector sector9= new Sector(null);
            sector9.SetSolarSystems(sector9Systems);
            sectors.Add(sector9);
            
            Sector sector10= new Sector(null);
            sector10.SetSolarSystems(sector10Systems);
            sectors.Add(sector10);
            
            Sector sector11= new Sector(null);
            sector11.SetSolarSystems(sector11Systems);
            sectors.Add(sector11);
            
            Sector sector12= new Sector(null);
            sector12.SetSolarSystems(sector12Systems);
            sectors.Add(sector12);
            
            Sector sector13= new Sector(null);
            sector13.SetSolarSystems(sector13Systems);
            sectors.Add(sector13);
            
            Sector sector14= new Sector(null);
            sector14.SetSolarSystems(sector14Systems);
            sectors.Add(sector14);
            
            Sector sector15= new Sector(null);
            sector15.SetSolarSystems(sector15Systems);
            sectors.Add(sector15);
            
            Sector sector16= new Sector(null);
            sector16.SetSolarSystems(sector16Systems);
            sectors.Add(sector16);
            
            Sector sector17= new Sector(null);
            sector17.SetSolarSystems(sector17Systems);
            sectors.Add(sector17);
            
            Sector sector18= new Sector(null);
            sector18.SetSolarSystems(sector18Systems);
            sectors.Add(sector18);

            _sectors = sectors;
        }

        [Test]
        public void AgriFactionTest() {
            Assert.AreEqual(0, FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[0]));
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[1]));
            Assert.AreEqual(750,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[2]));
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[3]));
            Assert.AreEqual(450,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[4]));
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[5]));
            Assert.AreEqual(200,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[6]));
            Assert.AreEqual(400,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[7]));
            Assert.AreEqual(451,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[8]));
            Assert.AreEqual(-894,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[9]));
            Assert.AreEqual(-700,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[10]));
            Assert.AreEqual(-497,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[11]));
            Assert.AreEqual(-900,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[12]));
            Assert.AreEqual(-447,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[13]));
            Assert.AreEqual(903,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[14]));
            
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[15]));
            Assert.AreEqual(1212,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[16]));
            Assert.AreEqual(-543,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[17]));
            Assert.AreEqual(-941,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Agriculture, _sectors[18]));
        }
        
        [Test]
        public void CommerceFactionTest() {
            Assert.AreEqual(0, FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[0]));
            Assert.AreEqual(14,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[1]));
            Assert.AreEqual(77,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[2]));
            Assert.AreEqual(14,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[3]));
            Assert.AreEqual(41,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[4]));
            Assert.AreEqual(14,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[5]));
            Assert.AreEqual(16,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[6]));
            Assert.AreEqual(26,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[7]));
            Assert.AreEqual(42,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[8]));
            Assert.AreEqual(-886,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[9]));
            Assert.AreEqual(-884,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[10]));
            Assert.AreEqual(-857,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[11]));
            Assert.AreEqual(-900,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[12]));
            Assert.AreEqual(-850,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[13]));
            Assert.AreEqual(91,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[14]));
            
            Assert.AreEqual(14,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[15]));
            Assert.AreEqual(146,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[16]));
            Assert.AreEqual(-1686,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[17]));
            Assert.AreEqual(-2516,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Commerce, _sectors[18]));
        }
        
        [Test]
        public void IndustrialFactionTest() {
            Assert.AreEqual(0, FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[0]));
            Assert.AreEqual(180,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[1]));
            Assert.AreEqual(77,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[2]));
            Assert.AreEqual(180,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[3]));
            Assert.AreEqual(91,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[4]));
            Assert.AreEqual(180,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[5]));
            Assert.AreEqual(16,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[6]));
            Assert.AreEqual(76,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[7]));
            Assert.AreEqual(121,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[8]));
            Assert.AreEqual(-270,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[9]));
            Assert.AreEqual(-434,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[10]));
            Assert.AreEqual(-290,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[11]));
            Assert.AreEqual(-450,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[12]));
            Assert.AreEqual(-269,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[13]));
            Assert.AreEqual(262,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[14]));
            
            Assert.AreEqual(180,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[15]));
            Assert.AreEqual(528,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[16]));
            Assert.AreEqual(-491,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[17]));
            Assert.AreEqual(-747,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Industrial, _sectors[18]));
        }
        
        [Test]
        public void MilitaryFactionTest() {
            Assert.AreEqual(0, FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[0]));
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[1]));
            Assert.AreEqual(300,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[2]));
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[3]));
            Assert.AreEqual(85,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[4]));
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[5]));
            Assert.AreEqual(80,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[6]));
            Assert.AreEqual(84,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[7]));
            Assert.AreEqual(86,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[8]));
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[9]));
            Assert.AreEqual(80,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[10]));
            Assert.AreEqual(106,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[11]));
            Assert.AreEqual(0,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[12]));
            Assert.AreEqual(88,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[13]));
            Assert.AreEqual(211,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[14]));
            
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[15]));
            Assert.AreEqual(397,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[16]));
            Assert.AreEqual(336,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[17]));
            Assert.AreEqual(405,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Military, _sectors[18]));
        }
        
        [Test]
        public void PirateFactionTest() {
            Assert.AreEqual(0, FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[0]));
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[1]));
            Assert.AreEqual(-150,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[2]));
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[3]));
            Assert.AreEqual(-35,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[4]));
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[5]));
            Assert.AreEqual(-40,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[6]));
            Assert.AreEqual(-36,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[7]));
            Assert.AreEqual(-34,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[8]));
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[9]));
            Assert.AreEqual(-40,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[10]));
            Assert.AreEqual(-44,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[11]));
            Assert.AreEqual(0,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[12]));
            Assert.AreEqual(-32,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[13]));
            Assert.AreEqual(-89,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[14]));
            
            Assert.AreEqual(6,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[15]));
            Assert.AreEqual(-173,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[16]));
            Assert.AreEqual(-144,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[17]));
            Assert.AreEqual(-165,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Pirate, _sectors[18]));
        }
        
        [Test]
        public void TechnologyFactionTest() {
            Assert.AreEqual(70, FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[0]));
            Assert.AreEqual(76,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[1]));
            Assert.AreEqual(520,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[2]));
            Assert.AreEqual(126,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[3]));
            Assert.AreEqual(390,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[4]));
            Assert.AreEqual(186,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[5]));
            Assert.AreEqual(300,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[6]));
            Assert.AreEqual(420,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[7]));
            Assert.AreEqual(451,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[8]));
            Assert.AreEqual(276,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[9]));
            Assert.AreEqual(390,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[10]));
            Assert.AreEqual(513,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[11]));
            Assert.AreEqual(640,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[12]));
            Assert.AreEqual(913,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[13]));
            Assert.AreEqual(613,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[14]));
            
            Assert.AreEqual(146,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[15]));
            Assert.AreEqual(1222,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[16]));
            Assert.AreEqual(1837,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[17]));
            Assert.AreEqual(2679,FactionTypeExtension.GetFactionSectorDesire(Faction.FactionTypeEnum.Technology, _sectors[18]));
        }
    }
}