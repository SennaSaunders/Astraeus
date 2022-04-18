using System.Collections.Generic;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.Star;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._GameControllers;
using Code._Ships.Controllers;
using UnityEngine;

namespace Code._Galaxy._SolarSystem {
    public class SolarSystemController : MonoBehaviour {
        public SolarSystem _solarSystem;
        private GameObject _solarSystemHolder;
        private List<(Body body, GameObject bodyObject)> bodyObjectMap = new List<(Body body, GameObject bodyObject)>();
        public const int ZOffset = 0;
        public const int SpaceStationZ = -80;
        public int _maxNPCsHabited;
        public int _maxNPCsUninhabited;
        public bool Active { get; set; }

        public void DisplaySolarSystem() {
            SetupSolarSystemHolder();
            DisplaySolarSystemObjects();
        }

        private void Update() {
            if (Active && !GameController.isPaused) {
                SpawnShips();
            }
        }

        private void SpawnShips() {
            bool habited = _solarSystem.OwnerFaction != null;
            GameObject npcShipHolder = GameObject.Find("NPC Ships");
            if (npcShipHolder != null) {
                int numNPCs = npcShipHolder.transform.childCount;
                if (habited) {
                    if (numNPCs < _maxNPCsHabited) {
                    
                    }
                }
                else {
                    if (numNPCs < _maxNPCsUninhabited) {
                    
                    }
                }
            }
        }


        public void AssignSystem(SolarSystem solarSystem) {
            _solarSystem = solarSystem;
        }

        private void SetupSolarSystemHolder() {
            const string systemName = "SolarSystem";
            _solarSystemHolder = GameObject.Find(systemName);
            if (Application.isEditor) {
                DestroyImmediate(_solarSystemHolder);
            }
            else {
                Destroy(_solarSystemHolder);
            }

            _solarSystemHolder = new GameObject(systemName);
        }

        public GameObject GetBodyGameObject(Body body) {
            return bodyObjectMap.Find(m => m.body == body).bodyObject;
        }

        private void DisplaySolarSystemObjects() {
            SetupSolarSystemHolder();
            _solarSystemHolder.transform.position = new Vector3(0, 0, ZOffset);
            List<GameObject> bodyHolders = new List<GameObject>();

            //adds all objects in SolarSystem to the scene before parenting them
            for (int i = 0; i < _solarSystem.Bodies.Count; i++) {
                Body body = _solarSystem.Bodies[i];
                GameObject bodyObject = body.GetSystemObject();
                bodyObjectMap.Add((body, bodyObject));
                bodyObject.transform.localPosition = new Vector3(0, 0, body.Tier.SystemScale()); //sets the forwards most part of all bodies so that they are on the same Z level
                bodyObject.name = "Body: " + i + " Tier: " + body.Tier;
                GameObject bodyHolder = new GameObject("Holder - " + body.Tier + " Body");
                bodyObject.transform.SetParent(bodyHolder.transform);
                string proximityGUIBase = "GUIPrefabs/Proximity/";
                if (body.GetType() == typeof(SpaceStation)) {
                    bodyObject.transform.localPosition = new Vector3(0, 0, SpaceStationZ);
                    PlayerBodyProximity playerBodyProximity = bodyObject.AddComponent<PlayerBodyProximity>();
                    playerBodyProximity.Setup(KeyCode.F, proximityGUIBase + "EnterSpaceStationGUI");
                    playerBodyProximity.SetProximityFunction<SpaceStation>(GameController.GUIController.SetupStationGUI, (SpaceStation)body);
                }

                if (body.GetType() == typeof(Star)) {
                    PlayerBodyProximity playerBodyProximity = bodyObject.AddComponent<PlayerBodyProximity>();
                    playerBodyProximity.Setup(KeyCode.Space, proximityGUIBase + "FuelScoopGUI");
                    playerBodyProximity.SetProximityFunction<Star>(GameController.CurrentShip.ShipObject.GetComponent<ShipController>().CargoController.FuelScoop, (Star)body);
                }
                bodyHolders.Add(bodyHolder);
            }

            //assigns bodies their correct positions and parents
            for (int i = 0; i < _solarSystem.Bodies.Count; i++) {
                Body currentBody = _solarSystem.Bodies[i];
                GameObject bodyHolder = bodyHolders[i];

                bool hasParent = currentBody.Primary != null;
                if (hasParent) {
                    int parentIndex = _solarSystem.Bodies.IndexOf(currentBody.Primary);
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