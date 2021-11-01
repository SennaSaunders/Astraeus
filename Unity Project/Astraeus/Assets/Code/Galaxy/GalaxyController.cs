using UnityEngine;

namespace Code.Galaxy {
    public class GalaxyController : MonoBehaviour {
        private Galaxy _galaxy;
        private static GalaxyView _view;
        
        private void SetupView() {
            _view = GameObject.FindObjectOfType<GalaxyView>();
            if (Application.isEditor) {
                DestroyImmediate(_view);
            }
            else {
                Destroy(_view);
            }
            _view = gameObject.AddComponent<GalaxyView>();
        }
        
        public void DisplayGalaxy(Galaxy galaxy) {
            _galaxy = galaxy;
            SetupView();
            _view.SetupGalaxyView();
            foreach (SolarSystem solarSystem in _galaxy.Systems) {   //can potentially be changed to only display some systems later e.g. hidden systems/maybe culling?
                _view.DisplaySolarSystem(solarSystem.Coordinate);
            }
        }
        //also contains functions that advance and control the galaxy's state e.g. public void ChangeGalaxyState(){}
    }
}