using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.PosePlus
{
    /// <summary>
    /// 每一帧的数据
    /// </summary>
    [Serializable]
    public class Frame : ICloneable
    {
        [SerializeField]
        public int fid;
        [SerializeField]
        public bool key;
        [SerializeField]
        public bool box_key;
        [SerializeField]
        public bool dot_key;
        [SerializeField]
        public float lerp;
        public Frame()
        {

        }
        public Frame(Frame last, int _fid, IList<Transform> trans)
        {
            //Debug.LogWarning("bones=" + trans.Length);

            this.fid = _fid;
            this.key = true;
            //bonesinfo = new PoseBoneMatrix[trans.Count];
            for (int i = 0; i < trans.Count; i++)
            {
              PoseBoneMatrix  b = new PoseBoneMatrix();

                bonesinfo.Add(b);
                bonesinfo[i].Record(trans[i], last == null ? null : last.bonesinfo[i]);

            }

        }
        public void LinkLoop(Frame last)
        {
            for (int i = 0; i < bonesinfo.Count; i++)
            {
                bonesinfo[i].Tag(last.bonesinfo[i]);
            }
        }
        /// <summary>
        /// 骨骼
        /// </summary>
        [SerializeField]
        public List<PoseBoneMatrix> bonesinfo = new List<PoseBoneMatrix>();
        
        /// <summary>
        /// 事件
        /// </summary>
        [SerializeField]
        public List<EventData> eventsinfo = new List<EventData>();
        
        /// <summary>
        /// 碰撞盒
        /// </summary>
        [SerializeField]
        public List<AniBoxCollider> boxesinfo = new List<AniBoxCollider>();
        /// <summary>
        /// 特殊点
        /// </summary>
        [SerializeField]
        public List<Dot> dotesinfo = new List<Dot>();
        /// <summary>
        /// 特效
        /// </summary>
        [SerializeField]
        public   List<Effect> effectList = new List<Effect>();
        /// <summary>
        /// 音效
        /// </summary>
        [SerializeField]
        public   List<String> aduioList = new List<string>();

        public object Clone()
        {
            Frame fnew = new Frame();
            fnew.fid = this.fid;
            fnew.key = this.key;
            fnew.bonesinfo = new List<PoseBoneMatrix>(bonesinfo);
            return fnew;
        }

        public static Frame Lerp(Frame left, Frame right, float lerp)
        {
            Frame f = new Frame();
            f.key = false;
            f.fid = left.fid;
            //f.bonesinfo = new PoseBoneMatrix[left.bonesinfo.Length];
            f.bonesinfo = new List<PoseBoneMatrix>(left.bonesinfo);
            for (int i = 0; i < f.bonesinfo.Count; i++)
            {
                f.bonesinfo[i] = PoseBoneMatrix.Lerp(left.bonesinfo[i], right.bonesinfo[i], lerp);
            }
            return f;

        }

    }
  //点操作模式
    [Serializable]
    public class Dot
    {
        [SerializeField]
        public string name ="noname";
        [SerializeField]
        public Vector3 position =Vector3.zero;


        public static Dot Lerp(Dot left, Dot right, float lerp)
        {
            Dot m = new Dot();
            m.name = right.name;
            m.position = Vector3.Lerp(left.position, right.position, lerp);
            return m;
        }
    }
    //特效
    [Serializable]
    public class Effect
    {
        [SerializeField]
        public string name = "noname";
        [SerializeField]
        public Vector3 position = Vector3.zero;
        [SerializeField]
        public string follow = "noObj";
        [SerializeField]
        public int lifeframe = 0;
        [SerializeField]
        public bool isFollow = false;
    }
     //事件
    [Serializable]
    public class EventData
    {
        [SerializeField]
        public string Name = "event";

        [SerializeField]
        public List<object> Params =  new List<object>();
    }

}
