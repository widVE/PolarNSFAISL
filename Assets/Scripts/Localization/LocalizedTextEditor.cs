using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

#if UNITY_EDITOR
public class LocalizedTextEditor : EditorWindow
{
    public LocalizationData localizationData;
    Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/Localized Text Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(LocalizedTextEditor)).Show();
    }

    private void OnGUI()
    {
        minSize = new Vector2(300, 300);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.padding.top = 6;
        style.padding.bottom = 4;
        style.fontSize = 14;
        style.fontStyle = FontStyle.Bold;
        GUILayout.Label("Localized Text Data:", style);
        
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load Dataset"))
        {
            LoadGameData();
        }

        if (GUILayout.Button("New Dataset"))
        {
            CreateNewData();
        }

        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        if (localizationData != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("localizationData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();
        }

        GUILayout.EndScrollView();

        if (localizationData != null)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save data"))
            {
                SaveGameData();
            }

            GUILayout.EndHorizontal();
        }
    }

    private void LoadGameData()
    {
        string filePath = EditorUtility.OpenFilePanel("Select localization data file", Application.streamingAssetsPath, "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);

            localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
        }
    }

    private void SaveGameData()
    {
        string filePath = EditorUtility.SaveFilePanel("Save localization data file", Application.streamingAssetsPath, "", "json");

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(localizationData);
            File.WriteAllText(filePath, dataAsJson);
        }
    }

    private void CreateNewData()
    {
        localizationData = new LocalizationData();
    }

}
#endif