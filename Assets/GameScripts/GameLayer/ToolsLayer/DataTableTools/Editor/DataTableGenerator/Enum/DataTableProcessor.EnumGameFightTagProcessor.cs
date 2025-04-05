using System;
using System.IO;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EnumGameFightTagProcessor : GenericDataProcessor<EnumGameFightTag>
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
                    return "EnumGameFightTag";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EnumGameFightTag"
                };
            }

            public override EnumGameFightTag Parse(string value)
            {
                if (int.TryParse(value, out int enumValue))
                {
                    if (Enum.IsDefined(typeof(EnumGameFightTag), enumValue))
                        return (EnumGameFightTag)enumValue;
                }
                else if (Enum.TryParse<EnumGameFightTag>(value, true, out EnumGameFightTag result))
                {
                    return result;
                }

                throw new Exception($"GameFightTagEnum枚举转换失败: {value}");
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write(Parse(value).ToString());
            }
        }
    }
}
