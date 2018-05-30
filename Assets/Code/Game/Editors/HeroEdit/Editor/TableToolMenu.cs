using System.Collections;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;
using UnityEditor;

public class TableToolMenu
{

    [MenuItem("TabelTool/配表窗口")]
    public static void OpenEditorWindow_TableMain()
    {
        var window = (EditorWindows_TableMain)EditorWindow.GetWindow(typeof(EditorWindows_TableMain), false, "配表主界面");
        window.Show();
    }

    public static void OpenEditorWindow_CreateHero()
    {
        var window = (EditorWindows_CreateHero)EditorWindow.GetWindow(typeof(EditorWindows_CreateHero), false, "Hero表编辑");
        window.Show<Hero>();
    }

    public static void OpenEditorWindow_SkillsSelect(object obj,TableConfig cfg)
    {
        var window = (EditorWindow_SkillsSelect)EditorWindow.GetWindow(typeof(EditorWindow_SkillsSelect), false, "选择skill");
        window.Show<Skill>(obj,cfg);
    }

    public static void OpenEditorWindow_BuffsSelect()
    {
        var window = (EditorWindow_BuffsSelect)EditorWindow.GetWindow(typeof(EditorWindow_BuffsSelect), false, "选择buff");
        window.Show();
    }

    public static void OpenEditorWindow_CreateSkill()
    {
        var window = (EditorWindows_CreateSkill)EditorWindow.GetWindow(typeof(EditorWindows_CreateSkill), false, "Skill表编辑");
        window.Show();
    }

    public static void OpenEditorWindow_CreateBuff()
    {
        var window = (EditorWindows_CreateBuff)EditorWindow.GetWindow(typeof(EditorWindows_CreateBuff), false, "buff表编辑");
        window.Show();
    }


    public static void Layout_DrawSeparator(Color color, float height = 4f)
    {

        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, height), EditorGUIUtility.whiteTexture);
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

}
