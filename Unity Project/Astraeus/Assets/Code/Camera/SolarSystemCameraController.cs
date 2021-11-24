namespace Code.Camera {
    public class SolarSystemCameraController : CameraController {
        public override void ToggleCameraControl() {
            FindObjectOfType<GalaxyCameraController>().enabled = true;
            FindObjectOfType<SolarSystemCameraController>().enabled = false;
        }

        public override void SetupCamera(float x2, float y2) {
            float boundsScale = 1.2f;
            float distance = x2 * boundsScale;
            SetupCamera(-distance, -distance,distance, distance, 2500, 1000, -5, 100, 20000, 5, 1500);
        }
    }
}