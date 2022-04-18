using Code._GameControllers;

namespace Code._Ships.ShipComponents.InternalComponents.JumpDrives {
    public class JumpDrive : InternalComponent{
        public JumpDrive(ShipComponentTier componentSize) : base("Jump Drive", componentSize, 200) {
            JumpRange = GetTierMultipliedValue(GameController.MinExclusionDistance * 2, componentSize);
            Energy = GetTierMultipliedValue(BaseEnergyCost, componentSize);
        }

        private float BaseEnergyCost = 200;
        
        public float JumpRange;
        public float Energy;
    }
}