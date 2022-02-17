using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.GUI.Utility {
    public class ShipRotatable :MonoBehaviour, IDragHandler {
        public GameObject RotatableObject { private get; set; }
        public void OnDrag(PointerEventData eventData) {
            Vector2 movement = eventData.delta;
            Vector3 oldRotation = RotatableObject.transform.rotation.eulerAngles;
            Vector3 newRotation = new Vector3(oldRotation.x, oldRotation.y - movement.x, oldRotation.z);
            
            RotatableObject.transform.rotation = Quaternion.Euler(newRotation);
        }
    }
}