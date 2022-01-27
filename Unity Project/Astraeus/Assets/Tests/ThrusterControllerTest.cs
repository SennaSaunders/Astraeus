using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using NUnit.Framework;
using UnityEngine;

namespace Tests {
    public class ThrusterControllerTest {
        private static ThrustersController _thrustersController;

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
            
            _thrustersController = new ThrustersController(mainThrusters, manoeuvringThrusters, shipMass);
        }

        [Test]
        public void ThrusterFireTest() {
            Vector2 up = Vector2.up;
            Vector2 down = Vector2.down;
            Vector2 left = Vector2.left;
            Vector2 right = Vector2.right;
           
            _thrustersController.facingAngle = 30;
            
            _thrustersController.velocity = up;
            _thrustersController.FireThrusters(up, 1);
            _thrustersController.velocity = up*1400;
            _thrustersController.FireThrusters(up, 1);
            
            
            _thrustersController.velocity = down;
            _thrustersController.FireThrusters(up, 1);
            _thrustersController.velocity = down *1400;
            _thrustersController.FireThrusters(up, 1);
        }
        
    }
}