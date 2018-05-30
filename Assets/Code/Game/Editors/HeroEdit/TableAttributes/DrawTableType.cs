using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class DrawTableType
{
    public static BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;

    public static object DrawWindow(object o)
    {
        Type t = o.GetType();
        PropertyInfo[] pi = t.GetProperties();
        //FieldInfo[] fs = t.GetFields(flag);
        Dictionary<string, TableConfig> classDic = Editor_TableTool.GetClassDrawConfig(t.Name);
        if (classDic == null) return null;
        for (int i = 0; i < pi.Length; i++)
        {
            PropertyInfo f = pi[i];
            TableConfig config;
            if (!classDic.TryGetValue(f.Name, out config))
            {
                Debug.Log("警告 " + f.Name + " 没有配置tableconfig项");
                return null;
            }

            if (config.AttributeType == "InputAttribute")
            {
                DrawInput(config, f, o);
            }
            else if (config.AttributeType == "ListGroupAttribute")
            {
                DrawGroupList(config, f, o, classDic);
            }
            else if (config.AttributeType == "SelectAttribute")
            {
                DrawSelect(config, f, o);
            }
            else if (config.AttributeType == "VariableListAttribute")
            {
                DrawVariableList(config, f, o, classDic);
            }
        }

        return o;
    }

    #region 小窗口数据处理代码
    public static void DrawVariableList(TableConfig config, PropertyInfo f, object o,
        Dictionary<string, TableConfig> dict)
    {
        double tpInt = (double) f.GetValue(o);
        var tp = EditorGUILayout.IntField(config.Des, (int) tpInt);
        f.SetValue(o, tp);
        foreach (string child in config.ParameterList)
        {
            GUILayout.BeginHorizontal();
            PropertyInfo fi = o.GetType().GetProperty(child);
            var childValue = fi.GetValue(o);
            Type childType = fi.PropertyType.GetGenericArguments()[0];
            TableConfig t;
            if (dict.TryGetValue(fi.Name, out t))
            {
                GUILayout.Label(t.Des);
            }

            int count = Convert.ToInt32(fi.PropertyType.GetProperty("Count").GetValue(childValue, null));
            if (tp > count)
            {
                int addRange = tp - count;
                MethodInfo childMethod = fi.PropertyType.GetMethod("Add");
                for (int i = 0; i < addRange; i++)
                {
                    if (childType.Equals(typeof(string)))
                    {
                        childMethod.Invoke(childValue, new object[] {default(string)});
                    }
                    else if (childType.Equals(typeof(double)))
                    {
                        childMethod.Invoke(childValue, new object[] {default(double)});
                    }
                }
            }
            else if (tp < count)
            {
                if (tp <= 0)
                {
                    MethodInfo childMethod = fi.PropertyType.GetMethod("Clear");
                    childMethod.Invoke(childValue, null);
                }
                else if (tp > 0)
                {
                    MethodInfo childMethod = fi.PropertyType.GetMethod("RemoveRange");
                    childMethod.Invoke(childValue, new object[] {(int) (tp - 1), (int) (count - tp)});
                }
            }

            GUILayout.BeginVertical();
            for (int i = 0; i < tp; i++)
            {
                if (childType.Equals(typeof(string)))
                {
                    List<string> value = (List<string>) childValue;
                    value[i] = EditorGUILayout.TextField(value[i]);
                }
                else if (childType.Equals(typeof(double)))
                {
                    List<double> value = (List<double>) childValue;
                    value[i] = EditorGUILayout.DoubleField(value[i]);
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }

    public static void DrawInput(TableConfig config, PropertyInfo f, object o)
    {
        Type t = f.PropertyType;
        if (t.Equals(typeof(double)))
        {
            var tp = EditorGUILayout.DoubleField(config.Des, (double) f.GetValue(o));
            f.SetValue(o, tp);
        }
        else if (t.Equals(typeof(string)))
        {
            var tp = EditorGUILayout.TextField(config.Des, (string) f.GetValue(o));
            f.SetValue(o, tp);
        }
        else if (t.Equals(typeof(bool)))
        {
            var tp = EditorGUILayout.Toggle(config.Des, (bool) f.GetValue(o));
            f.SetValue(o, tp);
        }
    }

    public static void DrawSelect(TableConfig config, PropertyInfo f, object o)
    {
        List<double> selectValue = (List<double>) f.GetValue(o);
        GUILayout.BeginHorizontal(GUILayout.Width(100));
        GUILayout.Label(config.Des, GUILayout.Width(100));
        string selects = Editor_TableTool.Join(selectValue);
        GUILayout.Label(selects, GUILayout.Width(100));
        if (GUILayout.Button("选择", GUILayout.Width(50)))
        {
            Type t = GetTypeByName(config.ParameterList[0]);
            EditorWindow window = EditorWindow.GetWindow(t, false, "选择窗口");
            MethodInfo selectMethod = t.GetMethod(config.ParameterList[1]);
            selectMethod.Invoke(window, new object[] {o,config});
        }

        GUILayout.EndHorizontal();
    }

    public static Type GetTypeByName(string typeName)
    {
        Type type = null;

        //如果该类型已经装载 
        type = Type.GetType(typeName);
        if (type != null)
        {
            return type;
        }

        //在EntryAssembly中查找 
        if (Assembly.GetEntryAssembly() != null)
        {
            type = Assembly.GetEntryAssembly().GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        //在CurrentDomain的所有Assembly中查找 
        Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
        int assemblyArrayLength = assemblyArray.Length;
        for (int i = 0; i < assemblyArrayLength; ++i)
        {
            type = assemblyArray[i].GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        for (int i = 0; (i < assemblyArrayLength); ++i)
        {
            Type[] typeArray = assemblyArray[i].GetTypes();
            int typeArrayLength = typeArray.Length;
            for (int j = 0; j < typeArrayLength; ++j)
            {
                if (typeArray[j].Name.Equals(typeName))
                {
                    return typeArray[j];
                }
            }
        }

        return type;
    }

    public static void DrawGroupList(TableConfig config, PropertyInfo f, object o, Dictionary<string, TableConfig> dict)
    {
        DrawGroupListTitle(config, o, dict);
        var value = f.GetValue(o);
        Type type = f.PropertyType;
        Type genericType = type.GetGenericArguments()[0];
        int count = Convert.ToInt32(type.GetProperty("Count").GetValue(value, null));
        if (count == 0)
        {
            if (GUILayout.Button("创建一行属性"))
            {
                MethodInfo methodInfo = f.PropertyType.GetMethod("Add");
                var parentValue = f.GetValue(o);
                if (genericType.Equals(typeof(string)))
                {
                    methodInfo.Invoke(parentValue, new object[] {default(string)});
                }
                else if (genericType.Equals(typeof(double)))
                {
                    methodInfo.Invoke(parentValue, new object[] {default(double)});
                }

                foreach (string child in config.ParameterList)
                {
                    PropertyInfo fi = o.GetType().GetProperty(child);
                    var childValue = fi.GetValue(o);
                    Type childType = fi.PropertyType.GetGenericArguments()[0];
                    MethodInfo childMethod = fi.PropertyType.GetMethod("Add");
                    if (childType.Equals(typeof(string)))
                    {
                        childMethod.Invoke(childValue, new object[] {default(string)});
                    }
                    else if (childType.Equals(typeof(double)))
                    {
                        childMethod.Invoke(childValue, new object[] {default(double)});
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                DrawGroupListItem(f, o, i, config);
                count = Convert.ToInt32(type.GetProperty("Count").GetValue(value, null));
            }
        }
    }

    public static void DrawGroupListItem(PropertyInfo f, object o, int index, TableConfig config)
    {
        GUILayout.BeginHorizontal();
        Type pGenericType = f.PropertyType.GetGenericArguments()[0];
        if (pGenericType.Equals(typeof(string)))
        {
            List<string> parentValue = (List<string>) f.GetValue(o);
            if (parentValue == null)
            {
                Debug.LogError(f.Name + "值为null,手动修改或重新生成excel表");
                return;
            }

            parentValue[index] = EditorGUILayout.TextField(parentValue[index], GUILayout.Width(config.UIWidth));
        }
        else if (pGenericType.Equals(typeof(double)))
        {
            List<double> parentValue = (List<double>) f.GetValue(o);
            if (parentValue == null)
            {
                Debug.LogError(f.Name + "值为null,手动修改或重新生成excel表");
                return;
            }

            parentValue[index] = EditorGUILayout.DoubleField(parentValue[index], GUILayout.Width(config.UIWidth));
        }

        foreach (string child in config.ParameterList)
        {
            PropertyInfo fi = o.GetType().GetProperty(child);
            Type childType = fi.PropertyType.GetGenericArguments()[0];
            if (childType.Equals(typeof(string)))
            {
                List<string> childValue = (List<string>) fi.GetValue(o);
                if (childValue == null)
                {
                    Debug.LogError(child + "值为null,手动修改或重新生成excel表");
                    return;
                }

                childValue[index] = EditorGUILayout.TextField(childValue[index], GUILayout.Width(config.UIWidth));
            }
            else if (childType.Equals(typeof(double)))
            {
                List<double> childValue = (List<double>) fi.GetValue(o);
                if (childValue == null)
                {
                    Debug.LogError(child + "值为null,手动修改或重新生成excel表");
                    return;
                }

                childValue[index] = EditorGUILayout.DoubleField(childValue[index], GUILayout.Width(config.UIWidth));
            }
        }

        if (GUILayout.Button("add", GUILayout.Width(50)))
        {
            MethodInfo methodInfo = f.PropertyType.GetMethod("Insert");
            var parentValue = f.GetValue(o);
            if (pGenericType.Equals(typeof(string)))
            {
                methodInfo.Invoke(parentValue, new object[] {index + 1, default(string)});
            }
            else if (pGenericType.Equals(typeof(double)))
            {
                methodInfo.Invoke(parentValue, new object[] {index + 1, default(double)});
            }

            foreach (string child in config.ParameterList)
            {
                PropertyInfo fi = o.GetType().GetProperty(child);
                var childValue = fi.GetValue(o);
                Type childType = fi.PropertyType.GetGenericArguments()[0];
                MethodInfo childMethod = fi.PropertyType.GetMethod("Insert");
                if (childType.Equals(typeof(string)))
                {
                    childMethod.Invoke(childValue, new object[] {index + 1, default(string)});
                }
                else if (childType.Equals(typeof(double)))
                {
                    childMethod.Invoke(childValue, new object[] {index + 1, default(double)});
                }
            }
        }

        if (GUILayout.Button("del", GUILayout.Width(50)))
        {
            MethodInfo methodInfo = f.PropertyType.GetMethod("RemoveAt");
            var parentValue = f.GetValue(o);
            methodInfo.Invoke(parentValue, new object[] {index});
            foreach (string child in config.ParameterList)
            {
                PropertyInfo fi = o.GetType().GetProperty(child);
                var childValue = fi.GetValue(o);
                MethodInfo childMethod = fi.PropertyType.GetMethod("RemoveAt");
                childMethod.Invoke(childValue, new object[] {index});
            }
        }

        GUILayout.EndHorizontal();
    }

    public static void DrawGroupListTitle(TableConfig config, object o, Dictionary<string, TableConfig> dict)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(config.Des);
        foreach (string child in config.ParameterList)
        {
            FieldInfo fi = o.GetType().GetField(child, flag);
            TableConfig t;
            if (dict.TryGetValue(child, out t))
            {
                GUILayout.Label(t.Des);
            }
        }

        GUILayout.EndHorizontal();
    }   

    #endregion
    
    /**
     *sortType == 1 等于0一组数据
     */
    public static List<object> DrawWindowWithSort(List<object> datas, int x, int y, int sortType, string sortField)
    {
        Dictionary<int, List<object>> sortList;
        if (sortType == 1)
        {
            sortList = Sort1(datas, sortField);
        }
        else
        {
            sortList = new Dictionary<int, List<object>>();
        }

        List<int> keyList = new List<int>(sortList.Keys);
        int maxId = Editor_TableTool.GetMaxId(datas);
        for (int i = 0; i < keyList.Count; i++)
        {
            int k = keyList[i];
            List<object> v;
            if (sortList.TryGetValue(k, out v))
            {
                GUILayout.BeginHorizontal();
                //展示一行数据
                for (int j = 0; j < v.Count; j++)
                {
//                    OnGUI_EditorHeroItem(v, j, k);
                    GUILayout.BeginVertical(GUILayout.Width(x));
                    OnGUI_EditorItem(v, j, sortField,maxId);
                    GUILayout.EndVertical();
                    Layout_DrawSeparatorV(Color.gray, 2);
                }

                GUILayout.EndHorizontal();
                Layout_DrawSeparator(Color.gray, 2);
            }
        }

        return DictToList(sortList);
    }


    public static List<object> DictToList(Dictionary<int, List<object>> sortList)
    {
        List<object> result = new List<object>();
        foreach (List<object> list in sortList.Values)
        {
            foreach (object obj in list)
            {
                result.Add(obj);
            }
        }

        return result;
    }

    //展示一份数据窗口
    public static void OnGUI_EditorItem(List<object> list, int index,string sortField,int maxId)
    {
        object o = list[index];
        GUILayout.BeginVertical();
//        bool isRepeat = Editor_TableTool.IsRepeatHeroId((int)h.Id, heroList);
//        if (isRepeat)
//        {
//            GUIStyle titleStyle2 = new GUIStyle();
//            titleStyle2.normal.textColor = new Color(1, 0, 0, 1);
//            GUILayout.Label("id重复", titleStyle2);
//        }
        GUILayout.BeginHorizontal(GUILayout.Width(200));
        if (GUILayout.Button("add", GUILayout.Width(50)))
        {
            object obj = Editor_TableTool.CreateInstance(o.GetType());
            list.Insert(index + 1, obj);
            PropertyInfo idProp = obj.GetType().GetProperty("Id");
            idProp.SetValue(obj,maxId+1);
            PropertyInfo sortProp = obj.GetType().GetProperty(sortField);
            sortProp.SetValue(obj,sortProp.GetValue(o));
            sortProp.SetValue(o,maxId+1);
        }

        bool isdel = false;
        if (GUILayout.Button("del", GUILayout.Width(50)))
        {
            list.Remove(o);
            isdel = true;
        }

        GUILayout.EndHorizontal();
        if (!isdel)
            list[index] = DrawTableType.DrawWindow(o);
        GUILayout.EndVertical();
    }

    public static Dictionary<int, List<object>> Sort1(List<object> datas, string sortField)
    {
        //布局字典
        Dictionary<int, List<object>> resultDict = new Dictionary<int, List<object>>();
        int index = 0;
        for (int i = 0; i < datas.Count; i++)
        {
            List<object> tpList;
            if (!resultDict.TryGetValue(index, out tpList))
            {
                tpList = new List<object>();
                resultDict.Add(index, tpList);
            }

            object o = datas[i];
            tpList.Add(o);
            if (sortField == "")
            {
                index++;
            }
            else
            {
                PropertyInfo pi = o.GetType().GetProperty(sortField);
                double value = (double) pi.GetValue(o);
                if (value == 0)
                {
                    index++;
                }
            }
        }

        return resultDict;
    }

    public static void DrawDisplayWindow(object o)
    {
        Type t = o.GetType();

        PropertyInfo[] fs = t.GetProperties();
        Dictionary<string, TableConfig> classDic = Editor_TableTool.GetClassDrawConfig(t.Name);
        if (classDic == null) return;
        for (int i = 0; i < fs.Length; i++)
        {
            PropertyInfo f = fs[i];
            TableConfig config;
            if (!classDic.TryGetValue(f.Name, out config))
            {
                Debug.LogError("警告 " + f.Name + " 没有配置tableconfig项");
                return;
            }

            if (config.AttributeType == "InputAttribute")
            {
                EditorGUILayout.LabelField(string.Format("{0}:{1}", config.Des, f.GetValue(o)));
            }
            else if (config.AttributeType == "ListGroupAttribute")
            {
                Type type = f.PropertyType.GetGenericArguments()[0];
                if (type.Equals(typeof(string)))
                {
                    List<string> value = (List<string>) f.GetValue(o);
                    EditorGUILayout.LabelField(string.Format("{0}:{1}", config.Des, Editor_TableTool.Join(value)));
                }
                else if (type.Equals(typeof(double)))
                {
                    List<double> value = (List<double>) f.GetValue(o);
                    EditorGUILayout.LabelField(string.Format("{0}:{1}", config.Des, Editor_TableTool.Join(value)));
                }
            }
            else if (config.AttributeType == "SelectAttribute")
            {
                List<double> value = (List<double>) f.GetValue(o);
                EditorGUILayout.LabelField(string.Format("{0}:{1}", config.Des, Editor_TableTool.Join(value)));
            }
            else if (config.AttributeType == "VariableListAttribute")
            {
                EditorGUILayout.LabelField(string.Format("{0}:{1}", config.Des, f.GetValue(o)));
            }
            else if (config.AttributeType == "ChildAttribute")
            {
                Type type = f.PropertyType.GetGenericArguments()[0];
                if (type.Equals(typeof(string)))
                {
                    List<string> value = (List<string>) f.GetValue(o);
                    EditorGUILayout.LabelField(string.Format("{0}:{1}", config.Des, Editor_TableTool.Join(value)));
                }
                else if (type.Equals(typeof(double)))
                {
                    List<double> value = (List<double>) f.GetValue(o);
                    EditorGUILayout.LabelField(string.Format("{0}:{1}", config.Des, Editor_TableTool.Join(value)));
                }
            }
        }
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