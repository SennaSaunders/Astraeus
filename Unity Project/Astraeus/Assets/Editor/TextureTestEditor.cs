using Code.TextureGen;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(TextureTest))]
    public class TextureTestEditor : UnityEditor.Editor {
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