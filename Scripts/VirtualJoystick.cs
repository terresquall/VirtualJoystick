﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Terresquall {

    [RequireComponent(typeof(Image),typeof(RectTransform))]
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        public Image controlStick;

        [Header("Debug")]
        public bool consolePrintAxis = false;
        public Text UITextPrintAxis;

        [Header("UI")]
        public Color dragColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        public Vector2 boundsPosition;
        [Range(0, 1)] public float boundsWidth;
        [Range(0, 1)] public float boundsHeight;

        [Header("Settings")]
        //[Tooltip("Sets the joystick back to its original position once it is let go of")] public bool snapToOrigin = false;
        public float sensitivity = 2f;
        public float radius = 30f;
        [Tooltip("Joystick only snaps when at the edge")] 
        public bool edgeSnap;
        [Tooltip("Number of directions of the joystick. " +
            "\nKeep at 0 for a free joystick. " +
            "\nWorks best with multiples of 4")]
        [Range(0, 16)] public int directions = 0;
        public enum DeadZoneType { 
            Radius, //0
            Value //1
        }
        public DeadZoneType deadZoneType;
        [HideInInspector] public float deadZoneRadius = 10f;
        [HideInInspector] public float deadZoneValue = 0.3f;

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
            controlStick.color = dragColor;
        }

        // Hook this to the EndDrag event of an EventTrigger.
        public void OnPointerUp(PointerEventData data) {
            desiredPosition = transform.position;
            controlStick.color = originalColor;
            currentPointerId = -2;

            //Snaps the joystick back to its original position
            /*if (snapToOrigin && (Vector2)transform.position != origin) {
                transform.position = origin;
                SetPosition(origin);
            }*/
        }

        protected void SetPosition(Vector2 position) {
            // Gets the difference in position between where we want to be,
            // and the center of the joystick.
            Vector2 diff = position - (Vector2)transform.position;

            //if no directions to snap to, joystick moves freely.
            if(directions <= 0){
                // Clamp the desired position within the radius.
                desiredPosition = (Vector2)transform.position + Vector2.ClampMagnitude(diff, radius);
            } else {
                // calculate nearest snap directional vectors
                Vector2 snapDirection = SnapDirection(diff.normalized, directions, 360 / directions * Mathf.Deg2Rad);
                if (diff.magnitude > radius) {
                    // Clamp the desired position within the radius and snapped to directional vector
                    desiredPosition = (Vector2)transform.position + snapDirection * radius;
                }
                else if (edgeSnap) {
                    desiredPosition = position;
                }
                else {
                    // Snaps to directional vector within the magnitude of the input position and the joystick
                    desiredPosition = (Vector2)transform.position + snapDirection * diff.magnitude;
                }
            }
        }

        // Calculates nearest directional snap vector to the actual directional vector of the joystick
        private Vector2 SnapDirection(Vector2 vector, int directions, float symmetryAngle) {
            //Gets the line of symmetry between 2 snap directions
            Vector2 symmetryLine = new Vector2(Mathf.Cos(symmetryAngle), Mathf.Sin(symmetryAngle));

            //Gets the angle between the joystick dir and the nearest snap dir
            float angle = Vector2.SignedAngle(symmetryLine, vector);

            // Divides the angle by the step size between directions, which is 180f / directions.
            // The result is that the angle is now expressed as a multiple of the step size between directions.
            angle /= 180f / directions;

            // Angle is then rounded to the nearest whole number so that it corresponds to one of the possible directions.
            angle = (angle >= 0f) ? Mathf.Floor(angle) : Mathf.Ceil(angle);

            // Checks if angle is odd
            if ((int)Mathf.Abs(angle) % 2 == 1) {
                // Adds or subtracts 1 to ensure that angle is always even.
                angle += (angle >= 0f) ? 1 : -1;
            }

            // Scale angle back to original scale as we divided it too make a multiple before.
            angle *= 180f / directions;
            angle *= Mathf.Deg2Rad;

            // Gets directional vector nearest to the joystick dir with a magnitude of 1.
            // Then multiplies it by the magnitude of the joytick vector.
            Vector2 result = new Vector2(Mathf.Cos(angle + symmetryAngle), Mathf.Sin(angle + symmetryAngle));
            result *= vector.magnitude;
            return result;
        }

        // Loops through children to find an appropriate component to put in.
        void Reset() {
            for(int i = 0; i < transform.childCount; i++) {
                // Once we find an appropriate Image component, abort.
                Image img = transform.GetChild(i).GetComponent<Image>();
                if(img) {
                    controlStick = img;
                    break;
                }
            }
        }

        // Function for us to modify the bounds value in future.
        public Rect GetBounds() {
            return new Rect(boundsPosition.x, boundsPosition.y, Screen.width * boundsWidth, Screen.height * boundsHeight);
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,radius);

            if(GetBounds().size.sqrMagnitude > 0) {
                // Draw the lines of the bounds.
                Gizmos.color = Color.yellow;

                // Get the 4 points in the bounds.
                Vector3 a = new Vector3(boundsPosition.x, boundsPosition.y),
                        b = new Vector3(boundsPosition.x, boundsPosition.y + Screen.currentResolution.height * boundsHeight),
                        c = new Vector2(boundsPosition.x + Screen.currentResolution.width * boundsWidth, boundsPosition.y + Screen.currentResolution.height * boundsHeight),
                        d = new Vector3(boundsPosition.x + Screen.currentResolution.width * boundsWidth, boundsPosition.y);
                Gizmos.DrawLine(a, b);
                Gizmos.DrawLine(b, c);
                Gizmos.DrawLine(c, d);
                Gizmos.DrawLine(d, a);
            }

            Gizmos.color = Color.green;
            if(deadZoneType == DeadZoneType.Radius)
                Gizmos.DrawWireSphere(transform.position, deadZoneRadius);
        }

        void Start() {
            origin = desiredPosition = transform.position;
            originalColor = controlStick.color;

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
            controlStick.transform.position = Vector2.MoveTowards(controlStick.transform.position, desiredPosition, sensitivity);

            // Also update the axis value.
            if(deadZoneType == DeadZoneType.Radius) {
                float magnitude = (controlStick.transform.position - transform.position).magnitude;

                // If distance of the joystick away from the centre is more than the deadzone radius then update axis.
                // Else set axis to 0
                axis = (magnitude > deadZoneRadius) ? (Vector2)(controlStick.transform.position - transform.position) / radius : Vector2.zero;

            } else if(deadZoneType == DeadZoneType.Value) {
                axis = (controlStick.transform.position - transform.position) / radius;
                if (Mathf.Round((Mathf.Abs(axis.x) + Mathf.Abs(axis.y)) * 100f) / 100f <= deadZoneValue) axis = Vector2.zero;               
            }

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
