using System;
using UnityEngine;

namespace Code.Galaxy {
    public class GalaxyController : MonoBehaviour {
        private Galaxy _galaxy;
        private static GalaxyView _view;

        void Update() {
            SolarSystemRaycast();
        }

        private void SetupView() {
            _view = GameObject.FindObjectOfType<GalaxyView>();
            if (Application.isEditor) {
                DestroyImmediate(_view);
            }
            else {
                Destroy(_view);
            }
            _view = gameObject.AddComponent<GalaxyView>();
            _view.SetupGalaxyView();
        }
        
        public void DisplayGalaxy(Galaxy galaxy) {
            _galaxy = galaxy;
            SetupView();
            for (int i = 0; i < _galaxy.Systems.Count; i++) {   //can potentially be changed to only display some systems later e.g. hidden systems/maybe culling?
                _view.DisplaySolarSystemPrimary(_galaxy.Systems[i], i+1);
            }
        }

        private SolarSystemController activeSystemController;
        private void SolarSystemRaycast() {
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            SolarSystemController bodyHit;

            if (Physics.Raycast(ray, out RaycastHit hit)) {
                bodyHit = hit.collider.gameObject.GetComponent<SolarSystemController>();
                if (bodyHit) {
                    Debug.Log("Hit " + hit.collider.gameObject.name);
                    if (Input.GetMouseButtonDown(0)) {
                        if (activeSystemController != null) {
                            activeSystemController.Active = false;
                        }
                        activeSystemController = bodyHit;
                        activeSystemController.Active = true;
                        activeSystemController.DisplaySolarSystem();
                    }
                }
                Debug.DrawLine(hit.transform.position, UnityEngine.Camera.main.transform.position);
            }
        }
        
        //also should contain functions that advance and control the galaxy's state e.g. public void ChangeGalaxyState(){}
    }
}