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
        
        public void DisplaySolarSystem(Vector2 coordinate) {
            GameObject primary = GameObject.CreatePrimitive(PrimitiveType.Sphere);  //change out for Instantiate() when prefabs are made
            primary.transform.SetParent(_galaxyHolder.transform);
            primary.transform.localPosition = new Vector3(coordinate.x, coordinate.y);
        }
    }
}