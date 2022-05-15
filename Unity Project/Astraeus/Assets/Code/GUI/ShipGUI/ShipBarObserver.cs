using Code.GUI.ObserverPattern;
using UnityEngine;

namespace Code.GUI.ShipGUI {
    public class ShipBarObserver : MonoBehaviour, IItemObserver<float> {
        private float _areaWidth;
        

        private void Awake() {
            _areaWidth = gameObject.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
            
        }

        public void UpdateSelf(float value) {
            float barWidth = value * _areaWidth;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(barWidth, rectTransform.sizeDelta.y);
        }
    }
}