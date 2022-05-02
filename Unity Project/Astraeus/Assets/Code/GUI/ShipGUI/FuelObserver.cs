using Code._Utility;
using Code.GUI.ObserverPattern;
using UnityEngine;

namespace Code.GUI.ShipGUI {
    public class FuelObserver :MonoBehaviour, IItemObserver<int> {
        public void UpdateSelf(int value) {
            GameObjectHelper.SetGUITextValue(gameObject, "FuelValue", value.ToString());
        }
    }
}