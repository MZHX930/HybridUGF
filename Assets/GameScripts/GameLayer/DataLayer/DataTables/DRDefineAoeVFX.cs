//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.424
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
    /// DefineAoeVFX。
    /// </summary>
    public class DRDefineAoeVFX : DataRowBase
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
        /// 获取aoe的视觉特效，如果是空字符串，就不会添加视觉特效。
        /// </summary>
        public string Prefab
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取移动速度因子，用于公式转换实际速度。
        /// </summary>
        public int MoveSpeedFactor
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取生命周期 秒。
        /// </summary>
        public float LifeTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取特殊的移动轨迹。
        /// </summary>
        public AoeMoveTween MoveTween
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取移动轨迹参数。
        /// </summary>
        public string[] MoveTweenParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取aoe的tag 1毒 2持续。
        /// </summary>
        public EnumGameFightTag[] Tags
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取aoe创建时的事件。
        /// </summary>
        public AoeOnCreate OnCreate
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取aoe创建时的事件参数。
        /// </summary>
        public string[] OnCreateParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取aoe每一跳的时间，单位：秒  如果这个时间小于等于0，或者没有onTick，则不会执行aoe的onTick事件。
        /// </summary>
        public float TickTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取aoe每一跳的事件，如果没有，就不会发生每一跳。
        /// </summary>
        public AoeOnTick OnTick
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取tick事件参数。
        /// </summary>
        public string[] OnTickParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取aoe结束时的事件。
        /// </summary>
        public AoeOnRemoved OnRemoved
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取结束时的事件参数。
        /// </summary>
        public string[] OnRemovedParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取有角色进入aoe时的事件，onCreate时候位于aoe范围内的人不会触发这个，但是在onCreate里面会已经存在。
        /// </summary>
        public AoeOnCharacterEnter OnChaEnter
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取角色进入aoe时的参数。
        /// </summary>
        public string[] OnChaEnterParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取有角色离开aoe结束时的事件。
        /// </summary>
        public AoeOnCharacterLeave OnChaLeave
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取角色离开aoe时的参数。
        /// </summary>
        public string[] OnChaLeaveParams
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
            Prefab = columnStrings[index++];
            MoveSpeedFactor = int.Parse(columnStrings[index++]);
            LifeTime = float.Parse(columnStrings[index++]);
            MoveTween = DataTableRuntimeParseTools.ParseAoeMoveTween(columnStrings[index++]);
            MoveTweenParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            Tags = DataTableRuntimeParseTools.ParseEnumGameFightTagArray(columnStrings[index++]);
            OnCreate = DataTableRuntimeParseTools.ParseAoeOnCreate(columnStrings[index++]);
            OnCreateParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            TickTime = float.Parse(columnStrings[index++]);
            OnTick = DataTableRuntimeParseTools.ParseAoeOnTick(columnStrings[index++]);
            OnTickParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnRemoved = DataTableRuntimeParseTools.ParseAoeOnRemoved(columnStrings[index++]);
            OnRemovedParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnChaEnter = DataTableRuntimeParseTools.ParseAoeOnCharacterEnter(columnStrings[index++]);
            OnChaEnterParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            OnChaLeave = DataTableRuntimeParseTools.ParseAoeOnCharacterLeave(columnStrings[index++]);
            OnChaLeaveParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);

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
