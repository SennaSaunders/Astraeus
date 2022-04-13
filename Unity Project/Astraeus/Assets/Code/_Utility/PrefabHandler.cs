using UnityEngine;

namespace Code._Utility {
    public class PrefabHandler : MonoBehaviour {
        public GameObject LoadPrefab(string prefabPath) {
            return (GameObject)Resources.Load(prefabPath);
        }

        public GameObject InstantiateObject(GameObject prefab) {
            return Instantiate(prefab);
        }

        public GameObject InstantiateObject(GameObject prefab, Transform parent) {
            return Instantiate(prefab, parent);
        }
    }
}