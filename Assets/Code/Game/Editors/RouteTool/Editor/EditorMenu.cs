using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Game.Editor
{

    static public class EditorMenu
    {

        [MenuItem("BDFrameWork工具箱/网络协议/生成代码", false, 0)]
        static void ExecGenNetCode()
        {
            var window =
              (EditorWindow_GenNetCode)EditorWindow.GetWindow(typeof(EditorWindow_GenNetCode), false, "网络协议生成");
            window.Show();
        }
    }
}
