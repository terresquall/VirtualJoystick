using UnityEngine;
using UnityEditor;

namespace Terresquall {
    [CustomEditor(typeof(VirtualJoystick))]
    public class VirtualJoystickEditor : Editor {    //    // Links to the script object.

        VirtualJoystick joystick;
        RectTransform rectTransform;
        Canvas canvas;

        private int scaleFactor;

        void OnEnable() {
            joystick = target as VirtualJoystick;
            rectTransform = joystick.GetComponent<RectTransform>();
            canvas = joystick.GetComponentInParent<Canvas>();
        }

        override public void OnInspectorGUI() {

            // Show an error text box if the new Input System is being used.
            try {
                Vector2 v = Input.mousePosition;
            } catch(System.InvalidOperationException e) {

                Texture2D icon = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D;

                // Create a horizontal layout for the icon and text
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    // Display the icon
                    GUILayout.Label(icon, GUILayout.Width(40), GUILayout.Height(40));

                    // Display the text
                    GUIStyle ts = new GUIStyle(EditorStyles.label);
                    ts.wordWrap = true;
                    ts.richText = true;
                    ts.alignment = TextAnchor.MiddleLeft;
                    ts.fontSize = EditorStyles.helpBox.fontSize;
                    GUILayout.Label("<b>This component does not work with the new Input System.</b> You will need to re-enable the old Input System by going to <b>Project Settings > Player > Other Settings > Active Input Handling</b> and selecting <b>Both</b>.", ts);
                }
                GUILayout.EndHorizontal();
            }

            // Draw a help text box if this is not attached to a Canvas.
            if(!canvas) {
                EditorGUILayout.HelpBox("This GameObject needs to be parented to a Canvas, or it won't work!", MessageType.Warning);
            }

            DrawDefaultInspector();

            //Increase Decrease buttons
            if(joystick) {

                if(!joystick.controlStick) {
                    EditorGUILayout.HelpBox("There is no Control Stick assigned. This joystick won't work.", MessageType.Warning);
                    return;
                }

                // Print out the Increase / Decrease Size buttons.
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Size Adjustments");
                GUILayout.BeginHorizontal();
                int gcd = Mathf.RoundToInt(FindGCD((int)rectTransform.sizeDelta.x, (int)joystick.controlStick.rectTransform.sizeDelta.x));
                if (GUILayout.Button("Increase Size", EditorStyles.miniButtonLeft)) {
                    RecordSizeChangeUndo(rectTransform, joystick, joystick.controlStick, joystick.controlStick.rectTransform);
                    rectTransform.sizeDelta += rectTransform.sizeDelta / new Vector2(gcd, gcd);
                    joystick.controlStick.rectTransform.sizeDelta += joystick.controlStick.rectTransform.sizeDelta / new Vector2(gcd, gcd);
                }
                if (GUILayout.Button("Decrease Size", EditorStyles.miniButtonRight)) {
                    RecordSizeChangeUndo(rectTransform, joystick, joystick.controlStick, joystick.controlStick.rectTransform);
                    rectTransform.sizeDelta -= rectTransform.sizeDelta / new Vector2(gcd, gcd);
                    joystick.controlStick.rectTransform.sizeDelta -= joystick.controlStick.rectTransform.sizeDelta / new Vector2(gcd, gcd);
                }
                GUILayout.EndHorizontal();
            }

            ////Boundaries Stuff
            //GUILayout.Space(15);
            //EditorGUILayout.LabelField("Boundaries:", EditorStyles.boldLabel);
            //joystick.snapsToTouch = EditorGUILayout.Toggle("Snap to Touch", joystick.snapsToTouch);

            //EditorGUILayout.LabelField("Boundaries");
            //EditorGUIUtility.labelWidth = 15;
            //GUILayout.BeginHorizontal();
            //joystick.boundaries.x = EditorGUILayout.Slider("X", joystick.boundaries.x, 0, 1);
            //joystick.boundaries.y = EditorGUILayout.Slider("Y", joystick.boundaries.y, 0, 1);
            //GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();
            //joystick.boundaries.width = EditorGUILayout.Slider("W", joystick.boundaries.width, 0, 1);
            //joystick.boundaries.height = EditorGUILayout.Slider("H", joystick.boundaries.height, 0, 1);
            //GUILayout.EndHorizontal();

            ////Bounds Anchor buttons
            //GUILayout.Space(3);
            //EditorGUILayout.LabelField("Bounds Anchor:", EditorStyles.boldLabel);
            //GUILayout.BeginHorizontal();
            //if (GUILayout.Button("Top Left", EditorStyles.miniButtonLeft))
            //{

            //}
            //if (GUILayout.Button("Top Right", EditorStyles.miniButtonRight))
            //{

            //}
            //GUILayout.EndHorizontal();

            //if (GUILayout.Button("Middle"))
            //{

            //}

            //GUILayout.BeginHorizontal();
            //if (GUILayout.Button("Bottom Left", EditorStyles.miniButtonLeft))
            //{

            //}
            //if (GUILayout.Button("Bottom Right", EditorStyles.miniButtonRight))
            //{

            //}
            //GUILayout.EndHorizontal();

            //if (EditorGUI.EndChangeCheck())
            //{

            //}
        }

        // Function to return gcd of a and b
        int GCD(int a, int b) {
            if (b == 0) return a;
            return GCD(b, a % b);
        }

        // Function to find gcd of array of numbers
        int FindGCD(params int[] numbers) {
            if (numbers.Length == 0) {
                Debug.LogError("No numbers provided");
                return 0; // or handle the error in an appropriate way
            }

            int result = numbers[0];
            for (int i = 1; i < numbers.Length; i++) {

                result = GCD(result, numbers[i]);

                if (result == 1) {
                    return 1;
                } else if (result <= 0) {
                    Debug.LogError("The size value for one or more of the Joystick elements is not more than 0");
                    // You might want to handle this error in an appropriate way
                    return 0; // or handle the error in an appropriate way
                }
            }
            return result;
        }

        void RecordSizeChangeUndo(params Object[] arguments) {
            for (int i = 0; i < arguments.Length; i++) {
                Undo.RecordObject(arguments[i], "Undo Stuff");
            }
        }
    }
}
