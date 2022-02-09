using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Camera {
    public abstract class CameraController : MonoBehaviour {
        public void TakeCameraControl() {
            List<CameraController> cameraControllers = FindObjectsOfType<CameraController>().ToList();
            foreach (CameraController cameraController in cameraControllers) {
                if (cameraController != this) {
                    cameraController.enabled = false;
                }
                else {
                    cameraController.enabled = true;
                }
            }
        }
    }
}