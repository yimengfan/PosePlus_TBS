using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SkinnedMeshRenderer))]
public class SkinMeshRenderer_Inspector : Editor
{
    //
    Dictionary<string, float> anipos = new Dictionary<string, float>();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SkinnedMeshRenderer r = this.target as SkinnedMeshRenderer;
        if (r == null) return;
        if (Application.isPlaying) return;

        EditorGUILayout.HelpBox("bonecount=" + r.bones.Length, MessageType.Info);

    }

}

