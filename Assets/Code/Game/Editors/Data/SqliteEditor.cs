
using System;
using Game.Data;
using UnityEditor;
namespace Code.Game.Editors.Data
{
   static public class SqliteEditor
    {


        [MenuItem("技宇工具箱/数据编辑/创建本地数据库")]
        static public void CreateDB()
        {
            try
            {
                SqliteHelper.CreateDBInAsset("LocalDB");
                EditorUtility.DisplayDialog("提示", "创建成功", "确定");
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("提示", "创建失败", "确定");
            }
          
        }
        
    }
}