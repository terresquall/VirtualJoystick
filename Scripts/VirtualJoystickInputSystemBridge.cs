using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
#endif

namespace Terresquall
{
#if ENABLE_INPUT_SYSTEM

    [AddComponentMenu(
        "Terresquall/Virtual Joystick Input System Bridge"
    )]
    public class VirtualJoystickInputSystemBridge
        : OnScreenControl
    {
        [InputControl(layout = "Vector2")]
        [SerializeField]
        [Tooltip(
            "The Input System control that receives " +
            "the Virtual Joystick axis."
        )]
        private string m_ControlPath =
            "<Gamepad>/leftStick";

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }

        public void SendAxis(Vector2 value)
        {
            if (!isActiveAndEnabled)
                return;

            SendValueToControl(value);
        }
    }

#else

    // Allows the asset to compile when the
    // New Input System is disabled.
    public class VirtualJoystickInputSystemBridge
        : MonoBehaviour
    {
        public void SendAxis(Vector2 value)
        {
        }
    }

#endif
}