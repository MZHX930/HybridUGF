using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    ///<summary>
    ///把技能当作一个发射器
    ///技能是角色拥有的东西，因为角色有技能，玩家或者ai才能操作角色释放技能
    ///</summary>
    public class SkillLauncherShell
    {
        ///<summary>
        ///技能的模板，创建于skillModel，但运行中还是会允许改变
        ///</summary>
        public SkillLauncherShellModel Model;

        ///<summary>
        ///技能等级
        ///</summary>
        public int Level;

        ///<summary>
        ///冷却时间，单位秒。
        ///</summary>
        public float CoolDown;


        /// <summary>
        /// 创建一个技能发射器
        /// </summary>
        /// <param name="dtId">技能id</param>
        /// <param name="level">技能等级</param>
        /// <returns></returns>
        public static SkillLauncherShell Create(int dtId, int level = 1)
        {
            var launcherConfig = GameEntry.DataTable.GetDataTable<DRDefineSkillLauncher>().GetDataRow(dtId);

            SkillLauncherShellModel model = new SkillLauncherShellModel()
            {
                DtId = launcherConfig.Id,
                Cost = GameEntry.DataTable.ToChaResource(launcherConfig.Cost),
                Condition = GameEntry.DataTable.ToChaResource(launcherConfig.Condition),
                Effect = GameEntry.DataTable.ToLogicTimeLineModel(launcherConfig.TimeLineId),
                Buff = null,
                SearchRadius = launcherConfig.SearchRadius,
                SearchType = launcherConfig.SearchType,
                Cooldown = launcherConfig.CD
            };

            return new SkillLauncherShell()
            {
                Model = model,
                Level = level,
                CoolDown = model.Cooldown
            };
        }
    }

    ///<summary>
    ///策划填表的技能
    ///定义这是一个什么样子的技能
    ///</summary>
    public struct SkillLauncherShellModel
    {
        ///<summary>
        ///技能的id
        ///</summary>
        public int DtId;

        ///<summary>
        ///技能使用的条件，这个游戏中只有资源需求，比如hp、ammo之类的
        ///</summary>
        public ChaResource Condition;

        ///<summary>
        ///技能的消耗，成功之后会扣除这些资源
        ///</summary>
        public ChaResource Cost;

        ///<summary>
        ///技能的效果，必然是一个timeline
        ///</summary>
        public LogicTimeLineModel Effect;

        ///<summary>
        ///学会技能的时候，同时获得的buff
        ///</summary>
        public AddBuffInfo[] Buff;

        /// <summary>
        /// 技能施法半径
        /// </summary>
        public float SearchRadius;

        /// <summary>
        /// 索敌逻辑：1随机 2最近单位
        /// </summary>
        public int SearchType;

        /// <summary>
        /// 技能冷却时间
        /// </summary>
        public float Cooldown;
    }
}