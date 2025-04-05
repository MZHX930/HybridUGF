using System;
using System.IO;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class EnumTutorialTriggerTypeProcessor : GenericDataProcessor<EnumTutorialTriggerType>
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
                    return "EnumTutorialTriggerType";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "EnumTutorialTriggerType"
                };
            }

            public override EnumTutorialTriggerType Parse(string value)
            {
                if (int.TryParse(value, out int enumValue))
                {
                    if (Enum.IsDefined(typeof(EnumTutorialTriggerType), enumValue))
                        return (EnumTutorialTriggerType)enumValue;
                }
                else if (Enum.TryParse<EnumTutorialTriggerType>(value, true, out EnumTutorialTriggerType result))
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
