using Code._Galaxy._SolarSystem;
using UnityEngine;

namespace Code.Camera {
    public class OutfittingCameraController : CameraController {
        public const int ZOffset = -SolarSystemController.ZOffset - 3000;

        public void SetCameraPos() {
            transform.position = new Vector3(0, 0, ZOffset);
            transform.rotation = Quaternion.Euler(0,0,0);
        }
    }
}