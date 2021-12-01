using System;
using System.Collections.Generic;
using System.ComponentModel;
using Code.GUI;
using Code.ObserverPattern;

namespace Code.Galaxy {
    [Serializable]
    public class GalaxyGeneratorInput : ISubject{
        public int MinValue { get; private set; }
        public int MaxValue{ get; private set; }

        public int Value { get; private set; }
        public GalaxyGeneratorInput(int value) {
            this.Value = value;
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
            foreach (IIntObserver observer in _observers) {
                if (observer.UpdateNeeded(Value)) {
                    observer.UpdateSelf(Value);
                }
            }
        }
    }
}