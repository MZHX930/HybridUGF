//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.087
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
    /// DefineMainChapterAction。
    /// </summary>
    public class DRDefineMainChapterAction : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取编号Level*10000+Round。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取关卡等级。
        /// </summary>
        public int Level
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取关卡轮。
        /// </summary>
        public int Round
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取类型0:招募 1：战斗 2：加血事件。
        /// </summary>
        public int BoutType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取提示类型 0无 1boss。
        /// </summary>
        public int NoticeType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取参数。
        /// </summary>
        public int[] ActionParams
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
            Level = int.Parse(columnStrings[index++]);
            Round = int.Parse(columnStrings[index++]);
            BoutType = int.Parse(columnStrings[index++]);
            NoticeType = int.Parse(columnStrings[index++]);
            ActionParams = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);

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
