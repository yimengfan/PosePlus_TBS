using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FB.PosePlus;
using Game.Battle.Skill;
using Game.Data;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Skill = Game.Battle.Skill.Skill;

public class SkillEditorData
{
    public SkillEventAttribute attr;
    public Type classdata;
}

public class Window_SkillTurnBase : EditorWindow
{
    private AniPlayer ani;
    private Skills skill;

    public void Show(AniPlayer ani, Skills skill)
    {
        this.skill = skill;
        this.ani = ani;
        if (this.skill.SkillList == null)
            this.skill.SkillList = new List<Game.Battle.Skill.Skill>();
        //var c = ani.clips[0];
        skillEditorDict = EditorSkillTool.GetSkillEditorDict();
        this.Show();
    }

    private Dictionary<string, SkillEditorData> skillEditorDict;

    private void OnGUI()
    {
        GUI.SetNextControlName("RefreshFocus");
        GUILayout.TextField("", GUILayout.Width(0), GUILayout.Height(0));
        GUILayout.BeginHorizontal(GUILayout.Height(800));
        {
            OnGUI_DrawSkillGroup();
            OnGUI_DrawSkillBlock();
            //
            GUILayout.BeginVertical();
            {
                OnGUI_DrawAniFrame();
                OnGUI_DrawSkillEventWindow();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
    }


    #region Skill列表渲染

    private int curSkillIndex = -1;
    private Skill curSkill = null;

    /// <summary>
    /// 所有技能
    /// </summary>
    private void OnGUI_DrawSkillGroup()
    {
        if (skill == null) return;
        GUILayout.BeginVertical(GUILayout.Width(300), GUILayout.Height(800));
        //保存按钮
        {
            var oc =  GUI.backgroundColor ;
            GUI.color = Color.yellow;
            if (GUILayout.Button("保存" ,GUILayout.Width(100),GUILayout.Height(30)))
            {
                EditorUtility.SetDirty(skill);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            // GUILayout.EndHorizontal();
            GUI.backgroundColor  = oc;
        }
        GUILayout.Label("技能列表:");
        //显示所有按钮
        int count = skill.SkillList.Count;
        for (int i = 0; i < count; i++)
        {
            if (curSkillIndex == i)
            {
                GUI.color = Color.green;
            }
            else
            {
                GUI.color = Color.white;
            }

            var s = skill.SkillList[i];
            s.Id = i + 1;
            GUILayout.BeginHorizontal(); //每一个技能的横条
            {
                if (GUILayout.Button(s.Id.ToString()))
                {
                    curSkillIndex = i;
                    curSkill = s;
                    this.curSkillblockList = curSkill.Blocks;

                    curSkillblockIndex = -1;
                    curSkillEvnetIndex = -1;
                    curSkillblock = null;
                    curSkillEventList = null;
                    curAniClip = null;
                    curSkillEventEditor = null;
                    GUI.FocusControl("RefreshFocus");
                }

                GUI.color = GUI.backgroundColor;

                if (GUILayout.Button("DEL", GUILayout.Width(35)))
                {
                    skill.SkillList.Remove(s);
                    curSkillIndex = -1;
                    curSkillblockIndex = -1;
                    curSkillEvnetIndex = -1;
                    curSkillblock = null;
                    curSkillEventList = null;
                    curAniClip = null;
                    curSkillEventEditor = null;
                    curSkillblockList = null;
                    count = skill.SkillList.Count;

                    GUI.FocusControl("RefreshFocus");
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(20);
        if (GUILayout.Button("创建skill"))
        {
            if (skill.SkillList == null || skill.SkillList.Count == 0)
            {
                skill.SkillList = new List<Game.Battle.Skill.Skill>();
            }

            var s = new Skill();
//            s.Id = skill.SkillList.Count;

            skill.SkillList.Add(s);
        }

        GUILayout.EndVertical();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    #endregion


    #region SkillBlock渲染

    private int curSkillblockIndex = -1;

    //当前技能块列表
    private List<SkillBlock> curSkillblockList = null;

    /// <summary>
    /// 当前技能块
    /// </summary>
    private SkillBlock curSkillblock = null;

    /// <summary>
    /// 当前技能block
    /// </summary>
    private void OnGUI_DrawSkillBlock()
    {
        GUILayout.BeginVertical(GUILayout.Width(300), GUILayout.Height(800));
       
        GUILayout.Label("技能块列表:");
        if (curSkillblockList != null)
        {
            int count = curSkillblockList.Count;
            for (int i = 0; i < count; i++)
            {
                GUILayout.BeginVertical();
                if (curSkillblockIndex == i)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }

                var sb = this.curSkillblockList[i];
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(string.Format("[{0}] -" + sb.AniName, i)))
                {
                    curSkillblockIndex = i;
                    curAniClip = ani.GetClip(sb.AniName);
                    curSkillblock = sb;
                    curSkillEventList = EditorSkillTool.GetCurFrameEventList(this.curframe, curSkillblock);
                    curSkillEvnetIndex = -1;
                    curSkillEventEditor = null;
                    GUI.FocusControl("RefreshFocus");
                }

                GUI.color = GUI.backgroundColor;
                if (GUILayout.Button("DEL", GUILayout.Width(35)))
                {
                    this.curSkillblockList.Remove(sb);
                    count = curSkillblockList.Count;
                    curSkillblockIndex = -1;
                    curSkillblock = null;
                    curSkillEventList = null;
                    curSkillEvnetIndex = -1;
                    curAniClip = null;
                    curSkillEventEditor = null;

                    GUI.FocusControl("RefreshFocus");
                }

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
        }

        GUILayout.Space(20);
        if (curSkillblockList != null)
        {
            if (GUILayout.Button("创建skillBlock"))
            {
                Window_SelectAniClip window =
                    (Window_SelectAniClip) EditorWindow.GetWindow(typeof(Window_SelectAniClip), false, "SelectAniClip");
                window.Show(this.curSkillblockList, this.ani);
            }
        }

        GUILayout.EndVertical();
        
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    #endregion


    #region 动画帧渲染

    private int curframe = 0;

    private Vector2 anipos;

    //当前选择的动画片段
    private AniClip curAniClip = null;

    private void OnGUI_DrawAniFrame()
    {
        GUILayout.Label("动画播放器:");
        var ani = curAniClip;
        anipos = EditorGUILayout.BeginScrollView(anipos, true, false, GUILayout.Height(230));
        if (curAniClip == null)
        {
            EditorGUILayout.EndScrollView();
            return;
        }

        GUILayout.Label("Animation pos:(" + curframe + "/" + ani.aniFrameCount + ")");
        int nf = (int) GUILayout.HorizontalScrollbar(curframe, 1, 0, ani.aniFrameCount);

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
                GUI.FocusControl("RefreshFocus");
            }

            GUI.color = oc;

            List<SkillEvent> tmp = EditorSkillTool.GetCurFrameEventList(i, curSkillblock);
            if (tmp.Count == 0 || tmp == null)
            {
                GUILayout.Space(60);
            }
            else
            {
                GUI.color = new Color(1.0f, 1f, 0f, 1.0f);
                GUILayout.Space(39);
                if (GUILayout.Button("e"))
                {
                    nf = i;
                    GUI.FocusControl("RefreshFocus");
                }
            }

            //关键帧下需要条垂直的空格
            if (ani.frames[i].key)
            {
                GUILayout.Space(60);
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
                        // EditorUtility.SetDirty(ani);
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
                GUI.FocusControl("RefreshFocus");
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
                GUI.FocusControl("RefreshFocus");
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
            ani.SetPose(curAniClip, f, true);
            curframe = f;
            //动画编辑器 切换帧刷新curSkillEventList
            curSkillEventList = EditorSkillTool.GetCurFrameEventList(this.curframe, curSkillblock);
        }
    }

    #endregion


    #region SkillEvent 渲染

    // 当前选中的动画帧率
    private int curSelectClipFrame = 0;

    // 当前技能事件
    private List<SkillEvent> curSkillEventList;

    /// <summary>
    /// 当前技能帧事件
    /// </summary>
    private void OnGUI_DrawSkillEvent()
    {
        GUILayout.BeginVertical(GUILayout.Width(300),GUILayout.Height(550));
        GUILayout.Label("技能事件列表:");
        if (curSkillEventList != null)
        {
            int count = curSkillEventList.Count;
            for (int i = 0; i < curSkillEventList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (curSkillEvnetIndex == i)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }

                var e = curSkillEventList[i];
                if (GUILayout.Button(string.Format("[{0}] " + e.EventName, i)))
                {
                    this.curSkillEvnetIndex = i;
                    //TODO 需要给Editor进行赋值
                    SkillEditorData data = skillEditorDict[e.EventName];
                    curSkillEventEditor =
                        (ISkillEventEditor) data.classdata.Assembly.CreateInstance(data.classdata.FullName);

                    GUI.FocusControl("RefreshFocus");
                }

                GUI.color = GUI.backgroundColor;
                if (GUILayout.Button("DEL", GUILayout.Width(35)))
                {
                    curSkillEventList.Remove(e);
                    curSkillblock.Events.Remove(e);
                    this.curSkillEvnetIndex = -1;
                    curSkillEventEditor = null;
                    count = curSkillEventList.Count;

                    GUI.FocusControl("RefreshFocus");
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(20);
        if (curSkillblock!=null)
        {
            
            if (GUILayout.Button("添加事件"))
            {
                Window_SelectSkillEvent window =
                    (Window_SelectSkillEvent) EditorWindow.GetWindow(typeof(Window_SelectSkillEvent), false,
                        "选择创建skillevent");
                window.Show(this.curframe, this.curSkillblock, this.curSkillEventList);
            }
        }

        GUILayout.EndVertical();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    private void OnGUI_DrawSkillEventWindow()
    {
        GUILayout.BeginHorizontal();
        this.OnGUI_DrawSkillEvent();
        this.OnGUI_DrawSkillEventEditor();
        GUILayout.EndHorizontal();
    }


    /// 当前选中技能事件的下标
    private int curSkillEvnetIndex = -1;

    //当前编辑器
    private ISkillEventEditor curSkillEventEditor = null;

    /// <summary>
    /// 当前技能帧事件编辑
    /// </summary>
    private void OnGUI_DrawSkillEventEditor()
    {
        if (curSkillEventEditor == null || curSkillEventList == null || curSkillEvnetIndex == -1 ||
            curSkillEventList.Count == 0) return;
        GUILayout.BeginVertical();
        var se = curSkillEventEditor.OnGuiEditor(this.curSkillEventList[curSkillEvnetIndex]);
        GUILayout.EndVertical();

        this.curSkillEventList[curSkillEvnetIndex] = se;
    }

    #endregion
}