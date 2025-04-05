//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.217
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
    /// DefineTerrain。
    /// </summary>
    public class DRDefineTerrain : DataRowBase
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
        /// 获取风格序号。
        /// </summary>
        public int Style
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取循环序号，是从屏幕顶部往底部排序。
        /// </summary>
        public int Scroll
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取层级0。
        /// </summary>
        public string Layer0
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取层级2。
        /// </summary>
        public string Layer2
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取层级7。
        /// </summary>
        public string Layer7
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
            Style = int.Parse(columnStrings[index++]);
            Scroll = int.Parse(columnStrings[index++]);
            Layer0 = columnStrings[index++];
            Layer2 = columnStrings[index++];
            Layer7 = columnStrings[index++];

            GeneratePropertyArray();
            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            return true;
        }

        private KeyValuePair<int, string>[] m_Layer = null;

        public int LayerCount
        {
            get
            {
                return m_Layer.Length;
            }
        }

        public string GetLayer(int id)
        {
            foreach (KeyValuePair<int, string> i in m_Layer)
            {
                if (i.Key == id)
                {
                    return i.Value;
                }
            }

            throw new GameFrameworkException(Utility.Text.Format("GetLayer with invalid id '{0}'.", id));
        }

        public string GetLayerAt(int index)
        {
            if (index < 0 || index >= m_Layer.Length)
            {
                throw new GameFrameworkException(Utility.Text.Format("GetLayerAt with invalid index '{0}'.", index));
            }

            return m_Layer[index].Value;
        }

        private void GeneratePropertyArray()
        {
            m_Layer = new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, Layer0),
                new KeyValuePair<int, string>(2, Layer2),
                new KeyValuePair<int, string>(7, Layer7),
            };
        }
    }
}
