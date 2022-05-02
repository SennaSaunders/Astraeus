using TMPro;
using UnityEngine;

namespace Code._Utility {
    public static class GameObjectHelper {
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
        
        public static void SetGUITextValue(GameObject parent, string gameObjectName, string text) {
            TextMeshProUGUI textObject = FindChild(parent, gameObjectName).GetComponent<TextMeshProUGUI>();
            textObject.text = text;
        }
        public static void SetGUITextValue(GameObject parent, string gameObjectName, string text, Color colour) {
            TextMeshProUGUI textObject = FindChild(parent, gameObjectName).GetComponent<TextMeshProUGUI>();
            textObject.text = text;
            textObject.color = colour;
        }
    }
}