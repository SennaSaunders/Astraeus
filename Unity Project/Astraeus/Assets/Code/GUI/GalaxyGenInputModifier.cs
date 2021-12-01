using Code.Galaxy;
using UnityEngine;

namespace Code.GUI {
    public abstract class GalaxyGenInputModifier : MonoBehaviour {
        protected GalaxyGeneratorInput _input;
        public virtual void SetInput(GalaxyGeneratorInput input) {
            _input = input;
        }
        protected abstract void ChangeGalaxyGenInputValue();
    }
}