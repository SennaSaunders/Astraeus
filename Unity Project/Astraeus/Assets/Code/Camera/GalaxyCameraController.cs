using System;
using Code._Galaxy;
using UnityEngine;

namespace Code.Camera {
    public class GalaxyCameraController : CameraController {
        
        private float _x, _y, _z, _x1, _y1, _x2, _y2, _minMoveSpeed, _maxMoveSpeed, _minScrollSpeed, _maxScrollSpeed, _maxZoomOutDistance, _minZoomInDistance;
        private UnityEngine.Camera _camera;

        private void Awake() {
            _camera = UnityEngine.Camera.main;
        }

        private void SetupCamera(float x1, float y1, float x2, float y2, int zoomOffset, int absoluteMaxZoomOut, int minZoomInDistance, float minScrollSpeed, float maxScrollSpeed, float minMoveSpeed, float maxMoveSpeed) {
            _x1 = x1;
            _y1 = y1;
            _x2 = x2;
            _y2 = y2;
            float hFov = UnityEngine.Camera.VerticalToHorizontalFieldOfView(_camera.fieldOfView, _camera.aspect);
            float vFov = _camera.fieldOfView;
            double a1 = x2 / Math.Sin(hFov * Math.PI / 180) * Math.Sin((180 - hFov) / 2 * Math.PI / 180);
            double a2 = y2 / Math.Sin(vFov * Math.PI / 180) * Math.Sin((180 - vFov) / 2 * Math.PI / 180);
            float b1 = x2 / 2;
            float b2 = y2 / 2;
            float c1 = -(float)Math.Sqrt(a1 * a1 - b1 * b1);
            float c2 = -(float)Math.Sqrt(a2 * a2 - b2 * b2);
            _maxZoomOutDistance = c1 < c2 ? c1 : c2;
            _maxZoomOutDistance += +zoomOffset;
            _maxZoomOutDistance = _maxZoomOutDistance > absoluteMaxZoomOut ? _maxZoomOutDistance : absoluteMaxZoomOut;
            
            _minZoomInDistance = minZoomInDistance + zoomOffset;
            _minScrollSpeed = minScrollSpeed;
            _maxScrollSpeed = maxScrollSpeed;
            _minMoveSpeed = minMoveSpeed;
            _maxMoveSpeed = maxMoveSpeed;
            
            _x = (_x1 +_x2) / 2;
            _y = (_y1 + _y2) / 2;
            _z = _minZoomInDistance;
            SetCamera();
        }

        void LateUpdate() {
            MoveCamera();
            SetCamera();
        }

        public void SetCamera(Vector2 pos) {
            _x = pos.x;
            _y = pos.y;
            _z = _minZoomInDistance;
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        private void SetCamera() {
            transform.position = new Vector3(_x, _y, _z);
        }

        private void MoveCamera() {
            Up();
            Down();
            Left();
            Right();
            ZoomIn();
            ZoomOut();
        }

        private float GetRelativeSpeed(float min, float max) {
            float speed = min + GetZoomRatio() * (max - min);
            //Debug.Log(speed);
            return speed;
        }

        private void Up() {
            if (_y < _y2) {
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                    _y += GetRelativeSpeed(_minMoveSpeed, _maxMoveSpeed) * Time.deltaTime;
                }
            }
            
            if (_y>_y2){
                _y = _y2;
            }
        }

        private void Down() {
            if (_y > _y1) {
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                    _y += -GetRelativeSpeed(_minMoveSpeed, _maxMoveSpeed) * Time.deltaTime;
                }
            }
            
            if (_y<_y1){
                _y = _y1;
            }
        }

        private void Left() {
            if (_x > _x1) {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                    _x += -GetRelativeSpeed(_minMoveSpeed, _maxMoveSpeed) * Time.deltaTime;
                }
            }

            if (_x < _x1) {
                _x = _x1;
            }
        }

        private void Right() {
            if (_x < _x2) {
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                    _x += GetRelativeSpeed(_minMoveSpeed, _maxMoveSpeed) * Time.deltaTime;
                }
            }

            if (_x > _x2) {
                _x = _x2;
            }
        }

        private void ZoomIn() {
            if (_z < _minZoomInDistance) {    //looks like it's the wrong way round but isn't because z axis is inverted
                if (Input.mouseScrollDelta.y > 0) {
                    _z += GetRelativeSpeed(_minScrollSpeed, _maxScrollSpeed) * Time.deltaTime;
                }
            }

            if (_z > _minZoomInDistance) {
                _z = _minZoomInDistance;
            }
        }

        private void ZoomOut() {
            if (_z > _maxZoomOutDistance) {    //looks like it's the wrong way round but isn't because z axis is inverted
                if (Input.mouseScrollDelta.y < 0) {
                    _z += -GetRelativeSpeed(_minScrollSpeed, _maxScrollSpeed) * Time.deltaTime;
                }
            }
            
            if (_z < _maxZoomOutDistance) {
                _z = _maxZoomOutDistance;
            }
        }

        private float GetZoomRatio() {
            return (_z - _minZoomInDistance) / (_maxZoomOutDistance - _minZoomInDistance);
        }

        public void SetupCamera(float x2, float y2) {
            TakeCameraControl();
            SetupCamera(0, 0, x2, y2, 0, GalaxyController.ZOffset-400, GalaxyController.ZOffset-35, 1000, 7500, 30, 500);
        }
    }
}