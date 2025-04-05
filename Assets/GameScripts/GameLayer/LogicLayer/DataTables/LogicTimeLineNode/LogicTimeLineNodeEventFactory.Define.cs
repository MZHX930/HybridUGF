using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 时间线事件工厂
    /// </summary>
    public static partial class LogicTimeLineNodeEventFactory
    {
        public static Dictionary<string, LogicTimeLineNodeEvent> Functions = new Dictionary<string, LogicTimeLineNodeEvent>()
        {
            #region 技能
            {"FireBulletFromCha", FireBulletFromCha},
            {"BasicAttack", BasicAttack},
            {"FireAoeFromCha", FireAoeFromCha},
            #endregion
        };
    }
}
