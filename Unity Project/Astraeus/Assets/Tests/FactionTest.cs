using System.Collections.Generic;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;
using Code._Galaxy._SolarSystem._CelestialObjects.Star;
using Code.TextureGen;
using NUnit.Framework;
using UnityEngine;

namespace Tests {
    public class FactionTest {
        private static SolarSystem solarSystem;
        private static List<int> agricultureBodyValues;
        private static List<int> commerceBodyValues;
        private static List<int> industrialBodyValues;
        private static List<int> militaryBodyValues;
        private static List<int> pirateBodyValues;
        private static List<int> techBodyValues;

        [SetUp]
        public static void SetupTest() {
            SetupSolarSystem();
        }

        private static void SetupSolarSystem() {
            Vector2 vector2 = new Vector2();
            Star t7Star = new Star(null, vector2, Star.StarType.A);
            Star t8Star = new Star(null, vector2, Star.StarType.B);
            Star t9Star = new Star(null, vector2, Star.StarType.O);

            BlackHole blackHole = new BlackHole(null, vector2);

            Planet t1RockyPlanet = new Planet(null, Body.BodyTier.T1, new RockyWorldGen(0, 0));
            Planet t2RockyPlanet = new Planet(null, Body.BodyTier.T2, new RockyWorldGen(0, 0));
            Planet t3RockyPlanet = new Planet(null, Body.BodyTier.T3, new RockyWorldGen(0, 0));

            Planet t4EarthPlanet = new Planet(null, Body.BodyTier.T4, new EarthWorldGen(0, 0));
            Planet t5EarthPlanet = new Planet(null, Body.BodyTier.T5, new EarthWorldGen(0, 0));
            Planet t6EarthPlanet = new Planet(null, Body.BodyTier.T6, new EarthWorldGen(0, 0));

            Planet t1WaterPlanet = new Planet(null, Body.BodyTier.T1, new WaterWorldGen(0, 0));
            Planet t3WaterPlanet = new Planet(null, Body.BodyTier.T3, new WaterWorldGen(0, 0));
            Planet t5WaterPlanet = new Planet(null, Body.BodyTier.T5, new WaterWorldGen(0, 0));
            List<Body> bodies = new List<Body>() {
                t7Star,
                t8Star,
                t9Star,
                blackHole,
                t1RockyPlanet,
                t2RockyPlanet,
                t3RockyPlanet,
                t4EarthPlanet,
                t5EarthPlanet,
                t6EarthPlanet,
                t1WaterPlanet,
                t3WaterPlanet,
                t5WaterPlanet
            };
            solarSystem = new SolarSystem(vector2, null, bodies, "");

            agricultureBodyValues = new List<int>() { 7,8,9,-900,1,2,3,200,250,300,50,150,250};
            commerceBodyValues = new List<int>() {7,8,9,-900,1,2,3,4,5,6,1,3,5 };
            industrialBodyValues = new List<int>() {7,8,9,-450,30,60,90,4,5,6,15,45,75 };
            militaryBodyValues = new List<int>() { 7,8,9,9,1,2,3,80,100,120,1,3,5};
            pirateBodyValues = new List<int>() { 7,8,9,9,1,2,3,-40,-50,-60,1,3,5};
            techBodyValues = new List<int>() { 70,120,180,270,1,2,3,120,150,180,30,90,150};
        }

        private void SystemBodyDesireTest(Faction.FactionType factionType, List<int> bodyDesireValues) {
            for (int i =0; i < solarSystem.Bodies.Count;i++) {
                Body body = solarSystem.Bodies[i];
                int expected = bodyDesireValues[i];
                Assert.AreEqual(expected, factionType.CelestialBodyDesire((CelestialBody)body));
            }
        }

        private void SystemDesireTest(Faction.FactionType factionType, int expected) {
            Assert.AreEqual(expected, factionType.FactionSystemDesire(solarSystem));
        }

        [Test]
        public void AgriFactionTest() {
            Faction.FactionType factionType = Faction.FactionType.Agriculture;
            SystemBodyDesireTest(factionType, agricultureBodyValues);
            SystemDesireTest(factionType, 330);
        }

        [Test]
        public void CommerceFactionTest() {
            Faction.FactionType factionType = Faction.FactionType.Commerce;
            SystemBodyDesireTest(factionType, commerceBodyValues);
            SystemDesireTest(factionType, -846);
        }

        [Test]
        public void IndustrialFactionTest() {
            Faction.FactionType factionType = Faction.FactionType.Industrial;
            SystemBodyDesireTest(factionType, industrialBodyValues);
            SystemDesireTest(factionType, -96);
        }

        [Test]
        public void MilitaryFactionTest() {
            Faction.FactionType factionType = Faction.FactionType.Military;
            SystemBodyDesireTest(factionType, militaryBodyValues);
            SystemDesireTest(factionType, 348);
        }

        [Test]
        public void PirateFactionTest() {
            Faction.FactionType factionType = Faction.FactionType.Pirate;
            SystemBodyDesireTest(factionType, pirateBodyValues);
            SystemDesireTest(factionType, -102);
        }

        [Test]
        public void TechnologyFactionTest() {
            Faction.FactionType factionType = Faction.FactionType.Technology;
            SystemBodyDesireTest(factionType, techBodyValues);
            SystemDesireTest(factionType, 1366);
        }
    }
}