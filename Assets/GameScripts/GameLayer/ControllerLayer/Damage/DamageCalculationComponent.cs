using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    ///<summary>
    ///负责处理游戏中所有的DamageInfo
    ///</summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/DamageCalculationComponent")]
    public sealed class DamageCalculationComponent : GameFrameworkComponent
    {
        private List<DamageInfo> damageInfos = new List<DamageInfo>();

        private void FixedUpdate()
        {
            int i = 0;
            while (i < damageInfos.Count)
            {
                DealWithDamage(damageInfos[i]);
                damageInfos.RemoveAt(0);
            }
        }

        ///<summary>
        ///处理DamageInfo的流程，也就是整个游戏的伤害流程
        ///<param name="dInfo">要处理的damageInfo</param>
        ///<retrun>处理完之后返回出一个damageInfo，依照这个，给对应角色扣血处理</return>
        ///</summary>
        private void DealWithDamage(DamageInfo dInfo)
        {
            //如果目标已经挂了，就直接return了
            if (!dInfo.Defender)
                return;
            UnitCharacterCtrl defenderChaState = dInfo.Defender;
            if (!defenderChaState)
                return;
            UnitCharacterCtrl attackerChaState = null;
            if (defenderChaState.IsDead == true)
                return;
            //先走一遍所有攻击者的onHit
            if (dInfo.Attacker)
            {
                attackerChaState = dInfo.Attacker;
                for (int i = 0; i < attackerChaState.CtrlData.Buffs.Count; i++)
                {
                    if (attackerChaState.CtrlData.Buffs[i].Model.OnHit != null)
                    {
                        attackerChaState.CtrlData.Buffs[i].Model.OnHit(attackerChaState.CtrlData.Buffs[i], dInfo, dInfo.Defender);
                    }
                }
            }
            //然后走一遍挨打者的beHurt
            for (int i = 0; i < defenderChaState.CtrlData.Buffs.Count; i++)
            {
                if (defenderChaState.CtrlData.Buffs[i].Model.OnBeHurt != null)
                {
                    defenderChaState.CtrlData.Buffs[i].Model.OnBeHurt(defenderChaState.CtrlData.Buffs[i], dInfo, dInfo.Attacker);
                }
            }
            if (defenderChaState.CanBeKilledByDamageInfo(dInfo) == true)
            {
                //如果角色可能被杀死，就会走OnKill和OnBeKilled，这个游戏里面没有免死金牌之类的技能，所以只要判断一次就好
                if (attackerChaState != null)
                {
                    for (int i = 0; i < attackerChaState.CtrlData.Buffs.Count; i++)
                    {
                        if (attackerChaState.CtrlData.Buffs[i].Model.OnKill != null)
                        {
                            attackerChaState.CtrlData.Buffs[i].Model.OnKill(attackerChaState.CtrlData.Buffs[i], dInfo, dInfo.Defender);
                        }
                    }
                }
                for (int i = 0; i < defenderChaState.CtrlData.Buffs.Count; i++)
                {
                    if (defenderChaState.CtrlData.Buffs[i].Model.OnBeKilled != null)
                    {
                        defenderChaState.CtrlData.Buffs[i].Model.OnBeKilled(defenderChaState.CtrlData.Buffs[i], dInfo, dInfo.Attacker);
                    }
                }
            }
            //最后根据结果处理：如果是治疗或者角色非无敌，才会对血量进行调整。
            bool isHeal = dInfo.IsHeal();
            int dVal = dInfo.DamageValue(isHeal);
            if (isHeal == true || defenderChaState.CtrlData.ImmuneTime <= 0)
            {
                if (dInfo.RequireDoHurt() == true && defenderChaState.CanBeKilledByDamageInfo(dInfo) == false)
                {
                    defenderChaState.Play(EnumSpineAnimKey.BeHurt);
                }
                defenderChaState.ModifyResource(new ChaResource(-dVal));
                //按游戏设计的规则跳数字，如果要有暴击，也可以丢在策划脚本函数（lua可以返回多参数）也可以随便怎么滴
                DamageFloatForm.PopUpNumberOnCharacter(dInfo.Defender, Mathf.Abs(dVal), isHeal);
            }

            //伤害流程走完，添加buff
            for (int i = 0; i < dInfo.AddBuffs.Count; i++)
            {
                var toCha = dInfo.AddBuffs[i].Target;
                UnitCharacterCtrl toChaState = toCha.Equals(dInfo.Attacker) ? attackerChaState : defenderChaState;

                if (toChaState != null && toChaState.IsDead == false)
                {
                    toChaState.AddBuff(dInfo.AddBuffs[i]);
                }
            }
        }

        /// <summary>
        /// 终止所有等待执行的伤害流程
        /// </summary>
        public void Clear()
        {
            damageInfos.Clear();
        }

        ///<summary>
        ///添加一个damageInfo
        ///<param name="attacker">攻击者，可以为null</param>
        ///<param name="target">挨打对象</param>
        ///<param name="damage">基础伤害值</param>
        ///<param name="damageDegree">伤害的角度</param>
        ///<param name="criticalRate">暴击率，0-1</param>
        ///<param name="tags">伤害信息类型</param>
        ///</summary>
        public void DoDamage(UnitCharacterCtrl attacker, UnitCharacterCtrl target, Damage damage, float damageDegree, float criticalRate, DamageInfoTag[] tags)
        {
            this.damageInfos.Add(new DamageInfo(attacker, target, damage, damageDegree, criticalRate, tags));
        }
    }
}