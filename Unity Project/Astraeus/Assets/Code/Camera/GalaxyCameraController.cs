namespace Code.Camera {
    public class GalaxyCameraController : CameraController {
        public override void ToggleCameraControl() {
            FindObjectOfType<SolarSystemCameraController>().enabled = true;
            FindObjectOfType<GalaxyCameraController>().enabled = false;
        }

        public override void SetupCamera(float x2, float y2) {
            SetupCamera(0, 0, x2, y2, 0, -1900, -30, 1000, 7500, 30, 500);
        }
    }
}