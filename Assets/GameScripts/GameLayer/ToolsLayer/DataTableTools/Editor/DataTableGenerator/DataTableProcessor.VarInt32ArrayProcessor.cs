using System.IO;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class VarInt32ArrayProcessor : GenericDataProcessor<VarInt32[]>
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
                    return "VarInt32[]";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "VarInt32[]"
                };
            }

            public override VarInt32[] Parse(string value)
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    return new VarInt32[] { };

                string[] splitedValue = value.Split(',');
                VarInt32[] parsedValue = new VarInt32[splitedValue.Length];
                for (int i = 0; i < splitedValue.Length; i++)
                    parsedValue[i] = int.Parse(splitedValue[i]);
                return parsedValue;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                VarInt32[] parsedValue = Parse(value);
                if (parsedValue.Length > 0)
                {
                    for (int i = 0; i < parsedValue.Length; i++)
                        binaryWriter.Write(parsedValue[i]);
                }
            }
        }
    }
}
