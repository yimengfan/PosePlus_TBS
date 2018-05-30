using System.Collections;
using System.Collections.Generic;
using Game.Battle.Skill;
using UnityEditor;
using UnityEngine;

[SkillEventAttribute("FlashesToTarget", "闪现到目标")]
public class SkillEvent_FlashesToTargetEditor : ISkillEventEditor
{
    public SkillEvent OnGuiEditor(SkillEvent se)
    {
        se.DoubleParams0 = EditorGUILayout.DoubleField("时间参数(帧)", se.DoubleParams0);
        return se;
    }
}