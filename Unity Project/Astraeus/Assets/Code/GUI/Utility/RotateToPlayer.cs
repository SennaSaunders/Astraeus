using System;
using Code._GameControllers;
using Code._Utility;
using UnityEngine;

namespace Code.GUI.Utility {
    public class RotateToPlayer : MonoBehaviour {
        private GameController _gameController;

        private void Start() {
            _gameController = GameObjectHelper.GetGameController();
        }

        private void Update() {
            Rotate();
        }

        private void Rotate() {
            GameObject playerGameObject = _gameController.CurrentShip.ShipObject;
            if (playerGameObject != null) {
                gameObject.transform.rotation = playerGameObject.transform.rotation;
            }
        }
    }
}