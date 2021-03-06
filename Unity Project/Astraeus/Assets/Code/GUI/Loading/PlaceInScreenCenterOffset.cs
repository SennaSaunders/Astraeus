using UnityEngine;

namespace Code.GUI.Loading {
    public class PlaceInScreenCenterOffset : MonoBehaviour {
        public float xOffset;
        public float yOffset;
        public float zOffset;
        private UnityEngine.Camera mainCamera;
        private void Start() {
            mainCamera = UnityEngine.Camera.main;
            Center();
        }

        private void Update() {
            Center();
        }

        private void Center() {
            if (mainCamera != null) {
                Vector3 cameraCenter = mainCamera.ScreenToWorldPoint(new Vector3((float)mainCamera.pixelWidth/2,(float)mainCamera.pixelHeight/2,mainCamera.nearClipPlane));
                Vector3 adjustedPos = new Vector3(cameraCenter.x + xOffset, cameraCenter.y+yOffset, cameraCenter.z+zOffset);
                transform.position = adjustedPos;
            }
        }
    }
}
