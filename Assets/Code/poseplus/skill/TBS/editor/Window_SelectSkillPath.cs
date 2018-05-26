using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Battle.Skill;
using UnityEngine;
using UnityEditor;

public class Window_SelectSkillPath : EditorWindow
{
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        foreach (KeyValuePair<string,GUIContent> kv in prefabDict)
        {
            this.ShowPrefab(kv);
        }

        GUILayout.EndVertical();
    }

    private void ShowPrefab(KeyValuePair<string,GUIContent> kv)
    {
        GUILayout.BeginHorizontal();
        GUIStyle style = "Label";
        Rect rt = GUILayoutUtility.GetRect(kv.Value, style);
        if (kv.Key.Equals(_se.StrParam0))
        {
            EditorGUI.DrawRect(rt, Color.gray);
        }

        if (GUI.Button(rt, kv.Value, style))
        {
            _se.StrParam0 = kv.Key;
        }
        GUILayout.Label("绝对路径:"+kv.Key);
        GUILayout.EndHorizontal();
    }

    private SkillEvent _se;


    public void Show(SkillEvent se)
    {
        this._se = se;
        base.Show();
    }

    private Dictionary<string,GUIContent> prefabDict = new Dictionary<string,GUIContent>();


    private void Awake()
    {
        string path = Application.dataPath + "/Resource";
        string[] files = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string curPath = file.Replace(Application.dataPath, "Assets").Replace("\\","/");
            GUIContent content = GetGUIContent(curPath);
            prefabDict.Add(curPath.Replace("Assets/Resource/Resources/","").Replace(".prefab",""),content);
        }
    }

    GUIContent GetGUIContent(string path)
    {
        Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        if (asset)
        {
            return new GUIContent(asset.name, AssetDatabase.GetCachedIcon(path));
        }

        return null;
    }
}