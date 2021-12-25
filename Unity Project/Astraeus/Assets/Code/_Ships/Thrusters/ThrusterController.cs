using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code._Ships.Thrusters {
    public class ThrusterController {
        private float _shipMass;
        
        private List<MainThruster> _mainThrusters;
        private float _mainThrustForce;
        
        private List<ManoeuvringThruster> _manoeuvringThrusters;
        private float _manoeuvreThrustForce;
        
        public Vector2 velocity = new Vector2(0,0);
        public float facingAngle = -90; //degrees - from 0 to 360
        
        
        public const float MaxSpeed = 100;
        public const float MaxRotationSpeed = 90;

        public ThrusterController(List<MainThruster> mainThrusters, List<ManoeuvringThruster> manoeuvringThrusters, float shipMass) {
            _shipMass = shipMass;
            
            _mainThrusters = mainThrusters;
            _mainThrustForce = SetThrustForce(_mainThrusters.ConvertAll(t => (Thruster)t));
            
            _manoeuvringThrusters = manoeuvringThrusters;
            _manoeuvreThrustForce = SetThrustForce(_manoeuvringThrusters.ConvertAll(t => (Thruster)t));
        }

        public float SetThrustForce(List<Thruster> thrusters) {
            float force=0;

            foreach (var thruster in thrusters) {
                force += thruster.Force;
            }

            return force;
        }

        //thrust
        public void FireThrusters(Vector2 shipThrustVector, float deltaTime) {//thrust vector forwards/backwards/sideways from -1 to 1
            //  a = f/m
            //  mass is affected by velocity
            //get movement angle
            //get thrust angle
            // calc delta thrust angle
            // calc delta thrust ratio - 0 at same angle|1 at perpendicular or facing away
            // scale the thrust proportionally to the delta ratio
            // sideways thrust is
            
            //forwards/backwards
            float swayForce = shipThrustVector.x * _manoeuvreThrustForce;//strafing
            float surgeForce = shipThrustVector.y * _mainThrustForce;//forwards/backwards

            shipThrustVector = new Vector2(swayForce, surgeForce);
            Quaternion shipFacingQuaternion = Quaternion.Euler(0, 0, facingAngle);
            shipFacingQuaternion.Normalize();
            Vector2 shipRotatedThrustVector = shipFacingQuaternion * shipThrustVector;

            var vAngle = Vector2.SignedAngle(velocity, Vector2.up);
            Quaternion vQuaternion = Quaternion.Euler(0,0,vAngle);
            vQuaternion.Normalize();
            
            float thrustVelocityDifferenceAngle = Vector2.Angle(velocity, shipRotatedThrustVector);
            Vector2 vRotatedThrustVector = vQuaternion * shipRotatedThrustVector;
            
            float v = velocity.magnitude;
            double lorentzFactor = 1 / Math.Sqrt(1 - (v * v) / (MaxSpeed * MaxSpeed));
            float adjustedMass = _shipMass * (float)lorentzFactor;
            
            Vector2 a;
            if (thrustVelocityDifferenceAngle < 90) {//reduce thrust effectiveness if less than perpendicular to direction of travel
                float perpendicularRatio = thrustVelocityDifferenceAngle/90;
                a = shipRotatedThrustVector / adjustedMass;
            }
            else {
                a = shipRotatedThrustVector / _shipMass;
            }
            
            // if (thrustVelocityDifferenceAngle < 90) {//reduce thrust effectiveness if less than perpendicular to direction of travel
            //     float perpendicularRatio = thrustVelocityDifferenceAngle/90;
            //     a = vRotatedThrustVector / adjustedMass;
            // }
            // else {
            //     a = vRotatedThrustVector / _shipMass;
            // }

            velocity += a*deltaTime;
            float scale = velocity.magnitude / MaxSpeed;

            if (scale > 1) {
                velocity /= scale;
            }
        }

        //turn
        public void TurnShip(float deltaTime, float lR) {
            facingAngle += MaxRotationSpeed * deltaTime * lR;
        }
    }
}