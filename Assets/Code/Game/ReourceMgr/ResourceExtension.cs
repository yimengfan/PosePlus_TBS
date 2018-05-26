using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BDFramework.ResourceMgr;
using UnityEngine;
namespace Game.ReourceMgr
{
   static  public class AniResource
   {
        private static Dictionary<int, Transform> effectMap = new Dictionary<int, Transform>();
        private static int effectCount = 1;
        static public void PlayEffect( string name, Vector3 pos, int dir)
        {
            
        }

        static public void PlayEffect(string name, Transform follow, Vector3 pos, bool isfollow,int dir)
        {
            
        }

        static public int PlayEffectLooped( string name, Vector3 pos, int dir = -1, Transform follow =null)
        {
            effectCount++;
            var o = GameObject.Instantiate(BResources.Load<GameObject>(name));
            effectMap[effectCount] = o.transform;
            if (follow != null)
            {
                o.transform.SetParent(follow, false);
                o.transform.localPosition =Vector3.zero;
            }

            return effectCount;
        }

        static public void CloseEffectLooped(int effid)
        {
            Transform t = null;
            effectMap.TryGetValue(effid, out t);
            if (t != null)
            {
                BResources.Destroy(t);
                effectMap.Remove(effid);
            }
        }

        static public void PlaySoundOnce(string name)
        {
            AudioMgr.Inst.PlayOneShot(name);
        }

        static public void CleanAllEffect()
        {
            
        }
    }
}