using Code.Camera;
using Code.Galaxy;
using Code.TextureGen.NoiseGeneration;
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
            if (GUILayout.Button("Test")) {
                NoiseGenerator.GetNoise(256, 1337);
            }
        }
    }
}
