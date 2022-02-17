using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using NUnit.Framework;
using UnityEngine;

namespace Tests {
    public class ThrusterControllerTest {
        private static ThrusterController _thrusterController;

        [SetUp]
        public static void SetupTest() {
            float shipMass = 10000;
            List<MainThruster> mainThrusters = new List<MainThruster>();
            MainThruster thruster1 = new PrimitiveThruster(ShipComponentTier.T1);
            mainThrusters.Add(thruster1);
            
            List<ManoeuvringThruster> manoeuvringThrusters=new List<ManoeuvringThruster>();
            ManoeuvringThruster manoeuvringThruster = new ManoeuvringThruster(ShipComponentTier.T1);
            List<float> centerOffsets = new List<float>(){-5,-5,5,5};
            _thrusterController = new ThrusterController(mainThrusters, (manoeuvringThruster, centerOffsets), shipMass);
        }

        [Test]
        public void ThrusterFireTest() {
            Vector2 up = Vector2.up;
            Vector2 down = Vector2.down;
            Vector2 left = Vector2.left;
            Vector2 right = Vector2.right;
           
            float facingAngle = 30;
            
            _thrusterController.velocity = up;
            _thrusterController.FireThrusters(up, 1, facingAngle);
            _thrusterController.velocity = up*1400;
            _thrusterController.FireThrusters(up, 1, facingAngle);
            
            
            _thrusterController.velocity = down;
            _thrusterController.FireThrusters(up, 1, facingAngle);
            _thrusterController.velocity = down *1400;
            _thrusterController.FireThrusters(up, 1, facingAngle);
        }
        
    }
}