//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.270
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
    /// DefineGameProps。
    /// </summary>
    public class DRDefineGameProps : DataRowBase
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
        /// 获取品质。
        /// </summary>
        public int Quality
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取分类，按照用途分类。
        /// </summary>
        public int Categroy
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取分类参数。
        /// </summary>
        public int[] CategroyParams
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
        /// 获取小图标，用于UI界面。
        /// </summary>
        public string SmallIcon
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取大图标，用于商店界面。
        /// </summary>
        public string BigIcon
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
            Quality = int.Parse(columnStrings[index++]);
            Categroy = int.Parse(columnStrings[index++]);
            CategroyParams = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            Desc = columnStrings[index++];
            SmallIcon = columnStrings[index++];
            BigIcon = columnStrings[index++];

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
