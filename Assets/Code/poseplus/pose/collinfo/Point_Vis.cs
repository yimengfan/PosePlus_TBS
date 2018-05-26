using UnityEngine;
using System.Collections;
[RequireComponent(typeof(LineRenderer))]
public class Point_Vis : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        UpdatePoint();

    }
    public Color lineColor = Color.blue;
    public float linewidth = 0.125f;

    public void UpdatePoint()
    {
        LineRenderer render = this.GetComponent<LineRenderer>();
        render.useWorldSpace = false;
        render.SetVertexCount(20);
       
        render.SetPosition(0, new Vector3(-0.1f, 0, 0));
        render.SetPosition(1, new Vector3(0.1f, 0, 0));
        render.SetPosition(2, new Vector3(0, 0, 0));
        render.SetPosition(3, new Vector3(0, -0.1f, 0));
        render.SetPosition(4, new Vector3(0, 0.1f, 0));
        render.SetPosition(5, new Vector3(0, 0, 0));
        render.SetPosition(6, new Vector3(0, 0, -0.1f));
        render.SetPosition(7, new Vector3(0, 0, 0.1f));
        render.SetPosition(8, new Vector3(0, 0, 0));
        render.SetPosition(9, new Vector3(-0.05f, 0.05f, -0.05f));
        render.SetPosition(10, new Vector3(0.05f, -0.05f, 0.05f));
        render.SetPosition(11, new Vector3(0, 0, 0));
        render.SetPosition(12, new Vector3(0.05f, 0.05f, -0.05f));
        render.SetPosition(13, new Vector3(-0.05f, -0.05f, 0.05f));
        render.SetPosition(14, new Vector3(0, 0, 0));
        render.SetPosition(15, new Vector3(0.05f, -0.05f, -0.05f));
        render.SetPosition(16, new Vector3(-0.05f, 0.05f, 0.05f));
        render.SetPosition(17, new Vector3(0, 0, 0));
        render.SetPosition(18, new Vector3(-0.05f, -0.05f, -0.05f));
        render.SetPosition(19, new Vector3(0.05f, 0.05f, 0.05f));
        render.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
        render.sharedMaterial.color = this.lineColor;
        render.SetWidth(0.05f, 0.05f);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
