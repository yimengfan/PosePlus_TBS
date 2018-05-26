using System;
using Game.Battle.Skill;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class EditorSkillTool
{
    public static void GetSkillEventDict(SkillBlock sb, ref Dictionary<int, List<SkillEvent>> dict)
    {
        dict.Clear();
        foreach (SkillEvent se in sb.Events)
        {
            List<SkillEvent> seList;
            if (!dict.TryGetValue(se.FrameId, out seList))
            {
                seList = new List<SkillEvent>();
                dict.Add(se.FrameId, seList);
            }

            seList.Add(se);
        }
    }

    public static void PushSkillEventToBlock(ref SkillBlock sb, Dictionary<int, List<SkillEvent>> dict)
    {
        sb.Events.Clear();
        foreach (List<SkillEvent> list in dict.Values)
        {
            foreach (SkillEvent se in list)
            {
                sb.Events.Add(se);
            }
        }
    }

    public static Type GetTypeBySEAttributeName(string name)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ISkillEventEditor))))
            .ToArray();
        foreach (Type t in types)
        {
            SkillEventAttribute attribute = (SkillEventAttribute) t.GetCustomAttribute(typeof(SkillEventAttribute));
            if (attribute != null && attribute.Name == name)
            {
                return t;
            }
        }

        return null;
    }

    public static bool CheckAniExist(List<FB.PosePlus.AniClip> clips,string aniName)
    {
        foreach (FB.PosePlus.AniClip clip in clips)
        {
            if (clip.name == aniName)
            {
                return true;
            }
        }

        return false;
    }
}