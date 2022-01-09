using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using UnityEngine;

namespace Code._Galaxy {
    public class GalaxyView : MonoBehaviour {
        private GameObject _galaxyHolder;

        public void SetupGalaxyView() {
            string holderName = "Galaxy";
            _galaxyHolder = GameObject.Find(holderName);
            if (Application.isEditor) {
                DestroyImmediate(_galaxyHolder);
            }
            else {
                Destroy(_galaxyHolder);
            }
            _galaxyHolder = new GameObject(holderName);
        }
        
        private void DisplaySolarSystemPrimary(SolarSystem solarSystem, int num) {
            CelestialBody primary = (CelestialBody)solarSystem.Bodies[0];
            GameObject primaryObject = primary.GetMapObject();  //change out for Instantiate() when prefabs are made
            primaryObject.transform.SetParent(_galaxyHolder.transform);
            primaryObject.transform.localPosition = new Vector3(solarSystem.Coordinate.x, solarSystem.Coordinate.y);
            primaryObject.name = "System: " + num + " Tier: " + primary.Tier;
            SolarSystemController controller = primaryObject.AddComponent<SolarSystemController>();
            controller.AssignSystem(solarSystem);

        }

        public void DisplayGalaxy(Galaxy galaxy) {
            for (int i = 0; i < galaxy.Systems.Count; i++) {   //can potentially be changed to only display some systems later e.g. hidden systems/maybe culling?
                DisplaySolarSystemPrimary(galaxy.Systems[i], i+1);
            }
        }
    }
}