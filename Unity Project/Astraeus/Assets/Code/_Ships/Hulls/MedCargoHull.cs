using System.Collections.Generic;
using System.Linq;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents;
using UnityEngine;

namespace Code._Ships.Hulls {
    public class MedCargoHull:Hull {
        private void Awake() {
            outfittingPosition = new Vector3(0, 0, 10);
            outfittingRotation = Quaternion.Euler(0, -90, 90);
            SetupHull(5000);
        }

        private static List<(ShipComponentType, int maxSize, int maxNum)> internalComponents = new List<(ShipComponentType, int maxSize, int maxNum)>() {
            (ShipComponentType.Internal, 1, 4),
            (ShipComponentType.Internal, 3, 2),
            (ShipComponentType.Internal, 2, 2)
        };
        
        protected override string GetHullFullPath() {
            return BaseHullPath+"Cargo/CargoHullMedium";
        }

        public override void SetExternalComponents() {
            List<Transform> hullObjects = hullObject.GetComponentsInChildren<Transform>().ToList();
            
            List<(ShipComponentType, ShipComponentTier maxSize, ExternalComponent concreteComponent, string parentTransformName)> externalComponents = new List<(ShipComponentType, ShipComponentTier maxSize, ExternalComponent concreteComponent, string parentTransformName)>() {
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null, "ThrusterHookFL"),
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null,"ThrusterHookFR"),
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null,"ThrusterHookBL"),
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null,"ThrusterHookBR"),
                (ShipComponentType.Weapon,ShipComponentTier.T2, null,"TurretHookB"),
                (ShipComponentType.Weapon, ShipComponentTier.T2, null,"TurretHookF")
            };

            ExternalComponents = externalComponents;
        }

        public override void SetInternalComponents() {
            //throw new System.NotImplementedException();
        }
    }
}