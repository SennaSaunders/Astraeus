using Code.Saves;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(GameSave))]
    public class GameSaveEditor : UnityEditor.Editor {
        private GameSave _gameSave;
        public override void OnInspectorGUI() {
            _gameSave = (GameSave)target;
            if (GUILayout.Button("Save")) {
                _gameSave.SaveGame();
            }
            base.OnInspectorGUI();
        }
    }
}