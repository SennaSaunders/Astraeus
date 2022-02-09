using Code._Galaxy._SolarSystem;
using UnityEngine;

namespace Code.Camera {
    public class OutfittingCameraController : CameraController {
        public const int zOffset = -SolarSystemController.ZOffset - 3000;

        public void SetCameraPos() {
            transform.position = new Vector3(0, 0, zOffset);
        }
    }
}