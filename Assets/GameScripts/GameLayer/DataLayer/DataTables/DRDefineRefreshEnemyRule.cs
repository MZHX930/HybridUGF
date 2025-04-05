//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.171
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
    /// DefineRefreshEnemyRule。
    /// </summary>
    public class DRDefineRefreshEnemyRule : DataRowBase
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
        /// 获取延迟多久执行规则，秒。
        /// </summary>
        public float Delay
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取每次创造间隔时间，秒。
        /// </summary>
        public float Interval
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取每次创建的数量。
        /// </summary>
        public int EachCreateCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取总共要创建数量。
        /// </summary>
        public int MonsterCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取怪物模型id。
        /// </summary>
        public int MonsterId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取属性衰减因子。
        /// </summary>
        public float ChaPropertyFactor
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
            Delay = float.Parse(columnStrings[index++]);
            Interval = float.Parse(columnStrings[index++]);
            EachCreateCount = int.Parse(columnStrings[index++]);
            MonsterCount = int.Parse(columnStrings[index++]);
            MonsterId = int.Parse(columnStrings[index++]);
            ChaPropertyFactor = float.Parse(columnStrings[index++]);

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
