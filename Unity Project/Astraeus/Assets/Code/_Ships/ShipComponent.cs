namespace Code._Ships {
    public enum ShipComponentType {
        MainThruster,
        ManoeuvringThruster,
        Weapon,
        Internal
    }
    public class ShipComponent {
        public ShipComponent(ShipComponentType componentType, int componentSize, float mass) {
            ComponentType = componentType;
            ComponentSize = componentSize;
            ComponentMass = mass;
        }

        public ShipComponentType ComponentType;
        public int ComponentSize;
        public float ComponentMass;
    }
}