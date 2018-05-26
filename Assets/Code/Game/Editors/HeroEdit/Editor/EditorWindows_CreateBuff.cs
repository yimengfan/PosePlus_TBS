using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Data;
using System.Linq;

public class EditorWindows_CreateBuff : EditorWindow
{
    private List<Buff> buffList = new List<Buff>();
    private void Awake()
    {
        Editor_TableTool.GetData_BuffData(ref buffList, "Buff");
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(600), GUILayout.Height(900));
        OnGUI_BuffList();
        OnGUI_EditorBottomButtons();
        TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
        GUILayout.EndVertical();
        TableToolMenu.Layout_DrawSeparatorV(Color.gray, 2);
    }

    private void OnGUI_EditorBottomButtons()
    {
        GUILayout.BeginHorizontal();
        OnGUI_EditorAddLineButton();
        OnGUI_EditorSaveButton();
        GUILayout.EndHorizontal();
    }

    private void OnGUI_EditorAddLineButton()
    {
        if (GUILayout.Button("新增加一行"))
        {
            Buff b = new Buff();
            b.Params_Formula = new List<string>();
            b.Params_NumValue = new List<double>();
            b.Params_StrValue = new List<string>();
            buffList.Add(b);
        }
    }

    private void OnGUI_EditorSaveButton()
    {
        if (GUILayout.Button("保存为json文件"))
        {
            if (Editor_TableTool.IsRepeatBuffId(buffList))
            {
                EditorUtility.DisplayDialog("失败", "buffid重名", "ok");
                return;
            }
            if (Editor_TableTool.CheckBuffParamsNull(buffList))
            {
                EditorUtility.DisplayDialog("失败", "列表没填写正确", "ok");
                return;
            }
            Editor_TableTool.SaveBuffJsonFile(buffList);
            EditorWindows_CreateSkill.RefreshBuffData();
        }
    }

    Vector2 windowSelectPosition = Vector2.zero;
    private void OnGUI_BuffList()
    {
        GUILayout.BeginVertical();
        windowSelectPosition = GUILayout.BeginScrollView(windowSelectPosition, GUILayout.Width(600), GUILayout.Height(900));
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                OnGUI_EditorBuffItem(i);
                TableToolMenu.Layout_DrawSeparator(Color.gray, 2);
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void OnGUI_EditorBuffItem(int index)
    {
        Buff b = buffList[index];
        //bool isRepeat = buffList.GroupBy(x => x.Id).Where(x => x.Count() > 1).Where(x => x.Key == b.Id).ToList().Count() > 1;
        bool isRepeat = Editor_TableTool.IsRepeatHeroId((int)b.Id, buffList);
        if (isRepeat)
        {
            GUIStyle titleStyle2 = new GUIStyle();
            titleStyle2.normal.textColor = new Color(1, 0, 0, 1);
            GUILayout.Label("id重复", titleStyle2);
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("add", GUILayout.Width(50)))
        {
            Buff newBuff = new Buff();
            newBuff.Params_StrValue = new List<string>();
            newBuff.Params_NumValue = new List<double>();
            newBuff.Params_Formula = new List<string>();
            buffList.Insert(index + 1, newBuff);
        }
        bool isdel = false;
        if (GUILayout.Button("del", GUILayout.Width(50)))
        {
            isdel = true;
            buffList.Remove(b);
        }
        GUILayout.EndHorizontal();
        if (!isdel)
            buffList[index] = (Buff)DrawTableType.DrawWindow(b);
    }

}
