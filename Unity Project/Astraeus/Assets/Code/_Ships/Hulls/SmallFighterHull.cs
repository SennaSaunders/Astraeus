using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using UnityEngine;

namespace Code._Ships.Hulls {
    public class SmallFighterHull:Hull {
        private void Awake() {
            outfittingPosition = new Vector3(0, 0, 10);
            SetupHull(5000);
        }

        private static List<(ShipComponentType, int maxSize, int maxNum)> internalComponents = new List<(ShipComponentType, int maxSize, int maxNum)>() {
            (ShipComponentType.Internal, 1, 3),
            (ShipComponentType.Internal, 2, 1)
        };

        protected override string GetHullFullPath() {
            return BaseHullPath+"Fighters/SmallFighter";
        }

        public override void SetThrusterComponents() {
            ThrusterComponents = new List<(ShipComponentType componentType, ShipComponentTier maxSize, Thruster concreteComponent, string parentTransformName, bool needsBracket)>() { (ShipComponentType.MainThruster, ShipComponentTier.T1, null, "ThrusterInternalMount", false) };
        }

        public override void SetWeaponComponents() {
            List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> externalComponents = new List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)>() {
                (ShipComponentType.Weapon, ShipComponentTier.T2, null,"LWingTopTurretMount"),
                (ShipComponentType.Weapon, ShipComponentTier.T2, null,"RWingTopTurretMount"),
                (ShipComponentType.Weapon, ShipComponentTier.T2, null,"LWingBottomTurretMount"),
                (ShipComponentType.Weapon, ShipComponentTier.T2, null,"RWingBottomTurretMount")
            };

            WeaponComponents = externalComponents;
        }

        public override void SetInternalComponents() {
            //throw new System.NotImplementedException();
        }
    }
}