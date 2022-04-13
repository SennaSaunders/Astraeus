using System;
using Code._GameControllers;
using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects {
    public class PlayerBodyProximity : MonoBehaviour {
        private Delegate _proximityFunction;
        private Body _param;
        private float _distance;

        private void Update() {
            ProximityCheck();
        }

        private void ProximityCheck() {
            Vector2 distanceBetween = (Vector2)transform.position - (Vector2)GameController.CurrentShip.ShipObject.transform.position;
            
            if (distanceBetween.magnitude < _distance) {
                if (Input.GetKeyDown(KeyCode.F)) {
                    _proximityFunction.DynamicInvoke(_param);
                }
            }
        }

        public void SetCollisionFunction<T>(Action<T> proximityFunction, Body bodyParam) {
            _proximityFunction = proximityFunction;
            _param = bodyParam;
            _distance = bodyParam.Tier.InteractDistance();
        }
    }
}