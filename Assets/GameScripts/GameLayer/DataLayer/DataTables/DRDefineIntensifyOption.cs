//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.064
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
    /// DefineIntensifyOption。
    /// </summary>
    public class DRDefineIntensifyOption : DataRowBase
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
        /// 获取选择后可解锁的选项列表。
        /// </summary>
        public int[] UnlockOptions
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取随机权重。
        /// </summary>
        public int RandomProb
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否自动添加到随机池中。
        /// </summary>
        public bool AutoInPool
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取品质等级。
        /// </summary>
        public int Quality
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取图标名。
        /// </summary>
        public string Icon
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取描述。
        /// </summary>
        public string Desc
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取使用的buff模板id。
        /// </summary>
        public int BuffTemplateId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取buff的一些参数，这些参数是逻辑使用的，比如wow中牧师的盾还能吸收多少伤害，就可以记录在buffParam里面。
        /// </summary>
        public Dictionary<string, string> BuffParam
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取持久性buff。
        /// </summary>
        public bool BuffIsPermanent
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取唯一性。
        /// </summary>
        public bool IsUniqueness
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取buff的目标对象Tag筛选。
        /// </summary>
        public EnumCharacterTag[] BuffTargetTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取指定强化士兵Id。
        /// </summary>
        public int[] SpecifiedSoldierId
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
            UnlockOptions = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            RandomProb = int.Parse(columnStrings[index++]);
            AutoInPool = bool.Parse(columnStrings[index++]);
            Quality = int.Parse(columnStrings[index++]);
            Icon = columnStrings[index++];
            Desc = columnStrings[index++];
            BuffTemplateId = int.Parse(columnStrings[index++]);
            BuffParam = DataTableRuntimeParseTools.ParseDicStrStr(columnStrings[index++]);
            BuffIsPermanent = bool.Parse(columnStrings[index++]);
            IsUniqueness = bool.Parse(columnStrings[index++]);
            BuffTargetTypes = DataTableRuntimeParseTools.ParseEnumCharacterTagArray(columnStrings[index++]);
            SpecifiedSoldierId = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);

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
