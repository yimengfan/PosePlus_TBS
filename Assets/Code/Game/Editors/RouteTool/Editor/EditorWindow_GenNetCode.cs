using UnityEditor;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.InteropServices;

public class EditorWindow_GenNetCode : EditorWindow
{


    private bool hasDone = false;
    private bool hasSelect = false;
    private string routeFile = "";
    private string path = "";
    private void OnGUI()
    {
        //OnGUI_GenNetWindow();
        OnMainWindow();
        if(hasSelect)
        {
            OnGUI_GenNetWindow();
        }
    }
    private void OnMainWindow()
    {
        if (GUILayout.Button("选择Route文件"))
        {
            OpenFileName openFileName = new OpenFileName();
            openFileName.structSize = Marshal.SizeOf(openFileName);
            openFileName.filter = "Json文件(*.json)\0*.json";
            openFileName.file = new string(new char[256]);
            openFileName.maxFile = openFileName.file.Length;
            openFileName.fileTitle = new string(new char[64]);
            openFileName.maxFileTitle = openFileName.fileTitle.Length;
            openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
            openFileName.title = "选择Route文件";
            openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

            if (LocalDialog.GetSaveFileName(openFileName))
            {
                //Debug.Log(openFileName.file);
                //Debug.Log(openFileName.fileTitle);
                var files = openFileName.file;
                if (File.Exists(files))
                {
                    routeFile = File.ReadAllText(openFileName.file);
                }
                hasSelect = true;
            }
            DeleteFile();
        }
    }

    Vector2 pos = Vector2.zero;
    List<string> list = new List<string>();
    string type = "client";
    bool isClient = true;
    private void OnGUI_GenNetWindow()
    {
        isClient = EditorGUILayout.Toggle("是否生成Client:", isClient);

        if (isClient)
        {
            type = "client";
        }
        else if (!isClient)
        {
            type = "server";
        }
        //一键全部生成
        GUI.skin.button.fixedWidth = 200;
        if (GUILayout.Button("一键生成所有Class"))
        {
            DeleteFileByURL(Application.dataPath + "/Code/Game/Protocol/RouteClass");
            DeleteFileByURL(Application.dataPath + "/Code/Game/Protocol/NetHelper");
            var dic = RouteJsonTool.StringToDic(true, routeFile);
            if (dic != null)
            {
                this.ShowNotification(new GUIContent("Succesed!"));
                CopyFile();
                AssetDatabase.Refresh();
            }
            else
            {
                this.ShowNotification(new GUIContent("Failed!"));
            }
        }
        pos = GUILayout.BeginScrollView(pos, GUILayout.Width(800));        
        GUILayout.BeginVertical();       
        if (!hasDone)
        {
            list = RouteJsonTool.StringToDic(false, routeFile);
            //将list按顺序排列
            //把未生成类的key给标红
            //TODO
            list.Sort();
            hasDone = true;         
        }
        foreach (var key in list)
        {
            //根据NetHelper中的文件来确定是否已经生成对应的class
            path = Application.dataPath + "/Code/Game/Protocol";
            if (key.Contains('.'))
            {
                var arr = key.Split('.');
                var file_Test = path + "/NetHelper/" + "NetHelper_" + arr[1] + ".cs";
                if(File.Exists(file_Test))
                {
                    if(File.ReadAllText(file_Test).Contains(arr[2]))
                    {
                        DrawOneProtocal(key, Color.green);
                    }
                    else
                    {
                        DrawOneProtocal(key, Color.red);
                    }
                }
                else
                {
                    DrawOneProtocal(key, Color.red);
                }
            }
            else
            {
                var file_Test = path + "/NetHelper/" + "NetHelper_" + key + ".cs";
                if(File.Exists(file_Test))
                {
                    DrawOneProtocal(key, Color.green);
                }
                else
                {
                    DrawOneProtocal(key, Color.red);
                }
            }
            
            
            
            
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();       
    }
    
    private void DrawOneProtocal(string key,Color color)
    {
        GUILayout.BeginHorizontal();
        GUI.skin.label.fixedWidth = 350f;
        GUI.color = color;
        if(color == Color.red)
        {
            GUILayout.Label(key + "   (miss)");
        }
        else
        {
            GUILayout.Label(key);
        }
        
        Layout_DrawSeparatorV(Color.gray);
        GUILayout.Space(170);
        
        GUI.skin.button.fixedWidth = 100;
        var btn = GUILayout.Button("覆盖");       
        if (btn)
        {
            //TODO
            if(color == Color.red)
            {
                this.ShowNotification(new GUIContent("Without File"));
            }
            else
            {
                if (RouteJsonTool.DicToOther_Editor(key, type))
                {
                    this.ShowNotification(new GUIContent("Succesed!"));
                    CopyFile();
                }
                else
                {
                    this.ShowNotification(new GUIContent("Failed!"));
                }
            }
            AssetDatabase.Refresh();
        }
        GUILayout.Space(50);
        if (GUILayout.Button("生成"))
        {
            if(color == Color.red)
            {
                if (RouteJsonTool.DicToOther_Editor(key, type))
                {
                    this.ShowNotification(new GUIContent("Succesed!"));
                    CopyFile();
                }
                else
                {
                    this.ShowNotification(new GUIContent("Failed!"));
                }
            }
            else
            {
                this.ShowNotification(new GUIContent("Already Producted"));
            }
                       
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();       
        Layout_DrawSeparator(Color.gray);
    }

    public static void Layout_DrawSeparator(Color color, float height = 1.5f)
    {
        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(height);
    }
    public static void Layout_DrawSeparatorV(Color color, float width = 1.5f)
    {
        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = color;
        GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, width, rect.height), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUILayout.Space(width);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    public class LocalDialog
    {
        //链接指定系统函数       打开文件对话框
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
        public static bool GetOFN([In, Out] OpenFileName ofn)
        {
            return GetOpenFileName(ofn);
        }

        //链接指定系统函数        另存为对话框
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
        public static bool GetSFN([In, Out] OpenFileName ofn)
        {
            return GetSaveFileName(ofn);
        }
    }

    //将文件复制到目标文件夹
    private void CopyFile()
    {
        DirectoryInfo dir = new DirectoryInfo("D:/TestClass/");
        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
        foreach (FileSystemInfo i in fileinfo)
        {
            if (i.FullName.Contains("NetHelper") || i.FullName.Contains("Request") || i.FullName.Contains("Response") || i.FullName.Contains("On"))
            {
                if (i.FullName.Contains("NetHelper"))
                {
                    File.Copy(i.FullName, Application.dataPath + "/Code/Game/Protocol/NetHelper" + "\\" + i.Name, true);
                }
                else
                {
                    File.Copy(i.FullName, Application.dataPath + "/Code/Game/Protocol/RouteClass" + "\\" + i.Name, true);
                }
                File.Delete(i.FullName);
            }
            
        }
    }
    //删除中转TestClass文件夹下面的所有文件
    private void DeleteFile()
    {
        DirectoryInfo dir = new DirectoryInfo("D:/TestClass/");
        if(!dir.Exists)
        {
            dir.Create();
        }
        FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
        foreach (FileSystemInfo i in fileinfo)
        {
            File.Delete(i.FullName);
        }
    }
    //删除指定文件夹下的所有文件
    private void DeleteFileByURL(string url)
    {
        DirectoryInfo dir_01 = new DirectoryInfo(url);
        if (dir_01.Exists)
        {
            FileSystemInfo[] fileinfo = dir_01.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                File.Delete(i.FullName);
            }
        }
    }
}
