using System;
using System.Management.Instrumentation;

namespace Game.Battle.Skill
{
    public class SkillEventAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Des { get; private set; }
        public SkillEventAttribute(string name , string des = "")
        {
            this.Name = name;
            this.Des = des;
        }
    }
}