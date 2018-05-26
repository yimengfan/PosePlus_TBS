using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using Game.Data;
using System.Text.RegularExpressions;
using System;
using System.Linq;

public class Editor_TableTool
{
    private static string tablePath = "/Resource/Table/";
    public static List<T> Prepare_DataByName<T>(string jsonName)
    {
        string jsonPath = Application.dataPath + tablePath + jsonName + ".json";
        if (!File.Exists(jsonPath))
        {
            UnityEditor.EditorUtility.DisplayDialog("读取失败", jsonPath + "不存在", "ok");
            return null;
        }
        string content = File.ReadAllText(jsonPath);
        return JsonMapper.ToObject<List<T>>(content);
    }

    public static void GetData_HeroData(ref Dictionary<int, List<Hero>> list, string jsonName)
    {
        list.Clear();
        List<Hero> heroList = Prepare_DataByName<Hero>(jsonName);
        if (heroList == null) return;
        int index = 0;
        for (int i = 0; i < heroList.Count; i++)
        {
            List<Hero> tpList;
            if (!list.TryGetValue(index, out tpList))
            {
                tpList = new List<Hero>();
                list.Add(index, tpList);
            }

            Hero h = heroList[i];
            tpList.Add(h);
            if (h.NextLevel == 0)
            {
                index++;
            }
        }
    }

    public static void GetData_SkillData(ref Dictionary<int, List<Skill>> list, string jsonName)
    {
        list.Clear();
        List<Skill> skillList = Prepare_DataByName<Skill>(jsonName);
        if (skillList == null) return;
        int index = 0;
        for (int i = 0; i < skillList.Count; i++)
        {
            List<Skill> tpList;
            if (!list.TryGetValue(index, out tpList))
            {
                tpList = new List<Skill>();
                list.Add(index, tpList);
            }

            Skill h = skillList[i];
            tpList.Add(h);
            if (h.NextLevelID == 0)
            {
                index++;
            }
        }
    }

    public static void GetData_BuffData(ref List<Buff> list, string jsonName)
    {
        list.Clear();
        list = Prepare_DataByName<Buff>(jsonName);
    }

    public static string Join<T>(List<T> list, string sp = ",")
    {
        return string.Join(sp, list);
    }

    public static List<string> StringToList(string str, char sp = ',')
    {
        str = str.Trim();
        string[] strs = str.Split(sp);
        List<string> result = new List<string>();
        foreach (string st in strs)
        {
            result.Add(st.Trim());
        }
        return result;
    }

    public static List<double> StringToDoubleList(string str, char sp = ',')
    {
        str = str.Trim();
        string[] strs = str.Split(sp);
        List<double> result = new List<double>();
        foreach (string st in strs)
        {
            if (string.IsNullOrEmpty(st.Trim()))
            {
                result.Add(0);
            }
            else
            {
                result.Add(double.Parse(st.Trim()));
            }
        }
        return result;
    }

    public static void SaveSkillJsonFile(Dictionary<int, List<Skill>> skillList)
    {
        List<Skill> skills = new List<Skill>();
        foreach (KeyValuePair<int, List<Skill>> kv in skillList)
        {
            if (kv.Value != null && kv.Value.Count > 0)
            {
                skills.AddRange(kv.Value);
            }
        }
        SaveJson<Skill>(skills, "Skill");
    }

    public static void SaveHeroJsonFile(Dictionary<int, List<Hero>> heroList)
    {
        List<Hero> heros = new List<Hero>();
        foreach (KeyValuePair<int, List<Hero>> kv in heroList)
        {
            if (kv.Value != null && kv.Value.Count > 0)
            {
                heros.AddRange(kv.Value);
            }
        }
        SaveJson<Hero>(heros, "HeroLogic");
    }

    public static void SaveBuffJsonFile(List<Buff> buffList)
    {
        SaveJson<Buff>(buffList, "Buff");
    }

    private static void SaveJson<T>(List<T> list, string fileName)
    {
        string listJson = JsonMapper.ToJson(list);
        string savePath = Application.dataPath + tablePath;
        if (!Directory.Exists(savePath))
        {
            UnityEditor.EditorUtility.DisplayDialog("文件不存在", savePath + "当前路径不存在", "ok");
            return;
        }
        savePath = savePath + string.Format("/{0}.json", fileName.Trim());
        Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
        var ss = reg.Replace(listJson, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
        File.WriteAllText(savePath, ss);
        UnityEditor.EditorUtility.DisplayDialog("成功", "保存成功=>" + savePath, "ok");
    }

    public static bool CheckAllSkillExist(List<double> skills, List<Skill> skillList)
    {
        bool allExist = true;
        foreach (double id in skills)
        {
            bool isExist = false;
            foreach (Skill s in skillList)
            {
                if (s.Id == id)
                {
                    isExist = true;
                    break;
                }
            }
            allExist = isExist;
            if (allExist == false) break;
        }
        return allExist;
    }

    public static bool CheckAllSkillExist(Dictionary<int, List<Hero>> heroList, List<Skill> skillList)
    {
        foreach (List<Hero> heros in heroList.Values)
        {
            foreach (Hero h in heros)
            {
                bool allExist = CheckAllSkillExist(h.Skills, skillList);
                if (!allExist) return false;
            }
        }
        return true;
    }

    public static bool isRepeatHeroIdOrNullAttributeName(Dictionary<int, List<Hero>> heroList)
    {
        List<Hero> heros = new List<Hero>();
        foreach (KeyValuePair<int, List<Hero>> kv in heroList)
        {
            heros.AddRange(kv.Value);
        }
        if (heros.GroupBy(x => x.Id).Where(x => x.Count() > 1).ToList().Count() > 0) return true;

        foreach (Hero h in heros)
        {
            foreach (string name in h.AttributeName)
            {
                if (string.IsNullOrEmpty(name)) return true;
            }
        }
        return false;
    }


    public static bool IsRepeatHeroId(int id, Dictionary<int, List<Hero>> list)
    {
        int index = 0;
        foreach (KeyValuePair<int, List<Hero>> kv in list)
        {
            foreach (Hero h in kv.Value)
            {
                if (h.Id == id) index++;
                if (index > 1) return true;
            }
        }
        return false;
    }

    public static bool IsRepeatHeroId(int id, List<Buff> list)
    {
        int index = 0;
        foreach (Buff h in list)
        {
            if (h.Id == id) index++;
            if (index > 1) return true;
        }
        return false;
    }

    public static bool IsRepeatSkillId(int id, Dictionary<int, List<Skill>> list)
    {
        int index = 0;
        foreach (KeyValuePair<int, List<Skill>> kv in list)
        {
            foreach (Skill h in kv.Value)
            {
                if (h.Id == id) index++;
                if (index > 1) return true;
            }
        }
        return false;
    }

    public static bool IsRepeatSkillId(Dictionary<int, List<Skill>> skillList)
    {
        List<Skill> skills = new List<Skill>();
        foreach (KeyValuePair<int, List<Skill>> kv in skillList)
        {
            skills.AddRange(kv.Value);
        }
        if (skills.GroupBy(x => x.Id).Where(x => x.Count() > 1).ToList().Count() > 0) return true;
        return false;
    }

    public static bool CheckAllBuffExist(Dictionary<int, List<Skill>> skillList, List<Buff> buffList)
    {
        foreach (List<Skill> skills in skillList.Values)
        {
            foreach (Skill s in skills)
            {
                //TODO by xiaofan
//                bool allExist = CheckAllBuffExist(s.Buffs, buffList);
//                if (!allExist) return false;
            }
        }
        return true;
    }


    public static bool CheckAllBuffExist(List<double> buffs, List<Buff> buffList)
    {
        bool allExist = true;
        foreach (double id in buffs)
        {
            bool isExist = false;
            foreach (Buff b in buffList)
            {
                if (b.Id == id)
                {
                    isExist = true;
                    break;
                }
            }
            allExist = isExist;
            if (allExist == false) break;
        }
        return allExist;
    }

    public static void ClearUnExistBuffInSkill(ref Dictionary<int, List<Skill>> skillList, List<Buff> buffList)
    {
        List<double> ids = new List<double>();
        foreach (Buff b in buffList)
        {
            ids.Add(b.Id);
        }
        foreach (KeyValuePair<int, List<Skill>> kv in skillList)
        {
            foreach (Skill s in kv.Value)
            {
                //TODO by xiaofan
                List<double> remove = new List<double>();
//                for (int i = 0; i < s.Buffs.Count; i++)
//                {
//                    bool isExist = ids.Contains(s.Buffs[i]);
//                    if (!isExist) remove.Add(s.Buffs[i]);
//                }
//                foreach (double d in remove)
//                {
//                    s.Buffs.Remove(d);
//                }
            }
        }
    }

    public static void ClearUnExistSkillInHero(ref Dictionary<int, List<Hero>> heroList, List<Skill> skillList)
    {
        List<double> ids = new List<double>();
        foreach (Skill s in skillList)
        {
            ids.Add(s.Id);
        }
        foreach (KeyValuePair<int, List<Hero>> kv in heroList)
        {
            foreach (Hero h in kv.Value)
            {
                List<double> remove = new List<double>();
                for (int i = 0; i < h.Skills.Count; i++)
                {
                    bool isExist = ids.Contains(h.Skills[i]);
                    if (!isExist) remove.Add(h.Skills[i]);
                }
                foreach (double d in remove)
                {
                    h.Skills.Remove(d);
                }
            }
        }
    }

    public static bool IsRepeatBuffId(List<Buff> buffList)
    {
        return (buffList.GroupBy(x => x.Id).Where(x => x.Count() > 1).ToList().Count() > 0);
    }

    public static bool CheckBuffParamsNull(List<Buff> buffList)
    {
        foreach (Buff b in buffList)
        {
            foreach (string str in b.Params_Formula)
            {
                if (string.IsNullOrEmpty(str)) return true;
            }
            foreach (string str in b.Params_StrValue)
            {
                if (string.IsNullOrEmpty(str)) return true;
            }
        }
        return false;
    }


    public static bool CheckIntNum(string tNum)
    {
        System.Text.RegularExpressions.Regex reg1
         = new System.Text.RegularExpressions.
             Regex(@"^[-]?[1-9]{1}\d*$|^[0]{1}$");
        return reg1.IsMatch(tNum);
    }

    public static bool CheckDoubleNum(string tNum)
    {
        if (CheckIntNum(tNum)) return true;
        System.Text.RegularExpressions.Regex reg1
         = new System.Text.RegularExpressions.
             Regex(@"^-?([1-9]\d*\.\d*|0\.\d*[1-9]\d*|0?\.0+|0)$");
        return reg1.IsMatch(tNum);

    }

    public static string GetRealName(string structName)
    {
        Regex reg1 = new Regex(@"([^<>]*)>");
        return reg1.Match(structName).Value;
    }

    public static Dictionary<string, Dictionary<string, TableConfig>> classDic = new Dictionary<string, Dictionary<string, TableConfig>>();

    public static void GetData_TableConfigData()
    {
        classDic.Clear();
        List<TableConfig> configList = Prepare_DataByName<TableConfig>("TableConfig");
        if (configList == null) return;
        for (int i = 0; i < configList.Count; i++)
        {
            TableConfig config = configList[i];
            Dictionary<string, TableConfig> tpList;
            if (!classDic.TryGetValue(config.ClassName, out tpList))
            {
                tpList = new Dictionary<string, TableConfig>();
                classDic.Add(config.ClassName, tpList);
            }
            tpList.Add(config.Name, config);
        }
    }

    public static Dictionary<string, TableConfig> GetClassDrawConfig(string className)
    {
        Dictionary<string, TableConfig> dict;
        if (!classDic.TryGetValue(className, out dict))
        {
            Debug.LogError("生成失败 " + className + " 没有对应的tableconfig配置");
            return null;
        }
        else
        {
            return dict;
        }
    }

}
