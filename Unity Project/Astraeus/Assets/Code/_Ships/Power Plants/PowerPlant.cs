namespace Code._Ships.Power_Plants {
    public class PowerPlant : ShipComponent {
        public float EnergyCapacity;
        public float CurrentEnergy;
        public float RechargeRate;
        
        public float DepletionRecoveryTime;
        public bool Depleted = false;
        
        public float DrainPower(float powerRequested) {
            float outputEffectiveness = 0; //modifies the effectiveness of the ship component requesting power

            if (!Depleted) { //if not depleted
                if (CurrentEnergy - powerRequested > 0) { //if there is enough power 
                    CurrentEnergy -= powerRequested;
                    outputEffectiveness = 1;
                }
                else { // get the relative power capacity left to  drain the capacity to 0, set Depleted to true
                    outputEffectiveness = EnergyCapacity / powerRequested;
                    CurrentEnergy = 0;
                    Depleted = true;
                }
            }
            return outputEffectiveness;
        }

        public PowerPlant(int componentSize,int mass) : base(ShipComponentType.Internal, componentSize, mass) {
        }
    }
}