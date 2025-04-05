using System.IO;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class Int64ArrayProcessor : GenericDataProcessor<long[]>
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
                    return "long[]";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "long[]",
                    "int64[]",
                    "system.int64[]"
                };
            }

            public override long[] Parse(string value)
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    return new long[] { };

                string[] splitedValue = value.Split(',');
                long[] parsedValue = new long[splitedValue.Length];
                for (int i = 0; i < splitedValue.Length; i++)
                    parsedValue[i] = long.Parse(splitedValue[i]);
                return parsedValue;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                long[] parsedValue = Parse(value);
                if (parsedValue.Length > 0)
                {
                    for (int i = 0; i < parsedValue.Length; i++)
                        binaryWriter.Write(parsedValue[i]);
                }
            }
        }
    }
}
