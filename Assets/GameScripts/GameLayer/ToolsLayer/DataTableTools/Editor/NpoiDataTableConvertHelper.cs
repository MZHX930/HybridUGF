using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;


namespace GameDevScript.EditorTools
{
    /// <summary>
    /// NPOI的数据表转换辅助类
    /// </summary>
    public static class NpoiDataTableConvertHelper
    {
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        public static string GetDataTableName(string xlsxFullPath)
        {
            if (!File.Exists(xlsxFullPath))
            {
                Debug.LogError($"不存在xlsx文件:{xlsxFullPath}");
                return null;
            }

            using (FileStream fs = new FileStream(xlsxFullPath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                XSSFWorkbook workbook = new XSSFWorkbook(fs);
                ISheet dataTableSheet = workbook.GetSheetAt(0);//第一个sheet页（列表）
                IRow firstRowData = dataTableSheet.GetRow(0);
                ICell tableNameCellData = firstRowData.GetCell(0);
                return tableNameCellData.StringCellValue;
            }
        }


        /// <summary>
        /// 解析xlsx表，生成txt和cs
        /// </summary>
        public static void ParseXlsx2DataTable(string xlsxFullPath, string unencryptedTextFolderPath, string encryptedTextFolderPath, string csFolderPath, string scriptNamespace, bool needExportCs)
        {
            if (!File.Exists(xlsxFullPath))
            {
                Debug.LogError($"不存在xlsx文件:{xlsxFullPath}");
                return;
            }

            using (FileStream fs = new FileStream(xlsxFullPath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                XSSFWorkbook workbook = new XSSFWorkbook(fs);
                ISheet dataTableSheet = workbook.GetSheetAt(0);//第一个sheet页（列表）

                if (dataTableSheet.SheetName.Equals("DefineConstant"))
                {
                    GenerateGameConst(dataTableSheet);
                }
                else if (dataTableSheet.SheetName.StartsWith("Enum"))
                {
                    GenerateEnumFile(dataTableSheet);
                }
                else if (dataTableSheet.SheetName.Contains("_vertical"))
                {
                    InnerXlsx2DataTable_Vertical(dataTableSheet, xlsxFullPath, unencryptedTextFolderPath, encryptedTextFolderPath, csFolderPath, scriptNamespace, needExportCs);
                }
                else
                {
                    InnerXlsx2DataTable_Horizontal(dataTableSheet, xlsxFullPath, unencryptedTextFolderPath, encryptedTextFolderPath, csFolderPath, scriptNamespace, needExportCs);
                }
            }
        }

        private static bool IsIgnoreField(string str)
        {
            if (string.IsNullOrEmpty(str) || str.StartsWith("#") || string.IsNullOrWhiteSpace(str))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 水平读取表格
        /// </summary>
        private static string InnerXlsx2DataTable_Horizontal(ISheet dataTableSheet, string xlsxFullPath, string unencryptedTextFolderPath, string encryptedTextFolderPath, string csFolderPath, string scriptNamespace, bool needExportCs)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(xlsxFullPath);
            int maxRowIndex = dataTableSheet.LastRowNum;//行数

            //第一行是表名
            IRow firstRowData = dataTableSheet.GetRow(0);
            ICell tableNameCellData = firstRowData.GetCell(0);
            string dataTableName = tableNameCellData.StringCellValue;

            List<int> validCellIndexList = new List<int>();//有效的字段索引
            List<string> _DataTableFieldNameList = new List<string>();//字段名
            List<string> _DataTableFieldTypeList = new List<string>();//字段类型
            List<string> _DataTableFieldDescList = new List<string>();//字段描述

            //第二行是字段名;第三行是字段类型;第四行是字段描述
            int mayFieldCount = dataTableSheet.GetRow(1).LastCellNum;
            for (int i = 0; i <= mayFieldCount; i++)
            {
                //字段名
                ICell fieldNameCell = dataTableSheet.GetRow(1).GetCell(i);
                if (fieldNameCell == null)
                    continue;
                string fieldNameStr = dataTableSheet.GetRow(1).GetCell(i).StringCellValue;
                if (IsIgnoreField(fieldNameStr))
                    continue;
                //字段类型
                ICell fieldTypeCell = dataTableSheet.GetRow(2).GetCell(i);
                if (fieldTypeCell == null)
                    continue;
                string fieldTypeStr = dataTableSheet.GetRow(2).GetCell(i).StringCellValue;
                if (IsIgnoreField(fieldTypeStr))
                    continue;
                //字段描述
                string fieldDescStr = "";
                ICell fieldDescCell = dataTableSheet.GetRow(3).GetCell(i);
                if (fieldDescCell != null)
                    fieldDescStr = fieldDescCell.StringCellValue;

                _DataTableFieldNameList.Add(fieldNameStr);
                _DataTableFieldTypeList.Add(fieldTypeStr);
                _DataTableFieldDescList.Add(fieldDescStr);

                validCellIndexList.Add(i);
            }

            //保存为txt>>>>>>>>>>>>>>>>>>
            int filedCountMaxIndex = validCellIndexList.Count - 1;
            StringBuilder sb_DataTableTxt = new StringBuilder();
            //表名
            sb_DataTableTxt.Append($"#\t{dataTableName}");
            for (int i = 1; i <= filedCountMaxIndex; i++)
            {
                sb_DataTableTxt.Append($"\t{i}");
            }
            //字段名
            sb_DataTableTxt.Append("\r\n#");
            for (int i = 0; i <= filedCountMaxIndex; i++)
            {
                sb_DataTableTxt.Append($"\t{_DataTableFieldNameList[i]}");
            }
            //字段类型
            sb_DataTableTxt.Append("\r\n#");
            for (int i = 0; i <= filedCountMaxIndex; i++)
            {
                sb_DataTableTxt.Append($"\t{_DataTableFieldTypeList[i]}");
            }
            //字段描述
            sb_DataTableTxt.Append("\r\n#");
            for (int i = 0; i <= filedCountMaxIndex; i++)
            {
                sb_DataTableTxt.Append($"\t{_DataTableFieldDescList[i]}");
            }

            //第五行开始才是数据
            for (int rowIndex = 4; rowIndex <= maxRowIndex; rowIndex++)
            {
                IRow row = dataTableSheet.GetRow(rowIndex);  //读取当前行数据
                if (row == null)
                    continue;

                if (row.GetCell(0) == null)
                    break; //表示该行数据为空，则结束转表

                string idStr = row.GetCell(0).ToString();
                if (string.IsNullOrEmpty(idStr) || string.IsNullOrWhiteSpace(idStr))
                    break;

                if (idStr.StartsWith("#"))
                    continue;//以#开头的跳过

                sb_DataTableTxt.Append("\r\n");
                for (int i = 0; i < validCellIndexList.Count; i++)
                {
                    int cellIndex = validCellIndexList[i];
                    ICell cellData = row.GetCell(cellIndex);
                    string value = "";
                    if (cellData == null)
                    {
                        //UnityEngine.Debug.LogWarning($"配置表[{fileNameWithoutExtension}]的({rowIndex + 1},{i + 1})为空值！！！");
                    }
                    else
                    {
                        switch (cellData.CellType)
                        {
                            case CellType.Unknown:
                                value.ToString();
                                break;
                            case CellType.Numeric:
                                value = cellData.NumericCellValue.ToString();
                                break;
                            case CellType.String:
                                value = cellData.StringCellValue;
                                break;
                            case CellType.Formula:
                                {
                                    //因为NPOI处理公式有问题，这里简单处理下公式。
                                    value = cellData.NumericCellValue.ToString();
                                    break;
                                }
                            case CellType.Blank:
                                value = "";
                                break;
                            case CellType.Boolean:
                                value = cellData.BooleanCellValue ? "true" : "false";
                                break;
                            case CellType.Error:
                                value.ToString();
                                break;
                            default:
                                break;
                        }
                    }

                    sb_DataTableTxt.Append($"\t{value}");
                }
            }

            SaveDataTableTextFile(sb_DataTableTxt, dataTableName, xlsxFullPath, unencryptedTextFolderPath, encryptedTextFolderPath, csFolderPath, scriptNamespace, needExportCs);

            return dataTableName;
        }

        /// <summary>
        /// 垂直读取表格
        /// </summary>
        private static string InnerXlsx2DataTable_Vertical(ISheet dataTableSheet, string xlsxFullPath, string unencryptedTextFolderPath, string encryptedTextFolderPath, string csFolderPath, string scriptNamespace, bool needExportCs)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(xlsxFullPath);

            //第一行是表名
            IRow firstRowData = dataTableSheet.GetRow(0);
            ICell tableNameCellData = firstRowData.GetCell(0);
            string dataTableName = tableNameCellData.StringCellValue;

            List<int> validRowIndexList = new List<int>();//有效的字段索引
            List<string> _DataTableFieldNameList = new List<string>();//字段名
            List<string> _DataTableFieldTypeList = new List<string>();//字段类型
            List<string> _DataTableFieldDescList = new List<string>();//字段描述

            //第二行是字段名;第三行是字段类型;第四行是字段描述
            IRow secondeRowData = dataTableSheet.GetRow(1);
            int maxColumnIndex = secondeRowData.LastCellNum - 1;//列数
            int mayFieldCount = dataTableSheet.LastRowNum;

            for (int i = 1; i <= mayFieldCount; i++)
            {
                var fieldRowData = dataTableSheet.GetRow(i);
                if (fieldRowData == null)
                    continue;

                //字段名
                ICell fieldNameCell = fieldRowData.GetCell(0);
                if (fieldNameCell == null)
                    continue;
                string fieldNameStr = fieldNameCell.StringCellValue;
                if (IsIgnoreField(fieldNameStr))
                    continue;

                //字段类型
                ICell fieldTypeCell = fieldRowData.GetCell(1);
                if (fieldTypeCell == null)
                    continue;
                string fieldTypeStr = fieldTypeCell.StringCellValue;
                if (IsIgnoreField(fieldTypeStr))
                    continue;

                //字段描述
                string fieldDescStr = "";
                ICell fieldDescCell = fieldRowData.GetCell(2);
                if (fieldDescCell != null)
                    fieldDescStr = fieldDescCell.StringCellValue;

                _DataTableFieldNameList.Add(fieldNameStr);
                _DataTableFieldTypeList.Add(fieldTypeStr);
                _DataTableFieldDescList.Add(fieldDescStr);

                validRowIndexList.Add(i);
            }

            //保存为txt>>>>>>>>>>>>>>>>>>
            int filedCountMaxIndex = validRowIndexList.Count - 1;
            StringBuilder sb_DataTableTxt = new StringBuilder();
            //表名
            sb_DataTableTxt.Append($"#\t{dataTableName}");
            for (int i = 1; i <= filedCountMaxIndex; i++)
            {
                sb_DataTableTxt.Append($"\t{i}");
            }
            //字段名
            sb_DataTableTxt.Append("\r\n#");
            for (int i = 0; i <= filedCountMaxIndex; i++)
            {
                sb_DataTableTxt.Append($"\t{_DataTableFieldNameList[i]}");
            }
            //字段类型
            sb_DataTableTxt.Append("\r\n#");
            for (int i = 0; i <= filedCountMaxIndex; i++)
            {
                sb_DataTableTxt.Append($"\t{_DataTableFieldTypeList[i]}");
            }
            //字段描述
            sb_DataTableTxt.Append("\r\n#");
            for (int i = 0; i <= filedCountMaxIndex; i++)
            {
                sb_DataTableTxt.Append($"\t{_DataTableFieldDescList[i]}");
            }

            //第4列开始才是数据
            for (int columnIndex = 3; columnIndex <= maxColumnIndex; columnIndex++)
            {
                ICell fieldNameCell = secondeRowData.GetCell(columnIndex);
                if (fieldNameCell == null)
                    break; //表示该行数据为空，则结束转表

                string idStr = fieldNameCell.ToString();
                if (string.IsNullOrEmpty(idStr) || string.IsNullOrWhiteSpace(idStr))
                    break;

                if (idStr.StartsWith("#"))
                    continue;//以#开头的跳过

                sb_DataTableTxt.Append("\r\n");

                for (int i = 0; i < validRowIndexList.Count; i++)
                {
                    int vaildRowIndex = validRowIndexList[i];
                    ICell cellData = dataTableSheet.GetRow(vaildRowIndex).GetCell(columnIndex);
                    string value = "";
                    if (cellData == null)
                    {
                        UnityEngine.Debug.LogWarning($"配置表[{fileNameWithoutExtension}]的({vaildRowIndex},{columnIndex})为空值！！！");
                    }
                    else
                    {
                        switch (cellData.CellType)
                        {
                            case CellType.Unknown:
                                value.ToString();
                                break;
                            case CellType.Numeric:
                                value = cellData.NumericCellValue.ToString();
                                break;
                            case CellType.String:
                                value = cellData.StringCellValue;
                                break;
                            case CellType.Formula:
                                {
                                    //因为NPOI处理公式有问题，这里简单处理下公式。
                                    value = cellData.NumericCellValue.ToString();
                                    break;
                                }
                            case CellType.Blank:
                                value = "";
                                break;
                            case CellType.Boolean:
                                value = cellData.BooleanCellValue ? "true" : "false";
                                break;
                            case CellType.Error:
                                value.ToString();
                                break;
                            default:
                                break;
                        }
                    }

                    sb_DataTableTxt.Append($"\t{value}");
                }
            }

            SaveDataTableTextFile(sb_DataTableTxt, dataTableName, xlsxFullPath, unencryptedTextFolderPath, encryptedTextFolderPath, csFolderPath, scriptNamespace, needExportCs);

            return dataTableName;
        }

        private static void SaveDataTableTextFile(StringBuilder drContent, string dataTableName, string xlsxFullPath, string unencryptedTextFolderPath, string encryptedTextFolderPath, string csFolderPath, string scriptNamespace, bool needExportCs)
        {
            //保存未加密的配置表txt
            string decryptTxtFilePath = Path.Combine(unencryptedTextFolderPath, $"{dataTableName}.txt");
            using (StreamWriter sw = new StreamWriter(decryptTxtFilePath, false, Encoding.UTF8))
            {
                sw.Write(drContent.ToString());
                sw.Flush();
                sw.Close();
            }

            //保存加密的配置表txt
            string txtFilePath = Path.Combine(encryptedTextFolderPath, $"{dataTableName}.txt");
            using (StreamWriter sw = new StreamWriter(txtFilePath, false, Encoding.UTF8))
            {
                sw.Write(DataTableEncryptUtility.Encrypt(drContent.ToString()));
                sw.Flush();
                sw.Close();
            }

            if (needExportCs)
            {
                //保存cs
                DataTableGenerator generator = new DataTableGenerator();
                DataTableProcessor dataTableProcessor = generator.CreateDataTableProcessor(decryptTxtFilePath, csFolderPath, Encoding.UTF8);
                if (generator.CheckRawData(dataTableProcessor, dataTableName))
                {
                    generator.GenerateCodeFile(dataTableProcessor, dataTableName, scriptNamespace);
                }
                else
                {
                    Debug.LogError($"配置表转换为cs失败 {dataTableName}");
                }
            }
        }

        /// <summary>
        /// 生成UIFormId
        /// </summary>
        public static void GenerateUIFormIds(string xlsxFullPath)
        {
            if (!File.Exists(xlsxFullPath))
            {
                Debug.LogError($"不存在xlsx文件:{xlsxFullPath}");
                return;
            }

            List<string> uiFormIdList = new List<string>();
            List<string> uiFormNameList = new List<string>();
            List<string> descList = new List<string>();

            using (FileStream fs = new FileStream(xlsxFullPath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                XSSFWorkbook workbook = new XSSFWorkbook(fs);
                ISheet dataTableSheet = workbook.GetSheetAt(0);//第一个sheet页（列表）
                IRow firstRowData = dataTableSheet.GetRow(0);
                ICell tableNameCellData = firstRowData.GetCell(0);

                for (int i = 4; i <= dataTableSheet.LastRowNum; i++)
                {
                    IRow row = dataTableSheet.GetRow(i);
                    ICell cell = row.GetCell(0);
                    if (cell == null)
                        continue;
                    string uiFormIdStr = cell.StringCellValue;
                    if (string.IsNullOrEmpty(uiFormIdStr) || string.IsNullOrWhiteSpace(uiFormIdStr))
                        continue;
                    if (uiFormIdStr.StartsWith("#"))
                        continue;
                    uiFormIdList.Add(uiFormIdStr);
                    if (row.GetCell(1) != null)
                        descList.Add(row.GetCell(1).StringCellValue);
                    else
                        descList.Add("");
                    string name = row.GetCell(2).StringCellValue;
                    string[] splitNames = name.Split(new char[] { '/', '\\' });
                    uiFormNameList.Add(splitNames[splitNames.Length - 1]);
                }
            }

            //覆盖Assets/GameScripts/GameLayer/DataLayer/Definition/UIFormId.cs
            StringBuilder newContent = new StringBuilder();
            newContent.AppendLine("/*脚本生成，请勿手动修改*/");
            newContent.AppendLine("namespace GameDevScript");
            newContent.AppendLine("{");
            newContent.AppendLine("    public enum UIFormId : int");
            newContent.AppendLine("    {");
            newContent.AppendLine("        /// <summary>");
            newContent.AppendLine("        /// 未定义");
            newContent.AppendLine("        /// </summary>");
            newContent.AppendLine("        Undefined = 0,");
            for (int i = 0; i < uiFormIdList.Count; i++)
            {
                newContent.AppendLine($"        /// <summary>");
                newContent.AppendLine($"        /// {descList[i]}");
                newContent.AppendLine($"        /// </summary>");
                newContent.AppendLine($"        {uiFormNameList[i]} = {uiFormIdList[i]},");
            }
            newContent.AppendLine("    }");
            newContent.AppendLine("}");


            string csFilePath = Path.Combine(Application.dataPath, "GameScripts/GameLayer/DataLayer/Definition/UIFormId.cs");
            File.WriteAllText(csFilePath, newContent.ToString(), Encoding.UTF8);
        }


        /// <summary>
        /// 生成枚举文件
        /// </summary>
        private static void GenerateEnumFile(ISheet dataTableSheet)
        {
            string enumName = "";
            List<string> enumIdList = new List<string>();
            List<string> enumNameList = new List<string>();
            List<string> enumDescList = new List<string>();

            IRow firstRowData = dataTableSheet.GetRow(0);
            ICell tableNameCellData = firstRowData.GetCell(0);
            enumName = tableNameCellData.StringCellValue;

            if (string.IsNullOrEmpty(enumName))
                return;

            for (int i = 4; i <= dataTableSheet.LastRowNum; i++)
            {
                IRow row = dataTableSheet.GetRow(i);
                ICell cell = row.GetCell(0);
                if (cell == null)
                    continue;

                string enumId = null;
                if (cell.CellType == CellType.Numeric)
                    enumId = cell.NumericCellValue.ToString();
                else
                    enumId = cell.StringCellValue;

                if (string.IsNullOrEmpty(enumId))
                    continue;

                if (string.IsNullOrEmpty(enumId) || string.IsNullOrWhiteSpace(enumId))
                    continue;
                if (enumId.StartsWith("#"))
                    continue;

                enumIdList.Add(enumId);
                enumNameList.Add(row.GetCell(1).StringCellValue);
                if (row.GetCell(2) != null)
                    enumDescList.Add(row.GetCell(2).StringCellValue);
                else
                    enumDescList.Add("");
            }

            //覆盖Assets/GameScripts/GameLayer/DataLayer/Definition/UIFormId.cs
            StringBuilder newContent = new StringBuilder();
            newContent.AppendLine("/*脚本生成，请勿手动修改*/");
            newContent.AppendLine("namespace GameDevScript");
            newContent.AppendLine("{");
            newContent.AppendLine($"    public enum {enumName} : int");
            newContent.AppendLine("    {");
            for (int i = 0; i < enumIdList.Count; i++)
            {
                newContent.AppendLine("        /// <summary>");
                newContent.AppendLine($"        /// {enumDescList[i]}");
                newContent.AppendLine("        /// </summary>");
                newContent.AppendLine($"        {enumNameList[i]} = {enumIdList[i]},");
            }
            newContent.AppendLine("    }");
            newContent.AppendLine("}");

            string csFilePath = Path.Combine(Application.dataPath, $"GameScripts/GameLayer/DataLayer/DataTables/{enumName}.cs");
            File.WriteAllText(csFilePath, newContent.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 解析常量表
        /// </summary>
        private static void GenerateGameConst(ISheet dataTableSheet)
        {
            string constAssetPath = "GameScripts/GameLayer/DataLayer/Definition/Constant/Constant.GameLogic.DataTable.cs";
            string constFullPath = Path.Combine(Application.dataPath, constAssetPath);

            List<string> typeList = new List<string>(); //字段类型
            List<string> nameList = new List<string>(); //字段名
            List<string> valueList = new List<string>(); //字段值
            List<string> descList = new List<string>(); //字段描述

            for (int i = 4; i <= dataTableSheet.LastRowNum; i++)
            {
                IRow row = dataTableSheet.GetRow(i);
                typeList.Add(row.GetCell(1).StringCellValue);
                nameList.Add(row.GetCell(2).StringCellValue);
                valueList.Add(row.GetCell(3).StringCellValue);
                descList.Add(row.GetCell(4).StringCellValue);
            }

            //覆盖Assets/GameScripts/GameLayer/DataLayer/Definition/UIFormId.cs
            StringBuilder newContent = new StringBuilder();
            newContent.AppendLine("/*脚本生成，请勿手动修改*/");
            newContent.AppendLine("using System;");
            newContent.AppendLine("using UnityEngine;");
            newContent.AppendLine("namespace GameDevScript");
            newContent.AppendLine("{");
            newContent.AppendLine($"\tpublic static partial class Constant");
            newContent.AppendLine("\t{");
            newContent.AppendLine("\t\tpublic static partial class GameLogic");
            newContent.AppendLine("\t\t{");

            //在这里实现转换
            for (int i = 0; i < typeList.Count; i++)
            {
                string lineStr = GenerateGameConstLineStr(typeList[i], nameList[i], valueList[i], descList[i]);
                if (lineStr != null)
                {
                    newContent.AppendLine(lineStr);
                    // newContent.AppendLine();
                }
            }

            newContent.AppendLine("\t\t}");
            newContent.AppendLine("\t}");
            newContent.AppendLine("}");

            File.WriteAllText(constFullPath, newContent.ToString(), Encoding.UTF8);
        }
        private static string GenerateGameConstLineStr(string type, string name, string value, string desc)
        {
            StringBuilder lineStr = new StringBuilder();

            if (!string.IsNullOrEmpty(desc))
            {
                lineStr.AppendLine($"\t\t\t/// <summary>");
                string[] descLines = desc.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < descLines.Length; i++)
                {
                    lineStr.AppendLine($"\t\t\t/// {descLines[i]}");
                }
                lineStr.AppendLine($"\t\t\t/// </summary>");
            }

            lineStr.AppendLine($"\t\t\tpublic readonly static {type} {name} = {value};");

            return lineStr.ToString();
        }
    }
}
