using System.Collections;
using System.Collections.Generic;
using FB.PosePlus;
using Game.Battle.Skill;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[CustomEditor(typeof(SkillPlayer_TBS))]
public class Inspector_SkillTurnBase : Editor
{
    public SkillPlayer_TBS SkillsPlayer = null;
    private AniPlayer ani;
    private Window_SkillTurnBase window;
     
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(SkillsPlayer == null)
        this.SkillsPlayer =  this.target as SkillPlayer_TBS;
        if (ani == null)
            ani = this.SkillsPlayer.transform.GetComponent<AniPlayer>();
        if (this.SkillsPlayer.GetComponent<AniPlayer>() == null)
        {
            EditorGUILayout.HelpBox("请添加Ani Player", MessageType.Info);
            return;
        }

        
       
        if (SkillsPlayer.Skills == null)
        {

            if (GUILayout.Button("创建.skill数据"))
            {
                var _skill = ScriptableObject.CreateInstance<Skills>();
                  
                var cp =AssetDatabase.GetAssetPath(ani.clips[0].GetInstanceID());

                string path = System.IO.Path.GetDirectoryName(cp);
                Debug.Log(path);
                string outpath = path + "/" + SkillsPlayer.name + ".Skills.asset";
                AssetDatabase.CreateAsset(_skill, outpath);
                var src = AssetDatabase.LoadAssetAtPath(outpath, typeof(Skills)) as Skills;

                this.SkillsPlayer.Skills = src;
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            if (GUILayout.Button("打开"))
            {
                this.window = (Window_SkillTurnBase)EditorWindow.GetWindow(typeof(Window_SkillTurnBase), false, "回合制技能编辑");
                window.Show(this.ani, this.SkillsPlayer.Skills);
                
            }
        }
    }
}
