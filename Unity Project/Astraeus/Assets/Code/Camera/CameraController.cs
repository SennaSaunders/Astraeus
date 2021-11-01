using System;
using UnityEngine;

namespace Code.Camera {
    public class CameraController : MonoBehaviour {
        //galaxy map controller
        //solar system map controller
        //shared features
        //different features
        private static float _x;
        private static float _y;
        private static float _z;

        private static float _width;
        private static float _height;
        private static float _maxDistance;
        private static float _minDistance = -30;

        public static void SetupCameraBounds(float width, float height) {
            _width = width;
            _height = height;
            float hFov = UnityEngine.Camera.VerticalToHorizontalFieldOfView(UnityEngine.Camera.main.fieldOfView, UnityEngine.Camera.main.aspect);
            float vFov = UnityEngine.Camera.main.fieldOfView;
            double a1 = width / Math.Sin(hFov * Math.PI / 180) * Math.Sin((180 - hFov) / 2 * Math.PI / 180);
            double a2 = height / Math.Sin(vFov * Math.PI / 180) * Math.Sin((180 - vFov) / 2 * Math.PI / 180);
            float b1 = width / 2;
            float b2 = height / 2;
            float c1 = -(float)Math.Sqrt(a1 * a1 - b1 * b1);
            float c2 = -(float)Math.Sqrt(a2 * a2 - b2 * b2);
            _maxDistance = c1 < c2 ? c1 : c2;
            _x = _width / 2;
            _y = _height / 2;
            _z = _maxDistance;
        }

        void Start() {
            SetCamera();
        }

        void LateUpdate() {
            SetCamera();
        }

        public void SetCamera() {
            transform.position = new Vector3(_x, _y, _z);
        }

        private void MoveCamera(Vector3 vec) {
            transform.Translate(vec);
        }

        private void Zoom() {
            //zoom speed is not linear
            //zoom ratio equivalent to: 0 - Pi/2
            float zoomPercentage = (_z - _minDistance) / (_maxDistance - _minDistance);
            
        }
    }
}