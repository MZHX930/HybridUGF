//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.792
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
    /// DefineTimeLineNode。
    /// </summary>
    public class DRDefineTimeLineNode : DataRowBase
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
        /// 获取运行多久(秒)后发生。
        /// </summary>
        public float TimeElapsed
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取执行的事件名。
        /// </summary>
        public LogicTimeLineNodeEvent DoEventName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取参数。
        /// </summary>
        public VarInt32[] EventArgs
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
            TimeElapsed = float.Parse(columnStrings[index++]);
            DoEventName = DataTableRuntimeParseTools.ParseLogicTimeLineNodeEvent(columnStrings[index++]);
            EventArgs = DataTableRuntimeParseTools.ParseVarInt32Array(columnStrings[index++]);

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
