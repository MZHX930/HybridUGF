//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.150
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
    /// DefineMainChapter。
    /// </summary>
    public class DRDefineMainChapter : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取关卡编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取场景编号。
        /// </summary>
        public int SceneId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取外面小图标名。
        /// </summary>
        public int IconIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取怪物图标id。
        /// </summary>
        public int MonsterIcon
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取怪物缩放。
        /// </summary>
        public float MonsterScale
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取标题。
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取宝箱奖励波次。
        /// </summary>
        public int[] BoxUnlock
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取宝箱1奖励id。
        /// </summary>
        public int[] Box1RewardIds
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取宝箱1奖励数量。
        /// </summary>
        public long[] Box1RewardCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取宝箱2奖励id。
        /// </summary>
        public int[] Box2RewardIds
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取宝箱2奖励数量。
        /// </summary>
        public long[] Box2RewardCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取宝箱3奖励id。
        /// </summary>
        public int[] Box3RewardIds
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取宝箱3奖励数量。
        /// </summary>
        public long[] Box3RewardCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取通关胜利奖励id。
        /// </summary>
        public int[] VictoryRewardIds
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取通关胜利奖励数量。
        /// </summary>
        public long[] VictoryRewardCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取通关失败奖励id。
        /// </summary>
        public int[] FailureRewardIds
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取通关失败奖励数量。
        /// </summary>
        public long[] FailureRewardCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取扫荡奖励id。
        /// </summary>
        public int[] SweepRewardIds
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取扫荡奖励数量。
        /// </summary>
        public long[] SweepRewardCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取挂机奖励资源id。
        /// </summary>
        public int[] AdventureRewardIds
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取挂机奖励数量（每分钟）。
        /// </summary>
        public float[] AdventureRewardCounts
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取关卡通关人数计算天数偏移量。
        /// </summary>
        public float LevelClearDaysOffset
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
            SceneId = int.Parse(columnStrings[index++]);
            IconIndex = int.Parse(columnStrings[index++]);
            MonsterIcon = int.Parse(columnStrings[index++]);
            MonsterScale = float.Parse(columnStrings[index++]);
            Title = columnStrings[index++];
            BoxUnlock = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            Box1RewardIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            Box1RewardCount = DataTableRuntimeParseTools.ParseLongArray(columnStrings[index++]);
            Box2RewardIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            Box2RewardCount = DataTableRuntimeParseTools.ParseLongArray(columnStrings[index++]);
            Box3RewardIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            Box3RewardCount = DataTableRuntimeParseTools.ParseLongArray(columnStrings[index++]);
            VictoryRewardIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            VictoryRewardCount = DataTableRuntimeParseTools.ParseLongArray(columnStrings[index++]);
            FailureRewardIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            FailureRewardCount = DataTableRuntimeParseTools.ParseLongArray(columnStrings[index++]);
            SweepRewardIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            SweepRewardCount = DataTableRuntimeParseTools.ParseLongArray(columnStrings[index++]);
            AdventureRewardIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            AdventureRewardCounts = DataTableRuntimeParseTools.ParseFloatArray(columnStrings[index++]);
            LevelClearDaysOffset = float.Parse(columnStrings[index++]);

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
