using System;
using Code._GameControllers;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Utility;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Weapons {
    public class WeaponController : MonoBehaviour {//should be individual weapon not a list of weapons
        public Weapon ControlledWeapon { get; private set; } //holds the Weapons from ship components with their fire group
        private PowerPlantController _powerPlantController;
        private float _fireCooldownTime = 0;
        private bool _flipRotation = false;

        public void Setup(Weapon weapon, PowerPlantController powerPlantController) {
            ControlledWeapon = weapon;
            _powerPlantController = powerPlantController;
            float epsilon = 1; 
            _flipRotation = Math.Abs(Math.Abs(ControlledWeapon.InstantiatedGameObject.transform.parent.transform.parent.localRotation.eulerAngles.y) - 180)<epsilon;
        }

        private void Update() {
            _fireCooldownTime = _fireCooldownTime < 0 ? 0 : _fireCooldownTime -= Time.deltaTime;
        }

        //fire
        public void Fire(Vector2 velocityOffset) {
            //fire timings
            if (_fireCooldownTime == 0) {
                float powerDrawEffectiveness = _powerPlantController.DrainPower(ControlledWeapon.PowerDraw);
                if (powerDrawEffectiveness > 0) { //only fires if there is power to expend
                    float damage = ControlledWeapon.Damage * powerDrawEffectiveness;
                    GameObject projectileSpawnPoint = FindChildGameObject.FindChild(ControlledWeapon.InstantiatedGameObject, "ProjectileSpawn");
                    Projectile projectile = projectileSpawnPoint.AddComponent<Projectile>();
                    projectile.transform.position = projectileSpawnPoint.transform.position;
                    Quaternion turretRotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
                    projectile.transform.rotation = _flipRotation ? Quaternion.Inverse(turretRotation):turretRotation;
                    projectile.Spawn(ControlledWeapon, velocityOffset, damage);
                    _fireCooldownTime = ControlledWeapon.FireDelay;
                }
            }
        }

        public void TurnWeapon(Vector2 target, Quaternion shipRotation) {
            Vector2 startingVector = shipRotation *Vector2.up;//need to pass this in from the rotation of the turret
            Vector2 relativeTarget = target - (Vector2)transform.position;
            float angle = Vector2.SignedAngle(startingVector, relativeTarget);
            angle = _flipRotation ? -angle : angle;
            //-angle is for the correct way up and angle does upside down turrets, need to find a way of flipping the angle if the turret is rotated upside down
            Quaternion localRotation = transform.localRotation;
            Quaternion quaternion = Quaternion.Euler(localRotation.x, localRotation.y, angle);
            transform.localRotation = Quaternion.RotateTowards(localRotation, quaternion, ControlledWeapon.RotationSpeed);
        }
    }
}