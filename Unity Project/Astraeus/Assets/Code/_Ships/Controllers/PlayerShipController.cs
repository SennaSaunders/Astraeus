using UnityEngine;

namespace Code._Ships.Controllers {
    public class PlayerShipController : ShipController {
        //takes user input and maps it to thrust/turn

        public override void AimWeapon(Vector2 target) {
            throw new System.NotImplementedException();
        }

        public override Vector2 GetThrustVector() {
            Vector2 forwards = Input.GetKey(KeyCode.W) ? Vector2.up : new Vector2();
            Vector2 backwards = Input.GetKey(KeyCode.S) ? Vector2.down : new Vector2();
            Vector2 left = Input.GetKey(KeyCode.Q) ? Vector2.left : new Vector2();
            Vector2 right = Input.GetKey(KeyCode.E) ? Vector2.right : new Vector2();

            return forwards + backwards + left + right;
        }

        public override float GetTurnDirection() {
            float left = Input.GetKey(KeyCode.A) ? 1 : 0;
            float right = Input.GetKey(KeyCode.D) ? -1 : 0;
            return left + right;
        }
    }
}