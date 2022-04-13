using System;
using Code._GameControllers;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Code._Ships.Controllers {
    public class NPCShipController : ShipController {
        //translates destination to thrust vectors and rotation angles

        public Vector2 destination; //public to change in the inspector
        private Vector2 _correctionVector;
        private Transform _shipTransform;
        private ShipController _currentTarget;

        public void Setup(Ship ship, Vector2 initialDestination) {
            destination = initialDestination;
            destination = GameController.CurrentShip.ShipObject.transform.position;
            base.Setup(ship);
            _shipTransform = transform;
            SetCurrentTarget();
        }

        private ShipController GetPriorityTarget() {
            //hostile distance
            //weapon range
            //time to kill hostile
            return GameController.CurrentShip.ShipObject.GetComponent<ShipController>();//temporarily get player ship
        }

        private void SetCurrentTarget() {
            _currentTarget = GetPriorityTarget();
        }

        //  Each weapon may shoot projectiles of different velocities
        //  So may need a separate target for each gun
        private Vector2 GetIntercept(Vector2 interceptPos, float interceptSpeed, Vector2 targetPos, Vector2 targetV) {
            //find travel time
            Vector2 posDelta = targetPos - interceptPos;
            float a = Vector2.Dot(targetV, targetV) - interceptSpeed * interceptSpeed;
            float b = 2 * Vector2.Dot(targetV, posDelta);
            float c = Vector2.Dot(posDelta, posDelta);

            float t1 = (float)(-b + Math.Sqrt(Math.Abs(b * b - 4 * a * c))) / (2 * a);
            float t2 = (float)(-b - Math.Sqrt(Math.Abs(b * b - 4 * a * c))) / (2 * a);
            
            float t;
            Vector2 aimVec;
            if (t1 < 0 && t2 < 0) {
                //aim directly at ship
                aimVec = targetPos;
            } else {
                t = t1 < t2 ? t2 : t1;
                aimVec = targetPos + targetV * t;
            }
            
            // t = t1 > 0 ? t1 < t2 ? t1 : t2 : t2;
            
            Debug.DrawLine(interceptPos, aimVec);
            
            return aimVec;
        }


        public override void AimWeapons() {
            Vector2 interceptPos = transform.position;
            Vector2 targetPos = _currentTarget.transform.position;
            Vector2 targetV = _currentTarget.ThrusterController.Velocity;
            foreach (WeaponController weaponController in _weaponControllers) {
                float interceptSpeed = weaponController.ControlledWeapon.ProjectileSpeed;
                Vector2 intercept = GetIntercept(interceptPos, interceptSpeed, targetPos, targetV);
                weaponController.TurnWeapon(intercept, transform.rotation);
            }
        }

        public override void FireCheck() {
            FireWeapons();
            //is weapon aimed at target
            //is weapon within range
            //if so then fire

            // throw new NotImplementedException();
        }

        public override Vector2 GetThrustVector() {
            Vector2 currentPosition = transform.position;
            Vector2 deltaVector = destination - currentPosition; //how far is the destination from the current position
            Vector2 velocity = ThrusterController.Velocity;

            float shipRotation = _shipTransform.rotation.eulerAngles.z;


            Vector2 thrustVec = new Vector2();
            const float offCourseAngle = 90;

            float velocityRotation = Vector2.SignedAngle(Vector2.up, velocity);
            velocityRotation = velocityRotation < 0 ? velocityRotation + 360 : velocityRotation;
            float velocityRotationDiff = Math.Abs(Math.Abs(shipRotation) - Math.Abs(velocityRotation));

            float angleEpsilon = 1;

            if (velocity.magnitude > 0) {
                float driftAngle = Vector2.SignedAngle(deltaVector, velocity);
                if (Math.Abs(driftAngle) < angleEpsilon) { // Velocity on target
                    //aim at target
                    _correctionVector = deltaVector;

                    // Check if Velocity is sufficient to reach the target, turn & stop
                    // Rotate -> distance travelled while rotating -> need time of rotation
                    // Initial Angular Velocity - How far is it going to rotate with current momentum
                    float initialAngularDisplacement = CalculateDisplacementTillStop(ThrusterController.AngularVelocity, ThrusterController.AngularAcceleration); //how far will the ship turn with it's current velocity
                    float initialDisplacementTime = CalculateDisplacementTime(ThrusterController.AngularVelocity, 0, ThrusterController.AngularAcceleration);

                    // How much more does it need to rotate
                    float targetRotation = 180;
                    Vector2 slowDownVector = Quaternion.Euler(0, 0, targetRotation) * _correctionVector; // Opposite vector to destination
                    Vector2 shipFacing = Quaternion.Euler(0, 0, shipRotation) * Vector2.up;
                    float angleLeftToTravel = Vector2.Angle(shipFacing, slowDownVector); //Angular displacement between ship facing and slowDownVector
                    float angleLeftToCalculate = angleLeftToTravel - initialAngularDisplacement;
                    float finalVHalfTurn = CalculateFinalV(ThrusterController.AngularVelocity, angleLeftToCalculate / 2, ThrusterController.AngularAcceleration);
                    // How much time will this take
                    float totalTurnTime = initialDisplacementTime + CalculateDisplacementTime(ThrusterController.AngularVelocity, finalVHalfTurn, ThrusterController.AngularAcceleration) * 2;
                    float distanceTravelledTurning = totalTurnTime * velocity.magnitude;
                    float distanceToSlow = CalculateDisplacementTillStop(velocity.magnitude, ThrusterController.GetMainThrusterAcceleration());
                    float totalStoppingDistance = distanceTravelledTurning + distanceToSlow;

                    if (totalStoppingDistance >= deltaVector.magnitude) {
                        _correctionVector = slowDownVector;
                    }

                    //thrust only when facing target
                    if (Vector2.Angle(shipFacing, _correctionVector) < angleEpsilon) {
                        thrustVec = Vector2.up;
                    }

                    Debug.Log("On target");
                }
                else if (Math.Abs(driftAngle) < offCourseAngle) { //if drifting perpendicular or less 
                    //turn perpendicular to destination and burn to reduce drift angle to 0
                    Debug.Log("Drifting");

                    //get perpendicular vector from destination
                    float perpendicularAngle = driftAngle < 0 ? 90 : -90;
                    Vector2 perpendicularVec = Quaternion.Euler(0, 0, perpendicularAngle) * (deltaVector);
                    _correctionVector = perpendicularVec;

                    if (driftAngle < 0) { //wrong - need to calculate the optimal vector of acceleration
                        thrustVec = new Vector2(-1, 1);
                    }
                    else {
                        thrustVec = new Vector2(1, 1);
                    }
                }
                else { //if drifting backwards
                    //burn in opposite direction else it will drift more off target
                    Debug.Log("Way off course");
                    Vector2 velocityOpposite = Quaternion.Euler(0, 0, 180) * velocity;
                    _correctionVector = velocityOpposite;

                    float burnAngle = 180;
                    if (Math.Abs(velocityRotationDiff - burnAngle) < angleEpsilon) {
                        thrustVec = Vector2.up; //wrong
                    }
                }
            }
            else {
                _correctionVector = deltaVector;
                float minTargetDistance = 5;
                if (velocityRotationDiff < angleEpsilon && deltaVector.magnitude > minTargetDistance) { // 
                    thrustVec = Vector2.up;
                }
            }

            Debug.DrawLine(currentPosition, currentPosition + deltaVector, Color.yellow);
            Debug.DrawLine(currentPosition, currentPosition + velocity, Color.red);
            Debug.DrawLine(currentPosition, currentPosition + _correctionVector, Color.blue);
            Vector2 shipRotatedThrust = Quaternion.Euler(0, 0, shipRotation) * thrustVec;
            Debug.DrawLine(currentPosition, currentPosition + (shipRotatedThrust * 10), Color.cyan);

            return thrustVec;
        }

        private float CalculateFinalV(float initialVelocity, float displacement, float acceleration) {
            // v^2 = u^2 + 2as
            // v = sqrt(u^2 + 2as)
            return (float)Math.Sqrt(initialVelocity * initialVelocity + (2 * acceleration * Math.Abs(displacement)));
        }

        private float CalculateDisplacementTime(float initialVelocity, float finalVelocity, float acceleration) {
            //v = u + at
            //t = (v - u) / a
            return (finalVelocity - initialVelocity) / acceleration;
        }

        private float CalculateDisplacementTillStop(float initialVelocity, float acceleration) {
            // v^2 = u^2 + 2as
            // (v^2 - u^2)/2a = s
            //v = 0 so (-u^2)/2a = s
            // just interested in the distance so 
            // (u^2)/2a = s is sufficient  
            // float finalVelocity = 0;
            float angularA = acceleration;
            return (initialVelocity * initialVelocity) / (2 * angularA);
        }

        public override float GetTurnDirection() {
            float angle = GetTurnAngle();
            float angularDisplacement = CalculateDisplacementTillStop(ThrusterController.AngularVelocity, ThrusterController.AngularAcceleration);

            // Debug.Log("\nAngularV: " + angularV + "\nAngularA: " + angularA+ "\nAngle: " + angle + "\nCurrent Displacement: " + angularDisplacement);
            float turnEpsilon = .0001f;
            float delta = Math.Abs(angle) - angularDisplacement;
            if (delta > turnEpsilon) {
                float turnDir = angle < 0 ? -1 : 1;
                return turnDir;
            }

            return 0;
        }

        public void SetDestination(Vector2 newDestination) {
            destination = newDestination;
        }

        private float GetTurnAngle() {
            var forwardVec = _shipTransform.TransformDirection(Vector3.up);
            // Debug.DrawLine(transform.position, forwardVec*100, Color.red, .1f);
            // Debug.DrawLine(transform.position, destination, Color.green);
            // return Vector2.SignedAngle(forwardVec, (destination+correctionVector) - (Vector2)transform.position);
            return Vector2.SignedAngle(forwardVec, _correctionVector);
        }
    }
}