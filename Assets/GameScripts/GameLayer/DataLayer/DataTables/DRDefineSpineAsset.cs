//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.337
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
    /// DefineSpineAsset。
    /// </summary>
    public class DRDefineSpineAsset : DataRowBase
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
        /// 获取组。
        /// </summary>
        public int Group
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取序号。
        /// </summary>
        public int Index
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取变体ID。
        /// </summary>
        public int Lv
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取spine资源路径。
        /// </summary>
        public string Path
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
            Index = int.Parse(columnStrings[index++]);
            Lv = int.Parse(columnStrings[index++]);
            Path = columnStrings[index++];

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
