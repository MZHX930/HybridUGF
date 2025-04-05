using System.IO;
using UnityEngine;
using GameDevScript;
using System;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        /// <summary>
        /// 当有子弹离开AOE范围的时候触发的事件
        /// </summary>
        private sealed class AoeOnBulletLeaveProcessor : GenericDataProcessor<string>
        {
            public override bool IsSystem
            {
                get
                {
                    return false;
                }
            }

            public override string LanguageKeyword
            {
                get
                {
                    return "AoeOnBulletLeave";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "AoeOnBulletLeave"
                };
            }

            public override string Parse(string value)
            {
                return value;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write(value);
            }
        }
    }
}