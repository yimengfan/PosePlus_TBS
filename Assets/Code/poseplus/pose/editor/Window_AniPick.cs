using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


class Window_AniPick : EditorWindow
{
    public static void Show(FB.PosePlus.AniPlayer ani, string tag)
    {
        // Get existing open window or if none, make a new one:
        //获取现有的打开窗口或如果没有，创建一个新的


        var window = EditorWindow.GetWindow<Window_AniPick>(true, "Window_AniPick", true);
        //window.title = "才";
        window.SelfInit(ani, tag);

    }
    FB.PosePlus.AniPlayer ani;
    string tag;
    void SelfInit(FB.PosePlus.AniPlayer ani, string tag)
    {
        this.ani = ani;
        this.tag = tag;

        //if (GUILayout.Button("Update Clips"))
        //{
    }
    public static void Layout_DrawSeparator(Color color, float height = 4f)
    {

        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(0f, rect.yMax, Screen.width, height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(height);
    }
    public static void Layout_DrawSeparatorV(Color color, float width = 4f)
    {

        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, width, rect.height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(width);
    }
    public void Update()
    {
    }
    //public void OnLostFocus()
    //{
    //    this.Focus();
    //}
    Vector2 treePos = Vector2.zero;
    public void OnGUI()
    {
        if (ani == null)
        {
            EditorGUILayout.HelpBox("无效信息", MessageType.Warning);
            return;

        }
        string label = ani.name;

        GUILayout.Label(label);
        Layout_DrawSeparator(Color.white);
        treePos = GUILayout.BeginScrollView(treePos);
        foreach (var c in ani.clips)
        {
            if (GUILayout.Button(c.name))
            {
                //ani.SetTag(tag, c.name);
                //data.playname = c.name;
                //data.loop = c.isLooping;
                //data.anilen = c.length;
                //aniex.CalcAnim();
                this.Close();
            }
        }
        GUILayout.EndScrollView();
        Layout_DrawSeparator(Color.white);

        GUILayout.Label("www.fbact.com");
    }

}

