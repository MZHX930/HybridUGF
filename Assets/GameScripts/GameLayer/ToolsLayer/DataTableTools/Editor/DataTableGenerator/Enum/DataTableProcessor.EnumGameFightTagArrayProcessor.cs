using System;
using System.IO;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EnumGameFightTagArrayProcessor : GenericDataProcessor<EnumGameFightTag[]>
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
                    return "EnumGameFightTag[]";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EnumGameFightTag[]",
                };
            }

            public override EnumGameFightTag[] Parse(string value)
            {
                string[] splitedValue = value.Split(',');
                EnumGameFightTag[] resultArray = new EnumGameFightTag[splitedValue.Length];

                for (int i = 0; i < splitedValue.Length; i++)
                {
                    if (int.TryParse(value, out int enumValue))
                    {
                        if (Enum.IsDefined(typeof(EnumGameFightTag), enumValue))
                            resultArray[i] = (EnumGameFightTag)enumValue;
                    }
                    else if (Enum.TryParse<EnumGameFightTag>(value, true, out EnumGameFightTag result))
                    {
                        resultArray[i] = result;
                    }
                    else
                        throw new Exception($"GameFightTagEnum枚举转换失败: {value}");
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
