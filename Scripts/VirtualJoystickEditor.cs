using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Terresquall {
    [CustomEditor(typeof(VirtualJoystick))]
    public class VirtualJoystickEditor : Editor {
        // Links to the script object.
        VirtualJoystick joystick;
        RectTransform rectTransform;

        private int scaleFactor;

        void OnEnable() {
            joystick = target as VirtualJoystick;
            rectTransform = joystick.GetComponent<RectTransform>();
        }

        override public void OnInspectorGUI() {
            DrawDefaultInspector();

            //Changes whether the deadzone is calculated based on Value or Radius
            switch (joystick.deadZoneType) {
                case VirtualJoystick.DeadZoneType.Radius:
                    joystick.deadZoneRadius = EditorGUILayout.FloatField("Dead Zone Area", joystick.deadZoneRadius);
                    break;

                case VirtualJoystick.DeadZoneType.Value:
                    joystick.deadZoneValue = EditorGUILayout.FloatField("Dead Zone Value", joystick.deadZoneValue);
                    break;
            }

            //Increase Decrease buttons
            EditorGUI.BeginChangeCheck();
            GUILayout.Space(12);
            EditorGUILayout.LabelField("Joystick Size:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            int gcd = Mathf.RoundToInt(FindGCD((int)rectTransform.sizeDelta.x, (int)joystick.controlStick.rectTransform.sizeDelta.x, (int)joystick.radius, (int)joystick.deadZoneRadius));
            if (GUILayout.Button("Increase Size", EditorStyles.miniButtonLeft)) {
                RecordSizeChangeUndo(rectTransform, joystick, joystick.controlStick, joystick.controlStick.rectTransform);
                rectTransform.sizeDelta += rectTransform.sizeDelta / new Vector2(gcd, gcd);
                joystick.controlStick.rectTransform.sizeDelta += joystick.controlStick.rectTransform.sizeDelta / new Vector2(gcd, gcd);
                joystick.radius +=joystick.radius / gcd;
                joystick.deadZoneRadius += joystick.deadZoneRadius/gcd;
            }
            if (GUILayout.Button("Decrease Size", EditorStyles.miniButtonRight)) {
                RecordSizeChangeUndo(rectTransform, joystick, joystick.controlStick, joystick.controlStick.rectTransform);
                rectTransform.sizeDelta -= rectTransform.sizeDelta / new Vector2(gcd, gcd);
                joystick.controlStick.rectTransform.sizeDelta -= joystick.controlStick.rectTransform.sizeDelta / new Vector2(gcd, gcd);
                joystick.radius -= joystick.radius / gcd;
                joystick.deadZoneRadius -= joystick.deadZoneRadius / gcd;
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

            if (EditorGUI.EndChangeCheck())
            {

            }
        }

        // Function to return gcd of a and b
        int GCD(int a, int b)
        {
            if (a == 0)
                return b;
            return GCD(b % a, a);
        }

        // Function to find gcd of array of numbers
        int FindGCD(params int[] numbers)
        {
            int result = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                result = GCD(numbers[i], result);

                if (result == 1)
                {
                    return 1;
                }
            }
            return result;
        }

        void RecordSizeChangeUndo(params Object[] arguments)
        {
            for(int i = 0; i < arguments.Length; i++)
            {
                Undo.RecordObject(arguments[i], "Undo Stuff");
            }
        }
    }
}
