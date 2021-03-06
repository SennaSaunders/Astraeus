using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using UnityEngine;

namespace Code._Ships.Hulls.Types.Cargo {
    public class SmallCargoHull : Hull {
        public SmallCargoHull() : base("Hermes",new Vector3(0, 0, 20),6,new Vector3(0,-10,0),15000,5, 20, 400, 15000) {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "Hull" }, new Color(.2f, 0, .6f)),
                (new List<string>() { "Cockpit" }, new Color(.1f, .1f, .1f)),
                (new List<string>() { "RearDoor" }, new Color(.3f, .3f, .2f))
            };
        }

        public override string GetHullFullPath() {
            return BaseHullPath + "Cargo/CargoSmall/CargoHullSmall";
        }

        protected override void SetThrusterComponents() {
            List<(ShipComponentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName, bool needsBracket)> mainThrusterComponents = new List<(ShipComponentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName, bool needsBracket)>() {
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null, "ThrusterHookFL", true),
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null, "ThrusterHookFR", true),
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null, "ThrusterHookBL", true),
                (ShipComponentType.MainThruster, ShipComponentTier.T2, null, "ThrusterHookBR", true)
            };
            MainThrusterComponents = mainThrusterComponents;
            TiedThrustersSets.Add(new List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string selectionTransformName, bool needsBracket)>(){mainThrusterComponents[0], mainThrusterComponents[1]});
            TiedThrustersSets.Add(new List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string selectionTransformName, bool needsBracket)>(){mainThrusterComponents[2], mainThrusterComponents[3]});
            
            ManoeuvringThrusterComponents = (componentType: ShipComponentType.ManoeuvringThruster, maxSize: ShipComponentTier.T3, null, "ThrusterManoeuvringSelector", new List<string>() {
                "ManThrusterBL", "ManThrusterBR", "ManThrusterFL", "ManThrusterFR","ManThrusterMBL", "ManThrusterMBR", "ManThrusterMFL","ManThrusterMFR"
            }) ;
        }

        protected override void SetWeaponComponents() {
            List<(ShipComponentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> weaponComponents = new List<(ShipComponentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)>() {
                (ShipComponentType.Weapon, ShipComponentTier.T2, null, "TurretHookB"),
                (ShipComponentType.Weapon, ShipComponentTier.T2, null, "TurretHookF")
            };

            WeaponComponents = weaponComponents;
        }

        protected override void SetInternalComponents() {
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