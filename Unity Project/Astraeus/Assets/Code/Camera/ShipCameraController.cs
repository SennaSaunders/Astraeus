using UnityEngine;

namespace Code.Camera {
    public class ShipCameraController : CameraController {
        private UnityEngine.Camera _camera;

        private float _maxZOffset = -1000; //camera is furthest from ship
        private float _maxZ;
        
        private float _minZOffset = -50; //camera is closest to ship - fully zoomed in
        private float _minZ;

        private float _currentZ;
        private float _shipZ;
        
        private float _scrollRate = 10;

        private void Awake() {
            _camera = UnityEngine.Camera.main;
            _shipZ = transform.position.z;
            _minZ = _shipZ + _minZOffset;
            _maxZ = _shipZ + _maxZOffset;

            _currentZ = _minZ;
            
        }

        //attached to ship and follows it
        private void Update() {
            FollowShip();
            ControlZoom();
        }

        private void FollowShip() {
            Transform shipTransform = gameObject.transform;
            Transform cameraTransform = _camera.transform;
            Vector3 shipPos = shipTransform.position;
            cameraTransform.position = new Vector3(shipPos.x, shipPos.y, _currentZ);
            
            Quaternion shipRotation = shipTransform.rotation;
            Quaternion camRotation = Quaternion.Euler(shipRotation.eulerAngles.x, shipRotation.eulerAngles.y, shipRotation.eulerAngles.z);
            cameraTransform.rotation = camRotation;
        }

        private void ControlZoom() {
            //change currentZ
            float scrollDelta = Input.mouseScrollDelta.y;//positive == up == zoom in, negative == down == zoom out
            _currentZ += scrollDelta * _scrollRate;
            
            
            
            //ensure the zoom does not go out of bounds
            _currentZ = _currentZ > _minZ ? _minZ : _currentZ < _maxZ ? _maxZ : _currentZ;
        }
    }
}