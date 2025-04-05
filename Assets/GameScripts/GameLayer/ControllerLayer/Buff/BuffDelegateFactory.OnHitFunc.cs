using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /*
    OnHitFuncDic
    在伤害流程中，持有这个buff的人作为攻击者会发生的事情
    */
    public partial class BuffDelegateFactory
    {
        /// <summary>
        /// OnHit
        /// xxx技能在对yyy类型怪物的伤害增加zzz%。乘法计算
        /// 最终伤害 = 伤害值 * (1+zzz%)
        /// xxx：   skillId int 技能id
        /// yyy：   tag int 怪物标签
        /// zzz：   inc int 增加的伤害百分比
        /// </summary>
        private static void IncreaseSkillDamage2Boss(BuffRTData buff, DamageInfo damageInfo, UnitCharacterCtrl target)
        {
        }
    }
}
