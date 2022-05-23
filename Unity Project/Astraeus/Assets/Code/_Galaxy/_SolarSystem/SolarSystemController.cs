using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;
using Code._Galaxy._SolarSystem._CelestialObjects.Star;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._GameControllers;
using Code._Ships.ShipComponents;
using UnityEngine;

namespace Code._Galaxy._SolarSystem {
    public class SolarSystemController : MonoBehaviour {
        public SolarSystem SolarSystem;
        private GameObject _solarSystemHolder;
        private List<(Body body, GameObject bodyObject)> _bodyObjectMap;
        public const int ZOffset = 0;
        public const int SpaceStationZ = -80;
        public int maxNpCs = 0;
        private float _spawnTimer = 0;
        private float timeTillSpawn = 5;
        public bool Active { get; set; }
        private Thread _generationThread;
        private bool _shipGUI;

        private Thread GeneratePlanetColours(SolarSystem solarSystem) {
            Thread generationThread = new Thread(solarSystem.GenerateSolarSystemPlanetColours);
            generationThread.Start();
            return generationThread;
        }

        private bool TexturesGenerated() {
            bool texturesGenerated = true;
            for (int i = 0; i < SolarSystem.Bodies.Count; i++) {
                if (SolarSystem.Bodies[i].GetType() == typeof(Planet)) {
                    Planet planet = (Planet)SolarSystem.Bodies[i];
                    if (planet.SurfaceTexture == null) {
                        texturesGenerated = false;
                    }

                    break;
                }
            }

            return texturesGenerated;
        }

        private bool ColoursGenerated() {
            bool coloursGenerated = true;
            for (int i = 0; i < SolarSystem.Bodies.Count; i++) {
                if (SolarSystem.Bodies[i].GetType() == typeof(Planet)) {
                    Planet planet = (Planet)SolarSystem.Bodies[i];
                    if (planet.PlanetGen.Colors == null) {
                        coloursGenerated = false;
                    }

                    break;
                }
            }

            return coloursGenerated;
        }

        public void DisplaySolarSystem(bool shipGUI) {
            _shipGUI = shipGUI;
            if (!TexturesGenerated()) {
                if (!ColoursGenerated()) {
                    GameController.GUIController.loadingScreenController.StartLoadingScreen("Generating Planet Textures", null);
                    _generationThread = GeneratePlanetColours(SolarSystem);
                }
                else {
                    SolarSystem.GenerateSolarSystemTextures();
                    Display(shipGUI);
                }
            }
            else {
                Display(shipGUI);
            }
        }

        private void Display(bool shipGUI) {
            SetupSolarSystemHolder();
            DisplaySolarSystemObjects();
            GameController.GUIController.loadingScreenController.FinishedLoading();
            GameController.GUIController.SetShipGUIActive(shipGUI);
            _spawnTimer = timeTillSpawn;
            Active = true;
        }

        private void Update() {
            if (_generationThread != null) {
                if (!_generationThread.IsAlive) {
                    _generationThread = null;
                    SolarSystem.GenerateSolarSystemTextures();
                    Display(_shipGUI);
                }
            }

            if (Active && !GameController.IsPaused) {
                SpawnShipsCheck();
            }
        }

        private void SpawnShipsCheck() {
            bool inhabited = SolarSystem.OwnerFaction != null;
            int numNpCs = GameObject.FindWithTag("GameController").GetComponent<GameController>().GetNPCCount();
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0) {
                if (inhabited) {
                    if (numNpCs < maxNpCs) {
                        SpawnShip();
                    }
                }
            }
        }

        private void SpawnShip() {
            _spawnTimer = timeTillSpawn;
            var spawns = GetOffScreenBodies();
            //TODO - figure out best way to vary factions within a system

            float systemFactionChance = .7f;
            System.Random r = new System.Random();

            bool spawnSystemFaction = r.NextDouble() < systemFactionChance;
            Faction faction;
            if (spawnSystemFaction && SolarSystem.OwnerFaction != null) {
                faction = SolarSystem.OwnerFaction;
            }
            else {
                //roll for pirate chance
                float pirateChance = .7f;
                bool spawnPirate = r.NextDouble() < pirateChance;
                do {
                    if (spawnPirate) {
                        var pirateFactions = GameController.GalaxyController._galaxy.Factions.Where(f => f.factionType == Faction.FactionType.Pirate).ToList();
                        faction = pirateFactions[r.Next(pirateFactions.Count)];
                    }
                    else {
                        var factionsExcludingOwnerAndPirates = GameController.GalaxyController._galaxy.Factions.Where(f => f.factionType != Faction.FactionType.Pirate && f != SolarSystem.OwnerFaction).ToList();
                        faction = factionsExcludingOwnerAndPirates[r.Next(factionsExcludingOwnerAndPirates.Count)];
                    }
                } while (faction == SolarSystem.OwnerFaction);
            }

            int numShipClasses = Enum.GetValues(typeof(ShipCreator.ShipClass)).Length;
            int classRoll = r.Next(numShipClasses);
            int numShipTiers = Enum.GetValues(typeof(ShipComponentTier)).Length;
            int tierRoll = r.Next(numShipTiers);
            if (spawns.Count > 0) {
                GameObject.FindWithTag("GameController").GetComponent<GameController>().CreateNPC(faction, (ShipCreator.ShipClass)classRoll, (ShipComponentTier)tierRoll, (float)r.NextDouble(), spawns[r.Next(spawns.Count)].transform.position);
            }
        }

        private List<GameObject> GetOffScreenBodies() {
            List<GameObject> offScreenBodies = new List<GameObject>();
            foreach ((Body body, GameObject bodyObject) bodies in _bodyObjectMap) {
                Vector3 viewportPoint = UnityEngine.Camera.main.WorldToViewportPoint(bodies.bodyObject.transform.position);

                if (!(viewportPoint.x < 1 && viewportPoint.x > 0 && viewportPoint.y < 1 && viewportPoint.y > 0)) {
                    offScreenBodies.Add(bodies.bodyObject);
                }
            }

            return offScreenBodies;
        }


        public void AssignSystem(SolarSystem solarSystem) {
            SolarSystem = solarSystem;
            bool inhabited = SolarSystem.OwnerFaction != null;
            maxNpCs = (inhabited ? solarSystem.Bodies.Count - 1 : SolarSystem.Bodies.Count / 2);
        }

        private void SetupSolarSystemHolder() {
            const string systemName = "SolarSystem";
            _solarSystemHolder = GameObject.Find(systemName);
            if (_solarSystemHolder) {
                DestroyImmediate(_solarSystemHolder);
            }            
            _solarSystemHolder = new GameObject(systemName);
        }

        public GameObject GetBodyGameObject(Body body) {
            return _bodyObjectMap.Find(m => m.body == body).bodyObject;
        }

        private void DisplaySolarSystemObjects() {
            SetupSolarSystemHolder();
            _solarSystemHolder.transform.position = new Vector3(0, 0, ZOffset);
            List<GameObject> bodyHolders = new List<GameObject>();
            _bodyObjectMap = new List<(Body body, GameObject bodyObject)>();
            //adds all objects in SolarSystem to the scene before parenting them
            for (int i = 0; i < SolarSystem.Bodies.Count; i++) {
                Body body = SolarSystem.Bodies[i];
                GameObject bodyObject = Instantiate(body.GetSystemObject());
                float scale = body.Tier.SystemScale();
                bodyObject.transform.localScale = new Vector3(scale, scale, scale);
                _bodyObjectMap.Add((body, bodyObject));
                bodyObject.transform.localPosition = new Vector3(0, 0, body.Tier.SystemScale()); //sets the forwards most part of all bodies so that they are on the same Z level
                bodyObject.name = "Body: " + i + " Tier: " + body.Tier;
                GameObject bodyHolder = new GameObject("Holder - " + body.Tier + " Body");
                bodyObject.transform.SetParent(bodyHolder.transform);

                //mini map
                GameObject miniMapObject = Instantiate(body.GetMiniMapObject(), bodyObject.transform, true);
                miniMapObject.layer = LayerMask.NameToLayer("LocalMap");
                float miniMapScale = body.Tier == Body.BodyTier.T0 ? 600 : 2;
                miniMapObject.transform.localScale = new Vector3(miniMapScale, miniMapScale, miniMapScale);
                miniMapObject.GetComponent<Renderer>().material.color = body.MapColour;

                string proximityGUIBase = "GUIPrefabs/Proximity/";
                //******* make proximity function take multiple functions - display them in a list - allow for claiming systems - setting up station industries - fuel scoop on stars 
                if (body.GetType() == typeof(SpaceStation)) {
                    bodyObject.transform.localPosition = new Vector3(0, 0, SpaceStationZ);
                    PlayerBodyProximity playerBodyProximity = bodyObject.AddComponent<PlayerBodyProximity>();
                    playerBodyProximity.Setup(KeyCode.F, proximityGUIBase + "EnterSpaceStationGUI");
                    playerBodyProximity.SetProximityFunction<SpaceStation>(GameController.GUIController.SetupStationGUI, (SpaceStation)body);
                }

                if (body.GetType() == typeof(Star)) {
                    PlayerBodyProximity playerBodyProximity = bodyObject.AddComponent<PlayerBodyProximity>();
                    playerBodyProximity.Setup(KeyCode.Space, proximityGUIBase + "FuelScoopGUI");
                    playerBodyProximity.SetProximityFunction<Star>(delegate { GameController.PlayerShipController.CargoController.FuelScoop((Star)body);},(Star)body );
                }

                bodyHolders.Add(bodyHolder);
            }

            //assigns bodies their correct positions and parents
            for (int i = 0; i < SolarSystem.Bodies.Count; i++) {
                Body currentBody = SolarSystem.Bodies[i];
                GameObject bodyHolder = bodyHolders[i];

                bool hasParent = currentBody.Primary != null;
                if (hasParent) {
                    int parentIndex = SolarSystem.Bodies.IndexOf(currentBody.Primary);
                    GameObject parentObject = bodyHolders[parentIndex];
                    GameObject rotationHolder = new GameObject(i.ToString());

                    rotationHolder.transform.localEulerAngles = new Vector3(0, 0, currentBody.RotationCurrent * 360);
                    rotationHolder.transform.SetParent(parentObject.transform, false);
                    bodyHolder.transform.SetParent(rotationHolder.transform, false);
                }
                else {
                    bodyHolder.transform.SetParent(_solarSystemHolder.transform, false);
                }

                bodyHolder.transform.localPosition = new Vector3(currentBody.Coordinate.x, currentBody.Coordinate.y);
                bodyHolder.transform.localEulerAngles = new Vector3(0, 0, 0);
            }

            GameObject projectileHolder = new GameObject("ProjectileHolder");
        }
    }
}