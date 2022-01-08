using Code._Galaxy;
using UnityEngine;

namespace Code.GUI {
    public abstract class GalaxyGenInputModifier : MonoBehaviour {
        protected GalaxyGeneratorInput Input;
        public virtual void SetInput(GalaxyGeneratorInput input) {
            Input = input;
        }
        protected abstract void ChangeGalaxyGenInputValue();
    }
}