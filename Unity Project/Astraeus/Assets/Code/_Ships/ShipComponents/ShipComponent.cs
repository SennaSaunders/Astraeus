using System;

namespace Code._Ships.ShipComponents {
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
        public ShipComponent(string componentName, ShipComponentType componentType, ShipComponentTier componentSize, float baseMass) {
            ComponentName = componentName;
            ComponentType = componentType;
            ComponentSize = componentSize;
            ComponentMass = GetTierMultipliedValue(baseMass, componentSize);
        }

        public string ComponentName;
        public ShipComponentType ComponentType;
        public ShipComponentTier ComponentSize;
        public float ComponentMass;

        public static float GetTierMultipliedValue(float value, ShipComponentTier tier) {
            return value * (1 + 0.5f * (int)tier);
        }
        
        public static float GetTierNormalizedStat(float min, float max, ShipComponentTier tier) {
            float ratio = (float)tier / Enum.GetValues(typeof(ShipComponentTier)).Length;

            return min + (max - min) * ratio;
        }
    }
}