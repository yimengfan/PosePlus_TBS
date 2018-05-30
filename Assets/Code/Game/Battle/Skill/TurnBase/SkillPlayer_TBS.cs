using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BDFramework.ResourceMgr;
using FB.PosePlus;
using Game.Battle.Skill;

public class SkillPlayer_TBS : MonoBehaviour
{
	public ADataDrive State { get; private set; }
	//技能列表
	public Skills Skills = null;
	private AniPlayer aniPlayer = null;

	private int curSkillId = -1;
	private Skill curSkill = null;

	private bool isPlaySkill = false;
	//攻击节奏
	public int CurSkillBlockIndex { get; private set; }

	private SkillBlock curSkillBlock = null;

	private void Awake()
	{
		aniPlayer = this.GetComponent<AniPlayer>();
		State = new DataDrive_Service();
		//攻击
		this.State.RegisterData("DoBlockEvent");//执行blockEvent
		this.State.RegisterData("CurBlockEnd"); //当前block结束
		this.State.RegisterData("AllBlockEnd"); //释放技能结束
		
	}

	/// <summary>
	/// 播放技能，默认一段攻击
	/// </summary>
	/// <param name="id"></param>
	/// <param name="block"></param>
	public void PlaySkill(int id, int block)
	{		
		isPlaySkill = true;
		this.CurSkillBlockIndex = block;
		try
		{
			var skill = Skills.SkillList[id];
			if (skill != null)
			{
				this.curSkill = skill;
				curSkillBlock = curSkill.Blocks[CurSkillBlockIndex];
				var c = aniPlayer.GetClip(curSkillBlock.AniName);
				aniPlayer.Play(c);
			}
		}
		catch (Exception e)
		{
			Debug.LogError( string.Format("技能数据错误: id-{0} block-{1}" , id, block) );
		}

	}

	

	private int lastAniFrame = -1;
	HashSet<int> eventCacheSet = new HashSet<int>();
	private void Update()
	{
		if (isPlaySkill && aniPlayer != null && curSkill != null&&  curSkillBlock!= null && lastAniFrame != aniPlayer.CurAniFrame)
		{
			lastAniFrame = aniPlayer.CurAniFrame;

			//event
			var eventList = curSkillBlock.Events.FindAll((e) => (int)e.FrameId == lastAniFrame);
			
			if (eventList.Count > 0)
			{
				BDebug.Log("执行 events!");
				//执行所有list
				this.State.SetData("DoBlockEvent", eventList);
			}
			

			//当前block结束
			if (lastAniFrame == aniPlayer.CurClip.aniFrameCount -1 )
			{
				//当前block结束
				if (this.CurSkillBlockIndex < this.curSkill.Blocks.Count -1)
				{
					this.State.TriggerEvent("CurBlockEnd");
				}
				//所有block结束
				else if (this.CurSkillBlockIndex == this.curSkill.Blocks.Count -1)
				{
					isPlaySkill = false;
					this.State.TriggerEvent("AllBlockEnd");
				}
				
			}
		}
	}
}
