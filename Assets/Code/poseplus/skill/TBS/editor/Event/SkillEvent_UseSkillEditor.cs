using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Activation;
using Game.Battle.Skill;
using UnityEngine;

[SkillEventAttribute("UseSkill","技能攻击")]
public class SkillEvent_UseSkillEditor : ISkillEventEditor {


	public SkillEvent OnGuiEditor(SkillEvent se)
	{
		return se;
	}
}
