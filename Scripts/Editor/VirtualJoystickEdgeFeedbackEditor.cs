using UnityEngine;
using UnityEditor;

namespace Terresquall {

    [CustomEditor(typeof(VirtualJoystickEdgeFeedback))]
    public class VirtualJoystickEdgeFeedbackEditor : Editor {

        public override void OnInspectorGUI() {

            VirtualJoystickEditorLocalization.DrawLanguageSelector();
            EditorGUILayout.Space();

            serializedObject.Update();

            SerializedProperty property = serializedObject.GetIterator();

            if (property.NextVisible(true)) {
                do {
                    if (property.name == "m_Script") {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(property, true);
                        EditorGUI.EndDisabledGroup();
                        continue;
                    }

                    EditorGUILayout.PropertyField(
                        property,
                        VirtualJoystickEditorLocalization.EdgeFeedbackPropertyContent(property),
                        true
                    );

                } while (property.NextVisible(false));
            }

            serializedObject.ApplyModifiedProperties();

            VirtualJoystickEdgeFeedback edgeFeedback =
                (VirtualJoystickEdgeFeedback)target;

            if (!edgeFeedback.GetComponent<AudioSource>()) {
                EditorGUILayout.HelpBox(
                    VirtualJoystickEditorLocalization.EdgeFeedbackAudioWarning,
                    MessageType.Warning
                );
            }
        }
    }
}