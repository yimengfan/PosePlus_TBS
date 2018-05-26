using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Window_AniTools : EditorWindow
{

    public enum Operation
    {
        none =0,
        left_bone,
        left_box,
        left_dot,
        left_effct,
        left_adudio,
        right_bone ,
        right_box,
        right_dot,
        right_effct,
        right_audio,
    }
    FB.PosePlus.AniClip clip_a;
    FB.PosePlus.AniClip clip_b;
    FB.PosePlus.AniClip clip_a_cache;
    FB.PosePlus.AniClip clip_b_cache;

    Operation lastoperation = Operation.none;
    public static void Show(FB.PosePlus.AniClip clipa, FB.PosePlus.AniClip clipb)
    {
        var window = EditorWindow.GetWindow<Window_AniTools>(true, "Window_Tools", true);
        window.SelfInit(clipa, clipb);
    }

   void SelfInit(FB.PosePlus.AniClip clipa, FB.PosePlus.AniClip clipb)
    {
       clip_a_cache= clip_a = clipa;
       clip_b_cache=  clip_b = clipb;
    }

   public void OnGUI()
   {
       GUILayout.BeginVertical();
     /*  Window_AniEditor.Layout_DrawSeparator(Color.white);*/
       GUILayout.BeginHorizontal();
       OnGUI_ShowLeft();
     /*  Window_AniEditor.Layout_DrawSeparatorV(Color.white);*/
       OnGUI_ShowRight();
       GUILayout.EndHorizontal();
     /*  Window_AniEditor.Layout_DrawSeparator(Color.white);*/
       GUILayout.EndVertical();
      
   }

   Vector2 leftpos;
    void OnGUI_ShowLeft()
   {
       GUILayout.BeginVertical();
       {
           {
               GUILayout.BeginHorizontal();
               GUILayout.Space(150);
               GUILayout.Label(clip_a.name);
               GUILayout.EndHorizontal();
           }
         
           leftpos = GUILayout.BeginScrollView(leftpos, false, false, GUILayout.Width(300));
           
           GUILayout.BeginVertical();

           {
               #region 动画
               GUILayout.BeginHorizontal();
               GUILayout.Label("骨 骼");
               int i =0;
               int j =0;
               if(GUILayout.Button("右移>>",GUILayout.Width(50)))
               {
                   foreach(var bone in clip_a.boneinfo)
                   {
                       int temp = i;
                       foreach (var b in clip_b.boneinfo)
                       {
                           if(bone == b)
                           {
                               i++;
                               break;
                           }
                       }

                       if (temp == i)
                           j++;
                   }

                  if(EditorUtility.DisplayDialog("对比信息","相同骨骼："+i.ToString()+"\n不同骨骼："+j.ToString()+"\n 是否复制到右边？","Yes","No"))
                   {

                       lastoperation = Operation.left_bone;

                       clip_b.boneinfo = new List<string>(clip_a.boneinfo);
                   }
               }
               GUILayout.EndHorizontal();
               #endregion 
               GUILayout.Space(10);
               #region Box
               GUILayout.BeginHorizontal();
               GUILayout.Label("碰撞盒");

               if (GUILayout.Button("右移>>", GUILayout.Width(50)))
               {
                   MoveData(Operation.left_box);
               }
               GUILayout.EndHorizontal();
               #endregion 
               GUILayout.Space(10);
               #region 触发点
               GUILayout.BeginHorizontal();
               GUILayout.Label("触发点");
               if (GUILayout.Button("右移>>", GUILayout.Width(50)))
               {
                   MoveData(Operation.left_dot);
               }
               GUILayout.EndHorizontal();
               #endregion 
               GUILayout.Space(10);
               #region 特效
               GUILayout.BeginHorizontal();
               GUILayout.Label("特 效");
               if (GUILayout.Button("右移>>", GUILayout.Width(50)))
               {
                   MoveData(Operation.left_effct);
               }
               GUILayout.EndHorizontal();
               #endregion 
               GUILayout.Space(10);
               #region 音效
               GUILayout.BeginHorizontal();
               GUILayout.Label("音效");
               if (GUILayout.Button("右移>>", GUILayout.Width(50)))
               {
                   MoveData(Operation.left_adudio);
               }
               GUILayout.EndHorizontal();
               #endregion 
               GUILayout.Space(10);
               #region 撤销
               GUILayout.BeginHorizontal();
               GUILayout.Space(250);
               if (GUILayout.Button("还原", GUILayout.Width(50)))
               {
                   clip_a.frames = new List<FB.PosePlus.Frame>(clip_a_cache.frames);
               }
               GUILayout.EndHorizontal();

               if (GUILayout.Button("Save"))
               {
                   EditorUtility.SetDirty(clip_a);
                   EditorUtility.SetDirty(clip_b);
               }
               #endregion

           }
           GUILayout.EndVertical();
           GUILayout.EndScrollView();
         

       }
       GUILayout.EndVertical();
       Window_AniEditor.Layout_DrawSeparatorV(Color.white);
   }


    Vector2 rightpos;
    void OnGUI_ShowRight()
    {
        GUILayout.BeginVertical();
        {
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(150);
                GUILayout.Label(clip_b.name);
                GUILayout.EndHorizontal();
            }

            rightpos = GUILayout.BeginScrollView(rightpos, false, false, GUILayout.Width(300));

            GUILayout.BeginVertical();

            {
                #region 动画
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<<左移", GUILayout.Width(50)))
                {
                   
                }
                GUILayout.Space(200);
                GUILayout.Label("骨 骼", GUILayout.Width(50));

                GUILayout.EndHorizontal();
                #endregion
                GUILayout.Space(10);
                #region Box
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<<左移", GUILayout.Width(50)))
                {
                    MoveData(Operation.right_box);
                }
                GUILayout.Space(200);
                GUILayout.Label("碰撞盒", GUILayout.Width(50));

                GUILayout.EndHorizontal();
                #endregion
                GUILayout.Space(10);
                #region 触发点
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<<左移", GUILayout.Width(50)))
                {
                    MoveData(Operation.right_box);
                }
                GUILayout.Space(200);
                GUILayout.Label("触发点", GUILayout.Width(50));

                GUILayout.EndHorizontal();
                #endregion
                GUILayout.Space(10);
                #region 特效
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<<左移", GUILayout.Width(50)))
                {
                    MoveData(Operation.right_effct);
                }
                GUILayout.Space(200);
                GUILayout.Label("特 效", GUILayout.Width(50));

                GUILayout.EndHorizontal();
                #endregion
                GUILayout.Space(10);
                #region 音效
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("<<左移", GUILayout.Width(50)))
                {
                    MoveData(Operation.right_audio);
                }
                GUILayout.Space(200);
                GUILayout.Label("音 效", GUILayout.Width(50));

                GUILayout.EndHorizontal();
                #endregion
                GUILayout.Space(10);
                #region 撤销
                GUILayout.BeginHorizontal();
              
                if (GUILayout.Button("还原", GUILayout.Width(50)))
                {
                    clip_b.frames = new List<FB.PosePlus.Frame>(clip_b_cache.frames);
                }
                GUILayout.EndHorizontal();
                #endregion

            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();


        }
        GUILayout.EndVertical();
        Window_AniEditor.Layout_DrawSeparatorV(Color.white);
    }


   
    void MoveData(Operation opera)
    {
        int count = clip_a.frames.Count - clip_b.frames.Count;
        string str = "";
        string plan = "";
        if (opera == Operation.left_box || opera == Operation.left_dot || opera == Operation.left_effct || opera == Operation.left_adudio)
        {
            if (count > 0)
            {
                str = "左边多于右边" + Math.Abs(count).ToString() + "帧.\n 是否移动到右边？";
                plan = "\n 覆盖：将增加右边帧  复制：复制右边帧数内容,帧数不变";
            }
            else if (count < 0)
            {
                str = "左边少于右边" + Math.Abs(count).ToString() + "帧.\n 是否移动到右边？";
                plan = "\n 覆盖：将减少右边帧  复制：仅复制";

            }
            else
            {
                str = "左边等于于右边帧.\n 是否移动到右边？";
                plan = "\n 覆盖：帧数不变  复制：帧数不变";
            }
        }
        else if(opera == Operation.right_box || opera == Operation.right_dot || opera == Operation.right_audio || opera == Operation.right_effct)
        {
            if (count > 0)
            {
                str = "右边多于左边" + Math.Abs(count).ToString() + "帧.\n 是否移动到左边？";
                plan = "\n 覆盖：将增加左边帧  复制：复制左边帧数内容,帧数不变";
            }
            else if (count < 0)
            {
                str = "右边少于左边" + Math.Abs(count).ToString() + "帧.\n 是否移动到左边？";
                plan = "\n 覆盖：将减少左边帧  复制：仅复制";

            }
            else
            {
                str = "右边等于左边帧.\n 是否移动到左边？";
                plan = "\n 覆盖：帧数不变  复制：帧数不变";
            }
        }
        //
        if (EditorUtility.DisplayDialog("提示", str + plan, "覆盖", "复制"))
        {

            if (count >= 0)
            {
                for (int i = 0; i < count; i++)
                {
                    FB.PosePlus.Frame f = new FB.PosePlus.Frame();
                    if (opera == Operation.left_box || opera == Operation.left_dot || opera == Operation.left_effct || opera == Operation.left_adudio)
                        clip_b.frames.Add(f);
                    else if (opera == Operation.right_box || opera == Operation.right_dot || opera == Operation.right_audio || opera == Operation.right_effct)
                        if (clip_a.frames.Count > 0)
                        clip_a.frames.RemoveAt(clip_a.frames.Count - 1);
                }
            }
            else if(count <0)
            {

                for (int i = 0; i < Math.Abs(count); i++)
                {
                    FB.PosePlus.Frame f = new FB.PosePlus.Frame();

                    if (opera == Operation.left_box || opera == Operation.left_dot || opera == Operation.left_effct || opera == Operation.left_adudio)
                        if(clip_b.frames.Count>0)
                        clip_b.frames.RemoveAt(clip_b.frames.Count - 1);
                    else if (opera == Operation.right_box || opera == Operation.right_dot || opera == Operation.right_audio || opera == Operation.right_effct)
                        clip_a.frames.Add(f);
                }
            }
           
            switch (opera)
            {
                case Operation.left_box:
                    {
                        lastoperation = Operation.left_box;
                        for (int i = 0; i < clip_a.frames.Count; i++)
                        {
                            clip_b.frames[i].box_key = clip_a.frames[i].box_key;
                            clip_b.frames[i].boxesinfo = new List<FB.PosePlus.AniBoxCollider>(clip_a.frames[i].boxesinfo);
                        }
                    }
                    break;
                case Operation.left_dot:
                    {
                        lastoperation = Operation.left_dot;
                        for (int i = 0; i < clip_a.frames.Count; i++)
                        {
                            clip_b.frames[i].dotesinfo = new List<FB.PosePlus.Dot>(clip_a.frames[i].dotesinfo);
                        }
                    }
                    break;
                case Operation.left_effct:
                    {
                        lastoperation = Operation.left_effct;
                        for (int i = 0; i < clip_a.frames.Count; i++)
                        {
                            clip_b.frames[i].effectList = new List<FB.PosePlus.Effect>(clip_a.frames[i].effectList);
                        }
                    }
                    break;
                case Operation.left_adudio:
                    {
                        lastoperation = Operation.left_adudio;
                        for (int i = 0; i < clip_a.frames.Count; i++)
                        {
                            clip_b.frames[i].aduioList = new List<string>(clip_a.frames[i].aduioList);
                        }
                    }
                    break;
                case Operation.right_box:
                    {
                        lastoperation = Operation.right_box;
                        for (int i = 0; i < clip_b.frames.Count; i++)
                        {
                            clip_a.frames[i].box_key = clip_b.frames[i].box_key;
                            clip_a.frames[i].boxesinfo = new List<FB.PosePlus.AniBoxCollider>(clip_b.frames[i].boxesinfo);
                        }
                    }
                    break;
                case Operation.right_dot:
                    {
                        lastoperation = Operation.right_dot;
                        for (int i = 0; i < clip_b.frames.Count; i++)
                        {
                            clip_a.frames[i].dotesinfo = new List<FB.PosePlus.Dot>(clip_b.frames[i].dotesinfo);
                        }
                    }
                    break;
                case Operation.right_effct:
                    {
                        lastoperation = Operation.right_effct;
                        for (int i = 0; i < clip_a.frames.Count; i++)
                        {
                            clip_a.frames[i].effectList = new List<FB.PosePlus.Effect>(clip_b.frames[i].effectList);
                        }
                    }
                    break;
                case Operation.right_audio:
                    {
                        lastoperation = Operation.right_audio;
                        for (int i = 0; i < clip_a.frames.Count; i++)
                        {
                            clip_a.frames[i].aduioList = new List<string>(clip_b.frames[i].aduioList);
                        }
                    }
                    break;
                default:
                    break;
            }

          


        }
        else
        {

            switch (opera)
            {
                case Operation.left_box:
                    {
                        lastoperation = Operation.left_box;
                        for (int i = 0; i < clip_b.frames.Count; i++)
                        {
                            if (i > clip_a.frames.Count - 1) break;
                            clip_b.frames[i].boxesinfo = new List<FB.PosePlus.AniBoxCollider>(clip_a.frames[i].boxesinfo);
                        }
                    }
                    break;
                case Operation.left_dot:
                    {
                        lastoperation = Operation.left_dot;
                        for (int i = 0; i < clip_b.frames.Count; i++)
                        {
                            if (i > clip_a.frames.Count - 1) break;
                            clip_b.frames[i].dotesinfo = new List<FB.PosePlus.Dot>(clip_a.frames[i].dotesinfo);
                        }
                    }
                    break;
                case Operation.left_effct:
                    {
                        lastoperation = Operation.left_effct;
                        for (int i = 0; i < clip_b.frames.Count; i++)
                        {
                            if (i > clip_a.frames.Count - 1) break;
                            clip_b.frames[i].effectList = new List<FB.PosePlus.Effect>(clip_a.frames[i].effectList);
                        }
                    }
                    break;
                case Operation.left_adudio:
                    {
                        lastoperation = Operation.left_adudio;
                        for (int i = 0; i < clip_b.frames.Count; i++)
                        {
                            if (i > clip_a.frames.Count - 1) break;
                            clip_b.frames[i].aduioList = new List<string>(clip_a.frames[i].aduioList);
                        }
                    }
                    break;
                case Operation.right_box:
                    {
                        lastoperation = Operation.right_box;
                        for (int i = 0; i < clip_a.frames.Count; i++)
                        {
                            if (i > clip_b.frames.Count - 1) break;
                            clip_a.frames[i].box_key = clip_b.frames[i].box_key;
                            clip_a.frames[i].boxesinfo = new List<FB.PosePlus.AniBoxCollider>(clip_b.frames[i].boxesinfo);
                        }
                    }
                    break;
                case Operation.right_dot:
                    {
                        lastoperation = Operation.right_dot;
                        for (int i = 0; i < clip_b.frames.Count; i++)
                        {
                            if (i > clip_b.frames.Count - 1) break;
                            clip_a.frames[i].dotesinfo = new List<FB.PosePlus.Dot>(clip_b.frames[i].dotesinfo);
                        }
                    }
                    break;
                case Operation.right_effct:
                    {
                        lastoperation = Operation.right_effct;
                        for (int i = 0; i < clip_a.frames.Count; i++)
                        {
                            if (i > clip_b.frames.Count - 1) break;
                            clip_a.frames[i].effectList = new List<FB.PosePlus.Effect>(clip_b.frames[i].effectList);
                        }
                    }
                    break;
                case Operation.right_audio:
                    {
                        lastoperation = Operation.right_audio;
                        for (int i = 0; i < clip_a.frames.Count; i++)
                        {
                            if (i > clip_b.frames.Count - 1) break;
                            clip_a.frames[i].aduioList = new List<string>(clip_b.frames[i].aduioList);
                        }
                    }
                    break;
                default:
                    break;
            }

        }
    }
}