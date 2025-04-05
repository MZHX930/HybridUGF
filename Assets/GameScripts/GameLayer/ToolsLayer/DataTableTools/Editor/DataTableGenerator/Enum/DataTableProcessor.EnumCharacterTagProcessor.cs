using System;
using System.IO;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EnumCharacterTagTagProcessor : GenericDataProcessor<EnumCharacterTag>
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
                    return "EnumCharacterTag";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EnumCharacterTag"
                };
            }

            public override EnumCharacterTag Parse(string value)
            {
                if (int.TryParse(value, out int enumValue))
                {
                    if (Enum.IsDefined(typeof(EnumCharacterTag), enumValue))
                        return (EnumCharacterTag)enumValue;
                }
                else if (Enum.TryParse<EnumCharacterTag>(value, true, out EnumCharacterTag result))
                {
                    return result;
                }

                throw new Exception($"EnumCharacterTag枚举转换失败: {value}");
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write(Parse(value).ToString());
            }
        }
    }
}
