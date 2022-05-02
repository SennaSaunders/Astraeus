using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using UnityEngine;

namespace Code._Ships.Hulls.Types.Fighter {
    public class SleekFighter : Hull {
        public SleekFighter() : base("Achilles",new Vector3(0, 0, 30), 2500, 15, 45, 300, 20000) {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "Hull" }, new Color(.2f, 0, .6f)),
                (new List<string>() { "Cockpit" }, new Color(.1f, .1f, .1f))
            };
        }

        public override string GetHullFullPath() {
            return BaseHullPath + "Fighters/SleekFighter/SleekFighter";
        }

        protected override void SetThrusterComponents() {
            MainThrusterComponents = new List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string selectionTransformName, bool needsBracket)>() {
                (ShipComponentType.MainThruster, ShipComponentTier.T3, null, "Thruster1", true),
                (ShipComponentType.MainThruster, ShipComponentTier.T3, null, "Thruster2", true)
            };
            TiedThrustersSets.Add(new List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string selectionTransformName, bool needsBracket)>(){MainThrusterComponents[0], MainThrusterComponents[1]});
            
            ManoeuvringThrusterComponents = (componentType: ShipComponentType.ManoeuvringThruster, maxSize: ShipComponentTier.T4, null, "ThrusterManoeuvringSelector", new List<string>() {
                "MT1","MT1","MT2","MT3", "MT4", "MT5", "MT6"
            });
        }

        protected override void SetWeaponComponents() {
            List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> externalComponents = new List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)>() {
                (ShipComponentType.Weapon, ShipComponentTier.T3, null, "Weapon1"),
                (ShipComponentType.Weapon, ShipComponentTier.T3, null, "Weapon2"),
            };

            WeaponComponents = externalComponents;
        }

        protected override void SetInternalComponents() {
            List<(ShipComponentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)> internalComponents = new List<(ShipComponentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)>() {
                (ShipComponentType.Internal, ShipComponentTier.T2, null, "Internal1"),
                (ShipComponentType.Internal, ShipComponentTier.T2, null, "Internal2"),
                (ShipComponentType.Internal, ShipComponentTier.T3, null, "Internal3"),
                (ShipComponentType.Internal, ShipComponentTier.T3, null, "Internal4")
            };
            
            InternalComponents = internalComponents;
        }
    }
}