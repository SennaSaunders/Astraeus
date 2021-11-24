using Code.Camera;
using Code.Galaxy;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(GalaxyGenerator))]
    public class GalaxyGeneratorEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            GalaxyGenerator generator = (GalaxyGenerator)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Generate Galaxy")) {
                generator.Start();
            }
        }
    }
}
