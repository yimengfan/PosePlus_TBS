using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


class BitHelper
{
    public static byte[] getBytes(Vector2 pos)
    {
        byte[] buf = new byte[8];
        BitConverter.GetBytes(pos.x).CopyTo(buf, 0);
        BitConverter.GetBytes(pos.y).CopyTo(buf, 4);
        return buf;
    }
    public static Vector2 ToVector2(byte[] buf, int pos)
    {
        Vector2 vec;
        vec.x = BitConverter.ToSingle(buf, pos + 0);
        vec.y = BitConverter.ToSingle(buf, pos + 4);
        return vec;
    }
    public static byte[] getBytes(Vector3 pos)
    {
        byte[] buf = new byte[12];
        BitConverter.GetBytes(pos.x).CopyTo(buf, 0);
        BitConverter.GetBytes(pos.y).CopyTo(buf, 4);
        BitConverter.GetBytes(pos.z).CopyTo(buf, 8);
        return buf;
    }
    public static Vector3 ToVector3(byte[] buf, int pos)
    {
        Vector3 vec;
        vec.x = BitConverter.ToSingle(buf, pos + 0);
        vec.y = BitConverter.ToSingle(buf, pos + 4);
        vec.z = BitConverter.ToSingle(buf, pos + 8);
        return vec;
    }
    public static byte[] getBytes(Vector4 pos)
    {
        byte[] buf = new byte[16];
        BitConverter.GetBytes(pos.x).CopyTo(buf, 0);
        BitConverter.GetBytes(pos.y).CopyTo(buf, 4);
        BitConverter.GetBytes(pos.z).CopyTo(buf, 8);
         BitConverter.GetBytes(pos.w).CopyTo(buf, 12);
       return buf;
    }
    public static Vector4 ToVector4(byte[] buf, int pos)
    {
        Vector4 vec;
        vec.x = BitConverter.ToSingle(buf, pos + 0);
        vec.y = BitConverter.ToSingle(buf, pos + 4);
        vec.z = BitConverter.ToSingle(buf, pos + 8);
        vec.w = BitConverter.ToSingle(buf, pos + 12);
        return vec;
    }
    public static byte[] getBytes(Quaternion pos)
    {
        byte[] buf = new byte[16];
        BitConverter.GetBytes(pos.x).CopyTo(buf, 0);
        BitConverter.GetBytes(pos.y).CopyTo(buf, 4);
        BitConverter.GetBytes(pos.z).CopyTo(buf, 8);
        BitConverter.GetBytes(pos.w).CopyTo(buf, 12);
        return buf;
    }
    public static Quaternion ToQuaternion(byte[] buf, int pos)
    {
        Quaternion vec;
        vec.x = BitConverter.ToSingle(buf, pos + 0);
        vec.y = BitConverter.ToSingle(buf, pos + 4);
        vec.z = BitConverter.ToSingle(buf, pos + 8);
        vec.w = BitConverter.ToSingle(buf, pos + 12);
        return vec;
    }
    public static byte[] getBytes(Bounds bound)
    {
        byte[] buf = new byte[24];
        getBytes(bound.center).CopyTo(buf, 0);
        getBytes(bound.size).CopyTo(buf, 12);
        return buf;
    }
    public static Bounds ToBounds(byte[] bytes, int pos)
    {
        Bounds b = new Bounds(ToVector3(bytes, pos + 0), ToVector3(bytes, pos + 12));
        return b;
    }
    public static byte[] getBytes(Color32 color)
    {
        byte[] buf = new byte[4];
        buf[0] = color.a;
        buf[1] = color.r;
        buf[2] = color.g;
        buf[3] = color.b;
        return buf;
    }
    public static Color32 ToColor32(byte[] buf, int pos)
    {
        Color32 c;
        c.a = buf[pos + 0];
        c.r = buf[pos + 1];
        c.g = buf[pos + 2];
        c.b = buf[pos + 3];
        return c;
    }
    public static byte[] getBytes(Matrix4x4 mat)
    {
        byte[] buf = new byte[64];
        int seek = 0;
        BitConverter.GetBytes(mat.m00).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m01).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m02).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m03).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m10).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m11).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m12).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m13).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m20).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m21).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m22).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m23).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m30).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m31).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m32).CopyTo(buf, seek);
        seek += 4;
        BitConverter.GetBytes(mat.m33).CopyTo(buf, seek);
        seek += 4;
        return buf;
    }
    public static Matrix4x4 ToMatrix(byte[] buf, int pos)
    {
        Matrix4x4 m;
        int seek = pos;
        m.m00 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m01 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m02 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m03 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m10 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m11 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m12 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m13 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m20 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m21 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m22 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m23 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m30 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m31 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m32 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        m.m33 = BitConverter.ToSingle(buf, seek);
        seek += 4;
        return m;
    }
}
