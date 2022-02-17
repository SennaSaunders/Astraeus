using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Weapons {
    public class WeaponController : MonoBehaviour {//should be individual weapon not a list of weapons
        private Weapon _weapon; //holds the Weapons from ship components with their fire group
        private float rotation = 0;
        
        public WeaponController(Weapon weapon) {
            _weapon = weapon;
        }

        //fire
        public void Fire() {
            //get weapon projectile type
            //spawn it
            //set projectile velocity to ship velocity + firing angle/projectile speed vector
        }

        private void Update() {
            TurnWeapon();
        }

        public void TurnWeapon() {
            Vector3 startingVector = Vector3.right;//need to pass this in from the rotation of the turret 
            float maxRotationSpeed = 1;//needs to be pulled from the weapon
            Vector3 targetVector = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition).direction;
            float angle = Vector2.SignedAngle(startingVector, targetVector);
            Quaternion target = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, -angle);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, target, maxRotationSpeed);
        }
    }
}