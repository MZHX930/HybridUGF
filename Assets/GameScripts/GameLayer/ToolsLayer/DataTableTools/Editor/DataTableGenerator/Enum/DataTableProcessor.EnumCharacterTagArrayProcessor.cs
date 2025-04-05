using System;
using System.IO;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EnumCharacterTagArrayProcessor : GenericDataProcessor<EnumCharacterTag[]>
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
                    return "EnumCharacterTag[]";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EnumCharacterTag[]",
                };
            }

            public override EnumCharacterTag[] Parse(string value)
            {
                string[] splitedValue = value.Split(',');
                EnumCharacterTag[] resultArray = new EnumCharacterTag[splitedValue.Length];

                for (int i = 0; i < splitedValue.Length; i++)
                {
                    if (int.TryParse(value, out int enumValue))
                    {
                        if (Enum.IsDefined(typeof(EnumCharacterTag), enumValue))
                            resultArray[i] = (EnumCharacterTag)enumValue;
                    }
                    else if (Enum.TryParse<EnumCharacterTag>(value, true, out EnumCharacterTag result))
                    {
                        resultArray[i] = result;
                    }
                    else
                        throw new Exception($"CharacterTagEnum枚举转换失败: {value}");
                }
                return resultArray;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write(Parse(value).ToString());
            }
        }
    }
}
