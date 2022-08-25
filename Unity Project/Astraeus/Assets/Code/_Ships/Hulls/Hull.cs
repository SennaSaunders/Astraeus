using System.Collections.Generic;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.InternalComponents;
using Code.Camera;
using Code.GUI.ObserverPattern;
using UnityEngine;

namespace Code._Ships.Hulls {
    //ship blueprint for the components allowed on a particular hull 
    public abstract class Hull : ISubject<IItemObserver<float>> {
        protected Hull(string name, Vector3 outfittingPosition, float shipyardScale, Vector3 healthGUIOffset, float hullMass, float minTurnAccel, float maxTurnAccel, float hullStrength, int hullPrice) {
            HullName = name;
            OutfittingPosition = new Vector3(outfittingPosition.x, outfittingPosition.y, outfittingPosition.z + OutfittingCameraController.ZOffset);
            ShipyardScale = shipyardScale;
            HealthGUIOffset = healthGUIOffset;
            HullMass = hullMass;
            _minTurnAccel = minTurnAccel;
            _maxTurnAccel = maxTurnAccel;
            BaseHullStrength = hullStrength;
            CurrentHullStrength = hullStrength;
            HullPrice = hullPrice;
            SetupHull();
        }

        public string HullName { get; }
        public float BaseHullStrength { get; }
        public float HullMass { get; }
        private readonly float _minTurnAccel;
        private readonly float _maxTurnAccel;
        public int HullPrice { get; }
        public float CurrentHullStrength { get; set; }
        protected const string BaseHullPath = "Ships/Hulls/";
        
        public List<(ShipComponentType componentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)> InternalComponents;
        public List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> WeaponComponents;
        public List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName, bool needsBracket)> MainThrusterComponents;
        public (ShipComponentType componentType, ShipComponentTier maxSize, ManoeuvringThruster concreteComponent, string selectionTransformName, List<string> parentTransformNames) ManoeuvringThrusterComponents;
        public List<List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName, bool needsBracket)>> TiedThrustersSets = new List<List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName, bool needsBracket)>>();
        public List<(GameObject mesh, int channelIdx)> MeshObjects;
        public List<(List<string> objectName, Color colour)> ColourChannelObjectMap;
        private List<IItemObserver<float>> _observers = new List<IItemObserver<float>>();

        
        public Vector3 OutfittingPosition { get; }
        public float ShipyardScale { get; }
        public Vector3 HealthGUIOffset { get; }
        public Quaternion OutfittingRotation { get; } = Quaternion.Euler(50, 0, -30);

        public abstract string GetHullFullPath();

        private void SetupHull() {
            SetWeaponComponents();
            SetThrusterComponents();
            SetInternalComponents();
        }

        protected abstract void SetWeaponComponents();
        protected abstract void SetThrusterComponents();
        protected abstract void SetInternalComponents();

        public void TakeDamage(float damage) {
            CurrentHullStrength -= damage;
            NotifyObservers();
        }

        public void AddObserver(IItemObserver<float> observer) {
            _observers.Add(observer);
        }

        public void RemoveObserver(IItemObserver<float> observer) {
            _observers.Remove(observer);
        }

        public void ClearObservers() {
            _observers = new List<IItemObserver<float>>();
        }

        public void NotifyObservers() {
            float value = CurrentHullStrength / BaseHullStrength;
            foreach (IItemObserver<float> observer in _observers) {
                observer.UpdateSelf(value);
            }
        }

        public float CalcAngularAcceleration(ShipComponentTier manThrusterTier) {
            return ((float)manThrusterTier / (float)ManoeuvringThrusterComponents.maxSize) * _maxTurnAccel + _minTurnAccel;
        }
    }
}