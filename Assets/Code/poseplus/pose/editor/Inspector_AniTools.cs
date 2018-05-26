using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(FB.PosePlus.FBaniTools))]
public class Inspector_AniTools : Editor
{
    FB.PosePlus.FBaniTools aniTools;
    public override void OnInspectorGUI()
    {
        aniTools = target as FB.PosePlus.FBaniTools;
        base.OnInspectorGUI();

        if(GUILayout.Button("数据编辑"))
        {
            if(aniTools.clip_a == null || aniTools.clip_b == null)
            {
                EditorUtility.DisplayDialog("错误", "加上数据好吧!", "我错了大人！");
                return;
            }
            Window_AniTools.Show(aniTools.clip_a, aniTools.clip_b);
        }
    }



}