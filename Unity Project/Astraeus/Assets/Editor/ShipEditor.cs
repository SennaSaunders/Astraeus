using Code._Ships.Controllers;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(ShipController))]
    
    public class ShipEditor : UnityEditor.Editor {
        private ShipController _shipController;
        public override void OnInspectorGUI() {
            _shipController = (ShipController)target;
            if (GUILayout.Button("DestroyShip")) {
                _shipController.TakeDamage(10000000);
            }
            DrawDefaultInspector();
        }
    }
}