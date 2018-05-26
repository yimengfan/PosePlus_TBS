using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;


namespace FB.PosePlus
{
    
    public interface IPlayAni
    {
        //float aniFps
        //{
        //    get;
        //}
        bool aniLoop
        {
            get;
        }
        int aniFrameCount
        {
            get;
        }
        Frame GetFrame(int frame);

    }

    [Serializable]
    public class SubClip
    {
        [SerializeField]
        public String name = "noname";
        [SerializeField]
        public bool loop;
        [SerializeField]
        public uint startframe;
        [SerializeField]
        public uint endframe;
    }
    public class AniClip : ScriptableObject, IPlayAni
    {
        public List<string> boneinfo = new List<string>();
        public List<Frame> frames = new List<Frame>();

        public List<SubClip> subclips = new List<SubClip>();

        Dictionary<string, int> subclipcache = null;
        public SubClip GetSubClip(string name)
        {
            if (subclips == null || subclips.Count == 0) return null;
            if (subclipcache == null || subclipcache.Count != subclips.Count)
            {
                subclipcache = new Dictionary<string, int>();
                for (int i = 0; i < subclips.Count; i++)
                {
                    subclipcache[subclips[i].name] = i;
                }
            }
            int igot = 0;
            if (subclipcache.TryGetValue(name, out igot))
            {
                return subclips[igot];
            }
            return null;
        }
        public float fps = 24.0f;
        public bool loop;

        [NonSerialized]
        int _bonehash = -1;
        public int bonehash
        {
            get
            {
                if (_bonehash == -1)
                {
                    string name = "";
                    foreach (var s in boneinfo)
                    {
                        name += s + "|";
                    }
                    _bonehash = name.GetHashCode();
                }
                return _bonehash;
            }

        }
        //public float aniFps
        //{
        //    get { return fps; }
        //}

        public bool aniLoop
        {
            get { return loop; }
            set { loop = value; }
        }

        public int aniFrameCount
        {
            get { return frames.Count; }
        }

        public Frame GetFrame(int frame)
        {
            return frames[frame];
        }
        public void CalcLerpFrameOne(int frame)
        {
            //搜索开始与结束帧
            if (frames[frame].key) return;
            if (frame <= 0 || frame >= frames.Count - 1) return;
            int ibegin = frame;
            for (; ibegin >= 0; ibegin--)
            {
                if (frames[ibegin].key)
                {
                    break;
                }
            }
            if (ibegin == frame) return;
            int iend = frame;
            for (; iend < frames.Count; iend++)
            {
                if (frames[iend].key)
                {
                    break;
                }
            }
            if (iend == frame) return;

            int i = frame;
            {
                //float d1 = (i - ibegin);
                //float d2 = (iend - i);
                float lerp = frames[i].lerp;
                frames[i] = Frame.Lerp(frames[ibegin], frames[iend], lerp);
                frames[i].lerp = lerp;
                frames[i].fid = i;
            }
        }
        public void ResetLerpFrameSegment(int frame)
        {
            //搜索开始与结束帧
            if (frames[frame].key) return;
            if (frame <= 0 || frame >= frames.Count - 1) return;
            int ibegin = frame;
            for (; ibegin >= 0; ibegin--)
            {
                if (frames[ibegin].key)
                {
                    break;
                }
            }
            if (ibegin == frame) return;
            int iend = frame;
            for (; iend < frames.Count; iend++)
            {
                if (frames[iend].key)
                {
                    break;
                }
            }
            if (iend == frame) return;
            //找到最近两个关键帧之间 插值
            for (int i = ibegin + 1; i < iend; i++)
            {
                float d1 = (i - ibegin);
                float d2 = (iend - i);
                float lerp = d1 / (d1 + d2);
                frames[i] = Frame.Lerp(frames[ibegin], frames[iend], lerp);
                frames[i].lerp = lerp;
                frames[i].fid = i;
            }
        }
        public void ResetBoxLerpFrameSegment(int frame)
        {
            //搜索开始与结束帧
            if (frames[frame].box_key) return;
            if (frame <= 0 || frame >= frames.Count - 1) return;

            int ibegin = frame;
            for (; ibegin >= 0; ibegin--)
            {
                if (frames[ibegin].box_key)
                {
                    break;
                }
            }
            if (ibegin == frame) return;
            int iend = frame;
            for (; iend < frames.Count; iend++)
            {
                if (frames[iend].box_key)
                {
                    break;
                }
            }
            if (iend == frame) return;
            //建立对位关系
            Dictionary<AniBoxCollider, AniBoxCollider> map = new Dictionary<AniBoxCollider, AniBoxCollider>();
            var leftlist = frames[ibegin].boxesinfo;
            var rightlist = frames[iend].boxesinfo;
            for (int i = 0; i < leftlist.Count; i++)
            {
                //对位
                AniBoxCollider boxcollider = rightlist.Find(b => b.mIndex == leftlist[i].mIndex && b.mBoxType == leftlist[i].mBoxType);
                //
                map.Add(leftlist[i], boxcollider);
            }
            //找到两个Box关键帧之间插值
            for (int i = ibegin + 1; i < iend; i++)
            {
                float d1 = (i - ibegin);
                float d2 = (iend - i);
                float lerp = d1 / (d1 + d2);
                List<AniBoxCollider> colliderList = new List<AniBoxCollider>();
                foreach (var b in map.Keys)
                {
                    if (map[b] == null)
                    {
                        colliderList.Add(b);
                        continue;
                    }
                    else
                    {
                        colliderList.Add(AniBoxCollider.Lerp(b, map[b], lerp));
                    }
                }
                frames[i].boxesinfo = new List<AniBoxCollider>(colliderList);
            }
        }

        public void ResetDotLerpFrameSegment(int frame)
        {
            //搜索开始与结束帧
            if (frames[frame].dot_key) return;
            if (frame <= 0 || frame >= frames.Count - 1) return;

            int ibegin = frame;
            for (; ibegin >= 0; ibegin--)
            {
                if (frames[ibegin].dot_key)
                {
                    break;
                }
            }
            if (ibegin == frame) return;
            int iend = frame;
            for (; iend < frames.Count; iend++)
            {
                if (frames[iend].dot_key)
                {
                    break;
                }
            }
            if (iend == frame) return;
            //建立对位关系
            Dictionary<Dot, Dot> map = new Dictionary<Dot, Dot>();
            var leftlist = frames[ibegin].dotesinfo;
            var rightlist = frames[iend].dotesinfo;
            for (int i = 0; i < leftlist.Count; i++)
            {
                //对位
                Dot dot = rightlist.Find(b => b.name == leftlist[i].name);
                //
                map.Add(leftlist[i], dot);
            }
            //找到两个Box关键帧之间插值
            for (int i = ibegin + 1; i < iend; i++)
            {
                float d1 = (i - ibegin);
                float d2 = (iend - i);
                float lerp = d1 / (d1 + d2);
                List<Dot> colliderList = new List<Dot>();
                foreach (var b in map.Keys)
                {
                    if (map[b] == null)
                    {
                        colliderList.Add(b);
                        continue;
                    }
                    else
                    {
                        colliderList.Add(Dot.Lerp(b, map[b], lerp));
                    }
                }
                frames[i].dotesinfo = new List<Dot>(colliderList);
            }
        }
        public void ResetLerpFrameAll()
        {
            for (int ibegin = 0; ibegin < frames.Count - 1; ibegin++)
            {
                if (frames[ibegin].key)
                {
                    if (frames[ibegin + 1].key)
                    {
                        //下一帧就是关键帧，没法玩，坑了
                        continue;
                    }
                    for (int iend = ibegin + 2; iend < frames.Count; iend++)
                    {
                        if (frames[iend].key)
                        {
                            //发现一个需要计算的
                            //Debug.LogWarning("find need calc:" + ibegin + "-" + iend);
                            for (int i = ibegin + 1; i < iend; i++)
                            {
                                float d1 = (i - ibegin);
                                float d2 = (iend - i);
                                float lerp = d1 / (d1 + d2);
                                frames[i] = Frame.Lerp(frames[ibegin], frames[iend], lerp);
                                frames[i].lerp = lerp;
                                frames[i].fid = i;
                            }
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < frames.Count; i++)
            {
                if (i == 0 && !this.loop) continue;
                int ilast = i - 1;
                if (ilast < 0) ilast = frames.Count - 1;
                frames[i].LinkLoop(frames[ilast]);
            }
        }

        public void MatchBone(AniClip clip)
        {
            Dictionary<int, int> boneconvert = new Dictionary<int, int>();
            for (int i = 0; i < clip.boneinfo.Count; i++)
            {
                bool bhave = false;

                for (int j = 0; j < this.boneinfo.Count; j++)
                {

                    if (this.boneinfo[j] == clip.boneinfo[i])
                    {
                        boneconvert[j] = i;
                        bhave = true;
                    }
                }
                if (bhave == false)
                {//需要增加骨骼了，呵呵了
                    Debug.LogWarning("bone need add:" + clip.boneinfo[i]);
                }
                //if donthave
            }
            for (int i = 0; i < this.boneinfo.Count; i++)
            {
                if (boneconvert.ContainsKey(i) == false)
                {
                    Debug.LogWarning("bone need delete:" + this.boneinfo[i]);
                }
            }
            this.boneinfo = new List<string>(clip.boneinfo);

            foreach (var f in frames)
            {
                List<PoseBoneMatrix> list = new List<PoseBoneMatrix>();
                for (int i = 0; i < this.boneinfo.Count; i++)
                {
                    list.Add(null);
                }
                foreach (var c in boneconvert)
                {
                    list[c.Value] = f.bonesinfo[c.Key];
                }

                f.bonesinfo = list;
            }

        }
        //暂不维护这一丢丢代码
        //public void SaveTo(System.IO.Stream stream)
        //{
        //    byte[] head = new byte[4];
        //    head[0] = 0x2b;//magic code
        //    head[1] = 0x2c;
        //    head[2] = (byte)fps;
        //    head[3] = loop ? (byte)1 : (byte)0;

        //    stream.Write(head, 0, 4);
        //    byte[] buf = BitConverter.GetBytes((ushort)frames.Count);//framecount
        //    stream.Write(buf, 0, 2);
        //    ushort bcount = (ushort)frames[0].bonesinfo.Length;
        //    buf = BitConverter.GetBytes(bcount);//bonecount
        //    stream.Write(buf, 0, 2);
        //    for (int i = 0; i < frames.Count; i++)
        //    {
        //        for (int j = 0; j < bcount; j++)
        //        {
        //            frames[i].bonesinfo[j].Save(stream, i == 0 ? null : frames[i - 1].bonesinfo[j]);
        //        }
        //    }
        //}

        //public void ReadFrom(System.IO.Stream stream)
        //{
        //    byte[] head = new byte[4];
        //    stream.Read(head, 0, 4);
        //    fps = head[2];
        //    loop = head[3] == 1;
        //    byte[] buf = new byte[2];
        //    stream.Read(buf, 0, 2);
        //    ushort framecount = BitConverter.ToUInt16(buf, 0);
        //    stream.Read(buf, 0, 2);
        //    ushort bonecount = BitConverter.ToUInt16(buf, 0);
        //    Debug.Log("framecount=" + framecount);
        //    frames = new List<Frame>();
        //    for (int i = 0; i < framecount; i++)
        //    {
        //        Frame f = new Frame();
        //        frames.Add(f);
        //        f.fid = i;
        //        f.bonesinfo = new PoseBoneMatrix[bonecount];
        //        for (int j = 0; j < bonecount; j++)
        //        {
        //            f.bonesinfo[j] = new PoseBoneMatrix();
        //            f.bonesinfo[j].Load(stream, i == 0 ? null : frames[i - 1].bonesinfo[j]);
        //        }
        //    }
        //}
    }

}
