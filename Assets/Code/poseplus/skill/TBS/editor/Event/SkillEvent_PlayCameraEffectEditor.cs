using System.Collections;
using System.Collections.Generic;
using Game.Battle.Skill;
using UnityEditor;
using UnityEngine;

[SkillEventAttribute("PlayCameraEffect","播放镜头特效")]
public class SkillEvent_PlayCameraEffectEditor : ISkillEventEditor {
    public SkillEvent OnGuiEditor(SkillEvent se)
    {
        se.DoubleParams0 = EditorGUILayout.DoubleField("镜头效果持续时间", se.DoubleParams0);
        se.StrParam0 = EditorGUILayout.TextField("镜头效果名", se.StrParam0);
        return se;
    }
}
