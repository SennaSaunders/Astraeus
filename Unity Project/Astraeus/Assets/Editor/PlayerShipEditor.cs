using Code._Ships.Controllers;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(PlayerShipController))]
    
    public class PlayerShipEditor : UnityEditor.Editor {
        private PlayerShipController _playerShipController;
        public override void OnInspectorGUI() {
            _playerShipController = (PlayerShipController)target;
            if (GUILayout.Button("DestroyShip")) {
                _playerShipController.TakeDamage(10000000);
            }
            DrawDefaultInspector();
        }
    }
}