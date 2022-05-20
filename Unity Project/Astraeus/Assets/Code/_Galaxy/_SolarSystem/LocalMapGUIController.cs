using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.BlackHole;
using Code._Galaxy._SolarSystem._CelestialObjects.Star;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations;
using Code._GameControllers;
using Code._Utility;
using Code.Camera;
using Code.TextureGen;
using Unity.VectorGraphics;
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
            SetupKey();
            SetupExitButton();
            GameController.GUIController.SetShipGUIActive(false);
        }

        private void SetupKey() {
            string bodyIconPath = "Icons/BodyMapIcon"; 
            //star
            Sprite bodySprite = ((GameObject)Resources.Load(bodyIconPath)).GetComponent<SpriteRenderer>().sprite;
            SVGImage starImg = GameObjectHelper.FindChild(_guiGameObject, "StarImg").GetComponent<SVGImage>();
            starImg.sprite = bodySprite;
            Star star = new Star(null, 0);
            starImg.color = star.MapColour;

            //black hole
            SVGImage blackHoleImg = GameObjectHelper.FindChild(_guiGameObject, "BlackHoleImg").GetComponent<SVGImage>();
            blackHoleImg.sprite = bodySprite;
            BlackHole blackHole = new BlackHole(null,new Vector2());
            blackHoleImg.color = blackHole.MapColour;
            
            //rocky
            SVGImage rockyImg = GameObjectHelper.FindChild(_guiGameObject, "RockyImg").GetComponent<SVGImage>();
            rockyImg.sprite = bodySprite;
            rockyImg.color = new RockyWorldGen(0, 0).MapColour;
            //earth
            SVGImage earthImg = GameObjectHelper.FindChild(_guiGameObject, "EarthImg").GetComponent<SVGImage>();
            earthImg.sprite = bodySprite;
            earthImg.color = new EarthWorldGen(0, 0).MapColour;;
            //water
            SVGImage waterImg = GameObjectHelper.FindChild(_guiGameObject, "WaterImg").GetComponent<SVGImage>();
            waterImg.sprite = bodySprite;
            waterImg.color = new WaterWorldGen(0,0).MapColour;
            
            //space station
            string spaceStationIconPath = "Icons/SpaceStationMapIcon";
            Sprite spaceStationSprite = ((GameObject)Resources.Load(spaceStationIconPath)).GetComponent<SpriteRenderer>().sprite;
            SVGImage spaceStationImg = GameObjectHelper.FindChild(_guiGameObject, "SpaceStationImg").GetComponent<SVGImage>();
            spaceStationImg.sprite = spaceStationSprite;
            spaceStationImg.color = new SpaceStation(null, null).MapColour;
            
            //ships
            string shipMarkerPath = "Icons/ShipMarkerIcon";
            Sprite shipmarkerSprite = ((GameObject)Resources.Load(shipMarkerPath)).GetComponent<SpriteRenderer>().sprite;
            
            //player
            SVGImage playerImg = GameObjectHelper.FindChild(_guiGameObject, "PlayerImg").GetComponent<SVGImage>();
            playerImg.sprite = shipmarkerSprite;
            playerImg.color = Color.green;
            //neutral
            SVGImage neutralImg = GameObjectHelper.FindChild(_guiGameObject, "NeutralImg").GetComponent<SVGImage>();
            neutralImg.sprite = shipmarkerSprite;
            neutralImg.color = Color.blue;
            //hostile
            SVGImage hostileImg = GameObjectHelper.FindChild(_guiGameObject, "HostileImg").GetComponent<SVGImage>();
            hostileImg.sprite = shipmarkerSprite;
            hostileImg.color = Color.red;
        }

        private void SetupExitButton() {
            GameObjectHelper.FindChild(_guiGameObject, "ExitBtn").GetComponent<Button>().onClick.AddListener(Exit);
        }

        private void Exit() {
            _shipCameraController.enabled = true;
            _camera.farClipPlane = tempFarClip;
            _camera.cullingMask = tempMask;
            GameController.GUIController.SetShipGUIActive(true);
            GameController.InLocalMap = false;
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
            foreach (Body body in GameController.GalaxyController.activeSystemController.SolarSystem.Bodies) {
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
            
            float xChange = 0;
            
            float yChange = 0;
            
            int moveScale = 100;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                yChange += moveScale;
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                yChange -= moveScale;
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                xChange -= moveScale;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                xChange += moveScale;
            }

            
            var moveVec = new Vector3(xChange, yChange, 0);
            Quaternion camRotation = Quaternion.Euler(0,0,_shipCameraController.transform.rotation.eulerAngles.z);
            moveVec = camRotation * moveVec;
            
            float x = position.x+moveVec.x;
            float y = position.y+moveVec.y;
            float z = Input.mouseScrollDelta.y * scrollRate + _camTransform.position.z;
            
            x = x < x2 ? x > x1 ? x : x1 : x2;
            y = y < y2 ? y > y1 ? y : y1 : y2;
            z = z < maxZoomOut ? maxZoomOut : z > maxZoomIn ? maxZoomIn : z;
            
            _camTransform.position = new Vector3(x,y,z);
        }
    }
}