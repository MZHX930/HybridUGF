//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.464
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
    /// DefineSkillLauncher。
    /// </summary>
    public class DRDefineSkillLauncher : DataRowBase
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
        /// 获取技能使用的条件。对应角色属性表的id。
        /// </summary>
        public int Condition
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取技能的消耗。技能使用的条件。。
        /// </summary>
        public int Cost
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取索敌半径 米。
        /// </summary>
        public float SearchRadius
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取索敌逻辑1随机 2最近单位。
        /// </summary>
        public int SearchType
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取释放间隔 秒。
        /// </summary>
        public float CD
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取发射的技能效果的时间轴id。
        /// </summary>
        public int TimeLineId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取添加的buff信息id；DefineAddBuffInfo。
        /// </summary>
        public int[] AddBuffInfoIds
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
            Condition = int.Parse(columnStrings[index++]);
            Cost = int.Parse(columnStrings[index++]);
            SearchRadius = float.Parse(columnStrings[index++]);
            SearchType = int.Parse(columnStrings[index++]);
            CD = float.Parse(columnStrings[index++]);
            TimeLineId = int.Parse(columnStrings[index++]);
            AddBuffInfoIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);

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
