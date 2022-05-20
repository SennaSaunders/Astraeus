using UnityEngine;

namespace Code.GUI.Utility {
    public class CenterObjectToGUI:MonoBehaviour {
        public void SetGUIRect(RectTransform rectTransform) {
            _guiRect = rectTransform;
        }
        private RectTransform _guiRect;
        private void Update() {
            if (_guiRect) {
                var transformPoint = _guiRect.TransformPoint(new Vector3());
                gameObject.transform.position = transformPoint;
            }
            
        }
    }
}