//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.513
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
    /// DataSoldierLvProperty。
    /// </summary>
    public class DRDataSoldierLvProperty : DataRowBase
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
        /// 获取升级到下一级的消耗道具类型。
        /// </summary>
        public int[] UpgradePropsArray
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取升级到下一级的消耗道具数量。
        /// </summary>
        public int[] UpgradePropsCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取攻击力。
        /// </summary>
        public int Attack
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取移动速度因子。
        /// </summary>
        public int MoveSpeedFactor
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取行动速度因子。
        /// </summary>
        public int ActionSpeedFactor
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
            UpgradePropsArray = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            UpgradePropsCount = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            Attack = int.Parse(columnStrings[index++]);
            MoveSpeedFactor = int.Parse(columnStrings[index++]);
            ActionSpeedFactor = int.Parse(columnStrings[index++]);

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
