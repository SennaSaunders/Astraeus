using Code._Galaxy;
using UnityEngine;

namespace Code._GameControllers {
    public class GameController : MonoBehaviour {
        private static GalaxyController _galaxyController;
        
        public void SetupGalaxyController(Galaxy galaxy) {
            _galaxyController = FindObjectOfType<GalaxyController>();
            if (Application.isEditor) {
                DestroyImmediate(_galaxyController);
            }
            else {
                Destroy(_galaxyController);
            }

            _galaxyController = gameObject.AddComponent<GalaxyController>();
            _galaxyController.SetGalaxy(galaxy);
        }

        public static void ShowGalaxy() {
            _galaxyController.DisplayGalaxy();
        }
        
        public enum GameFocus {
            Map,
            SolarSystem,
            Paused
        }

        public static GameFocus GameStateFocus;
        
        
    }
}