using System;
using UnityEngine;

namespace Code.Camera {
    public class ShipyardCameraController : CameraController {
        private float zOffset = 0;

        public void SetCameraPos(RectTransform guiRect) {
            //calculate the camera depth
            //if width is of gui rect
            //then the depth required is:
            // Tan(angle) = opp/adj so: opp/tan(angle) = adj
            //angle is fov/2 - opp is guiWidth/2
            var mainCamera = UnityEngine.Camera.main;
            var guiWidth = guiRect.rect.width;
            var vFov = UnityEngine.Camera.VerticalToHorizontalFieldOfView(mainCamera.fieldOfView, mainCamera.aspect);
            var angleRads = ((vFov / 2) * Math.PI) / 180;
            float depth = (float)((guiWidth / 2) / Math.Tan(angleRads));
            //scaled depth = depth * pixelWidth/guiWidth
            var resolutionWidth = mainCamera.pixelWidth;
            var scaledDepth = depth * (resolutionWidth / guiWidth);
            var guiCenter = guiRect.TransformPoint(new Vector3());
            guiCenter.z = -scaledDepth;
            transform.position = guiCenter;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}