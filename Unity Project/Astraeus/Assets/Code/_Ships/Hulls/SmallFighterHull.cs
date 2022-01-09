using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code._Ships.Hulls {
    public class SmallFighterHull:Hull {
        private void Awake() {
            outfittingPosition = new Vector3(0, 0, 10);
            outfittingRotation = Quaternion.Euler(0, -90, 90);
            SetupHull(internalComponents,5000);
        }

        private static List<(ShipComponentType, int maxSize, int maxNum)> internalComponents = new List<(ShipComponentType, int maxSize, int maxNum)>() {
            (ShipComponentType.Internal, 1, 3),
            (ShipComponentType.Internal, 2, 1)
        };

        protected override string GetHullPath() {
            return "Fighters/SmallFighter";
        }

        public override void SetExternalComponents() {
            List<Transform> hullObjects = hullObject.GetComponentsInChildren<Transform>().ToList();
            Transform internalThruster = hullObjects.Find(g => g.name == "ThrusterInternalMount");
            Transform lWingTopTurret = hullObjects.Find(g => g.name == "LWingTopTurretMount");
            Transform rWingTopTurret = hullObjects.Find(g => g.name == "RWingTopTurretMount");
            Transform lWingBottomTurret = hullObjects.Find(g => g.name == "LWingBottomTurretMount");
            Transform rWingBottomTurret = hullObjects.Find(g => g.name == "RWingBottomTurretMount");
            
            List<(ShipComponentType, int maxSize, Transform parent)> externalComponents = new List<(ShipComponentType, int maxSize, Transform parent)>() {
                (ShipComponentType.MainThruster, 2, internalThruster),
                (ShipComponentType.Weapon, 2, lWingBottomTurret),
                (ShipComponentType.Weapon, 2, lWingTopTurret),
                (ShipComponentType.Weapon, 2, rWingBottomTurret),
                (ShipComponentType.Weapon, 2, rWingTopTurret)
            };

            ExternalComponents = externalComponents;
        }
    }
}