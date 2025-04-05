using GameFramework.Localization;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;


namespace GameDevScript.EditorTools
{
    /// <summary>
    /// 本地化（多语言）工具窗口
    /// >xlsx转为多语言文件
    /// >裁剪TMP字库
    /// </summary>
    public class LocalizationToolWindow : EditorToolsRootWindow
    {
        #region 参数定义
        /// <summary>
        /// 筛选字库存放文件夹
        /// </summary>
        private const string CharactersFolderAssetPath = "Assets/LanguageFontText";
        /// <summary>
        /// TMP字库文件夹
        /// </summary>
        private const string TMPFontFolderAssetPath = "Assets/GameAssets/TMPResources/Fonts & Materials";
        /// <summary>
        /// 游戏多语言XML文件夹
        /// </summary>
        private const string GameLanguageXMLAssetPath = "Assets/GameAssets/Localization";
        #endregion

        private Dictionary<TMP_FontAsset, TextAsset> m_TMPCutInfoDict = new Dictionary<TMP_FontAsset, TextAsset>();

        private void OnEnable()
        {
            //初始化
            string tmpCutInfo = EditorPrefs.GetString("TMPCutInfo", "");
            if (!string.IsNullOrEmpty(tmpCutInfo))
            {
                string[] keyValueStrs = tmpCutInfo.Split(';');
                foreach (var item in keyValueStrs)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    string[] keyValue = item.Split('=');
                    if (keyValue.Length != 2)
                        continue;
                    TMP_FontAsset tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(keyValue[0]);
                    TextAsset fontText = AssetDatabase.LoadAssetAtPath<TextAsset>(keyValue[1]);
                    if (tmpFont == null)
                        continue;
                    m_TMPCutInfoDict.Add(tmpFont, fontText);
                }
            }

            RefreshTMPFontAssetList();
        }

        private void OnDisable()
        {
            string tmpCutInfo = "";
            foreach (var item in m_TMPCutInfoDict)
            {
                if (item.Value == null)
                    continue;
                tmpCutInfo += $"{AssetDatabase.GetAssetPath(item.Key)}={AssetDatabase.GetAssetPath(item.Value)};";
            }
            EditorPrefs.SetString("TMPCutInfo", tmpCutInfo);
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            #region 多语言路径配置
            //xlsx文件
            DrawFilePathInfo("多语言xlsx文件：", EditorSettingData.Localization_XlsxFilePath, p =>
            {
                EditorSettingData.Localization_XlsxFilePath = p;
            });
            #endregion

            if (string.IsNullOrEmpty(EditorSettingData.Localization_XlsxFilePath) || !File.Exists(EditorSettingData.Localization_XlsxFilePath))
            {
                EditorGUILayout.HelpBox("多语言xlsx文件不合规", MessageType.Error);
                return;
            }

            GUILayout.Space(10f);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("导出多语言XML", GUILayout.Width(200)))
            {
                ExportLocalization();
            }
            GUILayout.Space(10f);
            if (GUILayout.Button("裁剪TMP字库", GUILayout.Width(200)))
            {
                //CutTMPFontAsset().Forget();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);
            //绘制裁剪字库列表
            var tmpFontAssetList = new List<TMP_FontAsset>(m_TMPCutInfoDict.Keys);
            foreach (var key in tmpFontAssetList)
            {
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = false;
                EditorGUILayout.ObjectField(key, typeof(TMP_FontAsset), false, GUILayout.Width(300));
                GUI.enabled = true;
                GUILayout.Space(10f);
                var fontText = EditorGUILayout.ObjectField(m_TMPCutInfoDict[key], typeof(TextAsset), false, GUILayout.Width(200));
                if (fontText != m_TMPCutInfoDict[key])
                {
                    m_TMPCutInfoDict[key] = fontText as TextAsset;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        #region 导出多语言
        /// <summary>
        /// 转义符" ' < > &
        /// </summary>
        private Dictionary<string, string> SymbolConverterMap = new Dictionary<string, string>() {
            {"&","&amp;" },
            {"<","&lt;" },
            { ">","&gt;"},
            {"\"","&quot;" },
            {"\'","&apos;" },
        };

        /// <summary>
        /// 将xlsx转为xml多语言
        /// </summary>
        private void ExportLocalization()
        {
            string xmlFolderPath = Application.dataPath.Replace("Assets", GameLanguageXMLAssetPath);
            if (!Directory.Exists(xmlFolderPath))
                Directory.CreateDirectory(xmlFolderPath);
            //获取xlsx中的第一张表
            XSSFWorkbook workbook = new XSSFWorkbook(new FileStream(EditorSettingData.Localization_XlsxFilePath, FileMode.Open, FileAccess.Read, FileShare.None));
            ISheet dataTableSheet = workbook.GetSheetAt(0);//第一个sheet页（列表）

            //先提取出key
            List<string> keyList = new List<string>();
            for (int rowIndex = 1; rowIndex <= dataTableSheet.LastRowNum; rowIndex++)
            {
                keyList.Add(dataTableSheet.GetRow(rowIndex).GetCell(0).StringCellValue);
            }

            int mayLanguageCount = dataTableSheet.GetRow(0).LastCellNum;
            for (int columnIndex = 1; columnIndex <= mayLanguageCount; columnIndex++)
            {
                ICell languageNameCell = dataTableSheet.GetRow(0).GetCell(columnIndex);
                if (languageNameCell == null)
                    continue;
                if (IsIgnoreField(languageNameCell.StringCellValue))
                    continue;

                Language _language = Language.Unspecified;
                if (!System.Enum.TryParse<Language>(languageNameCell.StringCellValue, true, out _language))
                {
                    UnityEngine.Debug.LogError($"多语言表中(1,{columnIndex + 1})单元格无法转换为标准语言枚举。{languageNameCell.StringCellValue}");
                    continue;
                }

                List<string> valueList = new List<string>();
                for (int rowIndex = 1; rowIndex <= dataTableSheet.LastRowNum; rowIndex++)
                {
                    string value = "";
                    ICell cell = dataTableSheet.GetRow(rowIndex).GetCell(columnIndex);
                    if (cell == null)
                    {
                        Log.Warning($"多语言表中({rowIndex},{columnIndex})单元格为空。");
                        continue;
                    }
                    switch (cell.CellType)
                    {
                        case CellType.Numeric:
                            value = cell.NumericCellValue.ToString();
                            break;
                        case CellType.String:
                            value = cell.StringCellValue;
                            break;
                        case CellType.Unknown:
                        case CellType.Formula:
                        case CellType.Blank:
                        case CellType.Boolean:
                        case CellType.Error:
                        default:
                            value = $"(r,c)=({rowIndex},{columnIndex})";
                            break;
                    }
                    valueList.Add(value);
                }

                ExportLanguageXMLFile(_language.ToString(), keyList, valueList, xmlFolderPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("导出成功", $"导出成功，在{xmlFolderPath}中查看。", "确定");
        }

        /// <summary>
        /// 导出xml语言文本
        /// </summary>
        private void ExportLanguageXMLFile(string languageType, List<string> keyList, List<string> valueList, string xmlFolderPath)
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            content.AppendLine("<Dictionaries>");
            content.AppendLine($"    <Dictionary Language=\"{languageType}\">");

            int minIndex = System.Math.Min(keyList.Count, valueList.Count);
            for (int i = 0; i < minIndex; i++)
            {
                string convertStr = ConvertNormalStr2XmlSymbol(valueList[i]);
                convertStr = convertStr.Replace("\\n", "\n");
                content.AppendLine($"        <String Key=\"{keyList[i]}\" Value=\"{convertStr}\" />");
            }

            content.AppendLine("    </Dictionary>");
            content.AppendLine("</Dictionaries>");

            string filePath = Path.Combine(xmlFolderPath, $"{languageType}.xml");
            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                sw.Write(content.ToString());
                sw.Flush();
                sw.Close();
            }

            //导出纯文本
            string fontTextFolderPath = Application.dataPath.Replace("Assets", CharactersFolderAssetPath);
            if (!Directory.Exists(fontTextFolderPath))
                Directory.CreateDirectory(fontTextFolderPath);
            string fontFilePath = Path.Combine(fontTextFolderPath, $"{languageType}_s.xml");
            using (StreamWriter sw = new StreamWriter(fontFilePath, false, Encoding.UTF8))
            {
                foreach (var item in valueList)
                    sw.Write(item);
                sw.Flush();
                sw.Close();
            }
        }

        private string ConvertNormalStr2XmlSymbol(string normalStr)
        {
            foreach (var item in SymbolConverterMap)
            {
                normalStr = normalStr.Replace(item.Key, item.Value);
            }
            return normalStr;
        }
        #endregion


        //#region 裁剪字库
        private void RefreshTMPFontAssetList()
        {
            string[] tmpFontAssets = AssetDatabase.FindAssets("t:TMP_FontAsset", new string[] { TMPFontFolderAssetPath });
            string[] fontTextFolderPaths = AssetDatabase.FindAssets("t:TextAsset", new string[] { CharactersFolderAssetPath });
            foreach (var tmpFontAsset in tmpFontAssets)
            {
                TMP_FontAsset tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(tmpFontAsset));
                if (tmpFont.atlasPopulationMode != AtlasPopulationMode.Static)
                    continue;

                if (!m_TMPCutInfoDict.ContainsKey(tmpFont))
                    m_TMPCutInfoDict.Add(tmpFont, null);
            }

            var tmpFontAssetList = m_TMPCutInfoDict.Keys.ToList();
            foreach (var key in tmpFontAssetList)
            {
                if (key.atlasPopulationMode != AtlasPopulationMode.Static)
                {
                    m_TMPCutInfoDict.Remove(key);
                }
            }
        }
        ///// <summary>
        ///// 裁剪字库
        ///// </summary>
        //private async UniTaskVoid CutTMPFontAsset()
        //{
        //    foreach (var item in m_TMPCutInfoDict)
        //    {
        //        if (item.Value == null)
        //            continue;
        //        TMP_FontAsset tmpFont = item.Key;
        //        TextAsset fontText = item.Value;
        //        if (tmpFont == null || fontText == null || tmpFont.atlasPopulationMode != AtlasPopulationMode.Static)
        //            continue;

        //        TMPro_FontAssetCreatorWindow_Auto window = TMPro_FontAssetCreatorWindow_Auto.ShowFontAtlasCreatorWindow(tmpFont, fontText);
        //        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

        //        EditorUtility.DisplayProgressBar($"裁剪字库{item.Key.name}", "更新Atlas中", 0.2f);
        //        //更新Atlas
        //        window.ToGenerateFontAtlas();
        //        await UniTask.WaitUntil(() => window.IsDoneGenerateFontAtlas());
        //        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        //        EditorUtility.DisplayProgressBar($"裁剪字库{item.Key.name}", "保存Atlas中", 0.6f);

        //        //保存
        //        window.ToSaveAtlas();
        //        await UniTask.WaitUntil(() => window.IsDoneSaveAtlas());
        //        EditorUtility.DisplayProgressBar($"裁剪字库{item.Key.name}", "清理FontEngine中", 0.9f);
        //        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

        //        //关闭界面，进行下一个裁剪
        //        window.Close();
        //        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        //        EditorUtility.ClearProgressBar();

        //        Debug.Log($"更新{item.Value.name}");
        //    }
        //}
        //#endregion
    }
}
