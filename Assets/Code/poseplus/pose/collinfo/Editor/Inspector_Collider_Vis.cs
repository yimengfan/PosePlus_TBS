using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor(typeof(Collider_Vis))]
public class Inspector_Collider_Vis : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Collider_Vis _t = target as Collider_Vis;
        if (_t == null) return;
        var coll = _t.GetComponent<Collider>();
        GUILayout.Label("CollType:" + coll);
        if (GUILayout.Button("UpdateColl"))
        {
            LineRenderer lr = _t.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.enabled = true;
            }
            _t.updateColl();
        }
        if (GUILayout.Button("CloseColl"))
        {
            LineRenderer lr = _t.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.enabled = false;
            }
        }
    }
}
