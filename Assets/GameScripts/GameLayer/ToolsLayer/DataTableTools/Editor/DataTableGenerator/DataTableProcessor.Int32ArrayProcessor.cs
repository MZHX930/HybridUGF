using System.IO;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class Int32ArrayProcessor : GenericDataProcessor<int[]>
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
                    return "int[]";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "int[]",
                    "int32[]",
                    "system.int32[]"
                };
            }

            public override int[] Parse(string value)
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    return new int[] { };

                string[] splitedValue = value.Split(',');
                int[] parsedValue = new int[splitedValue.Length];
                for (int i = 0; i < splitedValue.Length; i++)
                    parsedValue[i] = int.Parse(splitedValue[i]);
                return parsedValue;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                int[] parsedValue = Parse(value);
                if (parsedValue.Length > 0)
                {
                    for (int i = 0; i < parsedValue.Length; i++)
                        binaryWriter.Write(parsedValue[i]);
                }
            }
        }
    }
}
