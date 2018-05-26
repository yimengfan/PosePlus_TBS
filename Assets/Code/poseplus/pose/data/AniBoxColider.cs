using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FB.PosePlus
{
  [Serializable]
  public  class AniBoxCollider
    {
      public AniBoxCollider()
      {
          mName = "noname";
          mBoxType = "Attack";
          mIndex = -1;
          mPosition = new Vector3(1, 1, 1);
          mSize = new Vector3(1, 1, 1);
      }
        [SerializeField]
        public string mBoxType;        //box类型，用于还原不同的box
        [SerializeField]
        public string mName;          //box唯一标识，
        [SerializeField]
        public int mIndex;
        [SerializeField]
        public Vector3 mPosition;
        //[SerializeField]
        //public Quaternion mRotation;
        [SerializeField]
        public Vector3 mSize;


        public static AniBoxCollider Lerp(AniBoxCollider left, AniBoxCollider right, float lerp)
        {
            AniBoxCollider m = new AniBoxCollider();
            m.mName = right.mName;
            m.mIndex = right.mIndex;
            m.mBoxType = right.mBoxType;
            m.mPosition = Vector3.Lerp(left.mPosition, right.mPosition, lerp);
            m.mSize = Vector3.Lerp(left.mSize, right.mSize, lerp);
            return m;
        }
    }

}