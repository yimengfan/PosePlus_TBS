using System;
using System.Runtime.CompilerServices;
using BDFramework.Mgr;
using BDFramework.UI;
using Code.Game.Battle;
using Game.Battle;
using UnityEngine;

namespace Game.Battle.Skill
{
  public class SkillEventFactory : MgrBase<SkillEventFactory>
  {

      public override void CheckType(Type type)
      {
          var attrs = type.GetCustomAttributes(typeof(SkillEventAttribute), false);
          if (attrs.Length > 0)
          {
              foreach (var attr in attrs)
              {
                  var _attr = (SkillEventAttribute)attr;
                  var name = _attr.Name.ToString();
                  SaveAttribute(name, new ClassData() { Attribute = _attr, Type = type });
              }
          }
      }

      public void DoEvent(SkillEvent skillEvent, IBattle battle, IHeroFSM selfFSM, IHeroFSM targetFSM)
      {
          
          ISkillEvent _event = null;

          var cd = this.GetCalssData(skillEvent.EventName);
          if (cd != null)
          {
              _event = Activator.CreateInstance(cd.Type, skillEvent)  as ISkillEvent;
          }
          
          //
          if (_event != null)
          {
              BDebug.Log("执行skillEvent:" + skillEvent.EventName);
              
              
              _event.Do(battle,selfFSM,targetFSM);
          }
          else
          {
              BDebug.LogError("未找到技能事件:" +  skillEvent.EventName);
          }
      }
  }
}