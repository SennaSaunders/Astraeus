using Code._Galaxy;
using Code.ObserverPattern;
using UnityEngine.UI;

namespace Code.GUI.GalaxyGeneration {
    public class GalaxyGenSliderController : GalaxyGenInputModifier, IIntObserver{
        private Slider _slider;

        public void Awake() {
            _slider = GetComponentInParent<Slider>();
            _slider.onValueChanged.AddListener(delegate { ChangeGalaxyGenInputValue(); });
        }

        public override void SetInput(GalaxyGeneratorInput input) {
            base.SetInput(input);
            _slider.maxValue = input.MaxValue;
            _slider.minValue = input.MinValue;
        }

        protected override void ChangeGalaxyGenInputValue() {
            Input.SetValue((int)_slider.value);
        }

        public void UpdateSelf(int value) {
            _slider.value = value;
        }

        public bool UpdateNeeded(int value) {
            return (int)_slider.value != value;
        }
    }
}