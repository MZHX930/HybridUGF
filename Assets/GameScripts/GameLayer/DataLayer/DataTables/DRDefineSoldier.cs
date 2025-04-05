//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.599
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
    /// DefineSoldier。
    /// </summary>
    public class DRDefineSoldier : DataRowBase
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
        /// 获取士兵标签。
        /// </summary>
        public int[] Tags
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取模型视图的资源路径。
        /// </summary>
        public int ViewPrefixPath
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取稀有度。
        /// </summary>
        public int Rarity
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取占用格子类型。
        /// </summary>
        public int Shape
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取技能发射器id。
        /// </summary>
        public int[] LauncherIds
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取技能强化选项解锁等级。
        /// </summary>
        public int[] BuffUnlockLvs
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取技能强化选项的id。
        /// </summary>
        public int[] BuffIds
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
            Tags = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            ViewPrefixPath = int.Parse(columnStrings[index++]);
            Rarity = int.Parse(columnStrings[index++]);
            Shape = int.Parse(columnStrings[index++]);
            LauncherIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            BuffUnlockLvs = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            BuffIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);

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
