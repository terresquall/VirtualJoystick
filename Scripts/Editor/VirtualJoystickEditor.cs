using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Terresquall {

    [CustomEditor(typeof(VirtualJoystick))]
    [CanEditMultipleObjects]
    public partial class VirtualJoystickEditor : Editor {

        VirtualJoystick joystick;
        RectTransform rectTransform;
        Canvas rootCanvas;

        const float HANDLE_SIZE = 5f;

        private static readonly List<int> usedIDs = new List<int>();

        public float GetHandleSize() {
            if (rootCanvas != null && rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                return HANDLE_SIZE / 90;
            return HANDLE_SIZE;
        }

        void OnEnable() {
            joystick = target as VirtualJoystick;
            rectTransform = joystick.GetComponent<RectTransform>();
            rootCanvas = joystick.GetRootCanvas();
        }

        static VirtualJoystick[] FindAll() {
#if UNITY_2022_2_OR_NEWER
            return FindObjectsByType<VirtualJoystick>(FindObjectsSortMode.None);
#else
            return FindObjectsOfType<VirtualJoystick>();
#endif
        }

        // Does the passed joystick have an ID that is unique to itself?
        bool HasUniqueID(VirtualJoystick vj) {
            foreach (VirtualJoystick v in FindAll()) {
                if (v == vj) continue;
                if (v.ID == vj.ID) return false;
            }
            return true;
        }

        // Is a given ID value already used by another joystick?
        bool IsAvailableID(int id) {
            foreach (VirtualJoystick v in FindAll()) {
                if (v.ID == id) return false;
            }
            return true;
        }

        // Do all the joysticks have unique IDs.
        bool HasRepeatIDs() {
            usedIDs.Clear();
            foreach (VirtualJoystick vj in FindAll()) {
                if (usedIDs.Contains(vj.ID)) return true;
                usedIDs.Add(vj.ID);
            }
            return false;
        }

        // Reassign all IDs for all Joysticks.
        void ReassignAllIDs(VirtualJoystick exception = null) {
            foreach (VirtualJoystick vj in FindAll()) {
                // Ignore joysticks that are already unique.
                if (exception == vj || HasUniqueID(vj)) continue;
                ReassignThisID(vj);
            }
        }

        // Reassign the ID for this Joystick only.
        void ReassignThisID(VirtualJoystick vj) {

            // Save the action in the History.
            Undo.RecordObject(vj, "Generate Unique Joystick ID");

            // Get all joysticks so that we can check against it if the ID is valid.
            VirtualJoystick[] joysticks = FindAll();
            for (int i = 0; i < joysticks.Length; i++) {
                if (IsAvailableID(i)) {
                    vj.ID = i; // If we find an unused ID, use it.
                    EditorUtility.SetDirty(vj);
                    return;
                }
            }

            // If all of the IDs are used, we will have to use length + 1 as the ID.
            vj.ID = joysticks.Length;
            EditorUtility.SetDirty(vj);
        }

        public override void OnInspectorGUI() {

            VirtualJoystickEditorLocalization.DrawLanguageSelector();
            EditorGUILayout.Space();

            //Checks if Joystick's Pivot is centred
            if (rectTransform != null && (Mathf.Abs(rectTransform.pivot.x - 0.5f) > 0.01f || Mathf.Abs(rectTransform.pivot.y - 0.5f) > 0.01f)) {
                //displays warning and button to recentre pivot
                string pivotWarning =
                    VirtualJoystickEditorLocalization.PivotWarning;

                EditorGUILayout.HelpBox(
                    pivotWarning,
                    MessageType.Error
                );

                Debug.LogError(pivotWarning);

                if (GUILayout.Button(
                    VirtualJoystickEditorLocalization.FixCentrePivot)) {
                    Undo.RecordObject(rectTransform, "Center Pivot");
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    EditorUtility.SetDirty(rectTransform);
                }
                EditorGUILayout.Space();
            }


            // Draw a help text box if this is not attached to a Canvas.
            if (!EditorUtility.IsPersistent(target)) {
                if (!rootCanvas)
                    EditorGUILayout.HelpBox(
                        VirtualJoystickEditorLocalization.CanvasRequired,
                        MessageType.Error
                    );
                else if (rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                    EditorGUILayout.HelpBox(
                        VirtualJoystickEditorLocalization.WrongCanvasMode,
                        MessageType.Error
                    );
            }

            // Show this only when both input systems are used.
#if ENABLE_INPUT_SYSTEM
#if ENABLE_LEGACY_INPUT_MANAGER
            EditorGUILayout.HelpBox(
                VirtualJoystickEditorLocalization.BothInputSystems,
                MessageType.Info
            );
#endif
#endif

            // Draw all the inspector properties.
            serializedObject.Update();
            SerializedProperty property = serializedObject.GetIterator();
            bool snapsToTouch = true;
            int directions = 0;

            if (property.NextVisible(true)) {
                do {
                    // If the property name is snapsToTouch, record its value.
                    switch (property.name) {
                        case "m_Script":
                            continue;
                        case "snapsToTouch":
                            snapsToTouch = property.boolValue;
                            break;
                        case "directions":
                            directions = property.intValue;
                            break;
                        case "boundaries":
                            // If snapsToTouch is off, don't render boundaries.
                            if (!snapsToTouch) continue;
                            break;
                        case "angleOffset":
                            if (directions <= 0) continue;
                            break;
                    }

                    string sectionHeader =
                        VirtualJoystickEditorLocalization.SectionHeader(property.name);

                    if (!string.IsNullOrEmpty(sectionHeader)) {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField(
                            sectionHeader,
                            EditorStyles.boldLabel
                        );
                    }

                    EditorGUI.BeginChangeCheck();

                    // Print different properties based on what the property is.
                    if (property.name == "angleOffset") {
                        float maxAngleOffset = 360f / directions / 2;

                        EditorGUILayout.Slider(
                            property,
                            -maxAngleOffset,
                            maxAngleOffset,
                            VirtualJoystickEditorLocalization.PropertyContent(property)
                        );
                    } else {
                        EditorGUILayout.PropertyField(
                            property,
                            VirtualJoystickEditorLocalization.PropertyContent(property),
                            true
                        );
                    }

                    EditorGUI.EndChangeCheck();

                    // If the property is an ID, show a button allowing us to reassign the IDs.
                    if (property.name == "ID" && !EditorUtility.IsPersistent(target)) {
                        if (!HasUniqueID(joystick)) {
                            EditorGUILayout.HelpBox(
                                VirtualJoystickEditorLocalization.NonUniqueID,
                                MessageType.Warning
                            );

                            if (GUILayout.Button(
                                VirtualJoystickEditorLocalization.GenerateUniqueID)) {
                                ReassignThisID(joystick);
                            }

                            EditorGUILayout.Space();
                        } else if (HasRepeatIDs()) {
                            EditorGUILayout.HelpBox(
                                VirtualJoystickEditorLocalization.RepeatedIDs,
                                MessageType.Warning
                            );

                            EditorGUILayout.Space();
                        }
                    }

                } while (property.NextVisible(false));
            }

            serializedObject.ApplyModifiedProperties();

            //Increase Decrease buttons
            if (joystick) {

                if (!joystick.controlStick) {
                    EditorGUILayout.HelpBox(
                        VirtualJoystickEditorLocalization.NoControlStick,
                        MessageType.Warning
                    );

                    return;
                }

                if (!joystick.controlStick.transform.IsChildOf(joystick.transform)) {
                    EditorGUILayout.HelpBox(
                        VirtualJoystickEditorLocalization.ControlStickNotChild,
                        MessageType.Warning
                    );

                    return;
                }

                // Add the heading for the size adjustments.
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField(
                    VirtualJoystickEditorLocalization.SizeAdjustments
                );

                GUILayout.BeginHorizontal();

                bool increaseSize = GUILayout.Button(
                        VirtualJoystickEditorLocalization.IncreaseSize,
                        EditorStyles.miniButtonLeft
                    ),
                    decreaseSize = GUILayout.Button(
                        VirtualJoystickEditorLocalization.DecreaseSize,
                        EditorStyles.miniButtonRight
                    );

                if (increaseSize || decreaseSize) {

                    // Record actions for all elements.
                    RectTransform[] affected = rectTransform.GetComponentsInChildren<RectTransform>();
                    RecordSizeChangeUndo(affected);

                    // Increase / decrease size actions.
                    foreach (RectTransform r in affected) {
                        Vector2 newSize;
                        if (increaseSize) newSize = r.rect.size * 1.15f;
                        else newSize = r.rect.size * 0.85f;

                        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
                        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);
                    }

                }

                GUILayout.EndHorizontal();

                EditorGUI.EndChangeCheck();
            }
        }


        void OnSceneGUI() {
            VirtualJoystick vj = (VirtualJoystick)target;

            GUILayout.Space(10);
            float radius = vj.GetRadius();

            // Draw the radius of the joystick.
            Handles.color = new Color(0, 1, 0, 0.1f);
            Handles.DrawSolidArc(vj.transform.position, Vector3.forward, Vector3.right, 360, radius);
            Handles.color = new Color(0, 1, 0, 0.5f);
            Handles.DrawWireArc(vj.transform.position, Vector3.forward, Vector3.right, 360, radius, 3f);

            // Draw the deadzone.
            Handles.color = new Color(1, 0, 0, 0.2f);
            Handles.DrawSolidArc(vj.transform.position, Vector3.forward, Vector3.right, 360, radius * vj.deadzone);
            Handles.color = new Color(1, 0, 0, 0.5f);
            Handles.DrawWireArc(vj.transform.position, Vector3.forward, Vector3.right, 360, radius * vj.deadzone, 3f);

            // Draw the boundaries of the joystick.
            if (vj.GetBounds().size.sqrMagnitude > 0) {

                // Draw the lines of the bounds.
                Handles.color = Color.yellow;

                // Get the 4 points in the bounds (in pixels).
                Vector3 bottomLeft = new Vector3(vj.boundaries.x, vj.boundaries.y);
                Vector3 topLeft = new Vector3(vj.boundaries.x, vj.boundaries.y + vj.boundaries.height);
                Vector3 topRight = new Vector3(vj.boundaries.x + vj.boundaries.width, vj.boundaries.y + vj.boundaries.height);
                Vector3 bottomRight = new Vector3(vj.boundaries.x + vj.boundaries.width, vj.boundaries.y);

                // Convert the anchors if the canvas is a different screen space.
                Canvas c = vj.GetRootCanvas();
                if (c != null && c.renderMode != RenderMode.ScreenSpaceOverlay) {
                    RectTransform cr = rootCanvas.transform as RectTransform;
                    Camera cc = rootCanvas.worldCamera;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(cr, bottomLeft, cc, out bottomLeft);
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(cr, topLeft, cc, out topLeft);
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(cr, topRight, cc, out topRight);
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(cr, bottomRight, cc, out bottomRight);
                }

                // Draw the boundary lines
                Handles.DrawLine(bottomLeft, topLeft);
                Handles.DrawLine(topLeft, topRight);
                Handles.DrawLine(topRight, bottomRight);
                Handles.DrawLine(bottomRight, bottomLeft);

                // Calculate the center point of the boundaries
                Vector3 center = new Vector3(vj.boundaries.x + vj.boundaries.width / 2, vj.boundaries.y + vj.boundaries.height / 2);

                // Add a draggable handle in the center to move the boundaries
                Handles.color = Color.yellow;
                float size = GetHandleSize();
                EditorGUI.BeginChangeCheck();
#if UNITY_2022_1_OR_NEWER
                //Circle Handles
                Vector3 newCenter = Handles.FreeMoveHandle(center, size, Vector3.zero, Handles.CircleHandleCap);
#else
                Vector3 newCenter = Handles.FreeMoveHandle(center, Quaternion.identity, size, Vector3.zero, Handles.CircleHandleCap);
#endif
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(vj, "Move Joystick Boundaries");

                    // Move the boundaries based on the handle's new position
                    float offsetX = newCenter.x - center.x;
                    float offsetY = newCenter.y - center.y;

                    vj.boundaries.x += offsetX;
                    vj.boundaries.y += offsetY;

                    EditorUtility.SetDirty(vj);
                }

                // Add draggable handles for the corners
                EditorGUI.BeginChangeCheck();
#if UNITY_2022_1_OR_NEWER
                //Circle handles
                Vector3 newBottomLeft = Handles.FreeMoveHandle(bottomLeft, size, Vector3.zero, Handles.CircleHandleCap);
                Vector3 newTopLeft = Handles.FreeMoveHandle(topLeft, size, Vector3.zero, Handles.CircleHandleCap);
                Vector3 newTopRight = Handles.FreeMoveHandle(topRight, size, Vector3.zero, Handles.CircleHandleCap);
                Vector3 newBottomRight = Handles.FreeMoveHandle(bottomRight, size, Vector3.zero, Handles.CircleHandleCap);
#else
                //Circle handles
                Vector3 newBottomLeft = Handles.FreeMoveHandle(bottomLeft, Quaternion.identity, size, Vector3.zero, Handles.CircleHandleCap);
                Vector3 newTopLeft = Handles.FreeMoveHandle(topLeft, Quaternion.identity, size, Vector3.zero, Handles.CircleHandleCap);
                Vector3 newTopRight = Handles.FreeMoveHandle(topRight, Quaternion.identity, size, Vector3.zero, Handles.CircleHandleCap);
                Vector3 newBottomRight = Handles.FreeMoveHandle(bottomRight, Quaternion.identity, size, Vector3.zero, Handles.CircleHandleCap);
#endif
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(vj, "Resize Joystick Boundaries");

                    // Determine which handle moved and apply appropriate changes
                    if (newBottomLeft != bottomLeft) {
                        // Bottom left affects x, y, width, height
                        float deltaX = newBottomLeft.x - bottomLeft.x;
                        float deltaY = newBottomLeft.y - bottomLeft.y;
                        vj.boundaries.x += deltaX;
                        vj.boundaries.y += deltaY;
                        vj.boundaries.width -= deltaX;
                        vj.boundaries.height -= deltaY;
                    } else if (newTopLeft != topLeft) {
                        // Top left affects x and width (moving left edge) and height (moving top edge)
                        float deltaX = newTopLeft.x - topLeft.x;
                        float deltaY = newTopLeft.y - topLeft.y;
                        vj.boundaries.x += deltaX;
                        vj.boundaries.width -= deltaX;
                        vj.boundaries.height += deltaY;
                    } else if (newTopRight != topRight) {
                        // Top right affects width and height
                        float deltaX = newTopRight.x - topRight.x;
                        float deltaY = newTopRight.y - topRight.y;
                        vj.boundaries.width += deltaX;
                        vj.boundaries.height += deltaY;
                    } else if (newBottomRight != bottomRight) {
                        // Bottom right affects width (moving right edge) and y, height (moving bottom edge)
                        float deltaX = newBottomRight.x - bottomRight.x;
                        float deltaY = newBottomRight.y - bottomRight.y;
                        vj.boundaries.width += deltaX;
                        vj.boundaries.y += deltaY;
                        vj.boundaries.height -= deltaY;
                    }

                    // Ensure minimum size
                    vj.boundaries.width = Mathf.Max(1, vj.boundaries.width);
                    vj.boundaries.height = Mathf.Max(1, vj.boundaries.height);

                    EditorUtility.SetDirty(vj);
                }
            }

            // Draw the direction anchors of the joystick.
            if (vj.directions > 0) {
                Handles.color = Color.blue;
                float partition = 360f / vj.directions;
                for (int i = 0; i < vj.directions; i++) {
                    Handles.DrawLine(vj.transform.position, vj.transform.position + Quaternion.Euler(0, 0, i * partition + vj.angleOffset) * Vector2.right * radius, 2f);
                }
            }
        }

        void RecordSizeChangeUndo(UnityEngine.Object[] arguments) {
            for (int i = 0; i < arguments.Length; i++) {
                Undo.RecordObject(arguments[i], "Undo Virtual Joystick Size Change");
            }
        }
    }

    // This partial class adds the prefab shortcuts to the Hierarchy menu.
    public partial class VirtualJoystickEditor {
        const string PREFABS_FOLDER_NAME = "Prefabs";
        const string SCRIPTS_FOLDER_NAME = "Scripts";
        const string HIERARCHY_MENU_ROOT = "GameObject/UI/Virtual Joystick/";
        const int HIERARCHY_MENU_PRIORITY = 2100;

        static readonly List<string> registeredHierarchyMenuItems = new List<string>();

        static string cachedPrefabsFolderPath;
        static string currentPrefabMenuSignature;
        static bool hierarchyMenuRefreshQueued;

        static MethodInfo addMenuItemMethod;
        static MethodInfo removeMenuItemMethod;
        static string pendingProjectFolderPath;

        [InitializeOnLoadMethod]
        static void InitialiseHierarchyMenu() {
            EditorApplication.projectChanged -= QueueHierarchyMenuRefresh;
            EditorApplication.projectChanged += QueueHierarchyMenuRefresh;

            AssemblyReloadEvents.beforeAssemblyReload -= RemoveRegisteredHierarchyMenuItems;
            AssemblyReloadEvents.beforeAssemblyReload += RemoveRegisteredHierarchyMenuItems;

            QueueHierarchyMenuRefresh();
        }

        static void QueueHierarchyMenuRefresh() {
            if (hierarchyMenuRefreshQueued) return;

            hierarchyMenuRefreshQueued = true;

            EditorApplication.delayCall -= RefreshHierarchyMenu;
            EditorApplication.delayCall += RefreshHierarchyMenu;
        }

        static void RefreshHierarchyMenu() {
            hierarchyMenuRefreshQueued = false;

            string prefabsFolderPath = GetPrefabsFolderPath();

            if (string.IsNullOrEmpty(prefabsFolderPath)) {
                RemoveRegisteredHierarchyMenuItems();
                return;
            }

            List<string> prefabPaths = FindBasePrefabPaths(prefabsFolderPath);
            string newSignature = prefabsFolderPath + "\n" + string.Join("\n", prefabPaths);

            // A project change does not necessarily mean the Joystick Prefabs have changed.
            // Avoid rebuilding identical menus.
            if (newSignature == currentPrefabMenuSignature && registeredHierarchyMenuItems.Count > 0) return;

            RemoveRegisteredHierarchyMenuItems();

            HashSet<string> usedMenuPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int priority = HIERARCHY_MENU_PRIORITY;
            bool registrationFailed = false;

            foreach (string prefabPath in prefabPaths) {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (!prefab) continue;

                string menuPath = HIERARCHY_MENU_ROOT + prefab.name;

                // Two Prefabs with the same name would create the same Unity menu path.
                if (!usedMenuPaths.Add(menuPath)) {
                    Debug.LogWarning($"Multiple regular Virtual Joystick Prefabs have the same name: {prefab.name}");
                    continue;
                }

                string capturedPrefabPath = prefabPath;

                // Remove a stale native menu entry if one survived
                // an earlier script reload.
                RemoveDynamicMenuItem(menuPath);

                bool registered = AddDynamicMenuItem(menuPath, priority++, () => CreateJoystick(capturedPrefabPath));
                if (registered) registeredHierarchyMenuItems.Add(menuPath);
                else registrationFailed = true;
            }

            string seeMoreMenuPath = HIERARCHY_MENU_ROOT + "See more...";

            RemoveDynamicMenuItem(seeMoreMenuPath);

            bool seeMoreRegistered = AddDynamicMenuItem(seeMoreMenuPath, priority + 10, OpenPrefabsFolder);
            if (seeMoreRegistered) registeredHierarchyMenuItems.Add(seeMoreMenuPath);
            else registrationFailed = true;

            // Only treat this menu state as complete when every required item was registered successfully.
            currentPrefabMenuSignature = registrationFailed ? null : newSignature;
        }

        static string GetPrefabsFolderPath([CallerFilePath] string sourceFilePath = "") {
            if (!string.IsNullOrEmpty(cachedPrefabsFolderPath) && AssetDatabase.IsValidFolder(cachedPrefabsFolderPath))
                return cachedPrefabsFolderPath;

            if (string.IsNullOrEmpty(sourceFilePath)) {
                Debug.LogError("Unable to locate VirtualJoystickEditor.cs.");
                return null;
            }

            string sourceFullPath = NormaliseFullPath(sourceFilePath);
            string projectRootPath = NormaliseFullPath(Path.Combine(Application.dataPath, ".."));
            DirectoryInfo directory = new FileInfo(sourceFullPath).Directory;

            while (directory != null) {
                string possibleAssetRoot = NormaliseFullPath(directory.FullName);
                if (!IsSameOrChildPath(possibleAssetRoot, projectRootPath)) break;

                string scriptsFullPath = NormaliseFullPath(Path.Combine(possibleAssetRoot, SCRIPTS_FOLDER_NAME));
                string prefabsFullPath = NormaliseFullPath(Path.Combine(possibleAssetRoot, PREFABS_FOLDER_NAME));

                // The correct Asset root must contain both:
                //
                // Asset Root/Scripts/.../VirtualJoystickEditor.cs
                // Asset Root/Prefabs
                //
                // This prevents the search from accidentally using an unrelated Assets/Prefabs folder.
                if (IsSameOrChildPath(sourceFullPath, scriptsFullPath) && Directory.Exists(prefabsFullPath)) {
                    string prefabsAssetPath = GetProjectRelativePath(prefabsFullPath, projectRootPath);
                    if (!string.IsNullOrEmpty(prefabsAssetPath) && AssetDatabase.IsValidFolder(prefabsAssetPath)) {
                        cachedPrefabsFolderPath = prefabsAssetPath;
                        return cachedPrefabsFolderPath;
                    }
                }

                directory = directory.Parent;
            }

            Debug.LogError(
                "Unable to find the Virtual Joystick Prefabs folder relative to VirtualJoystickEditor.cs.\n" +
                $"Editor script location: {sourceFilePath}"
            );

            return null;
        }

        static string NormaliseFullPath(string path) {
            return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        static bool IsSameOrChildPath(string path, string parentPath) {
            string normalisedPath = NormaliseFullPath(path);
            string normalisedParentPath = NormaliseFullPath(parentPath);

            if (normalisedPath.Equals(normalisedParentPath, StringComparison.OrdinalIgnoreCase))
                return true;

            return normalisedPath.StartsWith(normalisedParentPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        static string GetProjectRelativePath(string fullPath, string projectRootPath) {
            string normalisedFullPath = NormaliseFullPath(fullPath);
            string normalisedProjectRoot = NormaliseFullPath(projectRootPath);

            if (!IsSameOrChildPath(normalisedFullPath, normalisedProjectRoot))
                return null;

            string relativePath = normalisedFullPath.Substring(normalisedProjectRoot.Length).TrimStart(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
            );

            return relativePath.Replace('\\', '/');
        }

        static List<string> FindBasePrefabPaths(string prefabsFolderPath) {
            List<string> prefabPaths = new List<string>();

            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabsFolderPath });

            foreach (string guid in prefabGuids) {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (!prefab) continue;
                if (PrefabUtility.GetPrefabAssetType(prefab) != PrefabAssetType.Regular) continue;

                prefabPaths.Add(prefabPath);
            }

            prefabPaths.Sort((leftPath, rightPath) =>
                string.Compare(
                    Path.GetFileNameWithoutExtension(leftPath),
                    Path.GetFileNameWithoutExtension(rightPath),
                    StringComparison.OrdinalIgnoreCase
                )
            );

            return prefabPaths;
        }

        static bool AddDynamicMenuItem(string menuPath, int priority, Action execute) {
            try {
                if (addMenuItemMethod == null) {
                    addMenuItemMethod = typeof(Menu).GetMethod(
                        "AddMenuItem",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                        null, new Type[] {
                            typeof(string), typeof(string), typeof(bool),
                            typeof(int), typeof(Action), typeof(Func<bool>)
                        }, null
                    );
                }

                if (addMenuItemMethod == null) {
                    Debug.LogError("This Unity version does not expose Menu.AddMenuItem with the expected signature.");
                    return false;
                }

                addMenuItemMethod.Invoke(null, new object[] {
                    menuPath, string.Empty, false, priority,
                    execute, new Func<bool>(() => true)
                });
                return true;
            } catch (TargetInvocationException exception) {
                Debug.LogException(exception.InnerException ?? exception);
                return false;
            } catch (Exception exception) {
                Debug.LogException(exception);
                return false;
            }
        }

        static void RemoveRegisteredHierarchyMenuItems() {
            foreach (string menuPath in registeredHierarchyMenuItems) {
                RemoveDynamicMenuItem(menuPath);
            }

            registeredHierarchyMenuItems.Clear();
            currentPrefabMenuSignature = null;
        }

        static void RemoveDynamicMenuItem(string menuPath) {
            try {
                if (removeMenuItemMethod == null) {
                    removeMenuItemMethod = typeof(Menu).GetMethod(
                        "RemoveMenuItem",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                        null, new Type[] { typeof(string) }, null
                    );
                }
                removeMenuItemMethod?.Invoke(null, new object[] { menuPath });
            } catch (TargetInvocationException exception) {
                Debug.LogException(exception.InnerException ?? exception);
            } catch (Exception exception) {
                Debug.LogException(exception);
            }
        }

        static void CreateJoystick(string prefabPath) {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (!prefab) {
                Debug.LogError($"Virtual Joystick Prefab was not found at: {prefabPath}");
                return;
            }

            // Add action history.
            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName($"Create {prefab.name}");

            // Add the prefab.
            Transform parent = GetUIParent();
            GameObject joystick = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;

            if (!joystick) {
                Debug.LogError($"Failed to create {prefab.name}.");
                Undo.CollapseUndoOperations(undoGroup);
                return;
            }

            joystick.transform.SetAsLastSibling();

            Undo.RegisterCreatedObjectUndo(joystick, $"Create {prefab.name}");
            Selection.activeGameObject = joystick;
            EditorGUIUtility.PingObject(joystick);

            Undo.CollapseUndoOperations(undoGroup);
        }

        static Transform GetUIParent() {
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject) {
                RectTransform selectedRectTransform = selectedObject.GetComponent<RectTransform>();
                Canvas parentCanvas = selectedObject.GetComponentInParent<Canvas>();

                if (selectedRectTransform && parentCanvas)
                    return selectedObject.transform;
            }

#if UNITY_2022_2_OR_NEWER
            Canvas existingCanvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
#else
            Canvas existingCanvas = UnityEngine.Object.FindObjectOfType<Canvas>();
#endif

            if (existingCanvas)
                return existingCanvas.transform;
            return CreateCanvas().transform;
        }

        // Creates a canvas GameObject on the Scene.
        static Canvas CreateCanvas() {
            GameObject canvasObject = new GameObject(
                "Canvas", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster)
            );

            int uiLayer = LayerMask.NameToLayer("UI");

            if (uiLayer >= 0) canvasObject.layer = uiLayer;
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            Undo.RegisterCreatedObjectUndo(canvasObject, "Create Canvas");
            CreateEventSystem();

            return canvas;
        }

        // Creates an EventSystem. For housing the joysticks we create.
        static void CreateEventSystem() {
#if UNITY_2022_2_OR_NEWER
            EventSystem existingEventSystem = UnityEngine.Object.FindFirstObjectByType<EventSystem>();
#else
            EventSystem existingEventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
#endif

            if (existingEventSystem) return;
            GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            eventSystemObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
            Undo.RegisterCreatedObjectUndo(eventSystemObject, "Create EventSystem");
        }

        static void OpenPrefabsFolder() {
            string prefabsFolderPath = GetPrefabsFolderPath();

            if (string.IsNullOrEmpty(prefabsFolderPath) || !AssetDatabase.IsValidFolder(prefabsFolderPath)) {
                Debug.LogError("Virtual Joystick Prefabs folder was not found.");
                return;
            }

            pendingProjectFolderPath = prefabsFolderPath;

            // Open or focus the Project window first.
            EditorUtility.FocusProjectWindow();

            // Wait until Unity has finished focusing the window.
            EditorApplication.delayCall -= OpenPendingProjectFolder;
            EditorApplication.delayCall += OpenPendingProjectFolder;
        }

        static void OpenPendingProjectFolder() {
            string folderPath = pendingProjectFolderPath;
            pendingProjectFolderPath = null;

            if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath)) return;
            UnityEngine.Object folderAsset = AssetDatabase.LoadMainAssetAtPath(folderPath);

            if (!folderAsset) {
                Debug.LogError($"Unable to load Project folder at: {folderPath}");
                return;
            }

            Type projectBrowserType = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
            if (projectBrowserType == null) {
                SelectAndPingFolder(folderAsset);
                return;
            }

            EditorWindow projectBrowser = GetProjectBrowserWindow(projectBrowserType);
            if (!projectBrowser) {
                SelectAndPingFolder(folderAsset);
                return;
            }

            int folderInstanceID = GetProjectFolderInstanceID(folderPath, folderAsset);
            if (folderInstanceID == 0) {
                Debug.LogWarning($"Unable to obtain the Project Browser ID for folder: {folderPath}");
                SelectAndPingFolder(folderAsset);
                return;
            }

            BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo initialiseMethod = projectBrowserType.GetMethod("Init", instanceFlags, null, Type.EmptyTypes, null);
            FieldInfo viewModeField = projectBrowserType.GetField("m_ViewMode", instanceFlags);

            try {
                projectBrowser.Show();
                projectBrowser.Focus();

                initialiseMethod?.Invoke(projectBrowser, null);

                string viewMode = viewModeField?.GetValue(projectBrowser)?.ToString();
                if (viewMode == "OneColumn") {
                    // Unity's One Column Project Browser opens folders through AssetDatabase.OpenAsset.
                    Selection.activeObject = folderAsset;
                    AssetDatabase.OpenAsset(folderInstanceID);
                } else {
                    OpenFolderInTwoColumnProjectBrowser(projectBrowserType, projectBrowser, folderInstanceID, folderAsset);
                }

                projectBrowser.Repaint();
            } catch (TargetInvocationException exception) {
                Debug.LogWarning(
                    "Unable to open the Virtual Joystick Prefabs folder.\n" +
                    (exception.InnerException?.Message ?? exception.Message)
                );

                SelectAndPingFolder(folderAsset);
            } catch (Exception exception) {
                Debug.LogWarning($"Unable to open the Virtual Joystick Prefabs folder.\n{exception.Message}");
                SelectAndPingFolder(folderAsset);
            }
        }

        static void OpenFolderInTwoColumnProjectBrowser(Type projectBrowserType, EditorWindow projectBrowser, int folderInstanceID, UnityEngine.Object folderAsset) {
            BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            MethodInfo setFolderSelectionMethod = projectBrowserType.GetMethod(
                "SetFolderSelection", instanceFlags, null,
                new Type[] { typeof(int[]), typeof(bool) }, null
            );

            if (setFolderSelectionMethod == null) {
                Debug.LogWarning("This Unity version does not expose ProjectBrowser.SetFolderSelection.");
                SelectAndPingFolder(folderAsset);
                return;
            }

            setFolderSelectionMethod.Invoke(projectBrowser, new object[] { new[] { folderInstanceID }, true });
        }

        static EditorWindow GetProjectBrowserWindow(Type projectBrowserType) {
            EditorWindow focusedWindow = EditorWindow.focusedWindow;

            if (focusedWindow && projectBrowserType.IsInstanceOfType(focusedWindow))
                return focusedWindow;

            BindingFlags staticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            FieldInfo lastInteractedField = projectBrowserType.GetField("s_LastInteractedProjectBrowser", staticFlags);
            EditorWindow lastInteractedWindow = lastInteractedField?.GetValue(null) as EditorWindow;

            if (lastInteractedWindow) return lastInteractedWindow;

            UnityEngine.Object[] projectBrowsers = Resources.FindObjectsOfTypeAll(projectBrowserType);

            foreach (UnityEngine.Object browserObject in projectBrowsers) {
                EditorWindow browserWindow = browserObject as EditorWindow;
                if (browserWindow) return browserWindow;
            }

            return EditorWindow.GetWindow(projectBrowserType);
        }

        static int GetProjectFolderInstanceID(string folderPath, UnityEngine.Object folderAsset) {
            BindingFlags staticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            MethodInfo getFolderIDMethod = typeof(AssetDatabase).GetMethod(
                "GetMainAssetOrInProgressProxyInstanceID",
                staticFlags, null, new Type[] { typeof(string) }, null
            );

            if (getFolderIDMethod != null) {
                try {
                    object result = getFolderIDMethod.Invoke(null, new object[] { folderPath });

                    if (result is int) {
                        int folderInstanceID = (int)result;
                        if (folderInstanceID != 0) return folderInstanceID;
                    }
                } catch {
                    // Use the loaded folder asset ID below.
                }
            }

            return folderAsset ? folderAsset.GetInstanceID() : 0;
        }

        static void SelectAndPingFolder(UnityEngine.Object folderAsset) {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = folderAsset;
            EditorGUIUtility.PingObject(folderAsset);
        }
    }
}
