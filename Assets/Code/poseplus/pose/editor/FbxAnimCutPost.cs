using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class FbxAnimCutPost : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        Debug.Log("处理模型");
        Object selection = Selection.activeObject;
        if (Path.GetExtension(assetPath).ToLower() == ".fbx" && assetPath.Contains("@"))
        {

            string ClipText = Path.ChangeExtension(assetPath, ".txt");
            List<ModelImporterClipAnimation> list = new List<ModelImporterClipAnimation>();
            var lines = File.ReadAllLines(ClipText);
            foreach (string file in  lines)
            {
				ModelImporterClipAnimation node = ParseAnimFile(file);
				if(node!=null) list.Add(node);
            }

            ModelImporter modelImporter = assetImporter as ModelImporter;
            modelImporter.clipAnimations = list.ToArray();
  
        }
    }

    static ModelImporterClipAnimation ParseAnimFile(string lineStr )
    {
        if(string.IsNullOrEmpty(lineStr))  return null;
        
        lineStr = lineStr.Replace(" ", "").Replace("：", ":");
       // Debug.Log(lineStr);
        try
        {
            string[] slashArr = lineStr.Split('/', '/');
            string[] colonArr = slashArr[0].Split(':');
            string[] hArr = colonArr[1].Split('-');
//        Match match = regexString.Match(sAnimList);  
            ModelImporterClipAnimation clip = new ModelImporterClipAnimation();
            clip.firstFrame = float.Parse(hArr[0]);
            clip.lastFrame = float.Parse(hArr[1]);  
            clip.name = colonArr[0];
            return clip;
        }
        catch (Exception e)
        {
            Debug.LogError("该行数据出错,请检查中英文标点等:" + lineStr);
			return null;
        }
      
    }
}