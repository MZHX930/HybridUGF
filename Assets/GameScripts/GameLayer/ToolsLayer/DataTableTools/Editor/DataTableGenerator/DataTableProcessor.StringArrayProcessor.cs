using System;
using System.IO;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class StringArrayProcessor : GenericDataProcessor<string[]>
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
                    return "string[]";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "string[]",
                    "System.String[]"
                };
            }

            public override string[] Parse(string value)
            {
                return value.Split(';');
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                string[] parsedValue = Parse(value);
                for (int i = 0; i < parsedValue.Length; i++)
                {
                    binaryWriter.Write(parsedValue[i]);
                }
            }
        }
    }
}
