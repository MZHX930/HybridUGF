using System.IO;
using UnityEngine;
using GameDevScript;
using System;

namespace GameDevScript.EditorTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class LogicTimeLineEventProcessor : GenericDataProcessor<string>
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
                    return "LogicTimeLineNodeEvent";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "LogicTimeLineNodeEvent"
                };
            }

            public override string Parse(string value)
            {
                return value;
            }

            public override void WriteToStream(DataTableProcessor dataTableProcessor, BinaryWriter binaryWriter, string value)
            {
                binaryWriter.Write(value);
            }
        }
    }
}
