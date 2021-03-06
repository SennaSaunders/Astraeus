using Code._Galaxy;
using UnityEngine;

namespace Code.GUI.GalaxyGeneration {
    public abstract class GalaxyGenInputModifier : MonoBehaviour {
        protected GalaxyGeneratorInput Input;
        public virtual void SetInput(GalaxyGeneratorInput input) {
            Input = input;
        }
        protected abstract void ChangeGalaxyGenInputValue();
    }
}