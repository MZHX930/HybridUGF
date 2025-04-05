using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    ///<summary>
    ///策划填表的内容
    ///</summary>
    public struct BuffModel
    {
        ///<summary>
        ///buff的key
        ///</summary>
        public string BuffKey;

        ///<summary>
        ///buff的优先级，优先级越低的buff越后面执行，这是一个非常重要的属性
        ///比如经典的“吸收50点伤害”和“受到的伤害100%反弹给攻击者”应该反弹多少，取决于这两个buff的priority谁更高
        ///</summary>
        public int Priority;

        ///<summary>
        ///buff堆叠的规则中需要的层数，在这个游戏里只要id和caster相同的buffObj就可以堆叠
        ///激战2里就不同，尽管图标显示堆叠，其实只是统计了有多少个相同id的buffObj作为层数显示了
        ///</summary>
        public int MaxStack;

        ///<summary>
        ///buff的tag
        ///</summary>
        public EnumGameFightTag[] Tags;

        ///<summary>
        ///buff的工作周期，单位：秒。
        ///每多少秒执行工作一次，如果<=0则代表不会周期性工作，只要>0，则最小值为Time.FixedDeltaTime。
        ///</summary>
        public float TickTime;

        ///<summary>
        ///buff会给角色添加的属性，这些属性根据这个游戏设计只有2种，加法plus和乘法times，所以这个数组实际上只有2维
        ///</summary>
        public ChaProperty[] propMod;

        ///<summary>
        ///buff对于角色的ChaControlState的影响
        ///</summary>
        public ChaControlState stateMod;

        ///<summary>
        ///buff在被添加、改变层数时候触发的事件
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///<param name="modifyStack">会传递本次改变的层数</param>
        ///</summary>
        public BuffOnOccur OnOccurrence;
        public string[] OnOccurrenceParams;

        ///<summary>
        ///buff在每个工作周期会执行的函数，如果这个函数为空，或者tickTime<=0，都不会发生周期性工作
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///</summary>
        public BuffOnTick OnTick;
        public string[] OnTickParams;

        ///<summary>
        ///在这个buffObj被移除之前要做的事情，如果运行之后buffObj又不足以被删除了就会被保留
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///</summary>
        public BuffOnRemoved OnRemoved;
        public string[] OnRemovedParams;

        ///<summary>
        ///在释放技能的时候运行的buff，执行这个buff获得最终技能要产生的Timeline
        ///<param name="buff">会传递给脚本的buffObj</param>
        ///<param name="skill">即将释放的技能skillObj</param>
        ///<param name="timeline">释放出来的技能，也就是一个timeline，这里的本质就是让你通过buff还能对timeline进行hack以达到修改技能效果的目的</return>
        ///</summary>
        public BuffOnCast OnCast;
        public string[] OnCastParams;

        ///<summary>
        ///在伤害流程中，持有这个buff的人作为攻击者会发生的事情
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///<param name="damageInfo">这次的伤害信息</param>
        ///<param name="target">挨打的角色对象</param>
        ///</summary>
        public BuffOnHit OnHit;
        public string[] OnHitParams;

        ///<summary>
        ///在伤害流程中，持有这个buff的人作为挨打者会发生的事情
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///<param name="damageInfo">这次的伤害信息</param>
        ///<param name="attacker">打我的角色，当然可以是空的</param>
        ///</summary>
        public BuffOnBeHurt OnBeHurt;
        public string[] OnBeHurtParams;

        ///<summary>
        ///在伤害流程中，如果击杀目标，则会触发的啥事情
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///<param name="damageInfo">这次的伤害信息</param>
        ///<param name="target">挨打的角色对象</param>
        ///</summary>
        public BuffOnKill OnKill;
        public string[] OnKillParams;

        ///<summary>
        ///在伤害流程中，持有这个buff的人被杀死了，会触发的事情
        ///<param name="buff">会传递给脚本buffObj作为参数</param>
        ///<param name="damageInfo">这次的伤害信息</param>
        ///<param name="attacker">发起攻击造成击杀的角色对象</param>
        ///</summary>
        public BuffOnBeKilled OnBeKilled;
        public string[] OnBeKilledParams;

        public BuffModel(
            string buffKey, EnumGameFightTag[] tags, int priority, int maxStack, float tickTime,
            BuffOnOccur onOccur, string[] occurParam,
            BuffOnRemoved onRemoved, string[] removedParam,
            BuffOnTick onTick, string[] tickParam,
            BuffOnCast onCast, string[] castParam,
            BuffOnHit onHit, string[] hitParam,
            BuffOnBeHurt beHurt, string[] hurtParam,
            BuffOnKill onKill, string[] killParam,
            BuffOnBeKilled beKilled, string[] beKilledParam,
            ChaControlState stateMod, ChaProperty[] propMod = null
        )
        {
            this.BuffKey = buffKey;
            this.Tags = tags;
            this.Priority = priority;
            this.MaxStack = maxStack;
            this.stateMod = stateMod;
            this.TickTime = tickTime;

            this.propMod = new ChaProperty[2]{
                ChaProperty.zero,
                ChaProperty.zero
            };
            if (propMod != null)
            {
                for (int i = 0; i < Mathf.Min(2, propMod.Length); i++)
                {
                    this.propMod[i] = propMod[i];
                }
            }

            this.OnOccurrence = onOccur;
            this.OnOccurrenceParams = occurParam;
            this.OnRemoved = onRemoved;
            this.OnRemovedParams = removedParam;
            this.OnTick = onTick;
            this.OnTickParams = tickParam;
            this.OnCast = onCast;
            this.OnCastParams = castParam;
            this.OnHit = onHit;
            this.OnHitParams = hitParam;
            this.OnBeHurt = beHurt;
            this.OnBeHurtParams = hurtParam;
            this.OnKill = onKill;
            this.OnKillParams = killParam;
            this.OnBeKilled = beKilled;
            this.OnBeKilledParams = beKilledParam;
        }
    }
}