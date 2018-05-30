using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Battle.Skill;
using FB.PosePlus;

public class Window_SelectAniClip : EditorWindow
{
    private List<SkillBlock> _skillBlocks;

    private AniPlayer _ani;

    public void Show(List<SkillBlock> blocks, AniPlayer ani)
    {
        this._ani = ani;
        this._skillBlocks = blocks;
        this.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        this.ShowAniclipList();
        this.ShowCurFrame();
        GUILayout.EndHorizontal();
        TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
    }
    
    private Vector2 pos = Vector2.zero;

    private void ShowAniclipList()
    {
        pos = GUILayout.BeginScrollView(pos, GUILayout.Width(320), GUILayout.Height(300));
        GUILayout.BeginVertical();
        foreach (AniClip clip in this._ani.clips)
        {
            this.DrawAniClipItem(clip);
        }
        
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
        
    }

    private void DrawAniClipItem(AniClip clip)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(300));
        if (this._curClip != null)
        {
            if (this._curClip.name == clip.name)
            {
                GUI.color = Color.green;
            }
            else
            {
                GUI.color = Color.white;
            }
        }

        if (GUILayout.Button(clip.name))
        {
            this._curClip = clip;
        }

        GUI.color = GUI.backgroundColor;
        if (GUILayout.Button("确定",GUILayout.Width(100)))
        {
            SkillBlock sb = new SkillBlock();
            sb.AniName = clip.name;
            if(this._skillBlocks == null )this._skillBlocks = new List<SkillBlock>();
            this._skillBlocks.Add(sb);
            this.Close();
        }

        GUILayout.EndHorizontal();
    }

    private AniClip _curClip;

    private void ShowCurFrame()
    {
        GUILayout.BeginVertical();
        if (this._curClip!=null)
        {
            this.ShowClipFrame(this._curClip);
        }

        GUILayout.EndVertical();
    }

    #region 动画帧编辑器

    private int curframe = 0;

    private Vector2 anipos;

    //当前选择的动画片段
    //private AniClip curClip = null;
    private void ShowClipFrame(AniClip ani)
    {
        GUILayout.Label("Animation pos:(" + curframe + "/" + ani.aniFrameCount + ")");

        int nf = (int) GUILayout.HorizontalScrollbar(curframe, 1, 0, ani.aniFrameCount);
        anipos = EditorGUILayout.BeginScrollView(anipos, true, false, GUILayout.Height(230));
        GUILayout.BeginHorizontal();
        for (int i = 0; i < ani.aniFrameCount; i++)
        {
            var obc = GUI.backgroundColor;
            if (curframe == i)
            {
                GUI.backgroundColor = Color.green;
            }

            GUILayout.BeginVertical(GUILayout.Width(1));

            var oc = GUI.color;
            string txt = "F";
            if (ani.frames[i].key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                txt = "K";
            }

            GUILayout.Label(i.ToString());
            if (GUILayout.Button(txt))
            {
                nf = i;
            }

            //关键帧下需要条垂直的空格
            if (ani.frames[i].key)
            {
                GUILayout.Space(123);
            }

            GUI.color = oc;
            if (ani.frames[i].key == false)
            {
                //调整曲线
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                float lerp = GUILayout.VerticalScrollbar(ani.frames[i].lerp, 0.01f, 1.0f, 0, GUILayout.Height(120));
                if (lerp != ani.frames[i].lerp)
                {
                    ani.frames[i].lerp = lerp;
                    ani.CalcLerpFrameOne(i);
                    if (i == curframe)
                    {
                        SetFrame(curframe, true);
                        EditorUtility.SetDirty(ani);
                    }
                }

                GUILayout.EndHorizontal();
            }

            //box帧按钮
            string b_txt = "F";
            if (ani.frames[i].box_key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                b_txt = "K";
            }

            if (GUILayout.Button(b_txt))
            {
                nf = i;
            }


            GUI.color = oc;

            string d_txt = "○";
            if (ani.frames[i].dot_key)
            {
                GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                d_txt = "●";
            }

            if (GUILayout.Button(d_txt))
            {
                nf = i;
            }

            GUILayout.EndVertical();
            GUI.color = oc;
            GUI.backgroundColor = obc;
        }

        SetFrame(nf);

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
    }

    void SetFrame(int f, bool force = false)
    {
        if (curframe != f || force)
        {
            this._ani.SetPose(_curClip, f, true);
            curframe = f;
        }
    }

    #endregion
}