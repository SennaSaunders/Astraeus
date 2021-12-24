namespace Code._Ships.Weapons {
    public abstract class Weapon : ShipComponent{
        protected Weapon(int componentSize, int mass) : base(ShipComponentType.Weapon, componentSize, mass) {
        }
    }
}