using UnityEngine;
using UnityEditor;

namespace Terresquall {
    [CustomEditor(typeof(VirtualJoystick))]
    public class VirtualJoystickEditor : Editor {
        // Links to the script object.
        VirtualJoystick joystick;
        RectTransform rectTransform;

        void OnEnable() {
            joystick = target as VirtualJoystick;
            rectTransform = joystick.GetComponent<RectTransform>();
        }

        override public void OnInspectorGUI() {
            DrawDefaultInspector();

            //Changes whether the deadzone is calculated based on Value or Radius
            switch (joystick.deadZoneType) {
                case VirtualJoystick.DeadZoneType.Radius:
                    joystick.deadZoneRadius = EditorGUILayout.FloatField("Dead Zone Area:", joystick.deadZoneRadius);
                    break;

                case VirtualJoystick.DeadZoneType.Value:
                    joystick.deadZoneValue = EditorGUILayout.FloatField("Dead Zone Value:", joystick.deadZoneValue);
                    break;
            }

            //Increase Decrease buttons
            GUILayout.Space(12);
            EditorGUILayout.LabelField("Joystick Size:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Increase Size", EditorStyles.miniButtonLeft)) {
                rectTransform.sizeDelta += new Vector2(10,10);
                joystick.controlStick.rectTransform.sizeDelta += new Vector2(7,7);
                joystick.radius += 10;
                joystick.deadZoneRadius += 3;
            }
            if (GUILayout.Button("Decrease Size", EditorStyles.miniButtonRight)) {
                rectTransform.sizeDelta -= new Vector2(10, 10);
                rectTransform.sizeDelta -= new Vector2(7, 7);
                joystick.radius -= 10;
                joystick.deadZoneRadius -= 3;
            }
            GUILayout.EndHorizontal();            
            
            //Bounds Anchor buttons
            GUILayout.Space(12);
            EditorGUILayout.LabelField("Bounds Anchor:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Top Left", EditorStyles.miniButtonLeft)) {

            }
            if (GUILayout.Button("Top Right", EditorStyles.miniButtonRight)) {
 
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Middle")) {

            }
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Bottom Left", EditorStyles.miniButtonLeft)) {

            }
            if (GUILayout.Button("Bottom Right", EditorStyles.miniButtonRight)) {

            }
            GUILayout.EndHorizontal();
        }
    }
}
