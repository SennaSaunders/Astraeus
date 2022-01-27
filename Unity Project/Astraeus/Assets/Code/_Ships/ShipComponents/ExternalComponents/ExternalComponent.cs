namespace Code._Ships.ShipComponents.ExternalComponents {
    public abstract class ExternalComponent : ShipComponent {
        protected const string BaseComponentPath = "Ships/";
        protected ExternalComponent(string componentName, ShipComponentType componentType, ShipComponentTier componentSize, float baseMass) : base(componentName, componentType, componentSize, baseMass) {
        }

        public virtual string GetFullPath() {
            return BaseComponentPath;
        }
    }
}