using System;
using Code._GameControllers;
using Code._Utility;
using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects {
    public class PlayerBodyProximity : MonoBehaviour {
        private Delegate _proximityFunction;
        private Body _param;
        private float _distance;
        private KeyCode _interactKey;
        private GameObject _interactGUI;
        private string _guiPath;
        private GameController _gameController;


        public void Setup(KeyCode interactKey, string prefabPath) {
            _gameController = GameObjectHelper.GetGameController();
            _interactKey = interactKey;
            _guiPath = prefabPath;
        }
        
        private void Update() {
            ProximityCheck();
        }

        private void ProximityCheck() {
            if (!_gameController.IsPaused) {
                if (_gameController.CurrentShip != null) {
                    if (_gameController.CurrentShip.ShipObject != null) {
                        Vector2 distanceBetween = (Vector2)transform.position - (Vector2)_gameController.CurrentShip.ShipObject.transform.position;
                        if (distanceBetween.magnitude < _distance) {
                            if (_interactGUI == null) {
                                _interactGUI = Instantiate((GameObject)Resources.Load(_guiPath), _gameController.CurrentShip.ShipObject.transform);
                            }
                            if (Input.GetKey(_interactKey)) {
                                _proximityFunction.DynamicInvoke(_param);
                            }
                        }else if (_interactGUI != null) {
                            Destroy(_interactGUI);
                        }
                    }
                }
            }else if (_interactGUI != null) {
                Destroy(_interactGUI);
            }
        }

        public void SetProximityFunction<T>(Action<T> proximityFunction, Body bodyParam) {
            _proximityFunction = proximityFunction;
            _param = bodyParam;
            _distance = bodyParam.Tier.InteractDistance();
        }
    }
}