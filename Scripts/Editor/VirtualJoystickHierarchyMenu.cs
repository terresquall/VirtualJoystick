using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Terresquall
{
    public static class VirtualJoystickHierarchyMenu
    {
        private const string PrefabsFolder =
            "Assets/VirtualJoystick/Prefabs";

        private const string DefaultJoystickPath =
            PrefabsFolder +
            "/Default Joysticks/Default Joystick (White).prefab";

        private const string HorizontalJoystickPath =
            PrefabsFolder +
            "/Horizontal Joystick/Horizontal Joystick.prefab";

        private const string NativeJoystickPath =
            PrefabsFolder +
            "/Native Joysticks/Native Joystick (White).prefab";

        private const string SplitJoystickPath =
            PrefabsFolder +
            "/Split Joystick/Split Joystick (Grey).prefab";

        private const string VerticalJoystickPath =
            PrefabsFolder +
            "/Vertical Joystick/Vertical Joystick (White).prefab";


        [MenuItem(
            "GameObject/UI/Virtual Joystick/Default Joystick",
            false,
            2100
        )]
        private static void CreateDefaultJoystick(MenuCommand command)
        {
            CreateJoystick(
                DefaultJoystickPath,
                "Default Joystick",
                command
            );
        }


        [MenuItem(
            "GameObject/UI/Virtual Joystick/Horizontal Joystick",
            false,
            2101
        )]
        private static void CreateHorizontalJoystick(MenuCommand command)
        {
            CreateJoystick(
                HorizontalJoystickPath,
                "Horizontal Joystick",
                command
            );
        }


        [MenuItem(
            "GameObject/UI/Virtual Joystick/Native Joystick",
            false,
            2102
        )]
        private static void CreateNativeJoystick(MenuCommand command)
        {
            CreateJoystick(
                NativeJoystickPath,
                "Native Joystick",
                command
            );
        }


        [MenuItem(
            "GameObject/UI/Virtual Joystick/Split Joystick",
            false,
            2103
        )]
        private static void CreateSplitJoystick(MenuCommand command)
        {
            CreateJoystick(
                SplitJoystickPath,
                "Split Joystick",
                command
            );
        }


        [MenuItem(
            "GameObject/UI/Virtual Joystick/Vertical Joystick",
            false,
            2104
        )]
        private static void CreateVerticalJoystick(MenuCommand command)
        {
            CreateJoystick(
                VerticalJoystickPath,
                "Vertical Joystick",
                command
            );
        }


        [MenuItem(
            "GameObject/UI/Virtual Joystick/See more...",
            false,
            2120
        )]
        private static void ShowAllJoystickPrefabs()
        {
            DefaultAsset prefabFolder =
                AssetDatabase.LoadAssetAtPath<DefaultAsset>(
                    PrefabsFolder
                );

            if (prefabFolder == null)
            {
                Debug.LogError(
                    "Virtual Joystick Prefabs folder was not found at: " +
                    PrefabsFolder
                );

                return;
            }

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = prefabFolder;

            EditorGUIUtility.PingObject(prefabFolder);
        }


        private static void CreateJoystick(
            string prefabPath,
            string joystickName,
            MenuCommand command
        )
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError(
                    joystickName +
                    " prefab was not found at: " +
                    prefabPath
                );

                return;
            }

            Undo.IncrementCurrentGroup();

            int undoGroup = Undo.GetCurrentGroup();

            Undo.SetCurrentGroupName(
                "Create " + joystickName
            );

            Transform parent = GetUIParent(command);

            GameObject joystick =
                PrefabUtility.InstantiatePrefab(
                    prefab,
                    parent
                ) as GameObject;

            if (joystick == null)
            {
                Debug.LogError(
                    "Failed to create " + joystickName + "."
                );

                Undo.CollapseUndoOperations(undoGroup);

                return;
            }

            joystick.name = joystickName;

            joystick.transform.SetAsLastSibling();

            Undo.RegisterCreatedObjectUndo(
                joystick,
                "Create " + joystickName
            );

            Selection.activeGameObject = joystick;

            EditorGUIUtility.PingObject(joystick);

            Undo.CollapseUndoOperations(undoGroup);
        }


        private static Transform GetUIParent(MenuCommand command)
        {
            GameObject selectedObject =
                command.context as GameObject;

            if (selectedObject == null)
            {
                selectedObject = Selection.activeGameObject;
            }

            // If the selected object is already a UI object inside
            // a Canvas, create the Joystick under that object.
            if (selectedObject != null)
            {
                RectTransform selectedRectTransform =
                    selectedObject.GetComponent<RectTransform>();

                Canvas parentCanvas =
                    selectedObject.GetComponentInParent<Canvas>();

                if (
                    selectedRectTransform != null &&
                    parentCanvas != null
                )
                {
                    return selectedObject.transform;
                }
            }

            // Otherwise, use an existing Canvas.
            Canvas existingCanvas =
                Object.FindObjectOfType<Canvas>();

            if (existingCanvas != null)
            {
                return existingCanvas.transform;
            }

            // No Canvas exists, so create one.
            return CreateCanvas().transform;
        }


        private static Canvas CreateCanvas()
        {
            GameObject canvasObject = new GameObject(
                "Canvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );

            int uiLayer = LayerMask.NameToLayer("UI");

            if (uiLayer >= 0)
            {
                canvasObject.layer = uiLayer;
            }

            Canvas canvas =
                canvasObject.GetComponent<Canvas>();

            canvas.renderMode =
                RenderMode.ScreenSpaceOverlay;

            Undo.RegisterCreatedObjectUndo(
                canvasObject,
                "Create Canvas"
            );

            CreateEventSystem();

            return canvas;
        }


        private static void CreateEventSystem()
        {
            EventSystem existingEventSystem =
                Object.FindObjectOfType<EventSystem>();

            if (existingEventSystem != null)
            {
                return;
            }

            GameObject eventSystemObject =
                new GameObject(
                    "EventSystem",
                    typeof(EventSystem)
                );

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER

            eventSystemObject.AddComponent<
                UnityEngine.InputSystem.UI.InputSystemUIInputModule
            >();

#else

            eventSystemObject.AddComponent<
                StandaloneInputModule
            >();

#endif

            Undo.RegisterCreatedObjectUndo(
                eventSystemObject,
                "Create EventSystem"
            );
        }
    }
}