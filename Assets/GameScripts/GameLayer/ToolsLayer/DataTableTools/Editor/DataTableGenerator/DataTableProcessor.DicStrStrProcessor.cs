using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class DicStrStrProcessor : GenericDataProcessor<Dictionary<string, string>>
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
                    return "Dictionary<string, string>";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "DicStrStr",
                };
            }

            public override Dictionary<string, string> Parse(string value)
            {
                string[] splitedValue = value.Split(';');
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (var item in splitedValue)
                {
                    string[] keyValue = item.Split(':');
                    result.Add(keyValue[0], keyValue[1]);
                }
                return result;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                Dictionary<string, string> dic = Parse(value);
                foreach (var item in dic)
                {
                    binaryWriter.Write(item.Key);
                    binaryWriter.Write(item.Value);
                }
            }
        }
    }
}