using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.PosePlus
{

    [Serializable]
    public class PoseBoneMatrix : ICloneable
    {
        [SerializeField]
        public Vector3 t;
        //[SerializeField]
        //public Vector3 r;
        [SerializeField]
        public Vector3 s;

        [SerializeField]
        Quaternion r = Quaternion.identity;


        public enum changetag
        {
            NoChange = 0,
            Trans = 1,
            Rotate = 2,
            Scale = 4,
            TransRotate = 3,
            TransScale = 5,
            RotateScale = 6,
            All = 7,
        }
        [SerializeField]
        public changetag tag;
        public void UpdateTranLerp(Transform trans, PoseBoneMatrix mix, float lerp)
        {
            trans.localScale = Vector3.Lerp(s, mix.s, lerp);
            trans.localPosition = Vector3.Lerp(t, mix.t, lerp);

            if (r.z != 0 && mix.r.z != 0)
                //Debug.Log("have wrong.see it:" + mix.r + ',' + r);
                trans.localRotation = Quaternion.Lerp(r, mix.r, lerp);


        }
        public void UpdateTran(Transform trans, bool bAdd)
        {
            if (!bAdd || tag == changetag.All)
            {
                trans.localScale = s;
                trans.localPosition = t;
                trans.localRotation = r;

                //trans.localRotation = Quaternion.Euler(r);
                return;
            }
            switch (tag)
            {
                case changetag.NoChange:
                    return;
                case changetag.Rotate:
                    trans.localRotation = r;
                    //trans.localRotation = Quaternion.Euler(r);
                    break;
                case changetag.Trans:
                    trans.localPosition = t;
                    break;
                case changetag.Scale:
                    trans.localScale = s;
                    break;
                case changetag.RotateScale:
                    trans.localScale = s;
                    trans.localRotation = r;
                    //trans.localRotation = Quaternion.Euler(r);
                    break;
                case changetag.TransRotate:
                    trans.localPosition = t;
                    trans.localRotation = r;
                    //trans.localRotation = Quaternion.Euler(r);
                    break;
                case changetag.TransScale:
                    trans.localScale = s;
                    trans.localPosition = t;
                    break;
            };

        }
        //unity默认实现的四元数相等精度太低
        static bool QuaternionEqual(Quaternion left, Quaternion right)
        {
            return left.x == right.x && left.y == right.y && left.z == right.z && left.w == right.w;

        }
        public void Record(Transform trans, PoseBoneMatrix last)
        {
            //r = trans.localRotation.eulerAngles;
            t = trans.localPosition;
            r = trans.localRotation;
            s = trans.localScale;
            tag = PoseBoneMatrix.changetag.All;

            if (last == null)
                tag = PoseBoneMatrix.changetag.All;
            else
            {
                tag = PoseBoneMatrix.changetag.NoChange;
                //float decr=Vector3.Angle(bonesinfo[i].r,last.bonesinfo[i].r);
                //Debug.LogWarning("decr="+decr);
                if (!QuaternionEqual(r, last.r))
                {
                    tag |= PoseBoneMatrix.changetag.Rotate;
                }
                if (t != last.t)
                {
                    tag |= PoseBoneMatrix.changetag.Trans;
                }
                if (s != last.s)
                {
                    tag |= PoseBoneMatrix.changetag.Scale;
                }
                //if(bonesinfo[i].tag!= PoseBoneMatrix.changetag.All)
                //{
                //   // Debug.LogWarning("fid=" + _fid + " bone=" + i);
                //}
            }
        }

        public void Tag(PoseBoneMatrix last)
        {
            if (last == null)
                tag = PoseBoneMatrix.changetag.All;
            else
            {
                tag = PoseBoneMatrix.changetag.NoChange;
                if (r != last.r)
                {
                    tag |= PoseBoneMatrix.changetag.Rotate;
                }
                if (t != last.t)
                {
                    tag |= PoseBoneMatrix.changetag.Trans;
                }
                if (s != last.s)
                {
                    tag |= PoseBoneMatrix.changetag.Scale;
                }
            }
        }

        public void Save(System.IO.Stream stream, PoseBoneMatrix last)
        {
            byte[] btag = new byte[2];
            btag[0] = (byte)tag;
            btag[1] = last == null ? (byte)(PoseBoneMatrix.changetag.All) : (byte)tag;
            stream.Write(btag, 0, 2);
            if (last == null || (tag & PoseBoneMatrix.changetag.Rotate) > 0)
            {
                byte[] buf = BitHelper.getBytes(r);
                stream.Write(buf, 0, 16);
            }
            if (last == null || (tag & PoseBoneMatrix.changetag.Trans) > 0)
            {
                byte[] buf = BitHelper.getBytes(t);
                stream.Write(buf, 0, 12);
            }
            if (last == null || (tag & PoseBoneMatrix.changetag.Scale) > 0)
            {
                byte[] buf = BitHelper.getBytes(s);
                stream.Write(buf, 0, 12);
            }
        }
        //public void Load(System.IO.Stream stream, PoseBoneMatrix last)
        //{
        //    byte[] btag = new byte[2];
        //    stream.Read(btag, 0, 2);
        //    this.tag = (PoseBoneMatrix.changetag)btag[0];
        //    PoseBoneMatrix.changetag savetag = (PoseBoneMatrix.changetag)btag[1];
        //    //Debug.LogWarning("savetag=" + this.tag + "|" + savetag); 
        //    if ((savetag & PoseBoneMatrix.changetag.Rotate) > 0)
        //    {
        //        byte[] buf = new byte[16];
        //        stream.Read(buf, 0, 16);
        //        r = BitHelper.ToQuaternion(buf, 0);
        //    }
        //    else
        //    {
        //        r = last.r;
        //    }
        //    if ((savetag & PoseBoneMatrix.changetag.Trans) > 0)
        //    {
        //        byte[] buf = new byte[12];
        //        stream.Read(buf, 0, 12);
        //        t = BitHelper.ToVector3(buf, 0);
        //    }
        //    else
        //    {
        //        t = last.t;
        //    }
        //    if ((savetag & PoseBoneMatrix.changetag.Scale) > 0)
        //    {
        //        byte[] buf = new byte[12];
        //        stream.Read(buf, 0, 12);
        //        s = BitHelper.ToVector3(buf, 0);
        //    }
        //    else
        //    {
        //        s = last.s;
        //    }
        //}

        public object Clone()
        {
            PoseBoneMatrix bm = new PoseBoneMatrix();
            bm.tag = this.tag;
            bm.r = this.r;
            if (r.w == 0)
            {
                bm.r = Quaternion.identity;
            }
            bm.s = this.s;
            bm.t = this.t;
            return bm;
        }
        public static PoseBoneMatrix Lerp(PoseBoneMatrix left, PoseBoneMatrix right, float lerp)
        {
            PoseBoneMatrix m = new PoseBoneMatrix();
            m.tag = changetag.All;
            m.r = Quaternion.Lerp(left.r, right.r, lerp);
            if (float.IsNaN(m.r.x))
            {
                m.r = Quaternion.identity;
            }
            m.t = Vector3.Lerp(left.t, right.t, lerp);
            m.s = Vector3.Lerp(left.s, right.s, lerp);
            return m;

        }
    }
}
