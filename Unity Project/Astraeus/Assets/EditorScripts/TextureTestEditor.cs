using Code.TextureGen;
using UnityEditor;
using UnityEngine;

namespace EditorScripts {
    [CustomEditor(typeof(TextureTest))]
    public class TextureTestEditor : Editor {
        private TextureTest _textureTest;
        public override void OnInspectorGUI() {
            _textureTest = (TextureTest)target;

            if (GUILayout.Button("Gen Planet")) {
                _textureTest.GenPlanetTexture();
            }
            
            base.OnInspectorGUI();
        }
    }
}