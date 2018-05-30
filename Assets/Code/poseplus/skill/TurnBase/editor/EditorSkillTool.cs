using System;
using Game.Battle.Skill;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class EditorSkillTool
{
    public static List<SkillEvent> GetCurFrameEventList(int frame, SkillBlock block)
    {
        if (block == null || block.Events == null) return null;
        List<SkillEvent> evtList = new List<SkillEvent>();
        foreach (SkillEvent evt in block.Events)
        {
            if (evt.FrameId == frame)
            {
                evtList.Add(evt);
            }
        }

        return evtList;
    }

    public static Dictionary<string, SkillEditorData> GetSkillEditorDict()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ISkillEventEditor))))
            .ToArray();
        Dictionary<string, SkillEditorData> dict = new Dictionary<string, SkillEditorData>();
        foreach (Type t in types)
        {
            SkillEventAttribute attribute =
                (SkillEventAttribute) t.GetCustomAttribute(typeof(SkillEventAttribute));
            SkillEditorData data = new SkillEditorData();
            data.classdata = t;
            data.attr = attribute;
            dict.Add(attribute.Name, data);
        }

        return dict;
    }
}