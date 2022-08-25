using System;
using System.Collections.Generic;
using System.Linq;
using Code._GameControllers;
using Code._Ships.Controllers;
using Code._Utility;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Weapons {
    public class Projectile : MonoBehaviour {
        private float _damage;
        private float _maxTravelTime;
        private float _currentTravelTime;
        private GameObject _shipObject;
        private Vector2 _velocity;
        private bool _needsRefresh = false;
        private GameController _gameController;

        public void Awake() {
            _gameController = GameObjectHelper.GetGameController();
        }

        public void Spawn(Weapon weapon, Vector2 shipVelocity, float damage, GameObject shipObject) {
            gameObject.layer = LayerMask.NameToLayer("Projectile");
            _shipObject = shipObject;
            Rigidbody projectileRigidbody = gameObject.AddComponent<Rigidbody>();
            projectileRigidbody.useGravity = false;
            projectileRigidbody.isKinematic = false;
            CapsuleCollider projectileCollider = gameObject.AddComponent<CapsuleCollider>();
            List<Collider> shipColliders = _shipObject.GetComponents(typeof(Collider)).Cast<Collider>().ToList();
            foreach (Collider shipCollider in shipColliders) {
                Physics.IgnoreCollision(projectileCollider, shipCollider);
            }

            _currentTravelTime = 0;
            _maxTravelTime = weapon.MaxTravelTime;
            _damage = damage;
            projectileRigidbody.velocity = GetProjectileVelocity(shipVelocity, weapon.ProjectileSpeed);
            ;
        }

        private void Update() {
            if (_gameController.IsPaused) {
                Rigidbody projectileRigidbody = gameObject.GetComponent<Rigidbody>();
                if (!_needsRefresh) {
                    _velocity = projectileRigidbody.velocity;
                }
                projectileRigidbody.Sleep();
                _needsRefresh = true;
            }
            else if (_needsRefresh) {
                Rigidbody projectileRigidbody = gameObject.GetComponent<Rigidbody>();
                projectileRigidbody.velocity = _velocity;
                // projectileRigidbody.WakeUp();
                _needsRefresh = false;
            }
            else {
                MaxDistanceCheck();
            }
        }

        private Vector2 GetProjectileVelocity(Vector2 shipVelocity, float projectileSpeed) {
            Quaternion rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
            Vector2 firingVector = rotation * new Vector2(0, projectileSpeed);
            return shipVelocity + firingVector;
        }

        private void DealDamage(ShipController hitShipController) {
            //if firing ship hasn't been destroyed
            if (_shipObject != null) { 
                //set hostile status with ships that aren't currently hostile
                ShipController firingShipController = _shipObject.GetComponent<ShipController>();
                if (!firingShipController.hostiles.Contains(hitShipController._ship)) {
                    firingShipController.hostiles.Add(hitShipController._ship);
                    ChangeShipMarkerColour(hitShipController, firingShipController);
                }

                if (!hitShipController.hostiles.Contains(firingShipController._ship)) {
                    hitShipController.hostiles.Add(firingShipController._ship);
                    ChangeShipMarkerColour(hitShipController, firingShipController);
                }
            }
            
            hitShipController.TakeDamage(_damage);
            Debug.Log("Dealing damage");
        }

        private void ChangeShipMarkerColour(ShipController hitShipController, ShipController firingShipController) {
            if (hitShipController.GetType() == typeof(PlayerShipController)) {
                ChangeShipMarkerHostile(firingShipController._ship.ShipObject);
            }
            if (firingShipController.GetType() == typeof(PlayerShipController)) {
                ChangeShipMarkerHostile(hitShipController._ship.ShipObject);
            }
        }

        private void ChangeShipMarkerHostile(GameObject shipObject) {
            GameObject shipMarker = GameObjectHelper.FindChild(shipObject, "ShipMarker");
            shipMarker.GetComponent<SpriteRenderer>().color = Color.red;
        }

        private void OnCollisionEnter(Collision collision) {
            bool hitSelf = false;
            //prevents missing ref exception if the firing ship has been destroyed before the projectile
            if (_shipObject != null) {
                if (_shipObject.GetComponentsInChildren<Collider>().Contains(collision.collider)) {
                    hitSelf = true;
                }
            }
            if (!hitSelf) {
                ShipController shipController = collision.gameObject.GetComponentInParent<ShipController>();
                if (shipController != null) {
                    DealDamage(shipController);
                    DestroySelf();
                }
            }
        }

        private void MaxDistanceCheck() {
            if (_currentTravelTime > _maxTravelTime) {
                DestroySelf();
            }
            else {
                _currentTravelTime += Time.deltaTime;
            }
        }

        private void DestroySelf() {
            Destroy(gameObject);
            Destroy(this);
        }
    }
}