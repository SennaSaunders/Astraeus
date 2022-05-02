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
        private GameObject _shipObject;
        public void Setup(Weapon weapon, PowerPlantController powerPlantController, GameObject shipObject) {//passing the ship object so projectiles can ignore collisions with itself
            ControlledWeapon = weapon;
            _powerPlantController = powerPlantController;
            _shipObject = shipObject;
            float epsilon = 1; 
            _flipRotation = !(Math.Abs(Math.Abs(ControlledWeapon.InstantiatedGameObject.transform.parent.transform.parent.localRotation.eulerAngles.y) - 180) < epsilon);
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
                    GameObject projectileSpawnPoint = GameObjectHelper.FindChild(ControlledWeapon.InstantiatedGameObject, "ProjectileSpawn");
                    GameObject projectileObject = Instantiate((GameObject)Resources.Load(ControlledWeapon.GetProjectilePath()), GameObject.Find("ProjectileHolder").transform);
                    projectileObject.transform.position = projectileSpawnPoint.transform.position;
                    Quaternion turretRotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
                    projectileObject.transform.rotation = _flipRotation ? Quaternion.Inverse(turretRotation):turretRotation;
                    Projectile projectile = projectileObject.AddComponent<Projectile>();
                    
                    projectile.Spawn(ControlledWeapon, velocityOffset, damage, _shipObject);
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