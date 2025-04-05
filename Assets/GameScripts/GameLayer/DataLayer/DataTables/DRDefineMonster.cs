//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2025-04-03 17:09:52.578
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
    /// DefineMonster。
    /// </summary>
    public class DRDefineMonster : DataRowBase
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
        /// 获取角色标签。
        /// </summary>
        public EnumCharacterTag[] Tags
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取模型视图的资源路径。
        /// </summary>
        public string ViewPath
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
        /// 获取技能发射器id。
        /// </summary>
        public int[] LauncherIds
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取击中半径。
        /// </summary>
        public float HitRadius
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取最大生命。
        /// </summary>
        public int HP
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取最大魔力。
        /// </summary>
        public int MP
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
        /// 获取移动速度。
        /// </summary>
        public int MoveSpeed
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取行动速度。
        /// </summary>
        public int ActionSpeed
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
            Tags = DataTableRuntimeParseTools.ParseEnumCharacterTagArray(columnStrings[index++]);
            ViewPath = columnStrings[index++];
            Rarity = int.Parse(columnStrings[index++]);
            LauncherIds = DataTableRuntimeParseTools.ParseIntArray(columnStrings[index++]);
            HitRadius = float.Parse(columnStrings[index++]);
            HP = int.Parse(columnStrings[index++]);
            MP = int.Parse(columnStrings[index++]);
            Attack = int.Parse(columnStrings[index++]);
            MoveSpeed = int.Parse(columnStrings[index++]);
            ActionSpeed = int.Parse(columnStrings[index++]);

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
