using System.Collections;
using System.Collections.Generic;
using Game.Battle.Skill;
using UnityEditor;
using UnityEngine;

[SkillEventAttribute("JumpToTarget" , "跳跃到目标")]
public class SkillEvent_JumpToTargetEditor : ISkillEventEditor {

	public SkillEvent OnGuiEditor(SkillEvent se)
	{
		se.DoubleParams0 = EditorGUILayout.DoubleField("时间参数(帧)", se.DoubleParams0);
		return se;
	}
}
