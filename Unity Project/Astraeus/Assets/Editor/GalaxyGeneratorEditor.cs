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
                CameraController.SetupCameraBounds(generator.width,generator.height);
                GameObject.FindObjectOfType<CameraController>().SetCamera();
                generator.ShowGalaxy(generator.GenGalaxy());
            }
        }
    }
}
