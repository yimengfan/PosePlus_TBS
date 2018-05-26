using System;
using BDFramework.Mgr;
using BDFramework.UI;
using Game.Data;

namespace Game.Battle
{
    public class BuffFactory : MgrBase<BuffFactory>
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void CheckType(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(BuffTypeAttribute), false);
            if (attrs.Length > 0)
            {
                foreach (var attr in attrs)
                {
                    var _attr = (BuffTypeAttribute)attr;
                    var name = _attr.BuffType.ToString();
                    SaveAttribute(name, new ClassData() { Attribute = _attr, Type = type });
                }
            }
        }

        public IBuff CreateBuff(Buff buff)
        {
          //  UIMgr
            var cd = this.GetCalssData(buff.BuffType.ToString());
            var o = Activator.CreateInstance(cd.Type, buff);
            return o as IBuff;
        }
    }
}