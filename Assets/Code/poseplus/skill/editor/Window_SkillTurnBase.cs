using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FB.PosePlus;
using Game.Battle.Skill;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

public class Window_SkillTurnBase : EditorWindow
{
    private AniPlayer ani;
    private Skills skill;

    private void Awake()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ISkillEventEditor))))
            .ToArray();
        eventDesDict.Clear();
        foreach (Type t in types)
        {
            SkillEventAttribute attribute =
                (SkillEventAttribute) t.GetCustomAttribute(typeof(SkillEventAttribute));
            eventDesDict.Add(attribute.Name,attribute.Des);
        }
    }
    
    private Dictionary<string,string> eventDesDict = new Dictionary<string, string>();

    public void Show(AniPlayer ani, Skills skill)
    {
        this.skill = skill;
        this.ani = ani;
        //var c = ani.clips[0];
        this.Show();
    }


    private void OnGUI()
    {
//        if (this.curSkill == null)
//        {
//            GUILayout.BeginVertical(GUILayout.Width(1200));
//        }
        GUILayout.BeginVertical();
        
        GUI.SetNextControlName("RefreshFocus");
        GUILayout.TextField("", GUILayout.Width(0),GUILayout.Height(0));
        GUILayout.BeginHorizontal(GUILayout.Height(800));
        this.ShowSkills();
        this.ShowSkillBlock();
        this.EditorSkillBlock();
        GUILayout.EndHorizontal();
        TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
        GUILayout.EndVertical();
    }

    private void EditorSkillBlock()
    {
        if (this.curblock != null)
        {
            EditorSkillTool.GetSkillEventDict(this.curblock, ref seDict); //获取当前block event组
        }
        else
        {
            seDict.Clear();
            return;
        }

        GUILayout.BeginVertical();
        this.ShowCurAni();
        TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
        GUILayout.Space(10);
        this.ShowSkillEventWindow();
        //重组 覆盖当前block的List<SkillEvent>
        EditorSkillTool.PushSkillEventToBlock(ref this.curblock, seDict);
        EditorUtility.SetDirty(this.ani);
        GUILayout.EndVertical();
    }

    private void ShowSkillEventWindow()
    {
        GUILayout.BeginHorizontal();
        this.ShowEventList();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
        this.ShowCurEventWindow();
        GUILayout.EndHorizontal();
    }

    private void ShowCurEventWindow()
    {
        //展示窗口
        if (this.curEvent == null) return;
        GUILayout.BeginVertical();
        //卡卡卡卡
//        Type t = EditorSkillTool.GetTypeBySEAttributeName(this.curEvent.EventName);
//        ISkillEventEditor obj = (ISkillEventEditor) t.Assembly.CreateInstance(t.FullName);
//        obj.OnGuiEditor(this.curEvent);
        GUILayout.Label(string.Format("正在编辑第{0}帧第{1}个事件{2}", this.curframe, this.eventIndex, this.curEvent.EventName));
        ISkillEventEditor obj;
        if (!this.seEditorDict.TryGetValue(this.curEvent.EventName, out obj))
        {
            Type t = EditorSkillTool.GetTypeBySEAttributeName(this.curEvent.EventName);
            if (t == null)
            {
                Debug.LogError("不存在" + this.curEvent.EventName + "标签对应的类");
                return;
            }

            obj = (ISkillEventEditor) t.Assembly.CreateInstance(t.FullName);
            this.seEditorDict.Add(this.curEvent.EventName, obj);
        }

//        string str = "";
//        EditorGUILayout.TextField("切换event点一下这个输入框", str, GUILayout.Width(200));
        obj.OnGuiEditor(this.curEvent);
        GUILayout.EndVertical();
    }

    //防止编辑器卡
    private Dictionary<string, ISkillEventEditor> seEditorDict = new Dictionary<string, ISkillEventEditor>();

    private int eventIndex = 0;
    private SkillEvent curEvent;

    private void ShowEventList()
    {
        GUILayout.BeginVertical(GUILayout.Width(400), GUILayout.Height(500));
        GUILayout.Label(string.Format("第{0}帧事件组", this.curframe));
        seList = null; //当前帧对应的事件列表
        if (!seDict.TryGetValue(this.curframe, out seList))
        {
            seList = new List<SkillEvent>();
            seDict.Add(this.curframe, seList);
        }

        if (GUILayout.Button("新建一个skillevent"))
        {
            Window_SelectSkillEvent window =
                (Window_SelectSkillEvent) EditorWindow.GetWindow(typeof(Window_SelectSkillEvent), false,
                    "选择创建skillevent");
            window.Show(this.curframe, this.curblock);
        }

        int count = seList.Count;
        if (count == 0)
        {
            curEvent = null;
            eventIndex = 0;
        }

        for (int i = 0; i < count; i++)
        {
            GUILayout.BeginHorizontal();
            SkillEvent se = seList[i];

            if (eventIndex == i)
            {
                GUI.color = Color.green;
                curEvent = se;
            }
            else
            {
                GUI.color = Color.white;
            }

            string des;
            if (!eventDesDict.TryGetValue(se.EventName, out des))
            {
                GUIStyle titleStyle2 = new GUIStyle();
                titleStyle2.normal.textColor = new Color(1, 0, 0, 1);
                GUILayout.Label("不存在标签:" + se.EventName, titleStyle2);
            }
            else
            {
                if (GUILayout.Button( des+ ":" + i))
                {
                    this.eventIndex = i;
                    GUI.FocusControl("RefreshFocus");
                }
            }

            GUI.color = GUI.backgroundColor;
            if (GUILayout.Button("del", GUILayout.Width(100)))
            {
                seList.Remove(se);
                count = seList.Count;
                if (eventIndex == i)
                {
                    curEvent = null;
                    eventIndex = 0;
                }
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    private List<SkillEvent> seList;

    private Dictionary<int, List<SkillEvent>> seDict = new Dictionary<int, List<SkillEvent>>();

    private void ShowCurAni()
    {
        GUILayout.Label("当前AniClip");
        if (ani != null && this.curblock != null && this.curSkill != null)
        {
            curClip = null;
            foreach (AniClip clip in ani.clips)
            {
                if (clip.name == this.curblock.AniName)
                {
                    curClip = clip;
                    break;
                }
            }

            //curClip = ani.clips[0];
            if (curClip == null)
            {
                Debug.LogError("没有这个动作:" + this.curblock.AniName);
                return;
            }

            ShowClipFrame(curClip);
        }
    }

    #region 显示skills

    private int skillIndex = 0;
    private Vector2 skillpos;
    private Skill curSkill;

    private void ShowSkills()
    {
        int count = this.skill.SkillList.Count;
        skillpos = GUILayout.BeginScrollView(skillpos, GUILayout.Width(350), GUILayout.Height(800));
        GUILayout.BeginVertical(GUILayout.Height(780));
        GUILayout.Label("当前skill组");
        if (count == 0)
        {
            if (GUILayout.Button("新建一个Skill",GUILayout.Width(300)))
            {
//                Skill sk = new Skill();
//                sk.Blocks = new List<SkillBlock>();
//                this.skill.SkillList.Add(sk);
                Window_SelectSkill window =
                    (Window_SelectSkill) EditorWindow.GetWindow(typeof(Window_SelectSkill), false, "SelectSkillID");
                window.Show(this.skill, 0);
            }
        }

        count = this.skill.SkillList.Count;
        for (int i = 0; i < count; i++)
        {
            Skill sk = this.skill.SkillList[i];
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            if (skillIndex == i)
            {
                GUI.color = Color.green;
                curSkill = sk;
            }
            else
            {
                GUI.color = Color.white;
            }

            if (GUILayout.Button(sk.Id + ""))
            {
                if (skillIndex != i) this.blockIndex = 0;
                skillIndex = i;
                curSkill = sk;
                GUI.FocusControl("RefreshFocus");
            }

            GUI.color = GUI.backgroundColor;
            if (GUILayout.Button("add", GUILayout.Width(50)))
            {
//                Skill tp = new Skill();
//                tp.Blocks = new List<SkillBlock>();
//                this.skill.SkillList.Insert(i + 1, tp);
                Window_SelectSkill window =
                    (Window_SelectSkill) EditorWindow.GetWindow(typeof(Window_SelectSkill), false, "SelectSkillID");
                window.Show(this.skill, i);
            }

            if (GUILayout.Button("del", GUILayout.Width(50)))
            {
                this.skill.SkillList.Remove(sk);
                if (skillIndex == i)
                {
                    curSkill = null;
                    skillIndex = 0;
                }
                else if (skillIndex > i)
                {
                    skillIndex--;
                }
            }

            count = this.skill.SkillList.Count;

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    #endregion

    #region 显示当前skill的blocks

    private Vector2 blockpos;
    private int blockIndex = 0;
    private SkillBlock curblock;

    private void ShowSkillBlock()
    {
        blockpos = GUILayout.BeginScrollView(blockpos, GUILayout.Width(320), GUILayout.Height(800));
        GUILayout.BeginVertical( GUILayout.Height(780));
        GUILayout.Label("当前skill对应skillblock组");
        if (this.curSkill != null)
            this.ShowBlockList(this.curSkill.Blocks);
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    private void ShowBlockList(List<SkillBlock> blocks)
    {
        int count = blocks.Count;
        if (count == 0)
        {
            curblock = null;
            if (GUILayout.Button("新建一个Block",GUILayout.Width(300)))
            {
                Window_SelectAniClip window =
                    (Window_SelectAniClip) EditorWindow.GetWindow(typeof(Window_SelectAniClip), false, "SelectAniClip");
                window.Show(blocks, 0, this.ani);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                SkillBlock sb = blocks[i];
                if (!EditorSkillTool.CheckAniExist(this.ani.clips, sb.AniName))
                {
                    GUIStyle titleStyle2 = new GUIStyle();
                    titleStyle2.normal.textColor = new Color(1, 0, 0, 1);
                    GUILayout.Label("ani不存在动作:" + sb.AniName, titleStyle2);
                }

                GUILayout.BeginHorizontal(GUILayout.Width(300));
                if (this.blockIndex == i)
                {
                    GUI.color = Color.green;
                    curblock = sb;
                }
                else
                {
                    GUI.color = Color.white;
                }

                if (GUILayout.Button(sb.AniName + " index:" + i))
                {
                    blockIndex = i;
                    curblock = sb;
                    eventIndex = 0;
                    curEvent = null;

                    GUI.FocusControl("RefreshFocus");
                }

                GUI.color = GUI.backgroundColor;
                if (GUILayout.Button("add", GUILayout.Width(50)))
                {
                    Window_SelectAniClip window =
                        (Window_SelectAniClip) EditorWindow.GetWindow(typeof(Window_SelectAniClip), false,
                            "SelectAniClip");
                    window.Show(blocks, i + 1, this.ani);
                }

                if (GUILayout.Button("del", GUILayout.Width(50)))
                {
                    blocks.Remove(sb);
                    count = blocks.Count;
                    if (blockIndex == i)
                    {
                        curblock = null;
                        blockIndex = 0;
                    }
                    else if (blockIndex > i)
                    {
                        blockIndex--;
                    }
                }

                GUILayout.EndHorizontal();
            }
        }
    }

    #endregion


    #region 动画帧编辑器

    private int curframe = 0;

    private Vector2 anipos;

    //当前选择的动画片段
    private AniClip curClip = null;

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
                GUI.FocusControl("RefreshFocus");
            }

            GUI.color = oc;

            List<SkillEvent> tmp;
            if (!this.seDict.TryGetValue(i, out tmp))
            {
                GUILayout.Space(60);
            }
            else
            {
                if (tmp.Count == 0)
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
            ani.SetPose(curClip, f, true);
            curframe = f;
        }
    }

    #endregion
}