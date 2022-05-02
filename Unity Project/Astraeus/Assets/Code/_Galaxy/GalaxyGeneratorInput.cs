using System;
using System.Collections.Generic;
using System.Linq;
using Code.GUI.ObserverPattern;

namespace Code._Galaxy {
    [Serializable]
    public class GalaxyGeneratorInput : ISubject<IItemObserver<int>>{
        public int MinValue { get; private set; }
        public int MaxValue{ get; private set; }

        public int Value { get; private set; }
        public GalaxyGeneratorInput(int value) {
            Value = value;
            MaxValue = Int32.MaxValue;
            MinValue = Int32.MinValue;
        }

        public GalaxyGeneratorInput(int minValue, int maxValue, int value) {
            MinValue = minValue;
            MaxValue = maxValue;
            SetValue(value);
        }

        public void SetValue(int newValue) {
            Value = newValue <= MaxValue ? newValue >= MinValue ? newValue : MinValue : MaxValue;
            NotifyObservers();
        }
        
        public int GetValue() {
            return Value;
        }

        private List<IItemObserver<int>> _observers = new List<IItemObserver<int>>();

        public void AddObserver(IItemObserver<int> observer) {
            _observers.Add(observer);
        }

        public void RemoveObserver(IItemObserver<int> observer) {
            _observers.Remove(observer);
        }

        public void ClearObservers() {
            _observers = new List<IItemObserver<int>>();
        }

        public void NotifyObservers() {
            // foreach (var observer in _observers.Where(observer => observer.UpdateNeeded(Value))) {
            foreach (var observer in _observers) {
                observer.UpdateSelf(Value);
            }
        }
    }
}