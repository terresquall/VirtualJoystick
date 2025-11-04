#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Terresquall {
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [Preserve]
    [InputControlLayout(displayName = "Virtual Joystick")]
    public class VirtualJoystickDevice : Joystick {

#if UNITY_EDITOR
        static VirtualJoystickDevice() {
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            InputSystem.RegisterLayout<VirtualJoystickDevice>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AddDeviceIfMissing() {
            if (InputSystem.GetDevice<VirtualJoystickDevice>() == null) {
                InputSystem.AddDevice<VirtualJoystickDevice>();
                Debug.Log("[VirtualJoystickDevice] Added virtual joystick device");
            }
        }
    }
}
#endif