using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._GameControllers;
using Code._Utility;
using Code.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Code._Galaxy._SolarSystem {
    public class LocalMapGUIController : MonoBehaviour {
        private float x1, x2, y1, y2;
        private GameObject _guiGameObject;
        private UnityEngine.Camera _camera;
        private Transform _camTransform;
        private ShipCameraController _shipCameraController;
        private int maxZoomIn = -15000;
        public int maxZoomOut = -30000;
        public int scrollRate = 500;
        public LayerMask tempMask;
        public float tempFarClip;

        private void SetupCamera() {
            _shipCameraController = GameController.CurrentShip.ShipObject.GetComponent<ShipCameraController>();
            _shipCameraController.enabled = false;

            _camera = GameObject.FindWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            _camTransform = _camera.gameObject.transform;
            _camTransform.rotation = Quaternion.Euler(0, 0, 0);

            tempFarClip = _camera.farClipPlane;
            _camera.farClipPlane = 40000;
            tempMask = _camera.cullingMask;
            _camera.cullingMask = 64; //local map mask only
            Vector3 shipPos = GameController.CurrentShip.ShipObject.transform.position;
            _camTransform.position = new Vector3(shipPos.x, shipPos.y, maxZoomIn);
        }

        private void Update() {
            MoveCamera();
        }

        public void SetupGUI() {
            _guiGameObject = Instantiate((GameObject)Resources.Load("GUIPrefabs/Map/LocalMapGUI"));
            SetupCamera();
            SetBounds();
            SetupExitButton();
            GameController.GUIController.SetShipGUIActive(false);
        }

        private void SetupExitButton() {
            GameObjectHelper.FindChild(_guiGameObject, "ExitBtn").GetComponent<Button>().onClick.AddListener(Exit);
        }

        private void Exit() {
            _shipCameraController.enabled = true;
            _camera.farClipPlane = tempFarClip;
            _camera.cullingMask = tempMask;
            GameController.GUIController.SetShipGUIActive(true);
            GameController.IsPaused = false;
            //destroy gui
            Destroy(_guiGameObject);
            Destroy(this);
        }

        private void SetBounds() {
            //find bounds of map
            x1 = float.MaxValue;
            x2 = float.MinValue;
            y1 = float.MaxValue;
            y2 = float.MinValue;
            foreach (Body body in GameController.GalaxyController.activeSystemController._solarSystem.Bodies) {
                var bodyPos = GameController.GalaxyController.activeSystemController.GetBodyGameObject(body).transform.position;
                float x = bodyPos.x;
                float y = bodyPos.y;
                x1 = x1 > x ? x : x1;
                y1 = y1 > y ? y : y1;
                x2 = x2 < x ? x : x2;
                y2 = y2 < y ? y : y2;
            }
        }

        private void MoveCamera() {
            var position = _camTransform.position;
            float x = position.x;
            float y = position.y;

            int moveScale = 100;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                y += moveScale;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                y -= moveScale;
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                x -= moveScale;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                x += moveScale;
            }

            float z = Input.mouseScrollDelta.y * scrollRate + _camTransform.position.z;

            x = x < x2 ? x > x1 ? x : x1 : x2;
            y = y < y2 ? y > y1 ? y : y1 : y2;
            z = z < maxZoomOut ? maxZoomOut : z > maxZoomIn ? maxZoomIn : z;

            _camTransform.position = new Vector3(x, y, z);
        }
    }
}