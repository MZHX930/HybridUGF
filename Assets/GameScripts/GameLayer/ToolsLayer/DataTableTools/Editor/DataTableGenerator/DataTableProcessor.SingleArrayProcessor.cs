using System.IO;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class SingleArrayProcessor : GenericDataProcessor<float[]>
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
                    return "float[]";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "float[]",
                    "Single[]",
                    "system.Single[]"
                };
            }

            public override float[] Parse(string value)
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    return new float[] { };

                var splitedValue = value.Split(',');
                var parsedValue = new float[splitedValue.Length];
                for (int i = 0; i < splitedValue.Length; i++)
                    parsedValue[i] = float.Parse(splitedValue[i]);
                return parsedValue;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                var parsedValue = Parse(value);
                if (parsedValue.Length > 0)
                {
                    for (int i = 0; i < parsedValue.Length; i++)
                        binaryWriter.Write(parsedValue[i]);
                }
            }
        }
    }
}
