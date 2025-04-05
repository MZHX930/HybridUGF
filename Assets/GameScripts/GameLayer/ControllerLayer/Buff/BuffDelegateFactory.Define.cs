using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public partial class BuffDelegateFactory
    {
        /// <summary>
        /// buff在被添加、改变层数时候触发的事件
        /// </summary>
        public static Dictionary<string, BuffOnOccur> OnOccurFuncDic = new Dictionary<string, BuffOnOccur>()
        {

        };

        /// <summary>
        /// buff在每个工作周期会执行的函数，如果这个函数为空，或者tickTime<=0，都不会发生周期性工作
        /// </summary>
        public static Dictionary<string, BuffOnTick> OnTickFuncDic = new Dictionary<string, BuffOnTick>()
        {
            {"BarrelDurationLose", BarrelDurationLose}
        };

        /// <summary>
        /// 在这个buffObj被移除之前要做的事情，如果运行之后buffObj又不足以被删除了就会被保留
        /// </summary>
        public static Dictionary<string, BuffOnRemoved> OnRemovedFuncDic = new Dictionary<string, BuffOnRemoved>()
        {
            {"TeleportCarrier", TeleportCarrier}
        };

        /// <summary>
        /// 在释放技能的时候运行的buff，执行这个buff获得最终技能要产生的Timeline
        /// </summary>
        public static Dictionary<string, BuffOnCast> OnCastFuncDic = new Dictionary<string, BuffOnCast>()
        {
            {"ReduceSkillCoolDown", DecreaseSkillCoolDown},
            {"IncreaseSkillDamage", IncreaseSkillDamage},
            {"CloneSkill",CloneSkill}
        };

        /// <summary>
        /// 在伤害流程中，持有这个buff的人作为攻击者会发生的事情
        /// </summary>
        public static Dictionary<string, BuffOnHit> OnHitFuncDic = new Dictionary<string, BuffOnHit>()
        {

        };

        /// <summary>
        /// 在伤害流程中，持有这个buff的人作为挨打者会发生的事情
        /// </summary>
        public static Dictionary<string, BuffOnBeHurt> BeHurtFuncDic = new Dictionary<string, BuffOnBeHurt>()
        {
            {"OnlyTakeOneDirectDamage", OnlyTakeOneDirectDamage}
        };

        /// <summary>
        /// 在伤害流程中，如果击杀目标，则会触发的啥事情
        /// </summary>
        public static Dictionary<string, BuffOnKill> OnKillFuncDic = new Dictionary<string, BuffOnKill>()
        {

        };

        /// <summary>
        /// 在伤害流程中，持有这个buff的人被杀死了，会触发的事情
        /// </summary>
        public static Dictionary<string, BuffOnBeKilled> BeKilledFuncDic = new Dictionary<string, BuffOnBeKilled>()
        {
            {"BarrelExplosed", BarrelExplosed}
        };
    }

    public delegate void BuffOnOccur(BuffRTData buff, int modifyStack);
    public delegate void BuffOnRemoved(BuffRTData buff);
    public delegate void BuffOnTick(BuffRTData buff);
    public delegate void BuffOnHit(BuffRTData buff, DamageInfo damageInfo, UnitCharacterCtrl target);
    public delegate void BuffOnBeHurt(BuffRTData buff, DamageInfo damageInfo, UnitCharacterCtrl attacker);
    public delegate void BuffOnKill(BuffRTData buff, DamageInfo damageInfo, UnitCharacterCtrl target);
    public delegate void BuffOnBeKilled(BuffRTData buff, DamageInfo damageInfo, UnitCharacterCtrl attacker);
    public delegate LogicTimeLineShell BuffOnCast(BuffRTData buff, SkillLauncherShell skill, LogicTimeLineShell timeline);
}
