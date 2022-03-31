using UnityEngine;

namespace Code._Utility {
    public class ScreenshotCapture : MonoBehaviour {
        public string screenshotName = "Ship";
        public int scale = 5;
        public void TakeScreenshot() {
            ScreenCapture.CaptureScreenshot(screenshotName + " Screenshot.png", scale);
        }
    }
}