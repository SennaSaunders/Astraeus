using Code._GameControllers;
using Code._Utility;

namespace Code._Ships.ShipComponents.InternalComponents.JumpDrives {
    public class JumpDrive : InternalComponent{
        public JumpDrive(ShipComponentTier componentSize) : base("Jump Drive", componentSize, 200, 500) {
            JumpRange = GetTierMultipliedValue(GameObjectHelper.GetGameController().MinExclusionDistance * ExclusionRangeScale, componentSize);
        }
        
        private const int ExclusionRangeScale = 5;
        public float JumpRange;
        public const float EnergyPerLY = 1500;
    }
}