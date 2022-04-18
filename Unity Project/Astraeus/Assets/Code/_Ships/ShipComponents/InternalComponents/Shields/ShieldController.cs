using System.Collections.Generic;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;

namespace Code._Ships.ShipComponents.InternalComponents.Shields {
    public class ShieldController {
        private List<Shield> _shields;
        private PowerPlantController _powerPlantController;

        public ShieldController(List<Shield> shields, PowerPlantController powerPlantController) {
            _shields = shields;
            _powerPlantController = powerPlantController;
        }

        public float TakeDamage(float damage) {
            float damageLeft = damage;
            for (int i = 0; i < _shields.Count; i++) {
                Shield shield = _shields[i];
                
                if (shield.CurrentStrength - damageLeft < 0) {
                    damageLeft -= shield.CurrentStrength;
                    shield.CurrentStrength = 0;
                }
                else {
                    shield.CurrentStrength -= damageLeft;
                    damageLeft = 0;
                    break;
                }
            }

            return damageLeft;
        }
    }
}