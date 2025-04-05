//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.193
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
    /// DefineScene。
    /// </summary>
    public class DRDefineScene : DataRowBase
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
        /// 获取资源名。
        /// </summary>
        public string SceneName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取背景音乐。
        /// </summary>
        public int MusicId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取风格id。
        /// </summary>
        public int StyleId
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
            SceneName = columnStrings[index++];
            MusicId = int.Parse(columnStrings[index++]);
            StyleId = int.Parse(columnStrings[index++]);

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
