using System;
using System.Collections.Generic;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using UnityEngine;

namespace Code._Ships.ShipComponents.ExternalComponents.Thrusters {
    public class ThrusterController {
        public ThrusterController(List<MainThruster> mainThrusters, ManoeuvringThruster manoeuvringThruster, int manThrusterCount, float angularAcceleration, float shipMass, PowerPlantController powerPlantController) {
            _shipMass = shipMass;
            _powerPlantController = powerPlantController;
            _mainThrustForce = GetMainThrustForce(mainThrusters);
            _mainThrustPowerDraw = GetMainThrustPowerDraw(mainThrusters);

            _manoeuvreThrustForce = GetManoeuvreThrustForce(manoeuvringThruster, manThrusterCount);
            AngularAcceleration = angularAcceleration;
            _manouevreThrustPowerDraw = GetManoeuvringThrusterPowerDraw(manoeuvringThruster, manThrusterCount);
        }

        private float _shipMass;
        private PowerPlantController _powerPlantController;

        private float _mainThrustForce;
        private float _manoeuvreThrustForce;
        private float _mainThrustPowerDraw;
        private float _manouevreThrustPowerDraw;

        public const float MaxSpeed = 250;
        private float thrustScale = 5;
        public Vector2 Velocity = new Vector2(0, 0);
        
        private float maxAngularVelocity = 180;
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

            return force * thrustScale;
        }

        private float GetMainThrustPowerDraw(List<MainThruster> thrusters) {
            float powerDraw = 0;
            foreach (MainThruster mainThruster in thrusters) {
                powerDraw = +mainThruster.PowerDraw;
            }

            return powerDraw;
        }

        private float GetManoeuvreThrustForce(ManoeuvringThruster thruster, int thrusterCount) {
            if (thruster != null) {
                return (thruster.Force * thrusterCount / 2) * thrustScale;
            }

            return 0;
        }

        private float GetManoeuvringThrusterPowerDraw(ManoeuvringThruster thruster, int thrusterCount) {
            if (thruster != null) {
                return thruster.PowerDraw * thrusterCount / 2;
            }

            return 0;
        }

        public void FireThrusters(Vector2 shipThrustVector, float deltaTime, float facingAngle) { //thrust vector forwards/backwards/sideways from -1 to 1
            float surgeForce = shipThrustVector.y * _mainThrustForce; //forwards/backwards
            float surgePowerEffectiveness = Math.Abs(_powerPlantController.DrainPower(Math.Abs(shipThrustVector.y * _mainThrustPowerDraw * deltaTime)));

            float swayForce = shipThrustVector.x * _manoeuvreThrustForce; //strafing
            float swayPowerEffectiveness = Math.Abs(_powerPlantController.DrainPower(Math.Abs(shipThrustVector.x * _manouevreThrustPowerDraw * deltaTime)));

            shipThrustVector = new Vector2(swayForce * swayPowerEffectiveness, surgeForce * surgePowerEffectiveness);
            Quaternion shipFacingQuaternion = Quaternion.Euler(0, 0, facingAngle);
            shipFacingQuaternion.Normalize();
            Vector2 shipRotatedThrustVector = shipFacingQuaternion * shipThrustVector;

            float thrustVelocityDifferenceAngle = Vector2.Angle(Velocity, shipRotatedThrustVector);

            float v = Velocity.magnitude < MaxSpeed ? Velocity.magnitude : MaxSpeed; //correct for velocity being slightly larger than max speed

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
            float acceleration = deltaTime * AngularAcceleration * lR * _powerPlantController.DrainPower(_manouevreThrustPowerDraw * deltaTime);
            AngularVelocity += acceleration;
            if (AngularVelocity > maxAngularVelocity) {
                AngularVelocity = maxAngularVelocity;
            }
            else if (AngularVelocity < -maxAngularVelocity) {
                AngularVelocity = -maxAngularVelocity;
            }
        }

        public void StopTurn(float deltaTime) {
            if (AngularVelocity > 0) {
                AngularVelocity -= deltaTime * AngularAcceleration;
                if (AngularVelocity < 0) {
                    AngularVelocity = 0;
                }
            }
            else if (AngularVelocity < 0) {
                AngularVelocity += deltaTime * AngularAcceleration;
                if (AngularVelocity > 0) {
                    AngularVelocity = 0;
                }
            }
        }
    }
}