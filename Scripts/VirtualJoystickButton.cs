using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Terresquall {

    [RequireComponent(typeof(VirtualJoystick))]
    public class VirtualJoystickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        
        [Tooltip("How long you have to hold the button to register a button press.")]
        [Min(0)] public float holdDuration = 0.3f;
        [Tooltip("How much movement can you make to deregister the button press.")]
        [Range(0, 1)] public float maximumDelta = 0.5f;
        [Tooltip("The color that your joystick will be when the press is registered.")]
        public Color pressColor = Color.red;

        public bool isPressed { 
            get { 
                return currentHoldTime >= holdDuration && currentDelta < maximumDelta; 
            }
        }

        bool isHeld, lastIsPressed;
        float currentHoldTime, currentDelta;
        Vector2 lastPosition;

        VirtualJoystick joystick;

        internal static List<VirtualJoystickButton> instances = new List<VirtualJoystickButton>();

        void OnEnable() {

            // Tries to find the Joystick.
            joystick = GetComponent<VirtualJoystick>();
            if(!joystick) {
                enabled = false;
                Debug.LogError("Disabled the Virtual Joystick Button as the Virtual Joystick component is not found.");
            }

            instances.Insert(0, this);
        }
        void OnDisable()
        {
            instances.Remove(this);
        }

        // Update is called once per frame
        void Update() {
            if (isHeld) {
                currentHoldTime += Time.deltaTime;
                currentDelta += (joystick.axis - lastPosition).magnitude;
            }

            if (isPressed) joystick.controlStick.color = pressColor;
        }

        // Save the last held state.
        void LateUpdate() {
            lastIsPressed = isPressed;
        }

        // Hook this function to the Drag event of an EventTrigger.
        public void OnPointerDown(PointerEventData data) {
            isHeld = true;
            currentHoldTime = currentDelta = 0f;
        }

        public void OnPointerUp(PointerEventData data) {
            isHeld = false;
            currentHoldTime = currentDelta = 0f;
        }

        public static bool GetButtonDown(int index = 0) {
            return instances[index].isPressed && !instances[index].lastIsPressed;
        }

        public static bool GetButtonUp(int index = 0) {
            return !instances[index].isPressed && instances[index].lastIsPressed;
        }

        public static bool GetButton(int index = 0) {
            return instances[index].isPressed;
        }
    }
}