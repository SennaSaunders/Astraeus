using System;
using System.Linq;
using Code._GameControllers;
using Code._Ships.Controllers;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Weapons {
    public class Projectile : MonoBehaviour {
        private float _damage;
        private float _maxTravelTime;
        private float _currentTravelTime;
        private GameObject _shipObject;

        public void Spawn(Weapon weapon, Vector2 shipVelocity, float damage, GameObject shipObject) {
            _shipObject = shipObject;
            Rigidbody projectileRigidbody = gameObject.AddComponent<Rigidbody>();
            projectileRigidbody.useGravity = false;
            projectileRigidbody.isKinematic = false;
            gameObject.AddComponent<CapsuleCollider>();
            
            _currentTravelTime = 0;
            _maxTravelTime = weapon.MaxTravelTime;
            _damage = damage;
            projectileRigidbody.velocity = GetProjectileVelocity(shipVelocity, weapon.ProjectileSpeed);;
        }

        private void Update() {
            if (GameController.isPaused) {
                gameObject.GetComponent<Rigidbody>().Sleep();
            }
        }

        private Vector2 GetProjectileVelocity(Vector2 shipVelocity, float projectileSpeed) {
            Quaternion rotVec = Quaternion.Euler(0,0,transform.rotation.eulerAngles.z);
            Vector2 up = rotVec * new Vector2(0,projectileSpeed);
            return shipVelocity + up;
        }

        private void DealDamage(ShipController shipController) {
            //check for collisions with ships that aren't the ship that fired it
            //set hostile status with ships that aren't currently hostile
            //deals damage to hit ship
            // Debug.Log("Dealing damage: " + _damage);
            shipController.TakeDamage(_damage);
            Debug.Log("Dealing damage");
        }

        private void OnCollisionEnter(Collision collision) {
            if ( !_shipObject.GetComponents<Collider>().Contains(collision.collider)) {
                ShipController shipController = collision.gameObject.GetComponent<ShipController>();
                if (shipController!=null) {
                    DealDamage(shipController);
                    DestroySelf();
                }
            }
        }

        private void MaxDistanceCheck() {
            if (_currentTravelTime > _maxTravelTime) {
                DestroySelf();
            }
        }

        private void DestroySelf() {
            Destroy(gameObject);
            Destroy(this);
        }
    }
}