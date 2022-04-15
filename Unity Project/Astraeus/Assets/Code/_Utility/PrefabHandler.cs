using System.Collections.Generic;
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

        public List<GameObject> GetMeshObjects(GameObject meshObject) {
            List<GameObject> meshObjects = new List<GameObject>();
            MeshRenderer meshRenderer = meshObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null) {
                meshObjects.Add(meshObject);
            }
            for (int i = 0; i < meshObject.transform.childCount; i++) {
                meshObjects.AddRange(GetMeshObjects(meshObject.transform.GetChild(i).gameObject));
            }

            return meshObjects;
        }
    }
}