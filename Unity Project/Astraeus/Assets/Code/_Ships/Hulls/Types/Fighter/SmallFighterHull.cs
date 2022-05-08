using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using UnityEngine;

namespace Code._Ships.Hulls.Types.Fighter {
    public class SmallFighterHull : Hull {
        public SmallFighterHull() : base("Ares",new Vector3(0, 0, 20), 4000,20, 60, 500, 6000) {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "Hull" }, new Color(.4f, .2f, .7f)),
                (new List<string>() { "Cockpit" }, new Color(.2f, .2f, .4f)),
                (new List<string>() { "TailFins", "Wings" }, new Color(.2f, .2f, .2f))
            };
        }

        public override string GetHullFullPath() {
            return BaseHullPath + "Fighters/SmallFighter/SmallFighter";
        }

        protected override void SetThrusterComponents() {
            MainThrusterComponents = new List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string selectionTransformName, bool needsBracket)>() { (ShipComponentType.MainThruster, ShipComponentTier.T1, null, "ThrusterInternalMount", false) };
            ManoeuvringThrusterComponents = (componentType: ShipComponentType.ManoeuvringThruster, maxSize: ShipComponentTier.T3, null, "ThrusterManoeuvringSelector", new List<string>() {
                "ManThrusterBL", "ManThrusterBR", "ManThrusterFL", "ManThrusterFR"
            });
        }

        protected override void SetWeaponComponents() {
            List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> externalComponents = new List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)>() { (ShipComponentType.Weapon, ShipComponentTier.T2, null, "LWingTopTurretMount"), (ShipComponentType.Weapon, ShipComponentTier.T2, null, "RWingTopTurretMount"), (ShipComponentType.Weapon, ShipComponentTier.T2, null, "LWingBottomTurretMount"), (ShipComponentType.Weapon, ShipComponentTier.T2, null, "RWingBottomTurretMount") };

            WeaponComponents = externalComponents;
        }

        protected override void SetInternalComponents() {
            List<(ShipComponentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)> internalComponents = new List<(ShipComponentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)>() {
                (ShipComponentType.Internal, ShipComponentTier.T1, null, "InternalF"), (ShipComponentType.Internal, ShipComponentTier.T2, null, "InternalM"), (ShipComponentType.Internal, ShipComponentTier.T1, null, "InternalBT"), (ShipComponentType.Internal, ShipComponentTier.T2, null, "InternalBB"),
            };

            InternalComponents = internalComponents;
        }
    }
}