using Code.ObserverPattern;
using TMPro;

namespace Code.GUI.GalaxyGeneration {
    public class GalaxyGenInputController : GalaxyGenInputModifier, IIntObserver {
        private TMP_InputField _inputField;

        public void Awake() {
            _inputField = GetComponentInParent<TMP_InputField>();
            _inputField.onEndEdit.AddListener(delegate { ChangeGalaxyGenInputValue(); });
        }

        protected override void ChangeGalaxyGenInputValue() {
            long tempLong = long.Parse(_inputField.text);
            int tempInt = int.MaxValue < tempLong ? int.MaxValue : int.MinValue > tempLong ? int.MinValue : (int)tempLong;
            UpdateSelf(tempInt);
            Input.SetValue(tempInt);
        }

        public void UpdateSelf(int value) {
            _inputField.text = value.ToString();
        }

        public bool UpdateNeeded(int value) {
            return int.Parse(_inputField.text) != value;
        }
    }
}