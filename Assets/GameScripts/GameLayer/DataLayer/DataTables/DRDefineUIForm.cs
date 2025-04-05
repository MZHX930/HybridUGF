//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:51.816
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
    /// DefineUIForm。
    /// </summary>
    public class DRDefineUIForm : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取界面编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public string AssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取界面组名称。
        /// </summary>
        public string UIGroupName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否允许多个界面实例。
        /// </summary>
        public bool AllowMultiInstance
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否暂停被其覆盖的界面。
        /// </summary>
        public bool PauseCoveredUIForm
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取打开界面时播放音效 -1表示没有。
        /// </summary>
        public int OpenSoundId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取关闭界面时播放音效 -1表示没有。
        /// </summary>
        public int CloseSoundId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取是否全屏，渲染非安全区域。
        /// </summary>
        public bool IsRenderOutsideSafeArea
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Open动画委托。
        /// </summary>
        public string OpenAnim
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Open动画委托参数。
        /// </summary>
        public string[] OpenAnimParam
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Close动画委托。
        /// </summary>
        public string CloseAnim
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取Close动画委托参数。
        /// </summary>
        public string[] CloseAnimParam
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
            AssetName = columnStrings[index++];
            UIGroupName = columnStrings[index++];
            AllowMultiInstance = bool.Parse(columnStrings[index++]);
            PauseCoveredUIForm = bool.Parse(columnStrings[index++]);
            OpenSoundId = int.Parse(columnStrings[index++]);
            CloseSoundId = int.Parse(columnStrings[index++]);
            IsRenderOutsideSafeArea = bool.Parse(columnStrings[index++]);
            OpenAnim = columnStrings[index++];
            OpenAnimParam = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);
            CloseAnim = columnStrings[index++];
            CloseAnimParam = DataTableRuntimeParseTools.ParseStringArray(columnStrings[index++]);

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
