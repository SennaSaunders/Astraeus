using UnityEngine;

namespace Code._Utility {
    public static class CameraUtility {
        private static UnityEngine.Camera _camera;

        private static void SetCamera() {
            if (_camera == null) {
                _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            }
        }
        public static void SolidSkybox() {
            SetCamera();
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = Color.black;
        }

        public static void NormalSkybox() {
            SetCamera();
            _camera.clearFlags = CameraClearFlags.Skybox;
        }

        public static void ChangeCullingMask(LayerMask layerMask) {
            SetCamera();
            _camera.cullingMask = layerMask;
        }
    }
}