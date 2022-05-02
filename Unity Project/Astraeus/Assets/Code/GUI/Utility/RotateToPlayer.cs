using Code._GameControllers;
using UnityEngine;

namespace Code.GUI.Utility {
    public class RotateToPlayer : MonoBehaviour {
        private void Update() {
            Rotate();
        }

        private void Rotate() {
            GameObject playerGameObject = GameController.CurrentShip.ShipObject;
            if (playerGameObject != null) {
                gameObject.transform.rotation = playerGameObject.transform.rotation;
            }
        }
    }
}