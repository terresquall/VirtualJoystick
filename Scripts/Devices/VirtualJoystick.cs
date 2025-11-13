#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Terresquall.Devices {
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [Preserve]
    [InputControlLayout(displayName = "Virtual Joystick")]
    public class VirtualJoystick : Joystick {

#if UNITY_EDITOR
        static VirtualJoystick() {
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            InputSystem.RegisterLayout<VirtualJoystick>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AddDeviceIfMissing() {
            if (InputSystem.GetDevice<VirtualJoystick>() == null) {
                InputSystem.AddDevice<VirtualJoystick>();
                Debug.Log("[VirtualJoystickDevice] Added virtual joystick device");
            }
        }
    }
}
#endif