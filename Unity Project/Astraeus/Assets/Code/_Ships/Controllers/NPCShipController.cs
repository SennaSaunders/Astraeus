using System;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Code._Ships.Controllers {
    public class NPCShipController : ShipController {
        //translates destination to thrust vectors and rotation angles

        public Vector2 destination;
        private Vector2 _facingVector;
        private Transform _shipTransform;
        public ShipController currentTarget;
        private NPCItineraryController _npcItineraryController;

        public override void Setup(Ship ship) {
            base.Setup(ship);
            _shipTransform = transform;
            _npcItineraryController = gameObject.AddComponent<NPCItineraryController>();
        }

        //finds travel time to calculate the point of interception
        private Vector2 GetIntercept(Vector2 shipPos, Vector2 targetPos, Vector2 targetV, float projectileSpeed) {
            Vector2 posDelta = targetPos - shipPos;
            Double a = Vector2.Dot(targetV, targetV) - projectileSpeed * projectileSpeed;
            Double b = 2 * Vector2.Dot(targetV, posDelta);
            Double c = Vector2.Dot(posDelta, posDelta);

            //solve quadratic
            Double discriminant = b * b - 4 * a * c;
            Double divisor = 2 * a;
            Double t1 = (-b - Math.Sqrt(discriminant)) / divisor;
            Double t2 = (-b + Math.Sqrt(discriminant)) / divisor;
            Double t;
            if (t1 > 0 || t2 > 0) { //there is a solution
                if (t1 > 0 && t2 > 0) { //both valid
                    t = t1 < t2 ? t1 : t2; // use smallest positive value
                }
                else {
                    t = t1 > 0 ? t1 : t2; //use only positive value
                }
            }
            else { //no solution - can't hit - use 0 - will fire directly at target & will miss
                t = 0;
            }

            Vector2 aimVec = targetPos + targetV * (float)t;
            Debug.DrawLine(shipPos, aimVec);
            return aimVec;
        }

        protected override void AimWeapons() {
            if (currentTarget != null) {
                Vector2 shipPos = transform.position;
                Vector2 targetPos = currentTarget.transform.position;
                Vector2 targetV = currentTarget.ThrusterController.Velocity;
                foreach (WeaponController weaponController in WeaponControllers) {
                    float projectileSpeed = weaponController.ControlledWeapon.ProjectileSpeed;
                    Vector2 intercept = GetIntercept(shipPos, targetPos, targetV, projectileSpeed);
                    weaponController.TurnWeapon(intercept, transform.rotation);
                }
            }
        }

        protected override void FireCheck() {
            if (currentTarget) {
                float weaponRange = 0;
                foreach (WeaponController weaponController in WeaponControllers) {
                    Weapon weapon = weaponController.ControlledWeapon;
                    weaponRange += weapon.ProjectileSpeed * weapon.MaxTravelTime;
                }

                weaponRange /= WeaponControllers.Count;

                if ((currentTarget.transform.position - transform.position).magnitude < weaponRange) {
                    FireWeapons();
                }
            }
        }

        protected override Vector2 GetThrustVector() {
            Vector2 velocity = ThrusterController.Velocity;
            Quaternion shipRotation = Quaternion.Euler(0, 0, -_shipTransform.rotation.eulerAngles.z);

            Vector2 shipRelV = shipRotation * velocity;
            Vector2 slowDownVec = shipRelV / 2 * ThrusterController.GetAcceleration();
            
            Vector2 currentPosition = transform.position;
            Vector2 distanceToDest = destination - currentPosition;
            Vector2 shipRotatedDistToDest = shipRotation * distanceToDest;
            Vector2 distVDelta = shipRotatedDistToDest - slowDownVec;
            float epsilon = .5f;
            Vector2 thrustVec = new Vector2(Math.Abs(distVDelta.x) < epsilon ? 0 : distVDelta.x / Math.Abs(distVDelta.x), Math.Abs(distVDelta.y) < epsilon ? 0 : distVDelta.y / Math.Abs(distVDelta.y));

            _facingVector = distanceToDest;

            Debug.DrawLine(currentPosition, currentPosition + distanceToDest, Color.yellow);
            Debug.DrawLine(currentPosition, currentPosition + velocity, Color.red);

            return thrustVec;
        }

        private float CalculateDisplacementTillStop(float initialVelocity, float acceleration) {
            // v^2 = u^2 + 2as
            // (v^2 - u^2)/2a = s
            //v = 0 so (-u^2)/2a = s
            // just interested in the distance so 
            // (u^2)/2a = s is sufficient
            return (initialVelocity * initialVelocity) / (2 * acceleration);
        }

        protected override float GetTurnDirection() {
            float angle = GetTurnAngle();
            float angularDisplacement = CalculateDisplacementTillStop(ThrusterController.AngularVelocity, ThrusterController.AngularAcceleration);
            float turnEpsilon = .0001f;
            float delta = Math.Abs(angle) - angularDisplacement;
            if (delta > turnEpsilon) {
                float turnDir = angle < 0 ? -1 : 1;
                return turnDir;
            }

            return 0;
        }

        private float GetTurnAngle() {
            var forwardVec = _shipTransform.TransformDirection(Vector3.up);
            return Vector2.SignedAngle(forwardVec, _facingVector);
        }
    }
}