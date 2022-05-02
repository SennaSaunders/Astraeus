using Code._GameControllers;
using UnityEngine;

namespace Code.Camera {
    public class ShipCameraController : CameraController {
        private UnityEngine.Camera _mainCamera;
        private float _maxZOffset = -1000; //camera is furthest from ship
        private float _maxZ;
        private float _minZOffset = -50; //camera is closest to ship - fully zoomed in
        private float _minZ;
        private float _currentZ;
        private float _scrollRate = 10;
        
        private UnityEngine.Camera _miniMapCamera;
        private float _miniMapZ = -8000;
        
        private void Awake() {
            _mainCamera = UnityEngine.Camera.main;
            _minZ = GameController.ShipZ + _minZOffset;
            _maxZ = GameController.ShipZ + _maxZOffset;
            _currentZ = _minZ;
        }

        private void SetupMiniMapCam() {
            GameObject miniMapObject = GameObject.FindWithTag("MiniMapCam");
            if (miniMapObject != null) {
                _miniMapCamera = miniMapObject.GetComponent<UnityEngine.Camera>();
            }
        }

        //attached to ship and follows it
        private void Update() {
            FollowShip();
            ControlZoom();
            if (_miniMapCamera == null) {
                SetupMiniMapCam();
            }
        }

        private void FollowShip() {
            Transform shipTransform = gameObject.transform;
            Transform mainCameraTransform = _mainCamera.transform;
            Vector3 shipPos = shipTransform.position;
            mainCameraTransform.position = new Vector3(shipPos.x, shipPos.y, _currentZ);
            
            Quaternion shipRotation = shipTransform.rotation;
            Quaternion camRotation = Quaternion.Euler(shipRotation.eulerAngles.x, shipRotation.eulerAngles.y, shipRotation.eulerAngles.z);
            mainCameraTransform.rotation = camRotation;

            if (_miniMapCamera != null) {
                _miniMapCamera.transform.position = new Vector3(shipPos.x, shipPos.y, _miniMapZ);
                _miniMapCamera.transform.rotation = camRotation;
            }
        }

        private void ControlZoom() {
            if (!GameController.IsPaused) {
                //change currentZ
                float scrollDelta = Input.mouseScrollDelta.y;//positive == up == zoom in, negative == down == zoom out
                _currentZ += scrollDelta * _scrollRate;

                //ensure the zoom does not go out of bounds
                _currentZ = _currentZ > _minZ ? _minZ : _currentZ < _maxZ ? _maxZ : _currentZ;
            }
        }
    }
}