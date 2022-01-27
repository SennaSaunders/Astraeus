using UnityEngine;

namespace Code._Utility {
    public class PrefabHandler : MonoBehaviour {
        public GameObject loadPrefab(string prefabPath) {
            return (GameObject)Resources.Load(prefabPath);
        }

        public GameObject instantiateObject(GameObject prefab) {
            return Instantiate(prefab);
        }

        public GameObject instantiateObject(GameObject prefab, Transform parent) {
            return Instantiate(prefab, parent);
        }
    }
}