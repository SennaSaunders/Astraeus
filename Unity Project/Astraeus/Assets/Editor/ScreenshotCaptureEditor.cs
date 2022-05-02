using Code._Utility;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(ScreenshotCapture))]
    public class ScreenshotCaptureEditor : UnityEditor.Editor {
        private static ScreenshotCapture _screenshotCapture;

        public override void OnInspectorGUI() {
            _screenshotCapture = (ScreenshotCapture)target;
            if (GUILayout.Button("Take Screenshot")) {
                _screenshotCapture.TakeScreenshot();
            }
            
            base.OnInspectorGUI();
        }
    }
}