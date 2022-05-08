using System;
using Code._GameControllers;
using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects {
    public class PlayerBodyProximity : MonoBehaviour {
        private Delegate _proximityFunction;
        private Body _param;
        private float _distance;
        private KeyCode _interactKey;
        private GameObject _interactGUI;
        private string _guiPath;

        public void Setup(KeyCode interactKey, string prefabPath) {
            _interactKey = interactKey;
            _guiPath = prefabPath;
        }
        
        private void Update() {
            ProximityCheck();
        }

        private void ProximityCheck() {
            if (!GameController.IsPaused) {
                if (GameController.CurrentShip != null) {
                    if (GameController.CurrentShip.ShipObject != null) {
                        Vector2 distanceBetween = (Vector2)transform.position - (Vector2)GameController.CurrentShip.ShipObject.transform.position;
                        if (distanceBetween.magnitude < _distance) {
                            if (_interactGUI == null) {
                                _interactGUI = Instantiate((GameObject)Resources.Load(_guiPath), GameController.CurrentShip.ShipObject.transform);
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