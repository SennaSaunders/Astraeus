using System;
using Code._GameControllers;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Weapons {
    public class Projectile : MonoBehaviour {
        private GameObject _projectileObject;
        private float _damage;
        private float _maxTravelTime;
        private float _currentTravelTime;
        private Vector2 _velocity;

        private void Update() {
            Travel();
        }

        private void Travel() {
            float time = Time.deltaTime;
            Vector2 travelVector = _velocity * time;
            _projectileObject.transform.Translate(travelVector, Space.World);
            _currentTravelTime += time;
            MaxDistanceCheck();
        }

        public void Spawn(Weapon weapon, Vector2 shipVelocity, float damage) {
            _projectileObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(weapon.GetProjectilePath()));
            Rigidbody projectileRigidbody = _projectileObject.AddComponent<Rigidbody>();
            projectileRigidbody.useGravity = false;
            projectileRigidbody.isKinematic = false;
            CapsuleCollider capsuleCollider = _projectileObject.AddComponent<CapsuleCollider>();
            capsuleCollider.isTrigger = true;
            
            Transform projectileTransform = transform;
            _projectileObject.transform.position = projectileTransform.position;
            _projectileObject.transform.rotation = projectileTransform.rotation;
            _currentTravelTime = 0;
            _maxTravelTime = weapon.MaxTravelTime;
            _damage = damage;
            SetProjectileVelocity(shipVelocity, weapon.ProjectileSpeed);
        }

        private void SetProjectileVelocity(Vector2 shipVelocity, float projectileSpeed) {
            Quaternion rotVec = Quaternion.Euler(0,0,transform.rotation.eulerAngles.z);
            Vector2 up = rotVec * new Vector2(0,projectileSpeed);
            _velocity = shipVelocity + up;
        }

        private void DealDamage() {
            //check for collisions with ships that aren't the ship that fired it
            //set hostile status with ships that aren't currently hostile
            //deals damage to hit ship
            Debug.Log("Dealing damage: " + _damage);
        }

        private void OnTriggerEnter(Collider hitCollider) {
            DealDamage();
            Debug.Log(hitCollider.gameObject.name);
        }

        private void MaxDistanceCheck() {
            if (_currentTravelTime > _maxTravelTime) {
                DestroySelf();
            }
        }

        private void DestroySelf() {
            Destroy(_projectileObject);
            Destroy(this);
        }
    }
}