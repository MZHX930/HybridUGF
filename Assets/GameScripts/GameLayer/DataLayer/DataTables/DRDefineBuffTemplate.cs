//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.743
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// DefineBuffTemplate。
    /// </summary>
    public class DRDefineBuffTemplate : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取标签。
        /// </summary>
        public EnumGameFightTag[] Tags
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取优先级越低的buff越后面执行。相同则比较id。
        /// </summary>
        public int Priority
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取最大堆叠数。
        /// </summary>
        public int MaxStack
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取buff的工作周期，单位：秒。每多少秒执行工作一次，如果<=0则代表不会周期性工作，只要>0，则最小值为Time.FixedDeltaTime。。
        /// </summary>
        public float TickTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否可以移动坐标。
        /// </summary>
        public bool CanMove
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否可以转身。
        /// </summary>
        public bool CanRotate
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否可以开启技能释放流程。
        /// </summary>
        public bool CanUseSkill
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取关联角色属性配置表，第一个是加法计算；第二个是乘法计算。
        /// </summary>
        public int[] CharacterPropIds
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取buff在被添加、改变层数时候触发的事件。
        /// </summary>
        public string OnOccurrence
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取。
        /// </summary>
        public string[] OnOccurrenceParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取buff在每个工作周期会执行的函数，如果这个函数为空，或者tickTime<=0，都不会发生周期性工作。
        /// </summary>
        public string OnTick
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取。
        /// </summary>
        public string[] OnTickParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取在这个buffObj被移除之前要做的事情，如果运行之后buffObj又不足以被删除了就会被保留。
        /// </summary>
        public string OnRemoved
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取。
        /// </summary>
        public string[] OnRemovedParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取在释放技能的时候运行的buff，执行这个buff获得最终技能要产生的Timeline。
        /// </summary>
        public string OnCast
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取。
        /// </summary>
        public string[] OnCastParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取在伤害流程中，持有这个buff的人作为攻击者会发生的事情。
        /// </summary>
        public string OnHit
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取。
        /// </summary>
        public string[] OnHitParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取在伤害流程中，持有这个buff的人作为挨打者会发生的事情。
        /// </summary>
        public string OnBeHurt
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取。
        /// </summary>
        public string[] OnBeHurtParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取在伤害流程中，如果击杀目标，则会触发的啥事情。
        /// </summary>
        public string OnKill
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取。
        /// </summary>
        public string[] OnKillParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取在伤害流程中，持有这个buff的人被杀死了，会触发的事情。
        /// </summary>
        public string OnBeKilled
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取。
        /// </summary>
        public string[] OnBeKilledParams
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataTableRuntimeParseTools.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(DataTableRuntimeParseTools.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            Tags = DataTableRuntimeParseTools.ParseEnumGameFightTagArray(columnStrings[index++]);
            Priority = int.Parse(columnStrings[index++]);
            MaxStack = int.Parse(columnStrings[index++]);
            TickTime = float.Parse(columnStrings[index++]);
            CanMove = bool.Parse(columnStrings[index++]);
            CanRotate = bool.Parse(columnStrings[index++]);
            CanUseSkill = bool.Parse(columnStrings[index++]);
            CharacterPropIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            OnOccurrence = columnStrings[index++];
            OnOccurrenceParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnTick = columnStrings[index++];
            OnTickParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnRemoved = columnStrings[index++];
            OnRemovedParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnCast = columnStrings[index++];
            OnCastParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnHit = columnStrings[index++];
            OnHitParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnBeHurt = columnStrings[index++];
            OnBeHurtParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnKill = columnStrings[index++];
            OnKillParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnBeKilled = columnStrings[index++];
            OnBeKilledParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            return true;
        }

        private void GeneratePropertyArray()
        {

        }
    }
}
