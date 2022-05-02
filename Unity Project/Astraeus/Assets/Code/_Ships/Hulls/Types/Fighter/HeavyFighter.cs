using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using UnityEngine;

namespace Code._Ships.Hulls.Types.Fighter {
    public class HeavyFighter :Hull {
        public HeavyFighter() : base("Daedalus",new Vector3(0, 0, 30), 12000, 10, 30, 1000, 50000) {
            ColourChannelObjectMap = new List<(List<string> objectName, Color colour)>() {
                (new List<string>() { "Hull" }, new Color(.2f, 0, .6f)),
                (new List<string>() { "Cockpit" }, new Color(.1f, .1f, .1f)),
                (new List<string>() { "Vent" }, new Color(.3f, .3f, .2f))
            };
        }

        public override string GetHullFullPath() {
            return BaseHullPath + "Fighters/HeavyFighter/HeavyFighter";
        }

        protected override void SetThrusterComponents() {
            MainThrusterComponents = new List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string selectionTransformName, bool needsBracket)>() {
                (ShipComponentType.MainThruster, ShipComponentTier.T4, null, "BLThruster", true),
                (ShipComponentType.MainThruster, ShipComponentTier.T4, null, "BRThruster", true),
                (ShipComponentType.MainThruster, ShipComponentTier.T4, null, "TLThruster", true),
                (ShipComponentType.MainThruster, ShipComponentTier.T4, null, "TRThruster", true)
            };
            TiedThrustersSets.Add(new List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string selectionTransformName, bool needsBracket)>(){MainThrusterComponents[0], MainThrusterComponents[1]});
            TiedThrustersSets.Add(new List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string selectionTransformName, bool needsBracket)>(){MainThrusterComponents[2], MainThrusterComponents[3]});
            ManoeuvringThrusterComponents = (componentType: ShipComponentType.ManoeuvringThruster, maxSize: ShipComponentTier.T4, null, "ThrusterManoeuvringSelector", new List<string>() {
                "ManThrusterBL", "ManThrusterBR", "ManThrusterFL", "ManThrusterFR","ManThrusterMidR", "ManThrusterMidL"
            });
        }

        protected override void SetWeaponComponents() {
            List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> externalComponents = new List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)>() {
                (ShipComponentType.Weapon, ShipComponentTier.T3, null, "TurretFT"),
                (ShipComponentType.Weapon, ShipComponentTier.T3, null, "TurretFB"),
                (ShipComponentType.Weapon, ShipComponentTier.T3, null, "TurretBB"),
                (ShipComponentType.Weapon, ShipComponentTier.T3, null, "TurretBT"),
            };

            WeaponComponents = externalComponents;
        }

        protected override void SetInternalComponents() {
            List<(ShipComponentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)> internalComponents = new List<(ShipComponentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)>() {
                (ShipComponentType.Internal, ShipComponentTier.T3, null, "InternalBL"),
                (ShipComponentType.Internal, ShipComponentTier.T4, null, "InternalBM"),
                (ShipComponentType.Internal, ShipComponentTier.T3, null, "InternalBR"),
                (ShipComponentType.Internal, ShipComponentTier.T2, null, "InternalF"),
                (ShipComponentType.Internal, ShipComponentTier.T3, null, "InternalM"),
                (ShipComponentType.Internal, ShipComponentTier.T3, null, "InternalMF")
            };
            
            InternalComponents = internalComponents;
        }
    }
}