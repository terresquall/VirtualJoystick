using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lesson2 {

    [RequireComponent(typeof(EventTrigger),typeof(Image),typeof(RectTransform))]
    public class VirtualJoystick : MonoBehaviour {

        public Image control;

        [Header("Settings")]
        public float sensitivity = 2f;
        public float radius = 30f;

        [Header("UI")]
        public Color dragColor = new Color(0.9f,0.9f,0.9f,1f);

        [Header("Debug")]
        public bool consolePrintAxis = false;
        public Text UITextPrintAxis;

        // Private variables.
        Vector2 desiredPosition, axis;
        Color originalColor; // Stores the original color of the Joystick.

        public static VirtualJoystick instance;

        public const string VERSION = "0.1.0";
        public const string DATE = "25 April 2022";

        public static float GetAxis(string axe) {
            switch(axe.ToLower()) {
                case "horizontal": case "h": case "x":
                    return instance.axis.x;
                case "vertical": case "v": case "y":
                    return instance.axis.y;
            }
            return 0;
        }

        public static Vector2 GetAxis() { return instance.axis; }

        // Hook this function to the Drag event of an EventTrigger.
        public void ReceiveDrag(BaseEventData data) {
            // Make sure the data is a PointerEventData.
            if(data is PointerEventData) {
                PointerEventData p = data as PointerEventData;
                SetPosition(p.position);
                control.color = dragColor;
            } else {
                // Otherwise warn.
                Debug.LogWarning("Data sent in is not a PointerEventData.");
            }
        }

        // Hook this to the EndDrag event of an EventTrigger.
        public void ReleaseDrag(BaseEventData data) {
            desiredPosition = transform.position;
            control.color = originalColor;
        }

        protected void SetPosition(Vector2 position) {
            // Gets the difference in position between where we want to be,
            // and the center of the joystick.
            Vector2 diff = position - (Vector2)transform.position;

            // If the difference is greater than radius, that means
            // we are going too far.
            if(diff.magnitude > radius) {
                // Clamp the desired position within the radius.
                desiredPosition = (Vector2)transform.position + Vector2.ClampMagnitude(diff, radius);
            } else {
                desiredPosition = position;
            }
        }

        // Loops through children to find an appropriate component to put in.
        void Reset() {
            for(int i = 0; i < transform.childCount; i++) {
                // Once we find an appropriate Image component, abort.
                Image img = transform.GetChild(i).GetComponent<Image>();
                if(img) {
                    control = img;
                    break;
                }
            }
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,radius);
        }

        void Start() {
            desiredPosition = transform.position;
            originalColor = control.color;

            // Show error message if there are too many copies of the Joystick around.
            if(instance) {
                Debug.LogError("You have more than 1 instance of VirtualJoystick on the Scene. The component will not work properly.");
            } else {
                instance = this;
            }
        }

        void Update() {
            // Update the position of the joystick.
            control.transform.position = Vector2.MoveTowards(control.transform.position, desiredPosition, sensitivity);

            // Also update the axis value.
            axis = (control.transform.position - transform.position) / radius;

            // If debug is on, output to selected channel.
            string output = string.Format("Virtual Joystick: {0}", axis);
            if(consolePrintAxis) Debug.Log(output);
            if(UITextPrintAxis) UITextPrintAxis.text = output;
        }
    }
}
