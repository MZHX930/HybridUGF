using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /*
        OnCastFunc
        在释放技能的时候运行的buff，执行这个buff获得最终技能要产生的Timeline
    */
    public partial class BuffDelegateFactory
    {
        ///<summary>
        ///onCast
        ///xxx技能释放后冷却时间降低yyy%
        /// xxx：   skillId int 技能id
        /// yyy：   dec int 降低的百分比值
        ///</summary>
        private static LogicTimeLineShell DecreaseSkillCoolDown(BuffRTData buff, SkillLauncherShell skill, LogicTimeLineShell timeline)
        {
            return default;
        }

        /// <summary>
        /// onCast
        /// xxx技能伤害增加yyy%。乘法计算。
        /// 初始伤害值 * (1+yyy%)
        /// xxx：   skillId int 技能id
        /// yyy：   inc int 增加的伤害百分比
        /// </summary>
        private static LogicTimeLineShell IncreaseSkillDamage(BuffRTData buff, SkillLauncherShell skill, LogicTimeLineShell timeline)
        {
            return default;
        }

        /// <summary>
        /// onCast
        /// xxx技能释放时yyy%几率再额外释放zzz次，不重复
        /// xxx：   skillId int 技能id
        /// yyy：   prob    int 概率百分比值
        /// zzz：   count   int 额外的次数
        /// </summary>
        private static LogicTimeLineShell CloneSkill(BuffRTData buff, SkillLauncherShell skill, LogicTimeLineShell timeline)
        {
            return default;
        }
    }
}
