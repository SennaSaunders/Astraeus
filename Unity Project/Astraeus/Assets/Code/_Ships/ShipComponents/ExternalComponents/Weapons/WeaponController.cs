using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Weapons {
    public class WeaponController : MonoBehaviour {//should be individual weapon not a list of weapons
        private Weapon _weapon; //holds the Weapons from ship components with their fire group

        public void Setup(Weapon weapon) {
            _weapon = weapon;
        }

        //fire
        public void Fire() {
            //get weapon projectile type
            //spawn it
            //set projectile velocity to ship velocity + firing angle/projectile speed vector
        }

        public void TurnWeapon(Vector2 target, Quaternion shipRotation) {
            Vector2 startingVector = shipRotation *Vector2.up;//need to pass this in from the rotation of the turret 
            float maxRotationSpeed = 1;//needs to be pulled from the weapon
            Vector2 relativeTarget = target - (Vector2)transform.position;
            float angle = Vector2.SignedAngle(startingVector, relativeTarget);
            //need to flip the rotation if it's upside down
            Quaternion turretOrientation = transform.rotation;
            // turretOrientation = Quaternion.Euler(0,turretOrientation.y, 0);
            Debug.Log(turretOrientation);
            Quaternion angleQuaternion = turretOrientation * Quaternion.Euler(0,0, angle);
            //-angle is for the correct way up and angle does upside down turrets, need to find a way of flipping the angle if the turret is rotated upside down
            
            Quaternion quaternion = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, -angle);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, quaternion, maxRotationSpeed);
        }
    }
}