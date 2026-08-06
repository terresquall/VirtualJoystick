using UnityEngine;
using UnityEditor;

namespace Terresquall {
    public enum VirtualJoystickEditorLanguage {
        English,
        SimplifiedChinese,
        TraditionalChinese
    }

    public static class VirtualJoystickEditorLocalization {
        private const string LANGUAGE_KEY =
            "Terresquall.VirtualJoystick.EditorLanguage";

        // =========================================================
        // Language
        // =========================================================

        public static VirtualJoystickEditorLanguage Language {
            get {
                return (VirtualJoystickEditorLanguage)EditorPrefs.GetInt(
                    LANGUAGE_KEY,
                    (int)VirtualJoystickEditorLanguage.English
                );
            }

            set {
                EditorPrefs.SetInt(LANGUAGE_KEY, (int)value);
            }
        }

        public static string Text(
            string english,
            string simplifiedChinese,
            string traditionalChinese) {
            switch (Language) {
                case VirtualJoystickEditorLanguage.SimplifiedChinese:
                    return simplifiedChinese;

                case VirtualJoystickEditorLanguage.TraditionalChinese:
                    return traditionalChinese;

                default:
                    return english;
            }
        }

        // =========================================================
        // General
        // =========================================================

        // =========================================================
        // Section Headers
        // =========================================================

        public static string DebugHeader => Text(
            "Debug",
            "调试",
            "偵錯"
        );

        public static string SettingsHeader => Text(
            "Settings",
            "设置",
            "設定"
        );

        public static string InputSystemHeader => Text(
            "Input System",
            "输入系统",
            "輸入系統"
        );

        public static string SectionHeader(string propertyName) {
            switch (propertyName) {
                case "consolePrintAxis":
                    return DebugHeader;

                case "onlyOnMobile":
                    return SettingsHeader;

                case "addInputDevice":
                    return InputSystemHeader;

                case "totalMemoryFrames":
                    return InputMemoryHeader;

                default:
                    return null;
            }
        }

        public static string InputMemoryHeader => Text(
            "Input Memory",
            "输入记忆",
            "輸入記憶"
        );

        public static string[] LanguageNames =>
            new string[]
            {
                "English",
                "简体中文",
                "繁體中文"
            };
        public static string LanguageLabel => Text(
            "Language",
            "语言",
            "語言"
        );

        public static void DrawLanguageSelector() {
            int currentLanguage = (int)Language;

            int newLanguage = EditorGUILayout.Popup(
                LanguageLabel,
                currentLanguage,
                LanguageNames
            );

            if (newLanguage != currentLanguage) {
                Language = (VirtualJoystickEditorLanguage)newLanguage;
            }
        }

        // =========================================================
        // Pivot
        // =========================================================

        public static string PivotWarning => Text(
            "Your pivot is not centred (should be 0.5, 0.5). This can cause the joystick to be unusable.",
            "你的枢轴点未居中（应为 0.5, 0.5）。这可能导致摇杆无法使用。",
            "你的樞軸點未置中（應為 0.5, 0.5）。這可能導致搖桿無法使用。"
        );

        public static string FixCentrePivot => Text(
            "Fix: Centre Pivot",
            "修复：居中枢轴点",
            "修復：置中樞軸點"
        );

        // =========================================================
        // Canvas
        // =========================================================

        public static string CanvasRequired => Text(
            "This joystick needs to be parented to a Canvas, or it won't work!",
            "此摇杆需要放在 Canvas 下，否则无法工作！",
            "此搖桿需要放在 Canvas 下，否則無法運作！"
        );

        public static string WrongCanvasMode => Text(
            "This joystick is parented to a Canvas that is not set to Screen Space - Overlay. It may be buggy or fail to work entirely.",
            "此摇杆所在的 Canvas 未设置为 Screen Space - Overlay，可能出现异常或完全无法工作。",
            "此搖桿所在的 Canvas 未設定為 Screen Space - Overlay，可能出現異常或完全無法運作。"
        );

        // =========================================================
        // Input System
        // =========================================================

        public static string BothInputSystems => Text(
            "Both of Unity's Input Systems are enabled on this project. Virtual Joystick will default to using the old Input Manager to maintain compatibility with Unity Remote.",
            "此项目同时启用了 Unity 的两套输入系统。Virtual Joystick 将默认使用旧版 Input Manager，以保持与 Unity Remote 的兼容性。",
            "此專案同時啟用了 Unity 的兩套輸入系統。Virtual Joystick 將預設使用舊版 Input Manager，以保持與 Unity Remote 的相容性。"
        );

        // =========================================================
        // ID
        // =========================================================

        public static string NonUniqueID => Text(
            "This Virtual Joystick doesn't have a unique ID. Please assign a unique ID or click on the button below.",
            "此 Virtual Joystick 没有唯一 ID。请设置一个唯一 ID，或点击下方按钮自动生成。",
            "此 Virtual Joystick 沒有唯一 ID。請設定一個唯一 ID，或點擊下方按鈕自動產生。"
        );

        public static string GenerateUniqueID => Text(
            "Generate Unique Joystick ID",
            "生成唯一 Joystick ID",
            "產生唯一 Joystick ID"
        );

        public static string RepeatedIDs => Text(
            "At least one of your Virtual Joysticks doesn't have a unique ID. Please ensure that all of them have unique IDs, or they may not be able to collect input properly.",
            "至少有一个 Virtual Joystick 没有唯一 ID。请确保所有摇杆都有不同的 ID，否则可能无法正确接收输入。",
            "至少有一個 Virtual Joystick 沒有唯一 ID。請確保所有搖桿都有不同的 ID，否則可能無法正確接收輸入。"
        );

        // =========================================================
        // Control Stick
        // =========================================================

        public static string NoControlStick => Text(
            "There is no Control Stick assigned. This joystick won't work.",
            "尚未指定 Control Stick。此摇杆将无法工作。",
            "尚未指定 Control Stick。此搖桿將無法運作。"
        );

        public static string ControlStickNotChild => Text(
            "The control stick of this joystick is not a child of this joystick.",
            "此摇杆的 Control Stick 必须是该摇杆的子物体。",
            "此搖桿的 Control Stick 必須是該搖桿的子物件。"
        );

        // =========================================================
        // Size
        // =========================================================

        public static string SizeAdjustments => Text(
            "Size Adjustments",
            "尺寸调整",
            "尺寸調整"
        );

        public static string IncreaseSize => Text(
            "Increase Size",
            "增大尺寸",
            "增大尺寸"
        );

        public static string DecreaseSize => Text(
            "Decrease Size",
            "减小尺寸",
            "減小尺寸"
        );

        // =========================================================
        // Edge Feedback
        // =========================================================

        public static string EdgeFeedbackAudioWarning => Text(
            "If you would like your feedback to include sound, attach an Audio Source to this GameObject and assign a clip to the Audio Source component. Otherwise, you can ignore this message.",
            "如果你希望反馈包含声音，请在此 GameObject 上添加 Audio Source，并为 Audio Source 组件指定一个音频片段。否则可以忽略此提示。",
            "如果你希望回饋包含聲音，請在此 GameObject 上新增 Audio Source，並為 Audio Source 元件指定一個音訊片段。否則可以忽略此提示。"
        );

        public static GUIContent EdgeFeedbackPropertyContent(SerializedProperty property) {
            switch (property.name) {
                case "deltaThreshold":
                    return new GUIContent(
                        Text(
                            "Delta Threshold",
                            "变化阈值",
                            "變化閾值"
                        ),
                        Text(
                            "The amount of force you will need to hit the edge with to register feedback.",
                            "撞击摇杆边缘时，需要达到此强度才会触发反馈。",
                            "撞擊搖桿邊緣時，需要達到此強度才會觸發回饋。"
                        )
                    );

                case "minimumFeedbackGap":
                    return new GUIContent(
                        Text(
                            "Minimum Feedback Gap",
                            "最小反馈间隔",
                            "最小回饋間隔"
                        ),
                        Text(
                            "After a feedback fires, you will need to wait before another feedback occurs.",
                            "触发一次反馈后，需要等待这段时间才能再次触发。",
                            "觸發一次回饋後，需要等待這段時間才能再次觸發。"
                        )
                    );

                case "hasVibration":
                    return new GUIContent(
                        Text(
                            "Has Vibration",
                            "启用震动",
                            "啟用震動"
                        ),
                        Text(
                            "Does the feedback include vibration?",
                            "反馈是否包含震动。",
                            "回饋是否包含震動。"
                        )
                    );

                default:
                    return new GUIContent(
                        property.displayName,
                        property.tooltip
                    );
            }
        }

        // =========================================================
        // Serialized Properties
        // =========================================================

        public static GUIContent PropertyContent(SerializedProperty property) {
            switch (property.name) {
                case "ID":
                    return new GUIContent(
                        "ID",
                        Text(
                            "The unique ID for this joystick. Needs to be unique.",
                            "此摇杆的唯一 ID。每个摇杆都需要使用不同的 ID。",
                            "此搖桿的唯一 ID。每個搖桿都需要使用不同的 ID。"
                        )
                    );

                case "controlStick":
                    return new GUIContent(
                        Text(
                            "Control Stick",
                            "控制摇杆",
                            "控制搖桿"
                        ),
                        Text(
                            "The component that the user will drag around for joystick input.",
                            "用户拖动此组件来控制摇杆输入。",
                            "使用者拖動此元件來控制搖桿輸入。"
                        )
                    );

                case "consolePrintAxis":
                    return new GUIContent(
                        Text(
                            "Console Print Axis",
                            "在控制台输出轴值",
                            "在控制台輸出軸值"
                        ),
                        Text(
                            "Prints the control stick direction to the Console.",
                            "在 Console 中输出控制摇杆的方向。",
                            "在 Console 中輸出控制搖桿的方向。"
                        )
                    );

                case "onlyOnMobile":
                    return new GUIContent(
                        Text(
                            "Only On Mobile",
                            "仅限移动平台",
                            "僅限行動平台"
                        ),
                        Text(
                            "Disables the joystick when the application is not running on a mobile platform.",
                            "如果当前不是移动平台，则禁用摇杆。",
                            "如果目前不是行動平台，則停用搖桿。"
                        )
                    );

                case "dragColor":
                    return new GUIContent(
                        Text(
                            "Drag Color",
                            "拖动颜色",
                            "拖動顏色"
                        ),
                        Text(
                            "Colour of the control stick while it is being dragged.",
                            "拖动控制摇杆时显示的颜色。",
                            "拖動控制搖桿時顯示的顏色。"
                        )
                    );

                case "sensitivity":
                    return new GUIContent(
                        Text(
                            "Sensitivity",
                            "灵敏度",
                            "靈敏度"
                        ),
                        Text(
                            "How responsive the control stick is to dragging.",
                            "控制摇杆对拖动操作的灵敏程度。",
                            "控制搖桿對拖動操作的靈敏程度。"
                        )
                    );

                case "radius":
                    return new GUIContent(
                        Text(
                            "Radius",
                            "半径",
                            "半徑"
                        ),
                        Text(
                            "How far the control stick can be dragged away from the centre of the joystick.",
                            "控制摇杆可以从摇杆中心拖动的最大距离。",
                            "控制搖桿可以從搖桿中心拖動的最大距離。"
                        )
                    );

                case "deadzone":
                    return new GUIContent(
                        Text(
                            "Deadzone",
                            "死区",
                            "死區"
                        ),
                        Text(
                            "How far the control stick must move from the centre before input is registered.",
                            "控制摇杆离开中心一定距离后才开始记录输入。",
                            "控制搖桿離開中心一定距離後才開始記錄輸入。"
                        )
                    );

                case "edgeSnap":
                    return new GUIContent(
                        Text(
                            "Edge Snap",
                            "吸附边缘",
                            "吸附邊緣"
                        ),
                        Text(
                            "Automatically snaps the joystick to the edge when outside the deadzone.",
                            "摇杆离开死区后自动吸附到边缘。",
                            "搖桿離開死區後自動吸附到邊緣。"
                        )
                    );

                case "directions":
                    return new GUIContent(
                        Text(
                            "Directions",
                            "方向数量",
                            "方向數量"
                        ),
                        Text(
                            "Number of directions available on the joystick. Set to 0 for unrestricted movement.",
                            "摇杆可使用的方向数量。设为 0 时不限制移动方向。",
                            "搖桿可使用的方向數量。設為 0 時不限制移動方向。"
                        )
                    );

                case "angleOffset":
                    return new GUIContent(
                        Text(
                            "Angle Offset",
                            "角度偏移",
                            "角度偏移"
                        ),
                        Text(
                            "Adjusts the angle of the joystick directions.",
                            "调整摇杆各方向的角度。",
                            "調整搖桿各方向的角度。"
                        )
                    );

                case "snapsToTouch":
                    return new GUIContent(
                        Text(
                            "Snaps To Touch",
                            "吸附到触摸位置",
                            "吸附到觸控位置"
                        ),
                        Text(
                            "Moves the joystick to the touch position when the touch is within its boundaries.",
                            "当触摸位置位于指定范围内时，将摇杆移动到触摸位置。",
                            "當觸控位置位於指定範圍內時，將搖桿移動到觸控位置。"
                        )
                    );

                case "boundaries":
                    return new GUIContent(
                        Text(
                            "Boundaries",
                            "触摸范围",
                            "觸控範圍"
                        )
                    );

                case "addInputDevice":
                    return new GUIContent(
                        Text(
                            "Add Input Device",
                            "添加输入设备",
                            "新增輸入裝置"
                        ),
                        Text(
                            "Adds an input device for this joystick so it can be bound to an Input Action.",
                            "为此摇杆添加输入设备，使其可以绑定到 Input Action。",
                            "為此搖桿新增輸入裝置，使其可以綁定到 Input Action。"
                        )
                    );

                case "usage":
                    return new GUIContent(
                        Text(
                            "Usage",
                            "用途",
                            "用途"
                        )
                    );

                case "inputSystemBridge":
                    return new GUIContent(
                        Text(
                            "Input System Bridge",
                            "输入系统桥接",
                            "輸入系統橋接"
                        ),
                        Text(
                            "Sends this joystick's axis to the New Input System.",
                            "将此摇杆的轴输入发送到新版 Input System。",
                            "將此搖桿的軸輸入傳送到新版 Input System。"
                        )
                    );

                case "totalMemoryFrames":
                    return new GUIContent(
                        Text(
                            "Total Memory Frames",
                            "总记忆帧数",
                            "總記憶幀數"
                        )
                    );

                default:
                    return new GUIContent(
                        property.displayName,
                        property.tooltip
                    );
            }
        }
    }
}