using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Terresquall {

    [RequireComponent(typeof(Image),typeof(RectTransform))]
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        public Image control;

        [Header("Settings")]
        public float sensitivity = 2f;
        public float radius = 30f;
        public Rect bounds;

        [Header("UI")]
        public Color dragColor = new Color(0.9f,0.9f,0.9f,1f);

        [Header("Debug")]
        public bool consolePrintAxis = false;
        public Text UITextPrintAxis;

        // Private variables.
        Vector2 desiredPosition, axis, origin;
        Color originalColor; // Stores the original color of the Joystick.
        int currentPointerId = -2;

        public static List<VirtualJoystick> instances = new List<VirtualJoystick>();

        public const string VERSION = "0.2.0";
        public const string DATE = "30 April 2023";

        public static float GetAxis(string axe, int index = 0) {
            switch(axe.ToLower()) {
                case "horizontal": case "h": case "x":
                    return instances[index].axis.x;
                case "vertical": case "v": case "y":
                    return instances[index].axis.y;
            }
            return 0;
        }

        public static Vector2 GetAxis(int index = 0) { return instances[index].axis; }

        public static float GetAxisRaw(string axe, int index = 0) {
            return Mathf.Sign(GetAxis(axe,index));
        }

        public static Vector2 GetAxisRaw(int index = 0) {
            return new Vector2(
                Mathf.Sign(instances[index].axis.x),
                Mathf.Sign(instances[index].axis.y)
            );
        }

        // Hook this function to the Drag event of an EventTrigger.
        public void OnPointerDown(PointerEventData data) {
            currentPointerId = data.pointerId;
            SetPosition(data.position);
            control.color = dragColor;
        }

        // Hook this to the EndDrag event of an EventTrigger.
        public void OnPointerUp(PointerEventData data) {
            desiredPosition = transform.position;
            control.color = originalColor;
            currentPointerId = -2;
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

        // Function for us to modify the bounds value in future.
        Rect GetBounds() {
            return new Rect(origin - bounds.size / 2, bounds.size);
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,radius);

            if(bounds.size.sqrMagnitude > 0) {
                // Draw the lines of the bounds.
                Gizmos.color = Color.yellow;
                float hw = bounds.width * 0.5f, hh = bounds.height * 0.5f;

                // Get the 4 points in the bounds.
                Vector3 a = transform.position + new Vector3(bounds.x - hw, bounds.y - hh),
                        b = transform.position + new Vector3(bounds.x + hw, bounds.y - hh),
                        c = transform.position + new Vector3(bounds.x + hw, bounds.y + hh),
                        d = transform.position + new Vector3(bounds.x - hw, bounds.y + hh);
                Gizmos.DrawLine(a, b);
                Gizmos.DrawLine(b, c);
                Gizmos.DrawLine(c, d);
                Gizmos.DrawLine(d, a);
            }
        }

        void Start() {
            origin = desiredPosition = transform.position;
            originalColor = control.color;

            // Add this instance to the List.
            instances.Insert(0, this);
        }

        void Update() {
            PositionUpdate();

            // If the currentPointerId > -2, we are being dragged.
            if(currentPointerId > -2) {
                // If this is more than -1, the Joystick is manipulated by touch.
                if(currentPointerId > -1) {
                    // We need to loop through all touches to find the one we want.
                    for(int i = 0; i < Input.touchCount; i++) {
                        Touch t = Input.GetTouch(i);
                        if(t.fingerId == currentPointerId) {
                            SetPosition(t.position);
                            break;
                        }
                    }
                } else {
                    // Otherwise, we are being manipulated by the mouse position.
                    SetPosition(Input.mousePosition);
                }
            }

            // Update the position of the joystick.
            control.transform.position = Vector2.MoveTowards(control.transform.position, desiredPosition, sensitivity);

            // Also update the axis value.
            axis = (control.transform.position - transform.position) / radius;

            // If debug is on, output to selected channel.
            string output = string.Format("Virtual Joystick: {0}", axis);
            if(consolePrintAxis) Debug.Log(output);
            if(UITextPrintAxis) UITextPrintAxis.text = output;
        }

        void PositionUpdate() {

            if(Input.touchCount > 0) {
                // Also detect touch events too.
                for(int i = 0; i < Input.touchCount; i++) {
                    Touch t = Input.GetTouch(i);
                    switch(t.phase) {
                        case TouchPhase.Began:
                            if(GetBounds().Contains(t.position) && currentPointerId < -1) {
                                Uproot(t.position, t.fingerId);
                                return;
                            }
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            if(currentPointerId == t.fingerId)
                                OnPointerUp(new PointerEventData(EventSystem.current));
                            break;
                    }
                }

            } else if(Input.GetMouseButtonDown(0) && currentPointerId < -1) {
                if(GetBounds().Contains(Input.mousePosition)) {
                    Uproot(Input.mousePosition);
                }
            }
            
            if(Input.GetMouseButtonUp(0) && currentPointerId == -1) {
                OnPointerUp(new PointerEventData(EventSystem.current));
            }
        }

        // Roots the joystick to a new position.
        public void Uproot(Vector2 newPos, int newPointerId = -1) {
            // Don't move the joystick if we are not tapping too far from it.
            if(Vector2.Distance(transform.position, newPos) < radius) return;

            // Otherwise move the virtual joystick to where we clicked.
            transform.position = newPos;
            desiredPosition = transform.position;
                    
            // Artificially trigger the drag event.
            PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = newPos;
            data.pointerId = newPointerId;
            OnPointerDown(data);
        }

    }
}
