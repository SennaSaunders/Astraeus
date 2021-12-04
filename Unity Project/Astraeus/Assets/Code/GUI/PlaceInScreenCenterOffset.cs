using UnityEngine;

namespace Code.GUI {
    public class PlaceInScreenCenterOffset : MonoBehaviour {
        public float xOffset =0;
        public float yOffset=0;
        public float zOffset=0;
        private void Start() {
            UnityEngine.Camera camera = UnityEngine.Camera.main;
            Vector3 cameraCenter = camera.ScreenToWorldPoint(new Vector3((float)camera.pixelWidth/2,(float)camera.pixelHeight/2,camera.nearClipPlane));
            Vector3 adjustedPos = new Vector3(cameraCenter.x + xOffset, cameraCenter.y+yOffset, cameraCenter.z+zOffset);
            transform.position = adjustedPos;
        }
    }
}
