using UnityEngine;

namespace Code._Ships.Controllers {
    public class NPCShipController : ShipController {
        //translates destination to thrust vectors and rotation angles
        public override void AimWeapon(Vector2 target) {
            throw new System.NotImplementedException();
        }

        public override Vector2 GetThrustVector() {
            throw new System.NotImplementedException();
        }

        public override float GetTurnDirection() {
            throw new System.NotImplementedException();
        }
    }
}