using System;

namespace Game.Battle
{
    public class BuffTypeAttribute :Attribute
    {
        public int BuffType { get; private set; }
        public BuffTypeAttribute(int type)
        {
            this.BuffType = type;
        }
    }
}