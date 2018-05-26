using System;
using System.Collections.Generic;

using System.Text;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(FB.PosePlus.Dev_PosePart))]
public class PosePart_Inspector : Editor
{

    bool showSplit=true ;
    //
    public override void OnInspectorGUI()
    {
        if (this.target == null) return;
        //base.OnInspectorGUI();
        //if (Application.isPlaying) return;

        var con = target as FB.PosePlus.Dev_PosePart;



        if (con.split == null)
        {
            //}
            //if (GUILayout.Button("Split Charactor Mesh(Fast)"))
            //{
            //    DateTime time = DateTime.Now;

            //    con.SplitMesh();
            //    DateTime time2 = DateTime.Now;

            //    Debug.LogWarning("split by:" + (time2 - time).TotalSeconds);
            //    showSplit = true;
            //    con.ShowMeshSplit(showSplit);
            //}
            if (GUILayout.Button("Split Charactor Mesh(Slow Recalc Part Bound)"))
            {
                DateTime time = DateTime.Now;
                con.SplitMesh(true);
                DateTime time2 = DateTime.Now;
                Debug.LogWarning("split by:" + (time2 - time).TotalSeconds);

                showSplit = true;
                con.ShowMeshSplit(showSplit);
            }
        }
        else
        {
            if (GUILayout.Button("Delete Split Mesh"))
            {
                if (EditorUtility.DisplayDialog("warning", "重新生成splitmesh 可是慢的出翔，想好了", "删", "我再想想"))
                {
                    showSplit = false;
                    try
                    {
                        con.ShowMeshSplit(showSplit);
                    }
                    catch (Exception err)
                    {

                    }
                    con.DeleteSplit();
                }
            }
        }
        if (GUILayout.Button("Show Split Mesh/Src Mesh"))
        {
            showSplit = !showSplit;
            con.ShowMeshSplit(showSplit);
        }
        GUILayout.Space(14);
        var oc = GUI.color;
        GUI.color = Color.red;
        if (GUILayout.Button("强制清理所有骨骼上的SkinMesh"))
        {
            if (EditorUtility.DisplayDialog("warning", "重新生成splitmesh 可是慢的出翔，想好了。强制清理还有可能过于暴力，记得存好。", "删", "我再想想"))
            {
                showSplit = false;
                try
                {
                    con.ShowMeshSplit(showSplit);
                }
                catch (Exception err)
                {

                }
                con.DeleteSplitForce();
            }
        }
        GUI.color = oc;
    }
}

