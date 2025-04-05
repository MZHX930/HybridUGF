//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.375
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
    /// DefineGameTutorial。
    /// </summary>
    public class DRDefineGameTutorial : DataRowBase
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
        /// 获取引导组。
        /// </summary>
        public int Group
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取组步骤。
        /// </summary>
        public int Step
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取激活步骤条件。
        /// </summary>
        public EnumTutorialTriggerType StartType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取激活步骤参数。
        /// </summary>
        public string[] StartParams
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取遮罩模式 1:矩形 2:圆形 3:全遮罩。
        /// </summary>
        public int FocusType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取聚焦点。
        /// </summary>
        public string FocusPath
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取引导文本id。
        /// </summary>
        public string DescId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取结束步骤条件。
        /// </summary>
        public EnumTutorialTriggerType EndType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取结束步骤参数。
        /// </summary>
        public string[] EndParams
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
            Group = int.Parse(columnStrings[index++]);
            Step = int.Parse(columnStrings[index++]);
            StartType = DataTableRuntimeParseTools.ParseEnumTutorialTriggerType(columnStrings[index++]);
            StartParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            FocusType = int.Parse(columnStrings[index++]);
            FocusPath = columnStrings[index++];
            DescId = columnStrings[index++];
            EndType = DataTableRuntimeParseTools.ParseEnumTutorialTriggerType(columnStrings[index++]);
            EndParams = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);

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
