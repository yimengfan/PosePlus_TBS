using System.Collections.Generic;

namespace Game.Battle
{
     /// <summary>
     /// 战斗输入
     /// </summary>
    public interface IBattleInput
    {
        ADataDrive InputSate { get; }
        void EnqueueCmd(Cmd cmd);
    }
}