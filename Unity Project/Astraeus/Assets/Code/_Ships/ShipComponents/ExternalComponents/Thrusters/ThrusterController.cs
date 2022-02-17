using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters {
    public class ThrusterController {
        public ThrusterController(List<MainThruster> mainThrusters, (ManoeuvringThruster thruster, List<float> centerOffsets) manoeuvringThrusters, float shipMass) {
            _shipMass = shipMass;

            _mainThrusters = mainThrusters;
            _mainThrustForce = GetMainThrustForce(_mainThrusters);

            _manoeuvringThrusters = manoeuvringThrusters;
            _manoeuvreThrustForce = GetManoeuvreThrustForce(manoeuvringThrusters.thruster, manoeuvringThrusters.centerOffsets.Count);
            _angularAcceleration = GetAngularAcceleration(_manoeuvringThrusters.thruster, _manoeuvringThrusters.centerOffsets);
        }

        private float _shipMass;

        private List<MainThruster> _mainThrusters;
        private float _mainThrustForce;
        public Vector2 velocity = new Vector2(0, 0);

        private (ManoeuvringThruster thruster, List<float> centerOffsets) _manoeuvringThrusters;
        private float _manoeuvreThrustForce;
        private float _turnScale = 20;
        private float _angularAcceleration;
        public float angularVelocity = 0;
        private float maxAngularVelocity = 360;


        public const float MaxSpeed = 500;
        public const float MaxRotationSpeed = 90;


        public float GetMainThrustForce(List<MainThruster> thrusters) {
            float force = 0;

            foreach (var thruster in thrusters) {
                force += thruster.Force;
            }

            return force;
        }

        public float GetManoeuvreThrustForce(ManoeuvringThruster thruster, int thrusterCount) {
            return thruster.Force * thrusterCount / 2;
        }

        public float GetAngularAcceleration(ManoeuvringThruster thruster, List<float> centerOffsets) {
            //moments
            //get total force and then half it

            float baseForce = thruster.Force;
            float torque = 0;
            float r = 0; //turn radius = average of offsets
            foreach (float offset in centerOffsets) {
                float tmp = offset * baseForce;
                tmp = tmp > 0 ? tmp : tmp * -1;
                torque += tmp;
                r += offset > 1 ? offset : offset * -1; //sum of offsets (positive only)
            }

            torque /= 2; //only half the thrusters can work at once to rotate
            r /= centerOffsets.Count; //averaged

            return _turnScale * torque / (_shipMass * r * r);
        }

        //thrust
        public void FireThrusters(Vector2 shipThrustVector, float deltaTime, float facingAngle) { //thrust vector forwards/backwards/sideways from -1 to 1
            //  a = f/m
            //  mass is affected by velocity
            //get movement angle
            //get thrust angle
            // calc delta thrust angle
            // calc delta thrust ratio - 0 at same angle|1 at perpendicular or facing away
            // scale the thrust proportionally to the delta ratio
            // sideways thrust is

            //forwards/backwards
            float swayForce = shipThrustVector.x * _manoeuvreThrustForce; //strafing
            float surgeForce = shipThrustVector.y * _mainThrustForce; //forwards/backwards

            shipThrustVector = new Vector2(swayForce, surgeForce);
            Quaternion shipFacingQuaternion = Quaternion.Euler(0, 0, facingAngle);
            shipFacingQuaternion.Normalize();
            Vector2 shipRotatedThrustVector = shipFacingQuaternion * shipThrustVector;
            
            float thrustVelocityDifferenceAngle = Vector2.Angle(velocity, shipRotatedThrustVector);

            float v = velocity.magnitude;
            double lorentzFactor = 1 / Math.Sqrt(1 - (v * v) / (MaxSpeed * MaxSpeed));
            float adjustedMass = _shipMass * (float)lorentzFactor;

            Vector2 a;
            if (thrustVelocityDifferenceAngle < 90) { //reduce thrust effectiveness if less than perpendicular to direction of travel
                a = shipRotatedThrustVector / adjustedMass;
            }
            else {
                a = shipRotatedThrustVector / _shipMass;
            }

            velocity += a * deltaTime;
            float scale = velocity.magnitude / MaxSpeed;

            if (scale > 1) {
                velocity /= scale;
            }
        }
        
        public void Brake (float deltaTime) {
            
        }

        //turn
        public void TurnShip(float deltaTime, float lR) {
            angularVelocity += deltaTime * _angularAcceleration * lR;
            if (angularVelocity > maxAngularVelocity) {
                angularVelocity = maxAngularVelocity;
            }

            if (angularVelocity < -maxAngularVelocity) {
                angularVelocity = -maxAngularVelocity;
            }
            Debug.Log("Turn Rate: " + angularVelocity);
        }

        public void StopTurn(float deltaTime) {
            if (angularVelocity > 0) {
                angularVelocity -= deltaTime * _angularAcceleration;
                if (angularVelocity < 0) {
                    angularVelocity = 0;
                }
            } else if (angularVelocity < 0) {
                angularVelocity += deltaTime * _angularAcceleration;
                if (angularVelocity > 0) {
                    angularVelocity = 0;
                }
            }
        }
    }
}