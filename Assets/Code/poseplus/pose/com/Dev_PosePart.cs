using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FB.PosePlus
{
    public class Dev_PosePart : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        class MeshRef
        {
            public Dictionary<SkinnedMeshRenderer, List<int>> skinpointUse = new Dictionary<SkinnedMeshRenderer, List<int>>();
        }
        public void SplitMesh(bool recalcbound = false)
        {
            if (split != null)
            {
                foreach (var s in split)
                {
                    UnityEngine.Object.DestroyImmediate(s);
                }
            }
            var skins = this.GetComponentsInChildren<SkinnedMeshRenderer>();
            Debug.LogWarning("got skin:" + skins.Length);
            Dictionary<Transform, MeshRef> refs = new Dictionary<Transform, MeshRef>();
            src = new List<SkinnedMeshRenderer>();
            split = new List<SkinnedMeshRenderer>();
            foreach (var skin in skins)
            {
                src.Add(skin);
                //skin 使用的骨骼 s.bones[]
                for (int i = 0; i < skin.sharedMesh.boneWeights.Length; i++)
                {
                    //got max index
                    int i0 = skin.sharedMesh.boneWeights[i].boneIndex0;
                    int i1 = skin.sharedMesh.boneWeights[i].boneIndex1;
                    int i2 = skin.sharedMesh.boneWeights[i].boneIndex2;
                    int i3 = skin.sharedMesh.boneWeights[i].boneIndex3;
                    float w0 = skin.sharedMesh.boneWeights[i].weight0;
                    float w1 = skin.sharedMesh.boneWeights[i].weight1;
                    float w2 = skin.sharedMesh.boneWeights[i].weight2;
                    float w3 = skin.sharedMesh.boneWeights[i].weight3;
                    int iout = i0;
                    float mw = w0;
                    if (w1 > w0)
                    {
                        iout = i1;
                        mw = w1;
                    }
                    if (w2 > w1)
                    {
                        iout = i2;
                        mw = w2;
                    }
                    if (w3 > w2)
                    {
                        iout = i3;
                        mw = w3;
                    }

                    var bone = skin.bones[iout];
                    //int boneid = skin.bones[iout].GetInstanceID();
                    if (refs.ContainsKey(bone) == false)
                    {

                        refs[bone] = new MeshRef();
                    }

                    //int skinid = skin.GetInstanceID();
                    if (refs[bone].skinpointUse.ContainsKey(skin) == false)
                    {
                        refs[bone].skinpointUse[skin] = new List<int>();
                    }
                    refs[bone].skinpointUse[skin].Add(i);

                }
            }
            foreach (var r in refs)
            {

                SplitMeshSingle(r.Key, r.Value, recalcbound);
            }
        }
        public List<SkinnedMeshRenderer> split = null;
        public List<SkinnedMeshRenderer> src = null;
        void SplitMeshSingle(Transform bone, MeshRef mref, bool recalcBound = false)
        {
            if (mref.skinpointUse.Count == 0)
                return;

            if (mref.skinpointUse.Count == 1)
            {//只有一个引用模型，快速减面
                SkinnedMeshRenderer mr = bone.GetComponent<SkinnedMeshRenderer>();
                if (mr == null)
                {
                    mr = bone.gameObject.AddComponent<SkinnedMeshRenderer>();
                    //mr.hideFlags = HideFlags.DontSave;
                }
                mr.enabled = false;


                split.Add(mr);

                foreach (var rr in mref.skinpointUse)
                {//foreach 只会执行一次
                    //GameObject objr = new GameObject();
                    //
                    //objr.transform.SetParent(t, false);


                    mr.bones = rr.Key.bones;

                    mr.material = new Material(Shader.Find("Standard"));
                    Mesh m = new Mesh();
                    var srcmesh = rr.Key.sharedMesh;
                    m.name = srcmesh.name;
                    m.bounds = srcmesh.bounds;
                    m.vertices = srcmesh.vertices;
                    m.bindposes = srcmesh.bindposes;
                    m.boneWeights = srcmesh.boneWeights;
                    m.colors = srcmesh.colors;
                    m.normals = srcmesh.normals;
                    m.tangents = srcmesh.tangents;
                    //m.uv = srcmesh.uv;
                    Vector3[] vertices = srcmesh.vertices;
                    for (int i = 0; i < srcmesh.subMeshCount; i++)
                    {
                        int[] srcindex = srcmesh.GetIndices(i);
                        //int[] outinde = new int[srcindex.Length];
                        //int length = 0;
                        List<int> outinde = new List<int>();
                        for (int s = 0; s < srcindex.Length / 3; s++)
                        {
                            if (rr.Value.Contains(srcindex[s * 3 + 0])
                                || rr.Value.Contains(srcindex[s * 3 + 1])
                                || rr.Value.Contains(srcindex[s * 3 + 2])
                                )
                            {
                                if (recalcBound)
                                {
                                    if (rr.Value.Contains(srcindex[s * 3 + 0]) == false)
                                        rr.Value.Add(srcindex[s * 3 + 0]);
                                    if (rr.Value.Contains(srcindex[s * 3 + 1]) == false)
                                        rr.Value.Add(srcindex[s * 3 + 1]);
                                    if (rr.Value.Contains(srcindex[s * 3 + 2]) == false)
                                        rr.Value.Add(srcindex[s * 3 + 2]);
                                }
                                outinde.Add(srcindex[s * 3 + 0]);
                                outinde.Add(srcindex[s * 3 + 1]);
                                outinde.Add(srcindex[s * 3 + 2]);

                            }
                        }

                        m.SetTriangles(outinde.ToArray(), i);
                    }
                    if (recalcBound)
                    {
                        for (int i = 0; i < srcmesh.vertexCount; i++)
                        {
                            if (rr.Value.Contains(i) == false)
                            {
                                vertices[i] = vertices[rr.Value[0]];
                            }
                        }
                        m.vertices = vertices;
                    }
                    m.RecalculateBounds();

                    mr.sharedMesh = m;
                }

            }
            else
            {//多个引用模型，合并模型

                SkinnedMeshRenderer mr = bone.GetComponent<SkinnedMeshRenderer>();
                if (mr == null)
                {
                    mr = bone.gameObject.AddComponent<SkinnedMeshRenderer>();
                    //mr.hideFlags = HideFlags.DontSave;
                }
                mr.enabled = false;
                mr.material = new Material(Shader.Find("Standard"));

                split.Add(mr);

                List<Transform> bones = new List<Transform>();
                string mname = "";
                List<Bounds> bounds = new List<Bounds>();
                List<Vector3> vertices = new List<Vector3>();
                List<Matrix4x4> bindposes = new List<Matrix4x4>();
                List<BoneWeight> boneWeights = new List<BoneWeight>();
                List<Color> colors = new List<Color>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector4> tangents = new List<Vector4>();
                List<Vector2> uv = new List<Vector2>();
                List<int> Triangles = new List<int>();
                int vseed = 0;
                int bseed = 0;
                foreach (var rr in mref.skinpointUse)
                {
                    bones.AddRange(rr.Key.bones);
                    var srcmesh = rr.Key.sharedMesh;

                    mname += srcmesh.name + ";";

                    bounds.Add(srcmesh.bounds);
                    vertices.AddRange(srcmesh.vertices);

                    bindposes.AddRange(srcmesh.bindposes);
                    foreach (var b in srcmesh.boneWeights)
                    {
                        BoneWeight bb = b;
                        bb.boneIndex0 += bseed;
                        bb.boneIndex1 += bseed;
                        bb.boneIndex2 += bseed;
                        bb.boneIndex3 += bseed;
                        boneWeights.Add(bb);
                    }
                    //boneWeights.AddRange(srcmesh.boneWeights);
                    colors.AddRange(srcmesh.colors);
                    normals.AddRange(srcmesh.normals);
                    tangents.AddRange(srcmesh.tangents);
                    uv.AddRange(srcmesh.uv);
                    //m.uv = srcmesh.uv;

                    for (int i = 0; i < srcmesh.subMeshCount; i++)
                    {
                        int[] srcindex = srcmesh.GetIndices(i);
                        int[] outinde = new int[srcindex.Length];

                        for (int s = 0; s < outinde.Length / 3; s++)
                        {
                            if (rr.Value.Contains(srcindex[s * 3 + 0])
                                || rr.Value.Contains(srcindex[s * 3 + 1])
                                || rr.Value.Contains(srcindex[s * 3 + 2])
                                )
                            {
                                Triangles.Add(srcindex[s * 3 + 0] + vseed);
                                Triangles.Add(srcindex[s * 3 + 1] + vseed);
                                Triangles.Add(srcindex[s * 3 + 2] + vseed);

                            }
                        }

                    }
                    vseed += srcmesh.vertices.Length;
                    bseed += rr.Key.bones.Length;
                }
                if (recalcBound)
                {
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        if (Triangles.Contains(i) == false)
                        {
                            vertices[i] = vertices[Triangles[0]];
                        }
                    }
                }
                mr.bones = bones.ToArray();
                Mesh m = new Mesh();
                m.name = mname;
                m.vertices = vertices.ToArray();
                m.bindposes = bindposes.ToArray();
                m.boneWeights = boneWeights.ToArray();
                m.colors = colors.ToArray();
                m.normals = normals.ToArray();
                m.tangents = tangents.ToArray();
                m.SetTriangles(Triangles.ToArray(), 0);
                mr.sharedMesh = m;
                mr.sharedMesh.RecalculateBounds();
            }

        }
        public void DeleteSplit()
        {
            if (split != null)
            {
                foreach (var s in split)
                {
                    UnityEngine.Object.DestroyImmediate(s);
                }
            }

            split = null;
            src = null;
        }
        public void DeleteSplitForce()
        {
            if (split != null)
            {
                foreach (var s in split)
                {
                    UnityEngine.Object.DestroyImmediate(s);
                }
            }

            split = null;
            src = null;
            List<Transform> bones = new List<Transform>();
            SkinnedMeshRenderer[] allrender = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var ar in allrender)
            {
                foreach (var b in ar.bones)
                {
                    if (b == null) continue;
                    if (bones.Contains(b) == false)
                        bones.Add(b);
                }
            }
            foreach (var ar in allrender)
            {
                if (bones.Contains(ar.transform))//在骨骼上
                {
                    UnityEngine.Object.DestroyImmediate(ar);
                }
            }
        }
        public void ShowMeshSplit(bool show)
        {
            if (split != null)
            {
                foreach (var s in split)
                {
                    s.enabled = show;
                }
            }
            if (src != null)
            {
                foreach (var s in src)
                {
                    s.enabled = !show;
                }
            }
        }

    }
}
