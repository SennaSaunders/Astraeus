using System;
using Code._Utility;
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

        public void Spawn(GameObject projectileObject,Vector2 shipVelocity, float projectileSpeed, float maxTravelDistance, float damage) {
            _projectileObject = projectileObject;
            _projectileObject.transform.position = transform.position;
            _projectileObject.transform.rotation = transform.rotation;
            _currentTravelTime = 0;
            _maxTravelTime = maxTravelDistance;
            _damage = damage;
            SetProjectileVelocity(shipVelocity, projectileSpeed);
        }

        private void SetProjectileVelocity(Vector2 shipVelocity, float projectileSpeed) {
            Quaternion rotVec = Quaternion.Euler(0,0,transform.rotation.eulerAngles.z);
            Vector2 up = rotVec * new Vector2(0,projectileSpeed);
            _velocity = shipVelocity + up;
            Debug.Log(_velocity);
        }

        private void DealDamage() {
            
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