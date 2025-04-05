using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 角色类型的运行时数据
    /// 记录角色的模板、初始、运行时数据数据
    /// </summary>
    public sealed class UnitCharacterCtrlData : IUnitCtrlData
    {
        public static UnitCharacterCtrlData Create()
        {
            UnitCharacterCtrlData shellEntityData = ReferencePool.Acquire<UnitCharacterCtrlData>();

            return shellEntityData;
        }

        /*模板数据------------------------------------------------------------------------------------------------------------*/
        /// <summary>
        /// 角色稀有度
        /// </summary>
        public CharacterRarityEnum RarityType = CharacterRarityEnum.N;
        /// <summary>
        /// 网格信息
        /// </summary>
        public CharacterAreaInfo GridInfo = CharacterAreaInfo.None;
        /// <summary>
        /// 对应的配置表id
        /// </summary>
        public int DtId = 0;

        /*初始数据------------------------------------------------------------------------------------------------------------*/
        ///<summary>
        ///角色的基础属性，也就是每个角色"裸体"且不带任何buff的"纯粹的属性"
        ///先写死，正式的应该读表
        ///</summary>
        public ChaProperty BaseProp = ChaProperty.zero;
        /// <summary>
        /// 角色阵营
        /// </summary>
        public GameSideEnum GameSide = GameSideEnum.Player;
        /// <summary>
        /// 角色类型
        /// </summary>
        public CharacterTypeEnum ChaType = CharacterTypeEnum.Hero;
        /// <summary>
        /// 出生的世界坐标
        /// </summary>
        public Vector3 BornWorldPos = default;
        /// <summary>
        /// 等级
        /// </summary>
        public int Level = 1;
        /// <summary>
        /// 合成等级
        /// </summary>
        public int MergeLv = 1;
        /// <summary>
        /// 初始技能列表
        /// </summary>
        public int[] SkillLauncherIds;


        /*运行时数据------------------------------------------------------------------------------------------------------------*/
        /// <summary>
        /// 是否可以移动
        /// </summary>
        public bool CanMove = false;
        /// <summary>
        /// 是否可以旋转
        /// </summary>
        public bool CanRotate = false;

        #region 技能
        ///<summary>
        ///角色的技能
        ///</summary>
        public List<SkillLauncherShell> ChaSkillList = new List<SkillLauncherShell>();
        #endregion


        #region Buff
        ///<summary>
        ///角色身上的buff
        ///</summary>
        public List<BuffRTData> Buffs = new List<BuffRTData>();
        #endregion


        #region 角色属性
        ///<summary>
        ///角色现有的资源，比如hp之类的
        ///</summary>
        public ChaResource CurResource = new ChaResource(1);

        ///<summary>
        ///角色当前的属性（复合计算后的结果）
        ///</summary>
        public ChaProperty CurProperty { get; private set; }

        ///<summary>
        ///角色来自buff的属性
        ///这个数组并不是说每个buff可以占用一条数据，而是分类总和
        ///在这个游戏里buff带来的属性总共有2类，plus和times，用策划设计的公式就是plus的属性加完之后乘以times的属性
        ///所以数组长度其实只有2：[0]buffPlus, [1]buffTimes
        ///</summary>
        public ChaProperty[] BuffProp = new ChaProperty[2] { ChaProperty.zero, ChaProperty.zero };

        ///<summary>
        ///来自装备的属性
        ///</summary>
        public ChaProperty EquipmentProp = ChaProperty.zero;
        #endregion


        #region 控制状态
        ///<summary>
        //角色最终的可操作性状态
        ///</summary>
        public ChaControlState m_ControlState = new ChaControlState(true, true, true);

        ///<summary>
        ///GameTimeline专享的ChaControlState
        ///</summary>
        public ChaControlState TimelineControlState = new ChaControlState(true, true, true);

        /// <summary>
        /// 最终的角色可操作性状态
        /// </summary>
        public ChaControlState ControlState
        {
            get
            {
                return this.m_ControlState + this.TimelineControlState;
            }
        }

        ///<summary>
        ///角色的无敌状态持续时间，如果在无敌状态中，子弹不会碰撞，DamageInfo处理无效化
        ///单位：秒
        ///</summary>
        public float ImmuneTime { get; private set; } = 0f;
        #endregion


        public void Clear()
        {
            // 重置模板数据
            RarityType = CharacterRarityEnum.N;
            GridInfo = CharacterAreaInfo.None;
            DtId = 0;
            SkillLauncherIds = null;

            // 重置初始数据
            GameSide = GameSideEnum.Player;
            ChaType = CharacterTypeEnum.Hero;
            BornWorldPos = default;
            Level = 1;
            MergeLv = 1;

            // 重置运行时数据
            CanMove = false;
            CanRotate = false;

            // 清空技能和Buff列表
            ChaSkillList.Clear();
            Buffs.Clear();

            // 重置角色属性相关
            CurResource = ChaResource.Null;
            CurProperty = ChaProperty.zero;
            BaseProp = ChaProperty.zero;
            BuffProp = new ChaProperty[2] { ChaProperty.zero, ChaProperty.zero };
            EquipmentProp = ChaProperty.zero;

            // 重置控制状态
            m_ControlState = new ChaControlState(true, true, true);
            TimelineControlState = new ChaControlState(true, true, true);
            ImmuneTime = 0f;
        }

        ///<summary>
        ///初始化角色的属性
        ///</summary>
        public void InitBaseProp(ChaProperty cProp)
        {
            BaseProp = cProp;
            AttrRecheck();
            CurResource.HP = CurProperty.HP;
            CurResource.MP = CurProperty.MP;
            CurResource.Stamina = 100;
        }

        ///<summary>
        ///重新计算所有属性，并且获得一个最终属性
        ////其实这个应该走脚本函数返回，抛给脚本函数多个ChaProperty，由脚本函数运作他们的运算关系，并返回结果
        ///</summary>
        public void AttrRecheck()
        {
            m_ControlState.Origin();
            CurProperty.Zero();

            for (var i = 0; i < BuffProp.Length; i++)
                BuffProp[i].Zero();
            for (int i = 0; i < Buffs.Count; i++)
            {
                for (int j = 0; j < Mathf.Min(BuffProp.Length, Buffs[i].Model.propMod.Length); j++)
                {
                    BuffProp[j] += Buffs[i].Model.propMod[j] * Buffs[i].Stack;
                }
                m_ControlState += Buffs[i].Model.stateMod;
            }

            CurProperty = (BaseProp + EquipmentProp + BuffProp[0]) * BuffProp[1];
        }

        ///<summary>
        ///增加角色的血量等资源，直接改变数字的，属于最后一步操作了
        ///<param name="value">要改变的量，负数为减少</param>
        ///</summary>
        public void ModifyResource(ChaResource value)
        {
            CurResource += value;
            CurResource.HP = Mathf.Clamp(CurResource.HP, 0, CurProperty.HP);
            CurResource.MP = Mathf.Clamp(CurResource.MP, 0, CurProperty.MP);
            CurResource.Stamina = Mathf.Clamp(CurResource.Stamina, 0, 100);
        }

        ///<summary>
        ///设置无敌时间
        ///<param name="time">无敌的时间，单位：秒</param>
        ///</summary>
        public void SetImmuneTime(float time)
        {
            ImmuneTime = Mathf.Max(ImmuneTime, time);
        }

        ///<summary>
        ///根据id获得角色学会的技能（skillObj），如果没有则返回null
        ///<param name="id">技能的id</param>
        ///<return>skillObj or null</return>
        ///</summary>
        public SkillLauncherShell GetSkillById(int id)
        {
            for (int i = 0; i < ChaSkillList.Count; i++)
            {
                if (ChaSkillList[i].Model.DtId == id)
                {
                    return ChaSkillList[i];
                }
            }
            return null;
        }
    }
}
