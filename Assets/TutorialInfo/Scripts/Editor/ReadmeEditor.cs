using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Readme))]
[InitializeOnLoad]
public class ReadmeEditor : Editor
{
    private static readonly string s_ShowedReadmeSessionStateName = "ReadmeEditor.showedReadme";

    private static readonly string s_ReadmeSourceDirectory = "Assets/TutorialInfo";

    private const float k_Space = 16f;

    static ReadmeEditor()
    {
        EditorApplication.delayCall += SelectReadmeAutomatically;
    }

    private static void RemoveTutorial()
    {
        if (EditorUtility.DisplayDialog("Remove Readme Assets",

            $"All contents under {s_ReadmeSourceDirectory} will be removed, are you sure you want to proceed?",
            "Proceed",
            "Cancel"))
        {
            if (Directory.Exists(s_ReadmeSourceDirectory))
            {
                _ = FileUtil.DeleteFileOrDirectory(s_ReadmeSourceDirectory);
                _ = FileUtil.DeleteFileOrDirectory(s_ReadmeSourceDirectory + ".meta");
            }
            else
            {
                Debug.Log($"Could not find the Readme folder at {s_ReadmeSourceDirectory}");
            }

            Readme readmeAsset = SelectReadme();
            if (readmeAsset != null)
            {
                string path = AssetDatabase.GetAssetPath(readmeAsset);
                _ = FileUtil.DeleteFileOrDirectory(path + ".meta");
                _ = FileUtil.DeleteFileOrDirectory(path);
            }

            AssetDatabase.Refresh();
        }
    }

    private static void SelectReadmeAutomatically()
    {
        if (!SessionState.GetBool(s_ShowedReadmeSessionStateName, false))
        {
            Readme readme = SelectReadme();
            SessionState.SetBool(s_ShowedReadmeSessionStateName, true);

            if (readme && !readme.loadedLayout)
            {
                LoadLayout();
                readme.loadedLayout = true;
            }
        }
    }

    private static void LoadLayout()
    {
        Assembly assembly = typeof(EditorApplication).Assembly;
        Type windowLayoutType = assembly.GetType("UnityEditor.WindowLayout", true);
        MethodInfo method = windowLayoutType.GetMethod("LoadWindowLayout", BindingFlags.Public | BindingFlags.Static);
        _ = method.Invoke(null, new object[] { Path.Combine(Application.dataPath, "TutorialInfo/Layout.wlt"), false });
    }

    private static Readme SelectReadme()
    {
        string[] ids = AssetDatabase.FindAssets("Readme t:Readme");
        if (ids.Length == 1)
        {
            UnityEngine.Object readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));

            Selection.objects = new UnityEngine.Object[] { readmeObject };

            return (Readme)readmeObject;
        }
        else
        {
            Debug.Log("Couldn't find a readme");
            return null;
        }
    }

    protected override void OnHeaderGUI()
    {
        Readme readme = (Readme)target;
        Init();

        float iconWidth = Mathf.Min((EditorGUIUtility.currentViewWidth / 3f) - 20f, 128f);

        GUILayout.BeginHorizontal("In BigTitle");
        {
            if (readme.icon != null)
            {
                GUILayout.Space(k_Space);
                GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
            }
            GUILayout.Space(k_Space);
            GUILayout.BeginVertical();
            {

                GUILayout.FlexibleSpace();
                GUILayout.Label(readme.title, TitleStyle);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
    {
        Readme readme = (Readme)target;
        Init();

        foreach (Readme.Section section in readme.sections)
        {
            if (!string.IsNullOrEmpty(section.heading))
            {
                GUILayout.Label(section.heading, HeadingStyle);
            }

            if (!string.IsNullOrEmpty(section.text))
            {
                GUILayout.Label(section.text, BodyStyle);
            }

            if (!string.IsNullOrEmpty(section.linkText))
            {
                if (LinkLabel(new GUIContent(section.linkText)))
                {
                    Application.OpenURL(section.url);
                }
            }

            GUILayout.Space(k_Space);
        }

        if (GUILayout.Button("Remove Readme Assets", ButtonStyle))
        {
            RemoveTutorial();
        }
    }

    private bool m_Initialized;

    [field: SerializeField]
    private GUIStyle LinkStyle { get; set; }

    [field: SerializeField]
    private GUIStyle TitleStyle { get; set; }

    [field: SerializeField]
    private GUIStyle HeadingStyle { get; set; }

    [field: SerializeField]
    private GUIStyle BodyStyle { get; set; }

    [field: SerializeField]
    private GUIStyle ButtonStyle { get; set; }

    private void Init()
    {
        if (m_Initialized)
        {
            return;
        }

        BodyStyle = new GUIStyle(EditorStyles.label)
        {
            wordWrap = true,
            fontSize = 14,
            richText = true
        };

        TitleStyle = new GUIStyle(BodyStyle)
        {
            fontSize = 26
        };

        HeadingStyle = new GUIStyle(BodyStyle)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 18
        };

        LinkStyle = new GUIStyle(BodyStyle)
        {
            wordWrap = false
        };

        // Match selection color which works nicely for both light and dark skins
        LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
        LinkStyle.stretchWidth = false;

        ButtonStyle = new GUIStyle(EditorStyles.miniButton)
        {
            fontStyle = FontStyle.Bold
        };

        m_Initialized = true;
    }

    private bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
    {
        Rect position = GUILayoutUtility.GetRect(label, LinkStyle, options);

        Handles.BeginGUI();
        Handles.color = LinkStyle.normal.textColor;
        Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
        Handles.color = Color.white;
        Handles.EndGUI();

        EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

        return GUI.Button(position, label, LinkStyle);
    }
}
