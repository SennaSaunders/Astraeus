using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(TextureTest))]
    public class TextureTestEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            TextureTest texTest = (TextureTest)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Gen Tex")) {
                texTest.Start();
            }
        }
    }
}