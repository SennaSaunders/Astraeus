using System.Collections.Generic;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using UnityEngine;

namespace Code._Galaxy {
    public class GalaxyController : MonoBehaviour {
        public const int zOffset = 2500; 
        private Galaxy _galaxy;
        private GameObject _galaxyHolder;
        private SolarSystemController _activeSystemController;
        private List<SolarSystemController> _solarSystemControllers = new List<SolarSystemController>();
        private UnityEngine.Camera _camera;

        private void Start() {
            _camera = UnityEngine.Camera.main;
        }

        // void Update() {
        //     SolarSystemRaycast();
        // }

        public void SetGalaxy(Galaxy galaxy) {
            _galaxy = galaxy;
        }

        public void SetupGalaxyHolder() {
            string holderName = "Galaxy";
            _galaxyHolder = GameObject.Find(holderName);
            if (Application.isEditor) {
                DestroyImmediate(_galaxyHolder);
            }
            else {
                Destroy(_galaxyHolder);
            }
            _galaxyHolder = new GameObject(holderName);
            _galaxyHolder.transform.position = new Vector3(0, 0, zOffset);
        }
        
        private void DisplaySolarSystemPrimary(SolarSystem solarSystem, int num) {
            CelestialBody primary = (CelestialBody)solarSystem.Bodies[0];
            GameObject primaryObject = primary.GetMapObject();  //change out for Instantiate() when prefabs are made
            primaryObject.transform.SetParent(_galaxyHolder.transform);
            primaryObject.transform.localPosition = new Vector3(solarSystem.Coordinate.x, solarSystem.Coordinate.y);
            primaryObject.name = "System: " + num + " Tier: " + primary.Tier;
            SolarSystemController controller = primaryObject.AddComponent<SolarSystemController>();
            controller.AssignSystem(solarSystem);
            _solarSystemControllers.Add(controller);
        }

        public void DisplayGalaxy(Galaxy galaxy) {
            for (int i = 0; i < galaxy.Systems.Count; i++) {   //can potentially be changed to only display some systems later e.g. hidden systems/maybe culling?
                DisplaySolarSystemPrimary(galaxy.Systems[i], i+1);
            }
        }

        public SolarSystemController GetSolarSystemController(SolarSystem solarSystem) {
            return _solarSystemControllers.Find(ssc => ssc.solarSystem == solarSystem);
        }

        public void DisplayGalaxy() {
            SetupGalaxyHolder();
            DisplayGalaxy(_galaxy);
        }

        private void SolarSystemRaycast() {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit)) {
                var bodyHit = hit.collider.gameObject.GetComponent<SolarSystemController>();
                if (bodyHit) {
                    Debug.Log("Hit " + hit.collider.gameObject.name);
                    if (Input.GetMouseButtonDown(0)) {
                        if (_activeSystemController != null) {
                            _activeSystemController.Active = false;
                        }
                        _activeSystemController = bodyHit;
                        _activeSystemController.Active = true;
                        _activeSystemController.DisplaySolarSystem();
                    }
                }
                Debug.DrawLine(hit.transform.position, _camera.transform.position);
            }
        }

        public List<Faction> GetFactions() {
            return _galaxy.Factions;
        }
        
        //also should contain functions that advance and control the galaxy's state e.g. public void ChangeGalaxyState(){}
    }
}