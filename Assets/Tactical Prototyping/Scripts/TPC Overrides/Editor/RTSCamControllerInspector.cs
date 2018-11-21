using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Opsive.UltimateCharacterController;
using Opsive.UltimateCharacterController.Editor;
using Opsive.UltimateCharacterController.Editor.Inspectors.Camera;

namespace RTSPrototype
{
    [CustomEditor(typeof(RTSCameraController))]
    public class RTSCamControllerInspector : CameraControllerInspector
    {
        
    }
}