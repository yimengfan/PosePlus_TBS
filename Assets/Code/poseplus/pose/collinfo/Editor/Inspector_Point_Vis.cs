using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor(typeof(Point_Vis))]
public class Inspector_Point_Vis : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Point_Vis _t = target as Point_Vis;
        if (_t == null) return;
        if (GUILayout.Button("UpdateLineRenderer"))
        {
            _t.UpdatePoint();
        }

    }
}
