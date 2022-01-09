using System;

namespace Code._Ships {
    public enum ShipComponentType {
        MainThruster,
        ManoeuvringThruster,
        Weapon,
        Internal
    }

    public enum ShipComponentTier {
        T1,
        T2,
        T3,
        T4,
        T5
    }
    public class ShipComponent {
        public ShipComponent(string name, ShipComponentType componentType, ShipComponentTier componentSize, float baseMass) {
            Name = name;
            ComponentType = componentType;
            ComponentSize = componentSize;
            ComponentMass = GetTierMultipliedStat(baseMass, componentSize);
        }

        public string Name;
        public ShipComponentType ComponentType;
        public ShipComponentTier ComponentSize;
        public string ModelPath;
        public float ComponentMass;

        public static float GetTierMultipliedStat(float stat, ShipComponentTier tier) {
            return stat * (1 + 0.5f * (int)tier);
        }
        
        public static float GetTierNormalizedStat(float min, float max, ShipComponentTier tier) {
            float ratio = (float)tier / Enum.GetValues(typeof(ShipComponentTier)).Length;

            return min + (max - min) * ratio;
        }
    }
}