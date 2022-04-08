using UnityEngine;

namespace Code._Utility {
    public static class FindChildGameObject {
        public static GameObject FindChild(GameObject parent, string childName) {
            for (int i = 0; i < parent.transform.childCount; i++) {
                GameObject child = parent.transform.GetChild(i).gameObject;
                if (child.name == childName) {
                    return child;
                }

                if (child.transform.childCount > 0) {
                    child = FindChild(child, childName);
                    if (child != null) {
                        return child;
                    }
                }
            }

            return null;
        }
    }
}