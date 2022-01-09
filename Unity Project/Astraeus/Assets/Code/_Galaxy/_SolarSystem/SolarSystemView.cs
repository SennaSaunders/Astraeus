using System.Collections.Generic;
using Code._Galaxy._SolarSystem._CelestialObjects;
using UnityEngine;

namespace Code._Galaxy._SolarSystem {
    public class SolarSystemView : MonoBehaviour {
        private GameObject _solarSystemHolder;
        private const int ZOffset = 2500;

        private void SetupSolarSystemView() {
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

        public void DisplaySolarSystem(SolarSystem solarSystem) {
            SetupSolarSystemView();
            _solarSystemHolder.transform.position = new Vector3(0, 0, ZOffset);
            List<GameObject> bodyHolders = new List<GameObject>();

            //adds all objects in SolarSystem to the scene before parenting them
            for (int i = 0; i < solarSystem.Bodies.Count; i++) {
                Body body = solarSystem.Bodies[i];
                GameObject newBody = body.GetSystemObject();
                newBody.transform.localPosition = new Vector3(0, 0, body.Tier.SystemScale());
                newBody.name = "Body: " + i + " Tier: " + body.Tier;
                GameObject bodyHolder = new GameObject("Holder - " + body.Tier + " Body");
                newBody.transform.SetParent(bodyHolder.transform);
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

        //display the rotation of planets
    }
}