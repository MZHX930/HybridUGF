using System.IO;
using UnityEngine;
using GameDevScript;
using System;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        /// <summary>
        /// 子弹的轨迹函数
        /// </summary>
        private sealed class BulletTweenProcessor : GenericDataProcessor<string>
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
                    return "BulletTween";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "BulletTween"
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
