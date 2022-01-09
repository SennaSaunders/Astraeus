using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code._Ships.Hulls {
    public class MedCargoHull:Hull {
        private void Awake() {
            outfittingPosition = new Vector3(0, 0, 10);
            outfittingRotation = Quaternion.Euler(0, -90, 90);
            SetupHull(internalComponents,5000);
        }

        private static List<(ShipComponentType, int maxSize, int maxNum)> internalComponents = new List<(ShipComponentType, int maxSize, int maxNum)>() {
            (ShipComponentType.Internal, 1, 4),
            (ShipComponentType.Internal, 3, 2),
            (ShipComponentType.Internal, 2, 2)
        };
        
        protected override string GetHullPath() {
            return "Cargo/CargoHullMedium";
        }

        public override void SetExternalComponents() {
            List<Transform> hullObjects = hullObject.GetComponentsInChildren<Transform>().ToList();
            Transform frontLeftThruster = hullObjects.Find(g => g.name == "ThrusterHookFL");
            Transform frontRightThruster = hullObjects.Find(g => g.name == "ThrusterHookFR");
            Transform backLeftThruster = hullObjects.Find(g => g.name == "ThrusterHookBL");
            Transform backRightThruster = hullObjects.Find(g => g.name == "ThrusterHookBR");
            Transform backTurret = hullObjects.Find(g => g.name == "TurretHookB");
            Transform frontTurret = hullObjects.Find(g => g.name == "TurretHookF");
            
            List<(ShipComponentType, int maxSize, Transform parent)> externalComponents = new List<(ShipComponentType, int maxSize, Transform parent)>() {
                (ShipComponentType.MainThruster, 2, frontLeftThruster),
                (ShipComponentType.MainThruster, 2, frontRightThruster),
                (ShipComponentType.MainThruster, 2, backLeftThruster),
                (ShipComponentType.MainThruster, 2, backRightThruster),
                (ShipComponentType.Weapon, 2, backTurret),
                (ShipComponentType.Weapon, 2, frontTurret)
            };

            ExternalComponents = externalComponents;
        }
    }
}