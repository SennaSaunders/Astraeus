using System.Collections.Generic;
using System.Linq;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using UnityEngine;

namespace Code._Ships.Hulls {
    public class MedCargoHull : Hull {
        private void Awake() {
            outfittingPosition = new Vector3(0, 0, 10);
            SetupHull(5000);
        }

        private static List<(ShipComponentType, int maxSize, int maxNum)> internalComponents = new List<(ShipComponentType, int maxSize, int maxNum)>() { (ShipComponentType.Internal, 1, 4), (ShipComponentType.Internal, 3, 2), (ShipComponentType.Internal, 2, 2) };

        protected override string GetHullFullPath() {
            return BaseHullPath + "Cargo/CargoHullMedium";
        }

        public override void SetThrusterComponents() {
            List<(ShipComponentType, ShipComponentTier maxSize, Thruster concreteComponent, string parentTransformName, bool needsBracket)> thrusterComponents = new List<(ShipComponentType, ShipComponentTier maxSize, Thruster concreteComponent, string parentTransformName, bool needsBracket)>() {
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null, "ThrusterHookFL", true),
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null, "ThrusterHookFR", true),
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null, "ThrusterHookBL", true),
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null, "ThrusterHookBR", true)
            };
            ThrusterComponents = thrusterComponents;
        }

        public override void SetWeaponComponents() {
            List<(ShipComponentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> weaponComponents = new List<(ShipComponentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)>() {
                (ShipComponentType.Weapon, ShipComponentTier.T2, null, "TurretHookB"),
                (ShipComponentType.Weapon, ShipComponentTier.T2, null, "TurretHookF")
            };

            WeaponComponents = weaponComponents;
        }

        public override void SetInternalComponents() {
            //throw new System.NotImplementedException();
        }
    }
}