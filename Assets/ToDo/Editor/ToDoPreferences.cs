using UnityEngine;
using System.Collections;
using Todo.Utils;
using UnityEditor;

public class ToDoPreferences
{
    private static bool _prefsLoaded;

    private static bool _automaticScan;
    private static string _dataPath = "";

    [PreferenceItem("ToDo")]
    private static void ToDoPreferencesGUI()
    {
        if(!_prefsLoaded)
            LoadPreferences();

        _automaticScan = EditorGUILayout.Toggle("Auto scan", _automaticScan);
        if (_automaticScan)
        {
            EditorGUILayout.HelpBox("Note that this can cause freezes, when scanning large projects.", MessageType.Warning);
        }

        using (new HorizontalBlock())
        {
            GUILayout.Label(_dataPath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Browse", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                _dataPath = PathUtils.GlobalPathToRelative(EditorUtility.SaveFilePanel("", "Assets", "todo", "asset"));
        }

        if(GUI.changed)
            ApplyPreferences();

    }

    private static void LoadPreferences()
    {
        _automaticScan = EditorPrefs.GetBool("auto_scan", true);
        _dataPath = EditorPrefs.GetString("data_path", @"Assets/ToDo/todo.asset");
    }

    private static void ApplyPreferences()
    {
        EditorPrefs.SetBool("auto_scan", _automaticScan);
        EditorPrefs.SetString("data_path", _dataPath);
    }
}
