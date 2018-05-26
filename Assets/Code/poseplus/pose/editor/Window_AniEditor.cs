using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FB.PosePlus;
using UnityEngine.UI;
using Object = System.Object;

public class  BoxPool
{
    public string name = "noname";
    public  DateTime time = DateTime.Now;
    public List<AniBoxCollider> boxInfo = new List<AniBoxCollider>();
}
class Window_AniEditor : EditorWindow
{
 

    Vector2 clipfuncpos;
    Vector2 cachefuncpos;
    Vector2 emptyfuncpos;
    Vector2 boxcahcefuncpos;
    Vector2 anichildP;
    Vector2 dotpos;
    Vector2 effectpos;
    Vector2 audiopos;
    int mToolsBar = 0;
    float xime;
    public void OnGUI()
    {
        //if (mToolsBar != 0)
        //{
        //    isPlaySubAni = false;
        //}

        xime += Time.fixedDeltaTime;
        if (editor == null || editor.aniInEdit == null)
        {
            EditorGUILayout.HelpBox("无效信息", MessageType.Warning);
            return;

        }
        string label = editor.aniInEdit.name;
        GUILayout.BeginHorizontal();
        GUILayout.Label("PosePlus编辑:" + label + ",记得多多 Ctrl + S");
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(editor.aniInEdit);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        var ooc = GUI.color;
        if (bFrameRecord)
        {
            GUI.color = new Color(1.0f, 0.4f, 0.3f);
        }
        else
        {
            GUI.color = new Color(1.0f, 0.8f, 0.3f);
        }
        bFrameRecord = GUILayout.Toggle(bFrameRecord, "自动记录动画");

        if (bLockFrame)
        {
            GUI.color = new Color(1.0f, 0.4f, 0.3f);
        }
        else
        {
            GUI.color = new Color(1.0f, 0.8f, 0.3f);
        }
        bLockFrame = GUILayout.Toggle(bLockFrame, "锁定帧");
        GUILayout.EndHorizontal();
        string[]  toolsStr = new string[]{"动作编辑","事件编辑", "碰撞盒编辑","触发点编辑","特效编辑","声音编辑"};
        mToolsBar = GUILayout.Toolbar(mToolsBar, toolsStr);
        GUI.color = ooc;
        Layout_DrawSeparator(Color.white);

        GUI_AniPos();

        Layout_DrawSeparator(Color.white);

        GUILayout.BeginHorizontal();

        if (mToolsBar == 0) //动作编辑模式
        {
            clipfuncpos = GUILayout.BeginScrollView(clipfuncpos, GUILayout.Width(300));
            {
                if (!bFrameRecord)
                {
                    GUI_ClipFunc();
                }
                else
                {
                    GUI_FrameFunc();
                }
            }
            GUILayout.EndScrollView();
            Layout_DrawSeparatorV(Color.white);


            cachefuncpos = GUILayout.BeginScrollView(cachefuncpos, GUILayout.Width(300));
            {
                GUI_FrameCache();
            }
            GUILayout.EndScrollView();

            Layout_DrawSeparatorV(Color.white);
        }
        else if (mToolsBar == 1)  //事件编辑
        {
            
            
            
        }
        else if (mToolsBar == 2) //碰撞盒编辑模式
        {

            emptyfuncpos = GUILayout.BeginScrollView(emptyfuncpos, GUILayout.Width(350));
            {
                GUI_BoxCollider();
            }
            GUILayout.EndScrollView();
            Layout_DrawSeparatorV(Color.white);


            boxcahcefuncpos = GUILayout.BeginScrollView(boxcahcefuncpos, GUILayout.Width(300));
            {
                GUI_BoxCache();
            }
            GUILayout.EndScrollView();
            Layout_DrawSeparatorV(Color.white);
        }
        else if (mToolsBar == 3)  //触发点编辑模式
        {
            dotpos = GUILayout.BeginScrollView(dotpos, GUILayout.Width(300));
            {
                GUI_Dot(); 
            }
            GUILayout.EndScrollView();
            Layout_DrawSeparatorV(Color.white);
           
        }
        else if(mToolsBar == 4)  //特效编辑模式
        {
            effectpos = GUILayout.BeginScrollView(effectpos, GUILayout.Width(500));
            {
                GUI_Effect();
            }
            GUILayout.EndScrollView();
            Layout_DrawSeparatorV(Color.white);
        }
        else if(mToolsBar == 5) //音效编辑模式
        {
            audiopos = GUILayout.BeginScrollView(audiopos, GUILayout.Width(300));
            {
                GUI_Audio();
            }
            GUILayout.EndScrollView();
            Layout_DrawSeparatorV(Color.white);
        }

        anichildP = GUILayout.BeginScrollView(anichildP, GUILayout.Width(380));
        {
            GUI_AniChild();
        }
        GUILayout.EndScrollView();
        Layout_DrawSeparatorV(Color.white);


        GUILayout.EndHorizontal();
        Layout_DrawSeparator(Color.white);

        GUILayout.Label("www.fbact.com");

        //更新pose
        if (isPlaySubAni)
        {
            ShowAniChild(editor.aniInEdit, aniChild);
        }

       
    }

    #region 绘画辅助函数等
    FB.PosePlus.AniPlayer aniPlayer;
    public static void Show(FB.PosePlus.Dev_AniEditor editor)
    {
        // Get existing open window or if none, make a new one:
        //获取现有的打开窗口或如果没有，创建一个新的



        var window = EditorWindow.GetWindow<Window_AniEditor>(false, "AniExEditor");
        //window.title = "才";
        window.SelfInit(editor);
        

    }

    FB.PosePlus.Dev_AniEditor editor = null;
    void SelfInit(FB.PosePlus.Dev_AniEditor editor)
    {
        aniPlayer = editor.GetComponent<FB.PosePlus.AniPlayer>();
        this.editor = editor;
        curframe = 0;
        bLockFrame = false;
        editor.aniInEdit.frames[0].key = true;
        editor.aniInEdit.frames[editor.aniInEdit.aniFrameCount - 1].key = true;
        editor.aniInEdit.frames[0].box_key = true;
        editor.aniInEdit.frames[editor.aniInEdit.aniFrameCount-1].box_key = true;
        editor.aniInEdit.frames[0].dot_key = true; 
        editor.aniInEdit.frames[editor.aniInEdit.aniFrameCount - 1].dot_key = true; 

    }

    public static void Layout_DrawSeparator(Color color, float height = 4f)
    {

        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, Screen.width, height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(height);
    }
    public static void Layout_DrawSeparatorV(Color color, float width = 4f)
    {

        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, width, rect.height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(width);
    }
    Vector2 anipos = Vector2.zero;
    int curframe = 0;

    void SetFrame(int f, bool force = false)
    {
        if (bLockFrame) return;

        if (curframe != f || force)
        {
            aniPlayer.SetPose(editor.aniInEdit, f, true);
            curframe = f;
        }
    }
    #endregion

    #region 动作编辑模块
    private void GUI_AniPos()
    {
        GUILayout.Label("Animation pos:(" + curframe + "/" + editor.aniInEdit.aniFrameCount + ")");

        int nf = (int)GUILayout.HorizontalScrollbar(curframe, 1, 0, editor.aniInEdit.aniFrameCount);
        anipos = EditorGUILayout.BeginScrollView(anipos, true, false, GUILayout.Height(230));
        GUILayout.BeginHorizontal();
        for (int i = 0; i < editor.aniInEdit.aniFrameCount; i++)
        {
            var obc = GUI.backgroundColor;
            if (curframe == i)
            {
                GUI.backgroundColor = Color.green;
            }
            GUILayout.BeginVertical(GUILayout.Width(1));

            var oc = GUI.color;
            string txt = "F";
            if (editor.aniInEdit.frames[i].key)
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
            if (editor.aniInEdit.frames[i].key)
            {
                GUILayout.Space(123);
            }
            GUI.color = oc;
            if (editor.aniInEdit.frames[i].key == false)
            {//调整曲线
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                float lerp = GUILayout.VerticalScrollbar(editor.aniInEdit.frames[i].lerp, 0.01f, 1.0f, 0, GUILayout.Height(120));
                if (lerp != editor.aniInEdit.frames[i].lerp)
                {
                    editor.aniInEdit.frames[i].lerp = lerp;
                    editor.aniInEdit.CalcLerpFrameOne(i);
                    if (i == curframe)
                    {
                        SetFrame(curframe, true);
                        EditorUtility.SetDirty(editor.aniInEdit);
                    }
                }
                GUILayout.EndHorizontal();
            }
            //box帧按钮
            string b_txt = "F";
            if (editor.aniInEdit.frames[i].box_key)
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
            if (editor.aniInEdit.frames[i].dot_key)
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


    uint newanilen = 1;

    void GUI_ClipFunc()
    {
        GUILayout.Label("ClipFunc");

        bool bclip = GUILayout.Toggle(editor.aniInEdit.loop, "是否循环动画");
        if (bclip != editor.aniInEdit.loop)
        {
            editor.aniInEdit.loop = bclip;
            EditorUtility.SetDirty(editor.aniInEdit);
        }
        if (GUILayout.Button("切换当前关键帧/非关键帧"))
        {
            if (curframe == 0 || curframe == editor.aniInEdit.aniFrameCount - 1)
            {
                EditorUtility.DisplayDialog("warning", "第一帧和最后一帧必须是关键帧", "ok");
            }
            else
            {
                editor.aniInEdit.frames[curframe].key = !editor.aniInEdit.frames[curframe].key;
                EditorUtility.SetDirty(editor.aniInEdit);
            }
        }

        GUILayout.Space(12);
        GUILayout.BeginHorizontal();
        GUILayout.Label("插入帧", GUILayout.Width(70));
        var str = GUILayout.TextField(newanilen.ToString(), GUILayout.Width(40));
        uint.TryParse(str, out newanilen);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("插入帧,当前帧之前"))
        {
            //if (editor.aniInEdit.frames.Count == 0)
            //{
            //    var fout = f.Clone() as FB.PosePlus.Frame;
            //    fout.key = true;
            //    editor.aniInEdit.frames.Insert(curframe + 1, fout);
            //}
            var f = editor.aniInEdit.frames[curframe];

            for (int i = 0; i < newanilen; i++)
            {
                var fout = f.Clone() as FB.PosePlus.Frame;
                fout.key = false;
                editor.aniInEdit.frames.Insert(curframe, fout);
            }

            for (int i = 0; i < editor.aniInEdit.aniFrameCount; i++)
            {
                editor.aniInEdit.frames[i].fid = i;
                if (i == 0 || i == editor.aniInEdit.aniFrameCount - 1)
                {
                    editor.aniInEdit.frames[i].key = true;
                    editor.aniInEdit.frames[i].box_key = true;
                    editor.aniInEdit.frames[i].dot_key = true;
                }
            }
            EditorUtility.SetDirty(editor.aniInEdit);
        }
        if (GUILayout.Button("插入帧,当前帧之后"))
        {
            var f = editor.aniInEdit.frames[curframe];

            for (int i = 0; i < newanilen; i++)
            {
                var fout = f.Clone() as FB.PosePlus.Frame;
                fout.key = false;
                editor.aniInEdit.frames.Insert(curframe + 1, fout);
            }
            //首位帧为关键帧
            for (int i = 0; i < editor.aniInEdit.aniFrameCount; i++)
            {
                editor.aniInEdit.frames[i].fid = i;
                if (i == 0 || i == editor.aniInEdit.aniFrameCount - 1)
                {
                    editor.aniInEdit.frames[i].key = true;
                    editor.aniInEdit.frames[i].dot_key = true;
                    editor.aniInEdit.frames[i].box_key = true;
                }
            }
            EditorUtility.SetDirty(editor.aniInEdit);
        }
        GUILayout.Space(12);
        if (GUILayout.Button("删除当前帧"))
        {
            if (editor.aniInEdit.frames.Count > 1)
            {
                editor.aniInEdit.frames.RemoveAt(curframe);
                for (int i = 0; i < editor.aniInEdit.aniFrameCount; i++)
                {
                    editor.aniInEdit.frames[i].fid = i;
                    if (i == 0 || i == editor.aniInEdit.aniFrameCount - 1)
                    {
                        editor.aniInEdit.frames[i].key = true;
                        editor.aniInEdit.frames[i].box_key = true;
                        editor.aniInEdit.frames[i].dot_key = true;
                    }
                }
                if (curframe < 0) curframe = 0;
                if (curframe >= editor.aniInEdit.aniFrameCount)
                    curframe = editor.aniInEdit.aniFrameCount - 1;
                EditorUtility.SetDirty(editor.aniInEdit);
            }
        }
        if (GUILayout.Button("删除当前开始5帧"))
        {
            for (int c = 0; c < 5; c++)
            {
                if (editor.aniInEdit.frames.Count > 1 && curframe < editor.aniInEdit.aniFrameCount)
                {
                    editor.aniInEdit.frames.RemoveAt(curframe);
                }
            }
            for (int i = 0; i < editor.aniInEdit.aniFrameCount; i++)
            {
                editor.aniInEdit.frames[i].fid = i;
                if (i == 0 || i == editor.aniInEdit.aniFrameCount - 1)
                {
                    editor.aniInEdit.frames[i].key = true;
                }
            }
            if (curframe < 0) curframe = 0;
            if (curframe >= editor.aniInEdit.aniFrameCount)
                curframe = editor.aniInEdit.aniFrameCount - 1;
            EditorUtility.SetDirty(editor.aniInEdit);
        }
        GUILayout.Space(12);


        if (editor.aniInEdit.frames.Count >0&&editor.aniInEdit.frames[curframe].key)
        {

            var oc = GUI.color;
            GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
            if (GUILayout.Button("记录到当前帧"))
            {
                FB.PosePlus.AniClip ani = editor.aniInEdit;
                List<Transform> trans = new List<Transform>();
                foreach (var b in ani.boneinfo)
                {
                    trans.Add(editor.transform.Find(b));
                }
                var frame = new FB.PosePlus.Frame(null, curframe, trans);
                editor.aniInEdit.frames[curframe].bonesinfo = new List<PoseBoneMatrix>(frame.bonesinfo);
                editor.aniInEdit.frames[curframe].key = true;

                //前后自动插值
                if (curframe != 0)
                {
                    editor.aniInEdit.ResetLerpFrameSegment((curframe - 1));
                    EditorUtility.SetDirty(editor.aniInEdit);
                }
                if (curframe != editor.aniInEdit.frames.Count - 1)
                {
                    editor.aniInEdit.ResetLerpFrameSegment((curframe + 1));
                    EditorUtility.SetDirty(editor.aniInEdit);
                }
                EditorUtility.SetDirty(editor.aniInEdit);
            }
            GUI.color = oc;
        }
        else
        {
            if (GUILayout.Button("重置非关键帧（线性）"))
            {
                editor.aniInEdit.ResetLerpFrameSegment(curframe);
                EditorUtility.SetDirty(editor.aniInEdit);
            }
        }
    }
    bool bLockFrame = false;
    bool bFrameRecord = false;

    DateTime lastRec = DateTime.Now;
    void GUI_FrameFunc()
    {
        GUILayout.Label("记录模式");
        GUILayout.Label("非关键帧不自动记录修改");
        GUILayout.Label("关键帧自动记录角色动作的状态");
        GUILayout.Label("每秒钟记录一次");
        this.Repaint();

        if ((DateTime.Now - lastRec).TotalSeconds > 1.0f)
        {
            lastRec = DateTime.Now;
            //非关键帧不自动记录修改
            if (editor.aniInEdit.frames[curframe].key == false) return;

            Debug.LogWarning("rec frame." + lastRec.ToLongTimeString());

            //record ani
            FB.PosePlus.AniClip ani = editor.aniInEdit;
            List<Transform> trans = new List<Transform>();
            foreach (var b in ani.boneinfo)
            {
                trans.Add(editor.transform.Find(b));
            }
            var frame = new FB.PosePlus.Frame(null, curframe, trans);
            frame.key = true;
            frame.box_key = true;
            editor.aniInEdit.frames[curframe] = frame;

            //记录盒子
            SaveEditData();

        }
    }

    void GUI_FrameCache()
    {
        GUILayout.Label("FrameCache");
        if (GUILayout.Button("记录当前角色的动作"))
        {
            FB.PosePlus.AniClip ani = ScriptableObject.CreateInstance<FB.PosePlus.AniClip>();
            ani.boneinfo = new List<string>(editor.aniInEdit.boneinfo);

            List<Transform> trans = new List<Transform>();
            foreach (var b in ani.boneinfo)
            {
                trans.Add(editor.transform.Find(b));
            }
            ani.frames = new List<FB.PosePlus.Frame>();
            ani.frames.Add(new FB.PosePlus.Frame(null, 0, trans));
            FB.PosePlus.Dev_AniEditor.poolitem pi = new FB.PosePlus.Dev_AniEditor.poolitem();
            pi.cliponeframe = ani;
            pi.name = "noname";
            pi.time = DateTime.Now.ToBinary();
            editor.PosePool.Add(pi);
            EditorUtility.SetDirty(editor);
        }
        foreach (var a in editor.PosePool)
        {
            GUILayout.BeginHorizontal();
            DateTime t = DateTime.FromBinary(a.time);
            GUILayout.Label(t.ToShortDateString() + " " + t.ToShortTimeString());
            a.name = GUILayout.TextField(a.name, GUILayout.Width(100));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("展示动作"))
            {
                editor.GetComponent<FB.PosePlus.AniPlayer>().SetPose(a.cliponeframe, 0, true);
            }
            if (GUILayout.Button("展示局部"))
            {
                editor.GetComponent<FB.PosePlus.AniPlayer>().SetPose(a.cliponeframe, 0, true, Selection.activeTransform);
            }
            if (GUILayout.Button("删除"))
            {
                editor.PosePool.Remove(a);

                EditorUtility.SetDirty(editor);
                break;
            }
            GUILayout.EndHorizontal();

        }
    }

    #endregion

    #region 事件编辑器

    

    #endregion
    #region 碰撞盒模块

    List<BoxPool> boxCaCheList = new List<BoxPool>();
    void GUI_BoxCache()
    {
        GUILayout.Label("Box");
        if (GUILayout.Button("记录当前碰撞盒信息"))
        {
            BoxPool pool = new BoxPool();
            pool.boxInfo = new List<AniBoxCollider>(editor.aniInEdit.frames[curframe].boxesinfo);
            boxCaCheList.Add(pool);
        }
        for (int i = 0; i != boxCaCheList.Count;i++ )
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(boxCaCheList[i].time.ToString());
            boxCaCheList[i].name = GUILayout.TextField(boxCaCheList[i].name, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("粘贴到当前帧"))
            {
                editor.aniInEdit.frames[curframe].box_key = true;
                editor.aniInEdit.frames[curframe].boxesinfo = new List<AniBoxCollider>(boxCaCheList[i].boxInfo);
                SetFrame(curframe,true);
            }

            if (GUILayout.Button("删除"))
            {
                boxCaCheList.Remove(boxCaCheList[i]);
                break;
            }
            GUILayout.EndHorizontal();

        }
    }

    Vector2 Vec = Vector2.zero;
    //Dictionary<int, bool> mButtonData = new Dictionary<int, bool>();//单选框的数据
  
    void GUI_BoxCollider()
    {
        List<string> mLayerList = new List<string>();
        GUILayout.Label("CreateBox");
        GUILayout.BeginVertical();
/*        GUILayout.Space(5);*/
      /*  int boolCount = 0;*/
        for (int i = 8; i != 32; i++)
        {
            string str = LayerMask.LayerToName(i);
            if (str.Length > 4)
            if (str != null && str != ""&&str.Substring(0, 4) == "box_")
            {
                mLayerList.Add(str);
                //if ((boolCount+1) % 3 == 1) GUILayout.BeginHorizontal();//
                //{
                //    bool b = false;
                //    if (!mButtonData.ContainsKey(boolCount)) mButtonData.Add(boolCount, b);

                //    mButtonData[boolCount] = GUILayout.Toggle(mButtonData[boolCount], str, GUILayout.Width(120));
                //    if (mButtonData[boolCount])
                //    {
                //        for (int j = 0; j != mButtonData.Count; j++)
                //        {
                //            if (j == boolCount) continue;
                //            mButtonData[j] = false;
                //        }

                //    }
                //    if ((boolCount + 1) % 3 == 0) GUILayout.EndHorizontal();
                //    boolCount++;
                //}


            }
        }
        //if (boolCount % 3 != 0) GUILayout.EndHorizontal();
/*        GUILayout.Space(5);*/
        AniBoxCollider _boxColoder = new AniBoxCollider();
        int _count = 0;
        GUILayout.BeginHorizontal();
        foreach (var s in mLayerList)
        {
/*            GUILayout.Space(80);*/
            if (GUILayout.Button(s, GUILayout.Width(150)))
            {
                //bool isSelect = false;
                //foreach (var k in mButtonData.Keys)
                //{
                //    if (mButtonData[k])
                //    {
                //        isSelect = true;
                //        break;
                //    }
                //}

//                 if (!isSelect)
//                 {
//                     if (EditorUtility.DisplayDialog("Warning", "请选择Layer", "OK"))
//                     {
//                         return;
//                     };
/*                }*/
                if (!aniPlayer.IsShowBoxLine)
                {
                    if (EditorUtility.DisplayDialog("Warning", "当前模式不显示boxLine，是否显示", "Show", "Exit"))
                    {
                        aniPlayer.IsShowBoxLine = true;
                    };
                    return;
                }
                if (editor.aniInEdit.frames[curframe].box_key)
                {

                    //foreach (int key in mButtonData.Keys)
                    //{
                    //    if (mButtonData[key])
                    //    {
                    _boxColoder.mBoxType = s; /*mLayerList[key];*/
                            //index
                    var _list = editor.aniInEdit.frames[curframe].boxesinfo.FindAll(a => a.mBoxType == s);
                    _boxColoder.mIndex = _list.Count;
// 
//                         }
//                     }


                    editor.aniInEdit.frames[curframe].boxesinfo.Insert(0, _boxColoder);
                    SaveEditData();
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Warning", "必须是关键帧才能操作，是否将其设成关键帧！", "Place", "Exit"))
                    {
                        editor.aniInEdit.frames[curframe].box_key = true;
                    };
                }

            }
            if (_count % 2 == 1)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }

            _count++;

        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();

        var oc = GUI.color;

        if (editor.aniInEdit.frames[curframe].box_key)
        {
            GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
            if (GUILayout.Button("取消关键帧", GUILayout.Width(130)))
            {

                if (curframe == 0 || curframe == editor.aniInEdit.aniFrameCount - 1)
                {
                    EditorUtility.DisplayDialog("warning", "第一帧和最后一帧必须是关键帧", "ok");
                }
                else
                {
                    editor.aniInEdit.frames[curframe].box_key = !editor.aniInEdit.frames[curframe].box_key;
                    editor.aniInEdit.frames[curframe].boxesinfo = new List<AniBoxCollider>();
                    EditorUtility.SetDirty(editor.aniInEdit);
                }
            }
            GUI.color = oc;
        }
        else
        {
            if (GUILayout.Button("设置关键帧", GUILayout.Width(130)))
            {

                if (curframe == 0 || curframe == editor.aniInEdit.aniFrameCount - 1)
                {
                    EditorUtility.DisplayDialog("warning", "第一帧和最后一帧必须是关键帧", "ok");
                }
                else
                {
                    editor.aniInEdit.frames[curframe].box_key = !editor.aniInEdit.frames[curframe].box_key;
                    EditorUtility.SetDirty(editor.aniInEdit);
                }
            }
        }


        GUILayout.Space(40);
        if (editor.aniInEdit.frames[curframe].box_key)
        {
            if (GUILayout.Button("记录到当前帧", GUILayout.Width(130)))
            {
                SaveEditData();
                if ( curframe != 0)
                {
                    editor.aniInEdit.ResetDotLerpFrameSegment((curframe - 1));
                    EditorUtility.SetDirty(editor.aniInEdit);
                }
                if(curframe != editor.aniInEdit.frames.Count - 1)
                {
                    //SetFrame((curframe - 1));
                    editor.aniInEdit.ResetDotLerpFrameSegment((curframe + 2));
                    EditorUtility.SetDirty(editor.aniInEdit);
                    //SetFrame((curframe + 1));
                }
            }
        }
        else
        {
            GUI.color = new Color(1f, 0.4f, 0.6f, 1.0f);
            if (GUILayout.Button("生成非关键帧Box", GUILayout.Width(130)))

            {
                editor.aniInEdit.ResetBoxLerpFrameSegment(curframe);
                EditorUtility.SetDirty(editor.aniInEdit);
                SetFrame(curframe,true);
            }
            GUI.color = oc;
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("所有Box适应Z轴"))
        {
            AdaptX();
        }

        GUILayout.Label("碰撞盒数据:", GUILayout.Width(100));
        GUILayout.Space(1);
        GUILayout.BeginHorizontal();
        Vec = EditorGUILayout.BeginScrollView(Vec, true, true, GUILayout.Width(350),GUILayout.Height(350));
        List<string> layerList = new List<string>();
        foreach (var o in editor.aniInEdit.frames[curframe].boxesinfo)
         {
             if (o == null)  continue;
             if (o.mBoxType == "" || o.mBoxType ==null) continue;
             string ishave = null;
             ishave = layerList.Find(i => i == o.mBoxType);
             if (ishave == null)
             {
                 layerList.Add(o.mBoxType);
             }
         }
         foreach (var str in layerList)
         {
             GUILayout.Label(str+ ":", GUILayout.Width(100));
             int count = 0;
             for (int i  = editor.aniInEdit.frames[curframe].boxesinfo.Count -1;i>=0;i--)
             {
                 var o= editor.aniInEdit.frames[curframe].boxesinfo[i];
                 if (o != null  && o.mBoxType == str)
                 {
                     GUILayout.BeginHorizontal();
                     GUILayout.Label(count.ToString(),GUILayout.Width(18));
                     GUILayout.Space(15);
                     //show data
                     string name = o.mName;
                     name = GUILayout.TextField(name, GUILayout.Width(50));
                     o.mName = name;
                     oc = GUI.color;
                     GUILayout.Space(5);
                     GUI.color = new Color(1.0f, 0.4f, 0.6f, 1.0f);
                     if (GUILayout.Button("Del", GUILayout.Width(40), GUILayout.Height(18)))
                     {
                         editor.aniInEdit.frames[curframe].boxesinfo.Remove(o);
                         SaveEditData();
                     }
                     GUI.color = oc;

                     if (GUILayout.Button("调整", GUILayout.Width(40), GUILayout.Height(18)))
                     {
                         CrateSelectObjectBox(editor.aniInEdit.frames[curframe].boxesinfo[i]);
                     }
                     if (GUILayout.Button("调整(Child)", GUILayout.Width(80), GUILayout.Height(18)))
                     {
                         CrateSelectObjectBox(editor.aniInEdit.frames[curframe].boxesinfo[i],true);
                     }
                     if (GUILayout.Button("同步当前box", GUILayout.Width(80), GUILayout.Height(18)))
                     {
                         Transform trans = Selection.activeTransform;
                         if (trans.parent!= null&&trans.parent.name != "_boxes")
                         {
                             EditorUtility.DisplayDialog("错误", "请选择正确的box", "OK");
                         }
                         else if (Selection.activeTransform.name != editor.aniInEdit.frames[curframe].boxesinfo[i].mName)
                         {
                             if( EditorUtility.DisplayDialog("错误", "将要同步的box不是同一个,确定要同步？", "别BB,劳资就要!","rong老夫三思"))
                             {
                                 editor.aniInEdit.frames[curframe].boxesinfo[i].mPosition = trans.localPosition;
                                 editor.aniInEdit.frames[curframe].boxesinfo[i].mSize = trans.localScale;
                                 SaveEditData();
                             }
                         }
                         else
                         {
                             editor.aniInEdit.frames[curframe].boxesinfo[i].mPosition = trans.localPosition;
                             editor.aniInEdit.frames[curframe].boxesinfo[i].mSize = trans.localScale;
                             SaveEditData();
                         }
                     }
                     GUILayout.EndHorizontal();
                     count++;
                 }
              }
            
         }
         var x = editor.aniInEdit.frames[curframe].boxesinfo;
         EditorGUILayout.EndScrollView();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

    }


    //}
    Vector3[]  FindMesh(Transform t)
    {
        Mesh mesh = new Mesh();
        t.gameObject.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);

        return mesh.vertices;

    }
    private List<Transform> mTransformList = new List<Transform>();
    void TraversChild(Transform transform)
    {
        if (transform.childCount != 0)
        {
            foreach (Transform t in transform)
            {
                mTransformList.Add(transform);
                TraversChild(t); //递归遍历完子节点
            }
        }
        else
        {
            mTransformList.Add(transform);
        }
    }


    void CrateSelectObjectBox(AniBoxCollider anicollider,bool isIncludeChild = false)
    {
        bool bPick = false;
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Matrix4x4 matp = Matrix4x4.TRS(aniPlayer.transform.position, aniPlayer.transform.rotation, Vector3.one).inverse;

        //GameObject debug = new GameObject();
        List<Transform> transList = new List<Transform>();
        //是否包含子节点
        if (isIncludeChild)
        {
            foreach (var t in Selection.transforms)
            {
                TraversChild(t);//遍历所有选中的骨骼
            }
            transList = new List<Transform>(mTransformList);
        }
        else
        {
            transList = new List<Transform>(Selection.transforms);
        }
        //遍历计算box
        foreach (var t in transList)
        {

            SkinnedMeshRenderer sm = t.GetComponent<SkinnedMeshRenderer>();
            if (sm != null)
            {
                //Debug.LogWarning("选中了一个对象：" + sm.gameObject.name);
                Mesh cachem = new Mesh();
                sm.BakeMesh(cachem);
                Debug.Log(sm.sharedMesh.triangles.Length + "," + cachem.triangles.Length);
                bPick = true;
                Matrix4x4 mat = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
                for (int i = 0; i < cachem.triangles.Length / 3; i++)
                {
                    for (int si = 0; si < 3; si++)
                    {
                        int index = cachem.triangles[i * 3 + si];
                        var vtoworld = mat.MultiplyPoint
                            (cachem.vertices[index]);
                        Vector3? v = matp.MultiplyPoint(vtoworld);
                        //Vector3? v = aniPlayer.transform.InverseTransformPoint(vtoworld);
                        //Vector3? v = vtoworld;// cachem.vertices[index];

                        min.x = Mathf.Min(min.x, v.Value.x);
                        min.y = Mathf.Min(min.y, v.Value.y);
                        min.z = Mathf.Min(min.z, v.Value.z);
                        max.x = Mathf.Max(max.x, v.Value.x);
                        max.y = Mathf.Max(max.y, v.Value.y);
                        max.z = Mathf.Max(max.z, v.Value.z);

                        //测试顶点位置
                        //var ovv = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //ovv.transform.parent = debug.transform;
                        //ovv.transform.position = vtoworld;
                        //ovv.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    }
                }

                ////测试模型取对了嘛
                //GameObject obj = new GameObject();
                //obj.AddComponent<MeshFilter>().mesh = cachem;
                //obj.AddComponent<MeshRenderer>();
                //obj.transform.position = t.position;
                //obj.transform.rotation = t.rotation;

                //UnityEngine.Object.DestroyImmediate(cachem);
            }
        }
      
        if (!bPick)
        {
            if (!isIncludeChild)
            {
              EditorUtility.DisplayDialog("错误", "改模型没有找到mesh信息，请重新生成！", "OK");
            }
            return;
        }

        //var objbox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //objbox.transform.position = min;
        //objbox.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //var objbox2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //objbox2.transform.position = max;
        //objbox2.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);


        anicollider.mPosition = min * 0.5f + max * 0.5f;
        anicollider.mSize = (max - min);
        mTransformList.Clear();
        SaveEditData();
       
    }

    void AdaptX()
    {
       

        foreach (var f in editor.aniInEdit.frames)
        {
            foreach (var box in f.boxesinfo)
            {
                if (box.mSize.x == 0) continue;
                box.mSize.x = (Math.Abs(box.mPosition.x) + Math.Abs(box.mSize.x / 2) ) * 2;
                box.mPosition.x = 0;
            }
        }
        SaveEditData();
    }
#endregion

    #region 子动画预览模块

    uint startframe = 0;
    uint endftame = 0;
    Vector2 vecScroll = Vector2.zero;
    bool isLoop = false;
    void GUI_AniChild()
    {

        GUILayout.Label("子动画预览：");
        GUILayout.BeginHorizontal();
        GUILayout.Label("开始帧：", GUILayout.Width(40));
        var str = GUILayout.TextField(startframe.ToString(), GUILayout.Width(40));
        uint.TryParse(str, out startframe);
        GUILayout.Space(20);
        GUILayout.Label("结束帧：", GUILayout.Width(40));
        var endstr = GUILayout.TextField(endftame.ToString(), GUILayout.Width(40));
        uint.TryParse(endstr, out endftame);
        GUILayout.Space(20);
        isLoop = GUILayout.Toggle(isLoop,"是/否循环");
        GUILayout.EndHorizontal();


        GUILayout.Space(5);

        vecScroll = GUILayout.BeginScrollView(vecScroll, GUILayout.Width(380));
        GUILayout.BeginHorizontal();
        GUILayout.Space(50);
        if (GUILayout.Button("添加子节点",GUILayout.Width(250)))
        {
            if (startframe == endftame || startframe > endftame)
            {
                EditorUtility.DisplayDialog("warning", "起始帧必须大于结束帧！", "ok");
            }
            else if(endftame > editor.aniInEdit.aniFrameCount)
            {
                EditorUtility.DisplayDialog("warning", "该帧超出范围，请重新设置！", "ok");
            }
            else
            {
                SubClip a = new SubClip();
                a.startframe = startframe;
                a.endframe = endftame;
                a.loop = isLoop; 
                editor.aniInEdit.subclips.Add(a);
                EditorUtility.SetDirty(editor.aniInEdit);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        for (int i = 0; i < editor.aniInEdit.subclips.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(i.ToString() + ":   ");
            string start =  GUILayout.TextField(editor.aniInEdit.subclips[i].startframe.ToString(), GUILayout.Width(20));
            uint.TryParse(start, out editor.aniInEdit.subclips[i].startframe);
            GUILayout.Label("帧  - ");
            string end = GUILayout.TextField(editor.aniInEdit.subclips[i].endframe.ToString(), GUILayout.Width(20));
            uint.TryParse(end, out editor.aniInEdit.subclips[i].endframe);
            GUILayout.Label("帧");
            GUILayout.Space(10);
            editor.aniInEdit.subclips[i].name = GUILayout.TextField(editor.aniInEdit.subclips[i].name, GUILayout.Width(70));
      
            bool bloop = GUILayout.Toggle(editor.aniInEdit.subclips[i].loop, "loop", GUILayout.Width(45));
            if (bloop != editor.aniInEdit.subclips[i].loop)
            {
                editor.aniInEdit.subclips[i].loop = bloop;
                EditorUtility.SetDirty(editor.aniInEdit);
            }

            if (GUILayout.Button("Play",GUILayout.Width(40)))
            {
                if (editor.aniInEdit.subclips[i].endframe > editor.aniInEdit.aniFrameCount)
                {
                    EditorUtility.DisplayDialog("Warning", "该子动画不在帧范围内，已经失效！", "Exit");

                }
                else 
                {
                    isPlaySubAni = true;
                    last = DateTime.Now;
                    delta = 0;
                    aniChild = editor.aniInEdit.subclips[i];
                }         
            }
            if (GUILayout.Button("Stop", GUILayout.Width(40)))
            {
                isPlaySubAni = false;
            }
            if (GUILayout.Button("DEL", GUILayout.Width(35)))
            {
                editor.aniInEdit.subclips.Remove(editor.aniInEdit.subclips[i]);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    public bool isPlaySubAni = false;
    SubClip aniChild = new SubClip();
    DateTime last = DateTime.Now;
    float delta = 0;
    public void ShowAniChild(FB.PosePlus.AniClip clip, SubClip a)
    {
       
        DateTime _now = DateTime.Now;
        delta += (float)(_now - last).TotalSeconds;
        last = _now;

        int frame = (int)(delta * clip.fps) + (int)a.startframe;
        if (frame > a.endframe)
        {
            if (a.loop)
            {
                frame = (int)a.startframe + (frame - (int)a.endframe) % ((int)a.endframe - (int)a.startframe + 1);
            }
            else
            { 
                isPlaySubAni = false;
                return;
            }
        }
        aniPlayer.SetPose(clip, frame, true);
        //Debug.LogError(delta +":"+frame);
        Repaint();
    }


  #endregion

    #region 触发点模块
    Vector2 dotScrollVec;
    void  GUI_Dot()
    {
        GUILayout.Label("触发点模块：");
        if (editor.aniInEdit.frames[curframe].dot_key)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("hold"))
            {
                foreach (var _d in editor.aniInEdit.frames[curframe].dotesinfo)
                {
                    if (_d.name.Equals("hold"))
                    {
                        if (EditorUtility.DisplayDialog("错误", "已存在该点", "OK"))
                        {
                            return;
                        }
                    }
                }
                Dot d = new Dot();
                d.name = "hold";
                editor.aniInEdit.frames[curframe].dotesinfo.Add(d);
                aniPlayer.SetDotAttribute(d);
            }
            if (GUILayout.Button("behold"))
            {
                foreach (var _d in editor.aniInEdit.frames[curframe].dotesinfo)
                {
                    if (_d.name.Equals("behold"))
                    {
                        if (EditorUtility.DisplayDialog("错误", "已存在该点", "OK"))
                        {
                            return;
                        }
                    }
                }
                Dot d = new Dot();
                d.name = "behold";
                editor.aniInEdit.frames[curframe].dotesinfo.Add(d);
            }
            if (GUILayout.Button("create"))
            {
                foreach (var _d in editor.aniInEdit.frames[curframe].dotesinfo)
                {
                    if (_d.name.Equals("create"))
                    {
                        if (EditorUtility.DisplayDialog("错误", "已存在该点", "OK"))
                        {
                            return;
                        }
                    }
                }
                Dot d = new Dot();
                d.name = "create";
                editor.aniInEdit.frames[curframe].dotesinfo.Add(d);
            }
            GUILayout.EndHorizontal();
          
            if (GUILayout.Button(("取消关键帧")))
            {
                if (curframe == 0 || curframe == editor.aniInEdit.frames.Count - 1)
                {
                    EditorUtility.DisplayDialog("Waring", "第一帧和最后一帧 必须为关键帧", "OK");
                    return;
                }
                editor.aniInEdit.frames[curframe].dot_key = false;
                SaveEditData();
            }
        }
        else
        {
            if (GUILayout.Button("自动生成非关键帧Dot"))
            {
                editor.aniInEdit.ResetDotLerpFrameSegment(curframe);
            }
            if (GUILayout.Button("设置关键帧"))
            {
                if (curframe == 0 || curframe == editor.aniInEdit.frames.Count - 1)
                {
                    EditorUtility.DisplayDialog("Waring", "第一帧和最后一帧 必须为关键帧", "OK");
                    return;
                }
                editor.aniInEdit.frames[curframe].dot_key = true;
                editor.aniInEdit.ResetDotLerpFrameSegment(curframe - 1);
                editor.aniInEdit.ResetDotLerpFrameSegment(curframe + 1);
                SaveEditData();
            }
        }
        if (GUILayout.Button("保存修改"))
        {
            SaveEditData();
        }
        GUILayout.Label("触发点信息：");
        dotScrollVec= GUILayout.BeginScrollView(dotScrollVec,GUILayout.Width(300),GUILayout.Height(500));
        int count = 0;
        for ( int i  = editor.aniInEdit.frames[curframe].dotesinfo.Count -1;i>=0 ;i--)
        {
            var o = editor.aniInEdit.frames[curframe].dotesinfo[i];
            GUILayout.BeginHorizontal();
            GUILayout.Label(count.ToString() + ":", GUILayout.Width(20));

            o.name = GUILayout.TextField(o.name, GUILayout.Width(50));
            GUILayout.Label("[x:" + o.position.x + ",y:" + o.position.y + ",z:" + o.position.z+"]",GUILayout.Width(80));
            GUILayout.Space(10);
            if (GUILayout.Button("同步点"))
            {
                if (!Selection.activeTransform.parent.gameObject.name.Equals("_dotes"))
                {

                  if(  EditorUtility.DisplayDialog("错误", "选择的不是一个Point,请重选", "我就要继续","不了"))
                    {
                        o.position = Selection.activeTransform.localPosition;
                        SaveEditData();
                    }
                  return;
                }
                o.position = Selection.activeTransform.localPosition;
                SaveEditData();
            }
            if (GUILayout.Button("DEL"))
            {
                editor.aniInEdit.frames[curframe].dotesinfo.Remove(o);
            }
            GUILayout.EndHorizontal();
            count++;

        }

        GUILayout.EndScrollView();


    }

    void SaveEditData()
    {
        EditorUtility.SetDirty(editor.aniInEdit);
        SetFrame(curframe, true);
        //if (!aniPlayer.transform.FindChild("_dotes"))
        //{
        //    GameObject dot = new GameObject("_dotes");
        //    dot.transform.SetParent(aniPlayer.transform);
        //}
        //List<Dot> list = new List<Dot>();
        //foreach (Graphic o in aniPlayer.transform.FindChild("_dotes"))
        //{
        //    if (o != null && o.gameObject.activeSelf)
        //    {
        //        Dot d = new Dot();
        //        d.name = o.name;
        //        d.position = o.transform.localPosition;
        //        list.Add(d);
        //    }
        //}
        //editor.aniInEdit.frames[curframe].dotesinfo = new List<Dot>(list);
    }

    #endregion

    #region 特效模块
    string effectAddr = "";
    string effectName = "null";
    private UnityEngine.Object obj;
    int effectlife = 0;
    GameObject effect = null;
    Vector2 effectScroll;
    bool isFollow = false; 
    void GUI_Effect()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("跟随节点：");
        GUILayout.Space(5);
        effectAddr = GUILayout.TextField(effectAddr, GUILayout.Width(120));
        GUILayout.Space(5);
        if (GUILayout.Button("获得当前路径"))
        {

        //var name = Selection.activeTransform.gameObject.name;
         Transform tempObj = Selection.activeTransform.transform;
          string name = Selection.activeTransform.transform.name;
            for (int i = 0; ; i++)
            {
                if (tempObj.name != aniPlayer.transform.name)
                {
                    if (tempObj.parent == null)
                    {
                        EditorUtility.DisplayDialog("Warning", "请勿选择当前动画以外的节点！", "OK");
                        break;
                    }
                   
                   
                    tempObj = tempObj.parent;
                    if (tempObj.name != aniPlayer.transform.name)
                      name = tempObj.name + "/" + name;
                }
                else
                {
                    break;
                }
            }

            effectAddr = name;
        }
        GUILayout.Space(5);
        GUILayout.EndHorizontal();
        
        //
        EditorGUILayout.HelpBox("拖拽特效文件,位于Resources目录下" , MessageType.Info);
        GUILayout.BeginHorizontal();
//        GUILayout.Label("特效：",GUILayout.Width(40));
//        GUILayout.Space(5);
    
        obj =  EditorGUILayout.ObjectField("特 效:",obj,typeof(Object),false);
        if (obj != null)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            effectName = path.Replace("Assets/Resource/Resources/", "").Replace(".prefab", "");
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("路 径:" +  effectName );
        //
        effectlife = EditorGUILayout.IntField("生命周期(帧)：", effectlife);

        if (GUILayout.Button("添加特效"))
        {
            effect = Resources.Load(effectName) as GameObject;
            if (effect == null)
            {
                EditorUtility.DisplayDialog("waring", "Resource里不含改特效文件，请检查！", "OK");
            }
            else
            {
               Effect e = new Effect();
               e.name = effectName;
               e.follow = effectAddr;
               e.lifeframe = (int)effectlife;
               e.isFollow = isFollow;
               editor.aniInEdit.frames[curframe].effectList.Add(e);
               EditorUtility.SetDirty(editor.aniInEdit);
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.Label("特效数据:");
      //  effectScroll = GUILayout.BeginScrollView(effectScroll, GUILayout.Width(500));
        GUILayout.BeginHorizontal();
        GUILayout.Label("文件名", GUILayout.Width(50));
        GUILayout.Space(2);
        GUILayout.Label("跟随对象", GUILayout.Width(50));
        GUILayout.Space(2);
        GUILayout.Label("生命周期", GUILayout.Width(50));

        GUILayout.EndHorizontal();
        for (int m = editor.aniInEdit.frames[curframe].effectList.Count-1; m >= 0;m-- )
        {
            var e = editor.aniInEdit.frames[curframe].effectList[m];
            GUILayout.BeginHorizontal();
            e.name = GUILayout.TextField(e.name, GUILayout.Width(50));
            GUILayout.Space(2);
            e.follow = GUILayout.TextField(e.follow, GUILayout.Width(50));
            GUILayout.Space(2);
            string _str = GUILayout.TextField(e.lifeframe.ToString(), GUILayout.Width(50));
            int.TryParse(_str, out e.lifeframe);
            GUILayout.Space(2);
            if (GUILayout.Button("跟随当前选择", GUILayout.Width(80)))
            {
                Transform tempObj = Selection.activeTransform.transform;
                string name = Selection.activeTransform.transform.name;
                for (int i = 0; ; i++)
                {
                    if (tempObj.name != aniPlayer.transform.name)
                    {
                        if (tempObj.parent == null)
                        {
                            EditorUtility.DisplayDialog("Warning", "请勿选择当前动画以外的节点！", "OK");
                            return;
                        }
                        tempObj = tempObj.parent;
                        if (tempObj.name != aniPlayer.transform.name)
                        name = tempObj.name + "/" + name;
                    }
                    else
                    {
                        break;
                    }
                }

                e.follow = name;
            }
            GUILayout.Space(2);
            if (GUILayout.Button("DEL", GUILayout.Width(50)))
            {
                editor.aniInEdit.frames[curframe].effectList.Remove(e);
            }

            GUILayout.EndHorizontal();
            EditorUtility.SetDirty(editor.aniInEdit);
        }
        //GUILayout.EndScrollView();
    }
    #endregion

    #region 音效模块
    Vector2 audioposes;
    string audioaddr ="";
    void GUI_Audio()
    {
        GUILayout.Label("音乐系统：");
        GUILayout.BeginHorizontal();
        GUILayout.Label("路径：",GUILayout.Width(40));
        GUILayout.Space(2);
        audioaddr = GUILayout.TextField(audioaddr, GUILayout.Width(180));
        GUILayout.Space(2);
       
        if (GUILayout.Button("载入音乐"))
        {
          AudioClip  music = Resources.Load(audioaddr) as AudioClip;
          if (music == null)
            {
                EditorUtility.DisplayDialog("waring", "Resource里不含该音乐文件,或该文件不是音效文件,请检查！", "OK");
            }
           else
           {
                editor.aniInEdit.frames[curframe].aduioList.Add(audioaddr);
                EditorUtility.SetDirty(editor.aniInEdit);
            }
        }
        GUILayout.EndHorizontal();
        //显示所有音乐信息
        audioposes = GUILayout.BeginScrollView(audioposes, GUILayout.Width(300));
        
        for (int i = editor.aniInEdit.frames[curframe].aduioList.Count-1; i >= 0; i--)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label((editor.aniInEdit.frames[curframe].aduioList.Count - i).ToString() + ":",GUILayout.Width(40));
            GUILayout.Space(2);
            GUILayout.Label(editor.aniInEdit.frames[curframe].aduioList[i],GUILayout.Width(150));
            GUILayout.Space(2);
            if(GUILayout.Button("Play",GUILayout.Width(45)))
            {
                //AudioPlayer.Instance().PlaySoundOnce(editor.aniInEdit.frames[curframe].aduioList[i]);
                //aniPlayer.resourceMgr.PlaySound();
            }
             if(GUILayout.Button("DEL",GUILayout.Width(45)))
            {
                editor.aniInEdit.frames[curframe].aduioList.RemoveAt(i);
            }
             GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }
    #endregion
}
