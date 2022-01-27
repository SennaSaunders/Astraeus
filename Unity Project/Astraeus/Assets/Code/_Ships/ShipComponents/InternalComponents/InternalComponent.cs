namespace Code._Ships.ShipComponents.InternalComponents {
    public abstract class InternalComponent : ShipComponent{
        protected InternalComponent(string componentName, ShipComponentTier componentSize, float baseMass) : base(componentName, ShipComponentType.Internal, componentSize, baseMass) {
        }
    }
}