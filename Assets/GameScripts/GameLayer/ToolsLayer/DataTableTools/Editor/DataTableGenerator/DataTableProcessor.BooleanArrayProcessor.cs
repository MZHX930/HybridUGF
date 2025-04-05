//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.IO;
using System.Linq;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class BooleanArrayProcessor : GenericDataProcessor<bool[]>
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
                    return "bool[]";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "bool[]",
                    "boolean[]",
                    "system.boolean[]"
                };
            }

            public override bool[] Parse(string value)
            {
                string[] strArray = value.Split(',');
                bool[] boolArray = new bool[strArray.Length];
                for (int i = 0; i < strArray.Length; i++)
                    boolArray[i] = bool.Parse(strArray[i]);
                return boolArray;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                bool[] parsedValue = Parse(value);
                for (int i = 0; i < parsedValue.Length; i++)
                {
                    binaryWriter.Write(parsedValue[i]);
                }
            }
        }
    }
}
