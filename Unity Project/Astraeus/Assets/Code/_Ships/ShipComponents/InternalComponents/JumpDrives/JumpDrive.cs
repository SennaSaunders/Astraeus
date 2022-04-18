using Code._GameControllers;

namespace Code._Ships.ShipComponents.InternalComponents.JumpDrives {
    public class JumpDrive : InternalComponent{
        public JumpDrive(ShipComponentTier componentSize) : base("Jump Drive", componentSize, 200) {
            JumpRange = GetTierMultipliedValue(GameController.MinExclusionDistance * ExclusionRangeScale, componentSize);
        }
        
        private const int ExclusionRangeScale = 3;
        public float JumpRange;
        public const float EnergyPerLY = 1000;
    }
}