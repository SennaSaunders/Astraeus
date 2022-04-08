using System;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Utility;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Weapons {
    public class WeaponController : MonoBehaviour {//should be individual weapon not a list of weapons
        private Weapon _weapon; //holds the Weapons from ship components with their fire group
        private PowerPlantController _powerPlantController;
        private float fireCooldownTime = 0;
        private bool flipRotation = false;

        private PrefabHandler _prefabHandler;

        public void Setup(Weapon weapon, PowerPlantController powerPlantController) {
            _weapon = weapon;
            _powerPlantController = powerPlantController;
            _prefabHandler = gameObject.AddComponent<PrefabHandler>();
            float epsilon = 1; 
            flipRotation = Math.Abs(Math.Abs(_weapon.InstantiatedGameObject.transform.parent.transform.parent.localRotation.eulerAngles.y) - 180)<epsilon;
        }

        private void Update() {
            fireCooldownTime = fireCooldownTime < 0 ? 0 : fireCooldownTime -= Time.deltaTime;
        }

        //fire
        public void Fire(Vector2 velocityOffset) {
            //fire timings
            if (fireCooldownTime == 0) {
                float powerDrawEffectiveness = _powerPlantController.DrainPower(_weapon.PowerDraw);
                if (powerDrawEffectiveness > 0) { //only fires if there is power to expend
                    float damage = _weapon.Damage * powerDrawEffectiveness;
                    GameObject projectileSpawnPoint = FindChildGameObject.FindChild(_weapon.InstantiatedGameObject, "ProjectileSpawn");
                    Projectile projectile = projectileSpawnPoint.AddComponent<Projectile>();
                    projectile.transform.position = projectileSpawnPoint.transform.position;
                    Quaternion turretRotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
                    projectile.transform.rotation = flipRotation ? Quaternion.Inverse(turretRotation):turretRotation;
                    projectile.Spawn(_prefabHandler.instantiateObject(_prefabHandler.loadPrefab(_weapon.GetProjectilePath())), velocityOffset, _weapon.ProjectileSpeed, _weapon.MaxTravelTime, damage);
                    fireCooldownTime = _weapon.FireRate;
                }
            }
            
            //get weapon projectile type
            //spawn it
            //set projectile velocity to ship velocity + firing angle/projectile speed vector
        }

        public void TurnWeapon(Vector2 target, Quaternion shipRotation) {
             
            Vector2 startingVector = shipRotation *Vector2.up;//need to pass this in from the rotation of the turret
            Vector2 relativeTarget = target - (Vector2)transform.position;
            float angle = Vector2.SignedAngle(startingVector, relativeTarget);
            angle = flipRotation ? -angle : angle;
            //-angle is for the correct way up and angle does upside down turrets, need to find a way of flipping the angle if the turret is rotated upside down
            Quaternion quaternion = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, angle);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, quaternion, _weapon.RotationSpeed);
        }
    }
}