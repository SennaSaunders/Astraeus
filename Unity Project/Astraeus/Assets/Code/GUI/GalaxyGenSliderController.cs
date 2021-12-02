using Code.Galaxy;
using Code.ObserverPattern;
using UnityEngine.UI;

namespace Code.GUI {
    public class GalaxyGenSliderController : GalaxyGenInputModifier, IIntObserver{
        private Slider slider;

        public void Awake() {
            slider = GetComponentInParent<Slider>();
            slider.onValueChanged.AddListener(delegate { ChangeGalaxyGenInputValue(); });
        }

        public override void SetInput(GalaxyGeneratorInput input) {
            base.SetInput(input);
            slider.maxValue = input.MaxValue;
            slider.minValue = input.MinValue;
        }

        protected override void ChangeGalaxyGenInputValue() {
            _input.SetValue((int)slider.value);
        }

        public void UpdateSelf(int value) {
            slider.value = value;
        }

        public bool UpdateNeeded(int value) {
            if ((int)slider.value != value) {
                return true;
            }
            return false;
        }
    }
}