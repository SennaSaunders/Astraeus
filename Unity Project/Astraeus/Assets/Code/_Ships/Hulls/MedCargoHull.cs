using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using UnityEngine;

namespace Code._Ships.Hulls {
    public class MedCargoHull : Hull {
        private void Awake() {
            outfittingPosition = new Vector3(0, 0, 10);
            SetupHull(5000);
        }

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
            List<(ShipComponentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)> internalComponents = new List<(ShipComponentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)>() {
                (ShipComponentType.Internal, ShipComponentTier.T1, null, "InternalFL"),
                (ShipComponentType.Internal, ShipComponentTier.T1, null, "InternalFR"),
                (ShipComponentType.Internal, ShipComponentTier.T2, null, "InternalMF"),
                (ShipComponentType.Internal, ShipComponentTier.T2, null, "InternalMB"),
                (ShipComponentType.Internal, ShipComponentTier.T3, null, "InternalB")
            };

            InternalComponents = internalComponents;
        }
    }
}