using System.Collections.Generic;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using UnityEngine;

namespace Code._Galaxy {
    public class GalaxyController : MonoBehaviour {
        private Galaxy _galaxy;
        private static GalaxyView _view;

        private void Start() {
            _camera = UnityEngine.Camera.main;
        }

        void Update() {
            SolarSystemRaycast();
        }

        public void SetGalaxy(Galaxy galaxy) {
            _galaxy = galaxy;
        }

        private void SetupGalaxyView() {
            _view = FindObjectOfType<GalaxyView>();
            if (Application.isEditor) {
                DestroyImmediate(_view);
            }
            else {
                Destroy(_view);
            }
            _view = gameObject.AddComponent<GalaxyView>();
            _view.SetupGalaxyView();
        }
        
        public void DisplayGalaxy() {
            SetupGalaxyView();
            _view.DisplayGalaxy(_galaxy);
        }

        private SolarSystemController _activeSystemController;
        private UnityEngine.Camera _camera;

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