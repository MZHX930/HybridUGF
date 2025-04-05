using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDevScript.EditorTools
{
    /// <summary>
    /// 基于NPOI的配置表转换工具
    /// 1.缓存路径
    /// </summary>
    public class DataTableToolWindow : EditorToolsRootWindow
    {
        private class XlsxDirectoryState
        {
            /// <summary>
            /// 文件夹名字
            /// </summary>
            public int TestGroup;
            public bool IsSelected;
            public List<XlsxFileState> FileStateList = new List<XlsxFileState>();
        }

        public class XlsxFileState
        {
            public string FileFullPath;
            public string FileNameWithoutExtension;
            public bool IsSelected;
            public bool IsRecentModify;//最近1个小时内有过修改
        }

        private List<XlsxDirectoryState> mCachedXlsxDirectoryList = new List<XlsxDirectoryState>();

        private void OnEnable()
        {
            RefreshXlsxFiles();
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            #region //配置信息
            DrawConfigPathInfo("xlsx文件夹：", EditorSettingData.DataTable_XlsxPath, p => EditorSettingData.DataTable_XlsxPath = p);
            DrawConfigPathInfo("未加密的txt文件夹：", EditorSettingData.DataTable_UnencryptedTextPath, p => EditorSettingData.DataTable_UnencryptedTextPath = p);
            DrawConfigPathInfo("运行时txt文件夹：", EditorSettingData.DataTable_EncryptedTextPath, p => EditorSettingData.DataTable_EncryptedTextPath = p);
            DrawConfigPathInfo("scripts文件夹：", EditorSettingData.DataTable_ScriptPath, p => EditorSettingData.DataTable_ScriptPath = p);
            EditorSettingData.DataTable_ScriptNamespace = EditorGUILayout.TextField("script的namespace:", EditorSettingData.DataTable_ScriptNamespace);
            #endregion

            #region //按钮集合
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("导出配置", GUILayout.Width(150)))
            {
                ExportXlsxFiles();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("全选", GUILayout.Width(150)))
            {
                foreach (var dirStateInfo in mCachedXlsxDirectoryList)
                {
                    dirStateInfo.IsSelected = true;
                    foreach (var fileStateInfo in dirStateInfo.FileStateList)
                    {
                        fileStateInfo.IsSelected = true;
                    }
                }
            }
            GUILayout.Space(10);
            if (GUILayout.Button("反选", GUILayout.Width(150)))
            {
                foreach (var dirStateInfo in mCachedXlsxDirectoryList)
                {
                    foreach (var fileStateInfo in dirStateInfo.FileStateList)
                    {
                        fileStateInfo.IsSelected = !fileStateInfo.IsSelected;
                    }
                }
            }
            GUILayout.Space(10);
            if (GUILayout.Button("取消全部", GUILayout.Width(150)))
            {
                foreach (var dirStateInfo in mCachedXlsxDirectoryList)
                {
                    foreach (var fileStateInfo in dirStateInfo.FileStateList)
                    {
                        fileStateInfo.IsSelected = false;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("刷新", GUILayout.Width(150)))
            {
                RefreshXlsxFiles();
                Repaint();
            }
            #endregion

            //显示所有xlsx
            DrawXlsxFiles();
        }

        public void DrawXlsxFiles()
        {
            for (int dirIndex = 0; dirIndex < mCachedXlsxDirectoryList.Count; dirIndex++)
            {
                var directoryStateInfo = mCachedXlsxDirectoryList[dirIndex];
                directoryStateInfo.IsSelected = EditorGUILayout.ToggleLeft($"测试组-{directoryStateInfo.TestGroup}", directoryStateInfo.IsSelected);

                if (directoryStateInfo.IsSelected)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(20, false);
                    for (int fileIndex = 0; fileIndex < directoryStateInfo.FileStateList.Count; fileIndex++)
                    {
                        var fileStateData = directoryStateInfo.FileStateList[fileIndex];
                        string tag = fileStateData.IsRecentModify ? " *" : "";
                        fileStateData.IsSelected = EditorGUILayout.ToggleLeft($"{fileStateData.FileNameWithoutExtension}{tag}", fileStateData.IsSelected, GUILayout.Width(300));

                        if ((fileIndex + 1) % 3 == 0)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.Space(20, false);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.HelpBox("未选中此测试目录", MessageType.Info);
                }

                EditorGUILayout.Space();
            }

        }

        public void RefreshXlsxFiles()
        {
            mCachedXlsxDirectoryList.Clear();

            if (string.IsNullOrEmpty(EditorSettingData.DataTable_XlsxPath))
                return;

            if (!Directory.Exists(EditorSettingData.DataTable_XlsxPath))
                return;

            //按照目录来区分
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(EditorSettingData.DataTable_XlsxPath);

            DirectoryInfo[] childDirectoryInfos = rootDirectoryInfo.GetDirectories();
            for (int i = 0; i < childDirectoryInfos.Length; i++)
            {
                var childDirectoryInfo = childDirectoryInfos[i];
                if (!int.TryParse(childDirectoryInfo.Name, out int configTestGroup))
                    continue;

                var dirStateInfo = new XlsxDirectoryState();
                dirStateInfo.TestGroup = configTestGroup;
                dirStateInfo.IsSelected = true;

                foreach (var fileInfo in childDirectoryInfo.GetFiles("*.xlsx", SearchOption.AllDirectories))
                {
                    string _fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    if (_fileName.StartsWith("~$"))
                        continue;

                    bool isModify = (System.DateTime.Now - fileInfo.LastWriteTime.AddHours(1)).TotalSeconds <= 0;
                    dirStateInfo.FileStateList.Add(new XlsxFileState()
                    {
                        FileFullPath = fileInfo.FullName,
                        FileNameWithoutExtension = _fileName,
                        IsRecentModify = isModify,
                        IsSelected = isModify,
                    });
                }

                mCachedXlsxDirectoryList.Add(dirStateInfo);
            }
        }

        private void ExportXlsxFiles()
        {
            foreach (var dirStateInfo in mCachedXlsxDirectoryList)
            {
                if (!dirStateInfo.IsSelected)
                    continue;

                List<string> selectedFileNameList = (from v in dirStateInfo.FileStateList where v.IsSelected select v.FileFullPath).ToList();
                if (selectedFileNameList.Count <= 0)
                    continue;

                bool needExportCs = dirStateInfo.TestGroup == 0;
                string unencryptedTextFolderPath = Path.Combine(EditorSettingData.DataTable_UnencryptedTextPath, dirStateInfo.TestGroup.ToString());
                string encryptedTxtFolderPath = Path.Combine(EditorSettingData.DataTable_EncryptedTextPath, dirStateInfo.TestGroup.ToString());
                string csFolderPath = EditorSettingData.DataTable_ScriptPath;
                string scriptNamespace = EditorSettingData.DataTable_ScriptNamespace;

                if (!Directory.Exists(unencryptedTextFolderPath))
                    Directory.CreateDirectory(unencryptedTextFolderPath);
                if (!Directory.Exists(encryptedTxtFolderPath))
                    Directory.CreateDirectory(encryptedTxtFolderPath);
                if (!Directory.Exists(csFolderPath))
                    Directory.CreateDirectory(csFolderPath);

                foreach (XlsxFileState fileState in dirStateInfo.FileStateList)
                {
                    if (!fileState.IsSelected)
                        continue;

                    NpoiDataTableConvertHelper.ParseXlsx2DataTable(fileState.FileFullPath, unencryptedTextFolderPath, encryptedTxtFolderPath, csFolderPath, scriptNamespace, needExportCs);
                }
            }

            HashSet<string> checkDRNames = new HashSet<string>();
            string datatable_uiform_fileFull = null;
            //更新配置表名
            StringBuilder dataTableNames = new StringBuilder();
            foreach (var fileState in mCachedXlsxDirectoryList[0].FileStateList)
            {
                string datableName = NpoiDataTableConvertHelper.GetDataTableName(fileState.FileFullPath);
                if (datableName.Contains("Enum") || datableName.Contains("DefineConstant"))
                    continue;

                if (dataTableNames.Length > 0)
                    dataTableNames.Append(",");
                dataTableNames.Append($"{datableName}");

                if (datableName.Contains("UIForm"))
                {
                    datatable_uiform_fileFull = fileState.FileFullPath;
                }

                checkDRNames.Add(datableName);
            }
            Debug.Log($"DataTableNames: {dataTableNames}");
            GameDefaultConfigHelper.SetValue("DataTableNames", dataTableNames.ToString());

            //删除失效的DRxxx.cs
            string[] allDrFiles = Directory.GetFiles(EditorSettingData.DataTable_ScriptPath, "DR*.cs", SearchOption.AllDirectories);
            foreach (var drFile in allDrFiles)
            {
                string drName = Path.GetFileNameWithoutExtension(drFile).Replace("DR", "");
                if (!checkDRNames.Contains(drName))
                {
                    // Debug.Log($"删除失效的DR文件: {drName}");
                    File.Delete(drFile);
                }
            }

            //删除失效的txt
            string[] allTxtFiles = Directory.GetFiles(EditorSettingData.DataTable_EncryptedTextPath, "*.txt", SearchOption.AllDirectories);
            foreach (var txtFile in allTxtFiles)
            {
                string txtName = Path.GetFileNameWithoutExtension(txtFile);
                if (!checkDRNames.Contains(txtName))
                {
                    File.Delete(txtFile);
                }
            }

            //更新UIFormId
            NpoiDataTableConvertHelper.GenerateUIFormIds(datatable_uiform_fileFull);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
