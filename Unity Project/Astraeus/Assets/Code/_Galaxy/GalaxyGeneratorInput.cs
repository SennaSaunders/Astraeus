using System;
using System.Collections.Generic;
using System.Linq;
using Code.ObserverPattern;

namespace Code._Galaxy {
    [Serializable]
    public class GalaxyGeneratorInput : ISubject{
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

        private List<IIntObserver> _observers = new List<IIntObserver>();

        public void AddObserver(IIntObserver intObserver) {
            _observers.Add(intObserver);
        }

        public void RemoveObserver(IIntObserver intObserver) {
            _observers.Remove(intObserver);
        }

        public void NotifyObservers() {
            foreach (var observer in _observers.Where(observer => observer.UpdateNeeded(Value))) {
                observer.UpdateSelf(Value);
            }
        }
    }
}