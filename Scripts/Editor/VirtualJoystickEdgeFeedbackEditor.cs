using UnityEngine;
using UnityEditor;

namespace Terresquall {

    [CustomEditor(typeof(VirtualJoystickEdgeFeedback))]
    public class VirtualJoystickEdgeFeedbackEditor : Editor {

        public override void OnInspectorGUI() {
            // Draw the default inspector
            DrawDefaultInspector();

            // Get the target component
            VirtualJoystickEdgeFeedback edgeFeedback = (VirtualJoystickEdgeFeedback)target;

            // Check if AudioSource is not detected and display a warning message
            if (!edgeFeedback.GetComponent<AudioSource>()) {
                EditorGUILayout.HelpBox("If you would like your feedback to include sound, attach an Audio Source to this GameObject and assign a clip to the Audio Source component. Otherwise, you can ignore this message.", MessageType.Warning);
            }
        }
    }
}