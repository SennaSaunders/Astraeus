using System.Collections.Generic;
using Code._Ships;
using Code._Ships.Thrusters;
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
            ManoeuvringThruster manoeuvringThruster1 = new ManoeuvringThruster("",ShipComponentTier.T1, 20, 1000, 2);
            manoeuvringThrusters.Add(manoeuvringThruster1);
            manoeuvringThrusters.Add(manoeuvringThruster1);
            
            _thrusterController = new ThrusterController(mainThrusters, manoeuvringThrusters, shipMass);
        }

        [Test]
        public void ThrusterFireTest() {
            Vector2 up = Vector2.up;
            Vector2 down = Vector2.down;
            Vector2 left = Vector2.left;
            Vector2 right = Vector2.right;
           
            _thrusterController.facingAngle = 30;
            
            _thrusterController.velocity = up;
            _thrusterController.FireThrusters(up, 1);
            _thrusterController.velocity = up*1400;
            _thrusterController.FireThrusters(up, 1);
            
            
            _thrusterController.velocity = down;
            _thrusterController.FireThrusters(up, 1);
            _thrusterController.velocity = down *1400;
            _thrusterController.FireThrusters(up, 1);
        }
        
    }
}