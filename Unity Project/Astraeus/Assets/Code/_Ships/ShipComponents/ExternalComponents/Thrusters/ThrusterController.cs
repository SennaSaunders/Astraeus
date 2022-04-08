using System;
using System.Collections.Generic;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters {
    public class ThrusterController {
        public ThrusterController(List<MainThruster> mainThrusters, (ManoeuvringThruster thruster, List<float> centerOffsets) manoeuvringThrusters, float shipMass, PowerPlantController powerPlantController) {
            _shipMass = shipMass;
            _powerPlantController = powerPlantController;
            _mainThrustForce = GetMainThrustForce(mainThrusters);
            _mainThrustPowerDraw = GetMainThrustPowerDraw(mainThrusters);

            
            _manoeuvreThrustForce = GetManoeuvreThrustForce(manoeuvringThrusters.thruster, manoeuvringThrusters.centerOffsets.Count);
            AngularAcceleration = GetAngularAcceleration(manoeuvringThrusters.thruster, manoeuvringThrusters.centerOffsets);
            _manouevreThrustPowerDraw = GetManoeuvringThrusterPowerDraw(manoeuvringThrusters.thruster, manoeuvringThrusters.centerOffsets.Count);

        }
        
        private float _shipMass;
        private PowerPlantController _powerPlantController;
        
        private float _mainThrustForce;
        private float _manoeuvreThrustForce;
        private float _mainThrustPowerDraw;
        private float _manouevreThrustPowerDraw;

        public const float MaxSpeed = 500;
        public Vector2 Velocity = new Vector2(0, 0);
        
        private float _turnScale = 10;
        private float maxAngularVelocity = 360;
        public float AngularAcceleration { get; private set; }
        public float AngularVelocity = 0;
        
        public float GetMainThrusterAcceleration() {
            return _mainThrustForce / _shipMass;
        }
        
        private float GetMainThrustForce(List<MainThruster> thrusters) {
            float force = 0;

            foreach (var thruster in thrusters) {
                force += thruster.Force;
            }

            return force;
        }

        private float GetMainThrustPowerDraw(List<MainThruster> thrusters) {
            float powerDraw = 0;
            foreach (MainThruster mainThruster in thrusters) {
                powerDraw = +mainThruster.PowerDraw;
            }

            return powerDraw;
        }

        private float GetManoeuvreThrustForce(ManoeuvringThruster thruster, int thrusterCount) {
            return thruster.Force * thrusterCount / 2;
        }

        private float GetAngularAcceleration(ManoeuvringThruster thruster, List<float> centerOffsets) { 
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

        private float GetManoeuvringThrusterPowerDraw(ManoeuvringThruster thruster, int thrusterCount) {
            return thruster.PowerDraw * thrusterCount/2;
        }

        public void FireThrusters(Vector2 shipThrustVector, float deltaTime, float facingAngle) { //thrust vector forwards/backwards/sideways from -1 to 1
            float surgeForce = shipThrustVector.y * _mainThrustForce; //forwards/backwards
            float surgePowerEffectiveness = Math.Abs(_powerPlantController.DrainPower(shipThrustVector.y * _mainThrustPowerDraw));
            
            float swayForce = shipThrustVector.x * _manoeuvreThrustForce; //strafing
            float swayPowerEffectiveness = Math.Abs(_powerPlantController.DrainPower(shipThrustVector.x * _manouevreThrustPowerDraw));

            shipThrustVector = new Vector2(swayForce*swayPowerEffectiveness, surgeForce*surgePowerEffectiveness);
            Quaternion shipFacingQuaternion = Quaternion.Euler(0, 0, facingAngle);
            shipFacingQuaternion.Normalize();
            Vector2 shipRotatedThrustVector = shipFacingQuaternion * shipThrustVector;
            
            float thrustVelocityDifferenceAngle = Vector2.Angle(Velocity, shipRotatedThrustVector);

            float v = Velocity.magnitude;
            double lorentzFactor = 1 / Math.Sqrt(1 - (v * v) / (MaxSpeed * MaxSpeed));
            float adjustedMass = _shipMass * (float)lorentzFactor;

            Vector2 a;
            if (thrustVelocityDifferenceAngle < 90) { //reduce thrust effectiveness if less than perpendicular to direction of travel
                a = shipRotatedThrustVector / adjustedMass;
            }
            else {
                a = shipRotatedThrustVector / _shipMass;
            }

            Velocity += a * deltaTime;
            float scale = Velocity.magnitude / MaxSpeed;

            if (scale > 1) {
                Velocity /= scale;
            }
        }

        //turn
        public void TurnShip(float deltaTime, float lR) {
            float acceleration = deltaTime * AngularAcceleration * lR * _powerPlantController.DrainPower(_manouevreThrustPowerDraw);
            AngularVelocity += acceleration;
            if (AngularVelocity > maxAngularVelocity) {
                AngularVelocity = maxAngularVelocity;
            } else if (AngularVelocity < -maxAngularVelocity) {
                AngularVelocity = -maxAngularVelocity;
            }
        }

        public void StopTurn(float deltaTime) {
            if (AngularVelocity > 0) {
                AngularVelocity -= deltaTime * AngularAcceleration;
                if (AngularVelocity < 0) {
                    AngularVelocity = 0;
                }
            } else if (AngularVelocity < 0) {
                AngularVelocity += deltaTime * AngularAcceleration;
                if (AngularVelocity > 0) {
                    AngularVelocity = 0;
                }
            }
        }
    }
}