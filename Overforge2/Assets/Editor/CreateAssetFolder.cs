using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Adds a right-click option in the Project window to create a structured Asset Folder.
/// Creates: [Name]/Textures and [Name]/Materials with a default URP material named M_[Name].
/// Place this file inside any Editor folder in your Unity project (e.g. Assets/Editor/).
/// </summary>
public class CreateAssetFolder : EditorWindow
{
    // ── State ────────────────────────────────────────────────────────────────
    private string  _folderName      = "NewAsset";
    private string  _targetDirectory = "Assets";
    private bool    _focusField      = true;          // auto-focus the text field once

    // ── Menu item ────────────────────────────────────────────────────────────

    [MenuItem("Assets/Create/Asset Folder", false, 19)]   // 19 → sits just below "Folder"
    private static void OpenCreateAssetFolderWindow()
    {
        // Resolve where to create the folder (selected folder or Assets root)
        string selectedPath = GetSelectedFolderPath();

        CreateAssetFolder window = GetWindow<CreateAssetFolder>(true, "New Asset Folder", true);
        window._targetDirectory = selectedPath;
        window._folderName      = "NewAsset";
        window._focusField      = true;
        window.minSize          = new Vector2(340, 120);
        window.maxSize          = new Vector2(340, 120);
        window.ShowUtility();
    }

    // ── EditorWindow GUI ─────────────────────────────────────────────────────

    private void OnGUI()
    {
        GUILayout.Space(14);

        EditorGUILayout.LabelField("Asset Folder Name", EditorStyles.boldLabel);

        GUILayout.Space(4);

        GUI.SetNextControlName("FolderNameField");
        _folderName = EditorGUILayout.TextField(_folderName);

        // Auto-focus + select all text on first draw
        if (_focusField)
        {
            EditorGUI.FocusTextInControl("FolderNameField");
            _focusField = false;
        }

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Create", GUILayout.Height(28)))
        {
            TryCreate();
        }

        if (GUILayout.Button("Cancel", GUILayout.Height(28)))
        {
            Close();
        }

        EditorGUILayout.EndHorizontal();

        // Allow Enter key to confirm
        Event e = Event.current;
        if (e.type == EventType.KeyDown &&
            (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter))
        {
            TryCreate();
            e.Use();
        }

        // Allow Escape key to cancel
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            Close();
            e.Use();
        }
    }

    // ── Folder + material creation ───────────────────────────────────────────

    private void TryCreate()
    {
        string trimmed = _folderName.Trim();

        if (string.IsNullOrEmpty(trimmed))
        {
            EditorUtility.DisplayDialog("Invalid Name", "Please enter a valid folder name.", "OK");
            return;
        }

        // Sanitize characters that are invalid for folder/asset names
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            if (trimmed.Contains(c.ToString()))
            {
                EditorUtility.DisplayDialog(
                    "Invalid Name",
                    $"The name contains an invalid character: '{c}'",
                    "OK");
                return;
            }
        }

        CreateStructure(_targetDirectory, trimmed);
        Close();
    }

    private static void CreateStructure(string parentPath, string folderName)
    {
        // ── 1. Root folder ───────────────────────────────────────────────────
        string rootGUID      = AssetDatabase.CreateFolder(parentPath, folderName);
        string rootPath      = AssetDatabase.GUIDToAssetPath(rootGUID);

        // ── 2. Sub-folders ───────────────────────────────────────────────────
        AssetDatabase.CreateFolder(rootPath, "Textures");

        string materialsGUID = AssetDatabase.CreateFolder(rootPath, "Materials");
        string materialsPath = AssetDatabase.GUIDToAssetPath(materialsGUID);

        // ── 3. URP Lit material ──────────────────────────────────────────────
        Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");

        if (urpLitShader == null)
        {
            // Fallback: Standard shader (Built-in RP) so the asset is still created
            urpLitShader = Shader.Find("Standard");
            Debug.LogWarning(
                "[CreateAssetFolder] URP shader not found. " +
                "Make sure URP is installed and set as the active render pipeline. " +
                "Falling back to Standard shader.");
        }

        Material mat = new Material(urpLitShader)
        {
            name = $"M_{folderName}"
        };

        string materialPath = $"{materialsPath}/M_{folderName}.mat";
        AssetDatabase.CreateAsset(mat, materialPath);

        // ── 4. Refresh & ping the root folder ────────────────────────────────
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Highlight the created root folder in the Project window
        Object rootFolder = AssetDatabase.LoadAssetAtPath<Object>(rootPath);
        EditorGUIUtility.PingObject(rootFolder);
        Selection.activeObject = rootFolder;

        Debug.Log(
            $"[CreateAssetFolder] Created structure:\n" +
            $"  {rootPath}/\n" +
            $"  {rootPath}/Textures/\n" +
            $"  {rootPath}/Materials/\n" +
            $"  {materialPath}");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>Returns the path of the currently selected folder in the Project window,
    /// or "Assets" if nothing (or a file) is selected.</summary>
    private static string GetSelectedFolderPath()
    {
        Object selected = Selection.activeObject;

        if (selected == null)
            return "Assets";

        string path = AssetDatabase.GetAssetPath(selected);

        if (string.IsNullOrEmpty(path))
            return "Assets";

        // If a file is selected, use its parent directory
        if (!AssetDatabase.IsValidFolder(path))
            path = Path.GetDirectoryName(path)?.Replace('\\', '/') ?? "Assets";

        return path;
    }
}
