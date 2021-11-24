using Code.CelestialObjects;
using UnityEngine;

namespace Code.Galaxy {
    public class GalaxyView : MonoBehaviour {
        private GameObject _galaxyHolder;

        public void SetupGalaxyView() {
            string name = "Galaxy";
            _galaxyHolder = GameObject.Find(name);
            if (Application.isEditor) {
                DestroyImmediate(_galaxyHolder);
            }
            else {
                Destroy(_galaxyHolder);
            }
            _galaxyHolder = new GameObject(name);
        }
        
        public void DisplaySolarSystemPrimary(SolarSystem solarSystem, int num) {
            CelestialBody primary = (CelestialBody)solarSystem.Bodies[0];
            GameObject primaryObject = primary.GetMapObject();  //change out for Instantiate() when prefabs are made
            primaryObject.transform.SetParent(_galaxyHolder.transform);
            primaryObject.transform.localPosition = new Vector3(solarSystem.Coordinate.x, solarSystem.Coordinate.y);
            primaryObject.name = "System: " + num + " Tier: " + primary.Tier;
            SolarSystemController controller = primaryObject.AddComponent<SolarSystemController>();
            controller.AssignSystem(solarSystem);

        }
    }
}