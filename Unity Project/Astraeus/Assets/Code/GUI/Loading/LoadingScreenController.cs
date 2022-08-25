using System;
using Code._GameControllers;
using Code._Utility;
using TMPro;
using UnityEngine;

namespace Code.GUI.Loading {
    public class LoadingScreenController : MonoBehaviour {
        private GameObject _loadingIcon;
        private UnityEngine.Camera _camera;
        private Action _loadedFunction;
        private GameController _gameController;

        private void Awake() {
            _gameController = GameObjectHelper.GetGameController();
            _loadingIcon = GameObject.Find("LoadingIcon");
        }

        private void Update() {
            _loadingIcon.transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime, Space.World);
        }

        private static void SetLoadingText(string text) {
            GameObject loadingTextObj = GameObject.Find("LoadingTxt");
            loadingTextObj.GetComponent<TextMeshProUGUI>().text = text;
        }

        public void StartLoadingScreen(string loadMsg, Action loadedFunction) {
            _gameController.IsPaused = true;
            if (loadedFunction != null) {
                _loadedFunction = loadedFunction;
            }

            if (_gameController.GUIController != null) {
                _gameController.GUIController.SetShipGUIActive(false);
            }

            CameraUtility.SolidSkybox();
            CameraUtility.ChangeCullingMask(1 << LayerMask.NameToLayer("Loading"));
            gameObject.SetActive(true);
            SetLoadingText(loadMsg);
        }

        public void FinishedLoading() {
            CameraUtility.ChangeCullingMask(GameController.DefaultGameMask);
            CameraUtility.NormalSkybox();
            _gameController.IsPaused = false;
            if (_loadedFunction != null) {
                _loadedFunction.Invoke();
                _loadedFunction = null;
            }

            gameObject.SetActive(false);
        }
    }
}