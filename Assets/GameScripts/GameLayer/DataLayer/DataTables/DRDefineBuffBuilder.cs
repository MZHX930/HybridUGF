//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.719
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
    /// DefineBuffBuilder。
    /// </summary>
    public class DRDefineBuffBuilder : DataRowBase
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
        /// 获取使用的buff模板id。
        /// </summary>
        public int TemplateId
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
        /// 获取要添加的层数，负数则为减少。
        /// </summary>
        public int AddStack
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否是一个永久的buff，即便=true，时间设置也是有意义的，因为时间如果被减少到0以下，即使是永久的也会被删除。
        /// </summary>
        public bool Permanent
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取关于时间，是改变还是设置为, true代表覆盖，false代表叠加。
        /// </summary>
        public bool DurationSetTo
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取时间值，设置为这个值，或者加上这个值，单位：秒。
        /// </summary>
        public float Duration
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
            TemplateId = int.Parse(columnStrings[index++]);
            BuffParam = DataTableRuntimeParseTools.ParseDicStrStr(columnStrings[index++]);
            AddStack = int.Parse(columnStrings[index++]);
            Permanent = bool.Parse(columnStrings[index++]);
            DurationSetTo = bool.Parse(columnStrings[index++]);
            Duration = float.Parse(columnStrings[index++]);

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
