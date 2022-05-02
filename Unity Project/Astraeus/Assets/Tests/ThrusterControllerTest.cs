using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using NUnit.Framework;
using UnityEngine;

namespace Tests {
    public class ThrusterControllerTest {
        private static ThrusterController _thrusterController;

        [SetUp]
        public static void SetupTest() {
            List<MainThruster> mainThrusters = new List<MainThruster>();
            MainThruster thruster1 = new PrimitiveThruster(ShipComponentTier.T1);
            mainThrusters.Add(thruster1);
            
            List<ManoeuvringThruster> manoeuvringThrusters=new List<ManoeuvringThruster>();
            ManoeuvringThruster manoeuvringThruster = new ManoeuvringThruster(ShipComponentTier.T1);
            int manThrusterCount = 4;
            float angularAccel = 90;
            int shipMass = 1000;
            _thrusterController = new ThrusterController(mainThrusters, manoeuvringThruster, manThrusterCount, angularAccel, shipMass, new PowerPlantController(new List<PowerPlant>()));
        }

        [Test]
        public void ThrusterFireTest() {
            Vector2 up = Vector2.up;
            Vector2 down = Vector2.down;
            Vector2 left = Vector2.left;
            Vector2 right = Vector2.right;
           
            float facingAngle = 30;
            
            _thrusterController.Velocity = up;
            _thrusterController.FireThrusters(up, 1, facingAngle);
            _thrusterController.Velocity = up*1400;
            _thrusterController.FireThrusters(up, 1, facingAngle);
            
            
            _thrusterController.Velocity = down;
            _thrusterController.FireThrusters(up, 1, facingAngle);
            _thrusterController.Velocity = down *1400;
            _thrusterController.FireThrusters(up, 1, facingAngle);
        }
        
    }
}