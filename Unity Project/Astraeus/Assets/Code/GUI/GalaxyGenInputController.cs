using System;
using Code.ObserverPattern;
using TMPro;

namespace Code.GUI {
    public class GalaxyGenInputController : GalaxyGenInputModifier, IIntObserver {
        private TMP_InputField inputField;

        public void Awake() {
            inputField = GetComponentInParent<TMP_InputField>();
            inputField.onEndEdit.AddListener(delegate { ChangeGalaxyGenInputValue(); });
        }

        protected override void ChangeGalaxyGenInputValue() {
            long tempLong = Int64.Parse(inputField.text);
            int tempInt = Int32.MaxValue < tempLong ? Int32.MaxValue : Int32.MinValue > tempLong ? Int32.MinValue : (int)tempLong;
            UpdateSelf(tempInt);
            _input.SetValue(tempInt);
        }

        public void UpdateSelf(int value) {
            inputField.text = value.ToString();
        }

        public bool UpdateNeeded(int value) {
            if (Int32.Parse(inputField.text) != value) {
                return true;
            }

            return false;
        }
    }
}