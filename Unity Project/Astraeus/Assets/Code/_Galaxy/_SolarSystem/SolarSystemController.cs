using System.Collections.Generic;
using Code._Galaxy._SolarSystem._CelestialObjects;
using UnityEngine;

namespace Code._Galaxy._SolarSystem {
    public class SolarSystemController : MonoBehaviour {
        public SolarSystem _solarSystem;
        private GameObject _solarSystemHolder;
        private List<(Body body, GameObject bodyObject)> bodyObjectMap = new List<(Body body, GameObject bodyObject)>();
        public const int ZOffset = 0;
        public bool Active { get; set; }

        public void DisplaySolarSystem() {
            SetupSolarSystemHolder();
            DisplaySolarSystem(_solarSystem);
        }

        //update the rotation of planets
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

        public void DisplaySolarSystem(SolarSystem solarSystem) {
            SetupSolarSystemHolder();
            _solarSystemHolder.transform.position = new Vector3(0, 0, ZOffset);
            List<GameObject> bodyHolders = new List<GameObject>();

            //adds all objects in SolarSystem to the scene before parenting them
            for (int i = 0; i < solarSystem.Bodies.Count; i++) {
                Body body = solarSystem.Bodies[i];
                GameObject bodyObject = body.GetSystemObject();
                bodyObjectMap.Add((body, bodyObject));
                bodyObject.transform.localPosition = new Vector3(0, 0, body.Tier.SystemScale() / 2); //sets the forwards most part of all bodies so that they are on the same Z level
                bodyObject.name = "Body: " + i + " Tier: " + body.Tier;
                GameObject bodyHolder = new GameObject("Holder - " + body.Tier + " Body");
                bodyObject.transform.SetParent(bodyHolder.transform);
                bodyHolders.Add(bodyHolder);
            }

            //assigns bodies their correct positions and parents
            for (int i = 0; i < solarSystem.Bodies.Count; i++) {
                Body currentBody = solarSystem.Bodies[i];
                GameObject bodyHolder = bodyHolders[i];

                bool hasParent = currentBody.Primary != null;
                if (hasParent) {
                    int parentIndex = solarSystem.Bodies.IndexOf(currentBody.Primary);
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
        }
    }
}