using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Collider))]
public class Collider_Vis : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        updateColl();
    }
    static void updateColl_BoxCollider(BoxCollider coll, LineRenderer lrender,float width)
    {
        Vector3 position = coll.center;
        Vector3 radii = coll.size / 2f;
        Vector3 offset = new Vector3(-radii.x, radii.y, radii.z);
        Vector3[] corner = new Vector3[8];

        lrender.SetWidth(radii.magnitude / 20.0f * width, radii.magnitude / 20.0f * width);
        lrender.useWorldSpace = false;

        //top----
        corner[0] = position + (offset); //forward left

        offset.x = radii.x;
        corner[1] = position + (offset); //forward right

        offset.z = -radii.z;
        corner[2] = position + (offset); //back right

        offset.x = -radii.x;
        corner[3] = position + (offset); //back left
        //bottom----
        offset.y = -radii.y; offset.z = radii.z;
        corner[4] = position + (offset); //forward left

        offset.x = radii.x;
        corner[5] = position + (offset); //forward right

        offset.z = -radii.z;
        corner[6] = position + (offset); //back right

        offset.x = -radii.x;
        corner[7] = position + (offset); //back left



        int seek = 0;
        lrender.SetVertexCount(46);
        //top //13
        lrender.SetPosition(seek, corner[0]); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[0], corner[1], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[0], corner[1], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[1]); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[1], corner[2], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[1], corner[2], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[2]); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[2], corner[3], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[2], corner[3], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[3]); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[3], corner[0], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[3], corner[0], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[0]); seek++;
        //door1 //9
        lrender.SetPosition(seek, Vector3.Lerp(corner[0], corner[4 + 0], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[0], corner[4 + 0], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[4 + 0]); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 0], corner[4 + 1], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 0], corner[4 + 1], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[4 + 1]); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 1], corner[1], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 1], corner[1], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[1]); seek++;
        //door2 //9
        lrender.SetPosition(seek, Vector3.Lerp(corner[1], corner[4 + 1], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[1], corner[4 + 1], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[4 + 1]); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 1], corner[4 + 2], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 1], corner[4 + 2], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[4 + 2]); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 2], corner[2], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 2], corner[2], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[2]); seek++;
        //door3 //9
        lrender.SetPosition(seek, Vector3.Lerp(corner[2], corner[4 + 2], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[2], corner[4 + 2], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[4 + 2]); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 2], corner[4 + 3], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 2], corner[4 + 3], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[4 + 3]); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 3], corner[3], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 3], corner[3], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[3]); seek++;
        //door4 //6
        lrender.SetPosition(seek, Vector3.Lerp(corner[3], corner[4 + 3], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[3], corner[4 + 3], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[4 + 3]); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 3], corner[4 + 0], 0.05f)); seek++;
        lrender.SetPosition(seek, Vector3.Lerp(corner[4 + 3], corner[4 + 0], 0.95f)); seek++;
        lrender.SetPosition(seek, corner[4 + 0]); seek++;
        //lrender.SetPosition(seek, corner[0]); seek++;
    }
    static void updateColl_SphereCollider(SphereCollider coll, LineRenderer lrender,float width)
    {
        lrender.SetWidth(coll.radius / 10.0f * width, coll.radius / 10.0f * width);
        lrender.useWorldSpace = false;

        int SIDES = 40;
        //int p = 3;
        lrender.SetVertexCount((SIDES + 2) + (SIDES + SIDES / 4 + 1) + (SIDES + 2));
        int seek = 0;
        //calculate the corners corners

        //第一圈
        for (int i = 0; i < SIDES + 2; i++)
        {
            float phi = Mathf.PI * 2.0f * (i / (float)SIDES);
            Vector3 offset = new Vector3(
                coll.radius * Mathf.Sin(phi), //left/right
                coll.radius * Mathf.Cos(phi), 			//up/down
                0 	//forward/backward
                );
            lrender.SetPosition(seek, offset + coll.center); seek++;

        }
        //第二圈，过1/4
        for (int i = 0; i < SIDES + SIDES / 4 + 1; i++)
        {
            float phi = Mathf.PI * 2.0f * (i / (float)SIDES);
            Vector3 offset = new Vector3(
                //left/right
                0,
                coll.radius * Mathf.Cos(phi),		//up/down
                coll.radius * Mathf.Sin(phi)//forward/backward
                );
            lrender.SetPosition(seek, offset + coll.center); seek++;

        }
        //第三圈
        for (int i = SIDES / 4; i < SIDES + 2 + SIDES / 4; i++)
        {
            float phi = Mathf.PI * 2.0f * (i / (float)SIDES);
            Vector3 offset = new Vector3(
                //left/right

                coll.radius * Mathf.Cos(phi),		//up/down
                  0,
                coll.radius * Mathf.Sin(phi)//forward/backward
                );
            lrender.SetPosition(seek, offset + coll.center); seek++;

        }
    }

    static void updateColl_CapsuleCollider(CapsuleCollider coll, LineRenderer lrender,float width)
    {
        lrender.SetWidth(coll.radius / 10.0f * width, coll.radius / 10.0f * width);
        lrender.useWorldSpace = false;

        Quaternion qot = Quaternion.identity;
        if (coll.direction == 0)
        {
            qot = Quaternion.Euler(0f, 0f, 90f);
        }
        else if (coll.direction == 2)
        {
            qot = Quaternion.Euler(90f, 0f, 0f);
        }
        int SIDES = 40;
        //int p = 3;
        lrender.SetVertexCount((SIDES + 2) + (SIDES + SIDES / 4 + 1) + (SIDES + 2) + 1 + (SIDES + 2));
        int seek = 0;
        //calculate the corners corners

        //第一圈
        for (int i = 0; i < SIDES + 2; i++)
        {
            float phi = Mathf.PI * 2.0f * (i / (float)SIDES);
            Vector3 offset = new Vector3(
                coll.radius * Mathf.Sin(phi), //left/right
                coll.radius * Mathf.Cos(phi), 			//up/down
                0 	//forward/backward
                );
            if (offset.y < 0) offset.y -= coll.height / 2 - coll.radius;
            else offset.y += (coll.height / 2) - coll.radius;
            lrender.SetPosition(seek, qot*offset + coll.center); seek++;

        }
        //第二圈，过1/4
        for (int i = 0; i < SIDES + SIDES / 4 + 1; i++)
        {
            float phi = Mathf.PI * 2.0f * (i / (float)SIDES);
            Vector3 offset = new Vector3(
                //left/right
                0,
                coll.radius * Mathf.Cos(phi),		//up/down
                coll.radius * Mathf.Sin(phi)//forward/backward
                );
            if (offset.y < 0) offset.y -= coll.height / 2 - coll.radius;
            else offset.y += (coll.height / 2) - coll.radius;

            lrender.SetPosition(seek, qot*offset + coll.center); seek++;

        }
        //第三圈
        for (int i = SIDES / 4; i < SIDES + 2 + SIDES / 4; i++)
        {
            float phi = Mathf.PI * 2.0f * (i / (float)SIDES);
            Vector3 offset = new Vector3(
                //left/right

                coll.radius * Mathf.Cos(phi),		//up/down
                  0,
                coll.radius * Mathf.Sin(phi)//forward/backward
                );
            offset.y -= coll.height / 2 - coll.radius;
            lrender.SetPosition(seek, qot*offset + coll.center); seek++;

        }
        {
            float phi = Mathf.PI * 2.0f * 0.25f;
            Vector3 offset = new Vector3(
                //left/right

               coll.radius * Mathf.Cos(phi),		//up/down
                 0,
               coll.radius * Mathf.Sin(phi)//forward/backward
               );
            offset.y -= coll.height / 2 - coll.radius;
            lrender.SetPosition(seek, qot*offset + coll.center); seek++;
        }
        //第四圈
        for (int i = SIDES / 4; i < SIDES + 2 + SIDES / 4; i++)
        {
            float phi = Mathf.PI * 2.0f * (i / (float)SIDES);
            Vector3 offset = new Vector3(
                //left/right

                coll.radius * Mathf.Cos(phi),		//up/down
                  0,
                coll.radius * Mathf.Sin(phi)//forward/backward
                );
            offset.y += coll.height / 2 - coll.radius;
            lrender.SetPosition(seek, qot*offset + coll.center); seek++;

        }
    }

    public Color lineColor = Color.blue;
    public float linewidth = 1.0f;
    public void updateColl()
    {
        var coll = GetComponent<Collider>();
        var lrender = GetComponent<LineRenderer>();
        if (lrender == null)
            lrender = gameObject.AddComponent<LineRenderer>();
        if (coll is BoxCollider)
        {
            updateColl_BoxCollider(coll as BoxCollider, lrender, linewidth);
        }
        else if (coll is SphereCollider)
        {
            updateColl_SphereCollider(coll as SphereCollider, lrender, linewidth);
        }
        else if (coll is CapsuleCollider)
        {
            updateColl_CapsuleCollider(coll as CapsuleCollider, lrender, linewidth);
        }
        else
        {
            Debug.LogError("do not support that:" + coll);
            return;
        }

        lrender.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
        lrender.sharedMaterial.color = this.lineColor;
        //calculate all the points

    }
    // Update is called once per frame
    void Update()
    {

    }
}
