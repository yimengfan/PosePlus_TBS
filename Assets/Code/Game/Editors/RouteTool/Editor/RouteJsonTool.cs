using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using MyJson;
using Code.Core.BDFramework.SimpleGenCSharpCode;
using System.Reflection;
using UnityEditor;

public class RouteJsonTool : MonoBehaviour
{
    public static RouteJsonTool _instance;
    static int clientNetHelpeCount = 0;
    static int serverNetHelpeCount = 0;
    private static string path;
    private void Start()
    {
        //GetAllJosnData();
    }

    private static void GetAllJosnData()
    {        
        //var json = ReadJson("route").Replace("int32","int");
        //StringToDic(json);
        //读出字典中的数据
    }
    //解析字典
    

    //获取text文件
    private static string ReadJson(string path)
    {
        return Resources.Load<TextAsset>("route").text;
    }


    //将字符串转化为字典
#region 弃用
    private string StringDetails { get; set; }
    //private Dictionary<string, object> StringDetailList
    //{
    //    get
    //    {
    //        if (string.IsNullOrWhiteSpace(StringDetails))
    //        {
    //            return new Dictionary<string, object>();
    //        }
    //        try
    //        {
    //            var obj = JToken.Parse(StringDetails);
    //        }
    //        catch (Exception)
    //        {
    //            throw new FormatException("ProductDetails不符合json格式.");
    //        }
    //        return JsonConvert.DeserializeObject<Dictionary<string, object>>(StringDetails);
    //    }
    //}
    //private void ParsingDic(IDictionary<string, object> data)
    //{
    //    if (data.ContainsKey("protos"))
    //    {
    //        StringDetails = data["protos"].ToString();
    //        var protos = StringDetailList;

    //        //解析客户端的协议
    //        if (protos.ContainsKey("client"))
    //        {

    //            StringDetails = protos["client"].ToString();
    //            var client = StringDetailList;
    //            //Debug.Log(client.Count);
    //            //读取客户端的协议并且生成响应的类
    //            foreach (var value in client)
    //            {
    //                var gate = value.ToString();
    //                //Debug.Log(gate);
    //            }
    //        }
    //        //解析服务端的协议
    //        if (protos.ContainsKey("server"))
    //        {
    //            StringDetails = protos["server"].ToString();
    //            var server = StringDetailList;
    //            //Debug.Log(server.Count);
    //            //读取服务端的协议并且生成响应的类

    //        }

    //    }
    //}
    static List<MyClass> myClassList_Client = new List<MyClass>();
    static List<MyClass> myClassList_Server = new List<MyClass>();

    //private static void ProductMethodClass_All(MyClass mClass, string gate, List<string> strList, string IclassName)
    //{

    //    if (IclassName == "Request_")
    //    {
    //        var className = "NetHelper_Client_";
    //        var arr = gate.Split('.');
    //        className = className + arr[1];
    //        var myClass = myClassList_Client.Find((c) => c.Name == className);
    //        if (myClass == null)
    //        {
    //            MyClass mc = new MyClass(className);
    //            mc.AddNameSpace("System");
    //            mc.AddNameSpace("MyJson");
    //            mc.AddNameSpace("System.Linq");
    //            myClassList_Client.Add(mc);

    //            //用来添加方法体
    //            var m = CreateMethod_Client(mClass, gate, strList);
    //            mc.AddMethod(m);
    //        }
    //        else if (myClass != null)
    //        {
    //            //如果已经创建过这个class就可以直接在其内部添加方法
    //            //Debug.Log(myClass.ToString());
    //            var m = CreateMethod_Client(mClass, gate, strList);
    //            myClass.AddMethod(m);
    //        }
    //        if (myClassList_Client.Count == clientNetHelpeCount)
    //        {
    //            foreach (var mclass in myClassList_Client)
    //            {
    //                var code = mclass.ToString();

    //                var file = "D:/TestClass/" + mclass.Name + ".cs";
    //                if (File.Exists(file))
    //                {
    //                    File.Delete(file);
    //                }
    //                File.WriteAllText("D:/TestClass/" + mclass.Name + ".cs", code);
    //            }
    //        }
    //    }
    //    else if (IclassName == "Response_")
    //    {
    //        var className = "NetHelper_Server_";
    //        if (gate.Contains('.'))
    //        {
    //            return;
    //        }
    //        else
    //        {
    //            className = className + gate;
    //        }
    //        var myClass = myClassList_Server.Find((c) => c.Name == className);
    //        if (myClass == null)
    //        {
    //            MyClass mc = new MyClass(className);
    //            mc.AddNameSpace("System");
    //            mc.AddNameSpace("MyJson");
    //            mc.AddNameSpace("System.Linq");
    //            myClassList_Server.Add(mc);

    //            //用来添加方法体
    //            var m = CreateMethod_Server(mClass, gate, strList);
    //            mc.AddMethod(m);
    //        }
    //        else if (myClass != null)
    //        {
    //            //如果已经创建过这个class就可以直接在其内部添加方法
    //            //Debug.Log(myClass.ToString());
    //            var m = CreateMethod_Server(mClass, gate, strList);
    //            myClass.AddMethod(m);
    //        }
    //        if (myClassList_Server.Count == serverNetHelpeCount)
    //        {
    //            foreach (var mclass in myClassList_Server)
    //            {
    //                var code = mclass.ToString();

    //                var file = "D:/TestClass/" + mclass.Name + ".cs";
    //                if (File.Exists(file))
    //                {
    //                    File.Delete(file);
    //                }
    //                File.WriteAllText("D:/TestClass/" + mclass.Name + ".cs", code);
    //            }
    //        }
    //    }
    //}
    #endregion
    //缓存字典信息
    static IDictionary<string, IJsonNode> dic_Client = new Dictionary<string, IJsonNode>();
    static IDictionary<string, IJsonNode> dic_Server = new Dictionary<string, IJsonNode>();

    public static List<string>  StringToDic(bool isCreateAll,string str)
    {
        path = Application.dataPath + "/Code/Game/Protocol";
        //var str = Resources.Load<TextAsset>("Route/route").text;
        var json = str.Replace("int32", "int");
        var inode = MyJson.MyJson.Parse(json);       
        dic_Client = inode.asDict()["protos"].asDict()["client"].asDict();
        dic_Server = inode.asDict()["protos"].asDict()["server"].asDict();
        var keys_Client = dic_Client.Keys.ToList();
        var keys_Server = dic_Server.Keys.ToList();
        List<string> ikeys_Client = new List<string>();
        List<string> ikeys_Server = new List<string>();
        List<string> reList = new List<string>();
        //清空静态列表和变量
        if(isCreateAll)
        {
            clientNetHelpeCount = 0;
            serverNetHelpeCount = 0;
            myClassList_Client.Clear();
            myClassList_Server.Clear();
        }
        
        foreach (var key in keys_Client)
        {
            var arr = key.Split('.');
            var ikey = ikeys_Client.Find((k) => k == arr[1]);
            if (ikey == null)
            {
                ikeys_Client.Add(arr[1]);
                clientNetHelpeCount++;
            }

            var reKey = reList.Find((k) => k == key);
            if(reKey == null)
            {
                reList.Add(key);
            }
        }
        foreach (var key in keys_Server)
        {
            if(key.Contains('.'))
            {
                //var arr = key.Split('.');
                //var ikey = ikeys_Server.Find((k) => k == arr[1]);
                //if (ikey == null)
                //{
                //    ikeys_Server.Add(arr[1]);
                //    serverNetHelpeCount++;
                //}
            }
            else{
                var ikey = ikeys_Server.Find((k) => k == key);
                if(ikey == null)
                {
                    ikeys_Server.Add(ikey);
                    serverNetHelpeCount++;
                }
            }

            var reKey = reList.Find((k) => k == key);
            if (reKey == null)
            {
                reList.Add(key);
            }
        }

        if(isCreateAll)
        {
            DicToOther(keys_Client, dic_Client, "Request_");
            DicToOther(keys_Server, dic_Server, "Response_");
        }
        

        return reList;
    }

    private static void DicToOther(List<string> keys,IDictionary<string,IJsonNode> dic,string IclassName)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            var iJsonNode = dic[keys[i]].asDict();
            DicToList(keys[i], iJsonNode, IclassName);            
        }
    }

    public static bool DicToOther_Editor(string key,string type)
    {
        bool isSuccese = false;
        IDictionary<string, IJsonNode> dic = new Dictionary<string, IJsonNode>();
        string IclassName = "";
        if (type == "client")
        {
            dic = dic_Client;
            IclassName = "Request_";
        }
        else
        {
            dic = dic_Server;
            IclassName = "Response_";
        }
        foreach(var iKey in dic.Keys)
        {
            if(key == iKey)
            {
                var iJsonNode = dic[iKey].asDict();
                isSuccese = DicToList(iKey, iJsonNode, IclassName);
            }
        }
        return isSuccese;
    }


    //字典转list
    private static bool DicToList(string key, IDictionary<string, IJsonNode> iJsonNode, string IclassName)
    {
        //ListStr.Add(key);
        //有message的情况
        bool isSuccese = false;
        if (iJsonNode.Keys.Contains("message"))
        {
            //需要再解析一层
            //TODO
            List<string> Returnlist_Str = new List<string>();
            List<Array> ListArr = new List<Array>();
            var messageDic = iJsonNode["message"].asDict();
            var messageKey = messageDic.Keys.ToList();
            ListArr.Add(CuttingStr(key));
            for(int i = 0;i < messageDic.Count;i++)
            {
                var dic = messageDic[messageKey[i]].asDict();
                var list = dic.Keys.ToList();
                string[] Arr = new string[list.Count + 1];
                Arr[0] = messageKey[i];
                for (int n = 0; n < list.Count; n++)
                {
                    //Debug.Log(list[n]);
                    //string[] strArr = list[n].Split(' ');
                    var strArr = CuttingStr(list[n]);
                    Arr[n + 1] = strArr[1] + " " + strArr[2];

                }
                ListArr.Add(Arr);
            }
            
            foreach (var k in iJsonNode.Keys)
            {
                if (k != "message")
                {
                    string[] strArr = k.Split(' ');
                    Returnlist_Str.Add(strArr[1] + " " + strArr[2]);
                }
            }
            isSuccese = true;
            ProductClass_Message(ListArr,Returnlist_Str, IclassName,key);

        }
        else
        {
            List<string> ListStr = new List<string>();
            ListStr.Add(key);
            if(iJsonNode.Count > 0)
            {
                foreach (var ikey in iJsonNode)
                {
                    ListStr.Add(ikey.Key.ToString());
                }
                isSuccese = true;
                
            }
            ProductClass_NoMessage(CuttingString(ListStr), IclassName, key);

        }
        return isSuccese;
    }
    //将list中的字符串切割的规则
    private static List<string> CuttingString(List<string> ListStr)
    {
        List<string> Returnlist_Str = new List<string>();
        //协议头作为list[0]
        if (ListStr[0].Contains('.'))
        {           
            var ArrStr = ListStr[0].Split('.');
            Returnlist_Str.Add(ArrStr[2]);
        }
        else
        {
            Returnlist_Str.Add(ListStr[0]);
        }

        //如果传来的ListStr的数量大于一，说明有参数
        if (ListStr.Count > 1)
        {
            for (int i = 1; i < ListStr.Count; i++)
            {
                var ArrStr = ListStr[i].Split(' ');
                Returnlist_Str.Add(ArrStr[1] + " " + ArrStr[2]);
            }
        }
        return Returnlist_Str;
    }

    //字符串切割规则
    private static string[] CuttingStr(string str)
    {
        string[] reString = null;

        //协议头作为list[0]
        if (str.Contains('.'))
        {
            var ArrStr = str.Split('.');
            reString = ArrStr;
        }
        else if(str.Contains(' '))
        {
            var ArrStr = str.Split(' ');
            string[] arrStr = new string[1];
            reString = ArrStr;
        }
        else
        {
            string[] ArrStr = new string[1];
            ArrStr[0] = str;
            reString = ArrStr; 
        }

        return reString;
    }

    //创建类_Client
    private static void ProductClass_NoMessage(List<string> strList, string IclassName,string gate)
    {
        
        var className = "";
        var gateClassName = "";
        var result = "";
        string gateFile = "";
        if (gate.Contains('.'))
        {
            var gateArr = gate.Split('.');
            className = /*IclassName + gateArr[1] + "_" + */strList[0];
            gateClassName = IclassName + gateArr[1];
            gateFile = "D:/TestClass/" + gateClassName + ".cs";
            if (!File.Exists(gateFile))
            {
                MyClass gateClass = new MyClass(gateClassName);
                gateClass.AddNameSpace("System");
                result = gateClass.ToString();
            }
            else
            {
                
                result = File.ReadAllText(gateFile).ToString();
            }
        }
        else
        {
            className = IclassName + strList[0];
            gateClassName = gate;
            gateFile = "D:/TestClass/"+ IclassName + gateClassName + ".cs";
        }
        //if(strList.Count)
        
        //c.AddNameSpace("System");
        //添加field
        if (strList.Count > 1)
        {
            MyClass c = new MyClass(className);
            for (int i = 1; i < strList.Count; i++)
            {
                //先判断类型
                var f = new MyField();
                string field;
                if (i == 1)
                {
                    field = string.Format("public {0};", strList[i]);
                }
                else
                {
                    field = string.Format("public {0};", strList[i]);
                }
                f.SetContent(field);
                c.AddField(f);
            }
            var code = c.ToString();
            if (gate.Contains('.'))
            {
                result = result.Insert(result.LastIndexOf('}'), code);

            }
            else
            {
                //Debug.Log(className);
                result = code;
            }
        }
        
        
        //result = result.Insert(result.LastIndexOf('}'),code);
        var dir = Path.GetDirectoryName(path + "/RouteClass/");
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }

        var files = path + "/RouteClass/" + className + ".cs";
        if (File.Exists(files))
        {
            File.Delete(files);
        }

        //ProductMethodClass(c, gate, strList,IclassName);
        ProductMethodClass_Now(className, gate, strList, IclassName);
        File.WriteAllText(gateFile, result);
    }
    
    private static void ProductClass_Message(List<Array> listArr,List<string> listStr,string IclassName,string gate)
    {
        var classNameArr = listArr[0];
        var className = classNameArr.GetValue(classNameArr.Length - 1);
        string gateClassName = "";
        string result = "";
        string gateFile = "";
        if (gate.Contains('.'))
        {
            var gateArr = gate.Split('.');
            //className = /*IclassName + gateArr[1] + "_" + */className;
            gateClassName = IclassName + gateArr[1];
            gateFile = "D:/TestClass/" + gateClassName + ".cs";

            if (!File.Exists(gateFile))
            {
                MyClass gateClass = new MyClass(gateClassName);
                gateClass.AddNameSpace("System");
                result = gateClass.ToString();
            }
            else
            {
                result = File.ReadAllText(gateFile).ToString();
                gateFile = "D:/TestClass/" + gateClassName + ".cs";
            }
        }
        else
        {
            //className = IclassName + className;
            gateClassName = gate;
            gateFile = "D:/TestClass/" + IclassName + gateClassName + ".cs";
        }
        MyClass c = new MyClass(className.ToString());
        var subClassName = "";
        for (int i = 1;i < listArr.Count;i++)
        {
            var arr = listArr[i];
            subClassName = arr.GetValue(0).ToString();
            MyClass subClass = new MyClass(subClassName);

            for (int n = 1; n < arr.Length; n++)
            {

                var f = new MyField();
                //if(n == 1)
                //{
                //    field = string.Format("public {0};", arr.GetValue(n));
                //}
                //else
                //{
                //    field = string.Format("public {0};", arr.GetValue(n));
                //}
                string field = string.Format("public {0};", arr.GetValue(n));
                f.SetContent(field);
                subClass.AddField(f);

            }
            c.AddClass(subClass);
        }
        //添加field
        if (listStr.Count > 1)
        {
            for (int i = 0; i < listStr.Count; i++)
            {
                //先判断类型
                var f = new MyField();
                string field = "";
                var arrField = listStr[i].Split(' ');
                if(arrField[0] == subClassName)
                {
                    var str = listStr[i].Insert(listStr[i].LastIndexOf(' '), "[]");
                    field = string.Format("public {0};", str);
                }
                else
                {
                    field = string.Format("public {0};", listStr[i]);
                }
                //if (i == 0)
                //{
                //    field = string.Format("public {0};", listStr[i]);
                //}
                //else
                //{
                //    field = string.Format("public {0};", listStr[i]);
                //}
                f.SetContent(field);
                c.AddField(f);
            }
        }
        var code = c.ToString();
        if(gate.Contains('.'))
        {
            result = result.Insert(result.LastIndexOf('}'), code);
        }
        else
        {
            result = code;
        }
        var files = path + "/RouteClass/Response_" + c.Name + ".cs";
        if(File.Exists(files))
        {
            File.Delete(files);
        }
        File.WriteAllText(gateFile, result);
        //AssetDatabase.Refresh();
        //ProductMethodClass_All(c, gate, listStr, IclassName);
        ProductMethodClass_Now(className.ToString(), gate, listStr, IclassName);
    }

    //创建类方法
    //MyClassList
    
    private static void ProductMethodClass_Now(string subClassName, string gate, List<string> strList, string IclassName)
    {
        string className = "NetHelper_";
        if (IclassName == "Request_")
        {            
            var arr = gate.Split('.');
            className = className + arr[1];            
        }
        else if (IclassName == "Response_")
        {
            if (gate.Contains('.'))
            {
                return;
            }
            else
            {
                className = className + gate;
            }           
        }
        MyClass mc = new MyClass(className);
        //var file = path + "/RouteClass" + mc.Name + ".cs";
        var dir_Test = "D:/TestClass/";
        if (Directory.Exists(dir_Test) == false)
        {
            Directory.CreateDirectory(dir_Test);
        }
        var file = "D:/TestClass/" + mc.Name + ".cs";
        if (!File.Exists(file))
        {
            mc.AddNameSpace("System");
            mc.AddNameSpace("MyJson");
            mc.AddNameSpace("System.Linq");
        }
        //用来添加方法体
        MyMethod m = null;
        MyMethod m_Remove = null;
        if(IclassName == "Request_")
        {
            m = CreateMethod_Client(IclassName, gate, strList);
        }
        else
        {
            m = CreateMethod_Server(subClassName, gate, strList,false);
            m_Remove = CreateMethod_Server(subClassName, gate, strList,true);
        }        
        mc.AddMethod(m);
        if(m_Remove != null)
        {
            mc.AddMethod(m_Remove);
        }
        
        var code = mc.ToString();
        string result = "";
        if (File.Exists(file))
        {
            //File.Delete(file);
            //将当前类插入到最后一个“}”之前
            var str = File.ReadAllText(file).ToString();
            code = m.ToString();
            if(str.Contains(code))
            {
                result = str;
            }
            else
            {
                result = str.Insert(str.LastIndexOf('}'), code);
            }
            
        }
        else
        {
            mc.AddMethod(m);
            result = code;
        }
        var dir = Path.GetDirectoryName(path + "/NetHelper/");
        if(Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        
        //File.WriteAllText(path + "/NetHelper/" + mc.Name + ".cs", result);
        File.WriteAllText("D:/TestClass/" + mc.Name + ".cs", result);
    }

    private static MyMethod CreateMethod_Client(string IclassName, string gate, List<string> strList)
    {
        var m = new MyMethod();
        string test = @"""";
        string test_01 = "{";
        string test_02 = "        }";
        string param = "";
        if(strList.Count > 1)
        {
            param = IclassName + gate.Replace("gate.", "") + " " + strList[0] + ",";
        }
        
        string param_Action = string.Format("Action<object> callback");
        m.SetMethSign(null, strList[0], param + param_Action);
        string content = "var json = new JsonNode_Object();\r\n";
        for(int i = 1;i < strList.Count;i++)
        {
            var strArr = strList[i].Split(' ');
            content = content + StrRule(strArr,strList[0]);
        }
        content = content + string.Format("        Network.Inst.Request({0},json , " +
        "callback\r\n);", test + gate + test,test_01,test_02);
        m.SetMethodContent(content);
        return m;
    }
    
    private static string StrRule(string[] strArr,string className)
    {
        string test = @"""";
        string reStr = "";
        string type = "";
        if(strArr[0] == "int")
        {
            type = "new JsonNode_ValueNumber({1})";
        }
        else if (strArr[0] == "string")
        {
            type = "new JsonNode_ValueString({1})";
        }
        else if (strArr[0] == "double")
        {
            type = "new JsonNode_ValueNumber(BMathf.Accuracy4({1}))";
        }
        reStr = string.Format("        json[{0}] = " + type + ";\r\n", test + strArr[1] + test, className + "." + strArr[1]);
        return reStr;
    }
    
    private static MyMethod CreateMethod_Server(string subClassName, string gate, List<string> strList,bool isRemove)
    {
        var m = new MyMethod();
        string test = @"""";
        string test_01 = "{";
        string test_02 = "        }";
        //string param = "Response_" + strList[0] + " " + strList[0];
        string param_Action = string.Format("Action<object> callback");
        string content = "";
        

        var name = "";
        
        if(isRemove)
        {
            name = "Remove_" + subClassName.Replace("Response_", "") ;
            content = content + string.Format("Network.Inst.UnRegister({0}, " +
            "callback\r\n   );", test + gate + test, test_01, test_02);
            m.SetMethodContent(content);
        }
        else
        {
            name = subClassName.Replace("Response_", "");
            content = content + string.Format("Network.Inst.Register({0}, " +
            "callback\r\n   );", test + gate + test, test_01, test_02);
            m.SetMethodContent(content);
        }
        m.SetMethSign(null, name, param_Action);
        return m;
    }
}

