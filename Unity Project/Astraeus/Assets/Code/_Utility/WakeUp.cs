using System;
using UnityEngine;

namespace Code._Utility {
    public class WakeUp :MonoBehaviour{
        public Action wakeFunction;
        private void OnEnable() {
            if (wakeFunction!=null) {
                wakeFunction.Invoke();
            }
        }
    }
}