using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

static public class AnimationImpoter 
{
  [MenuItem("Assets/技宇工具箱/导入Animation")]
  static public void Impoter()
  {
      var o = Selection.objects;
      if (o.Length == 1)
      {
          var selectObj = o[0];
          string objPath = AssetDatabase.GetAssetPath(selectObj);
          

          var cs = (selectObj as GameObject).GetComponents<Component>();
          foreach (var c in cs)
          {
              Debug.Log(c.GetType().FullName);
          }
          
          //var ani =  (selectObj as GameObject).GetComponent<importsett>();
          //ani.
          //Debug.Log(objPath);
      }
      else
      {
          EditorUtility.DisplayDialog("提示", "没有选择ani,或者选择多个对象", "确定");
      }
  }
}
