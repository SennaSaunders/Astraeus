using Code.Camera;
using UnityEngine;

namespace Code._Galaxy._SolarSystem {
    public class SolarSystemController : MonoBehaviour {
        private SolarSystem _solarSystem;
        private static SolarSystemView _view;
        public bool Active { get; set; }

        private void SetupView() {
            _view = FindObjectOfType<SolarSystemView>();
            if (Application.isEditor) {
                DestroyImmediate(_view);
            }
            else {
                Destroy(_view);
            }
            _view = gameObject.AddComponent<SolarSystemView>();
        }

        public void DisplaySolarSystem() {
            SetupView();
            _view.DisplaySolarSystem(_solarSystem);
            float maxDistance = SolarSystem.GetFurthestDistance(_solarSystem.Bodies);
            FindObjectOfType<GalaxyCameraController>().ToggleCameraControl(); 
            FindObjectOfType<SolarSystemCameraController>().SetupCamera(maxDistance, maxDistance);
        }
        
        //update the rotation of planets
        public void AssignSystem(SolarSystem solarSystem) {
            _solarSystem = solarSystem;
        }
    }
}