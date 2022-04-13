using Code.GUI.ObserverPattern;
using TMPro;

namespace Code.GUI.GalaxyGeneration {
    public class GalaxyGenInputController : GalaxyGenInputModifier, IItemObserver<int> {
        private TMP_InputField _inputField;

        private void Awake() {
            _inputField = GetComponent<TMP_InputField>();
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