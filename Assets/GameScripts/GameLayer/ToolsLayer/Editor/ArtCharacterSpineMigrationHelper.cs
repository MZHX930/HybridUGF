using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    /// <summary>
    /// 美术角色Spine资源迁移工具
    /// </summary>
    public class ArtCharacterSpineMigrationHelper : EditorWindow
    {
        #region 常量
        private const string c_EditorPrefKey = "ArtSpineFolderRootPath";
        private const string c_ProjectSpinePath = "Assets/GameAssets/Spine";
        private const string c_SkeletonFileExtension = ".skel.bytes";
        #endregion

        #region 私有变量
        private string m_ArtSpineFolderRootPath = string.Empty;
        private List<FolderInfo> m_ArtFolders = new List<FolderInfo>(); // A文件夹列表
        private List<FolderInfo> m_ProjectFolders = new List<FolderInfo>(); // B文件夹列表
        private List<FolderCompareInfo> m_CompareResults = new List<FolderCompareInfo>(); // 比较结果
        private Vector2 m_ScrollPosition = Vector2.zero;
        private bool m_NeedRefresh = true;

        // 表格UI相关
        private readonly float[] m_ColumnWidths = new float[] { 20, 100, 50, 100, 50 };
        private readonly Color m_EvenRowColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        private readonly Color m_HeaderColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        private readonly string[] m_Headers = new string[] { "序号", "美术文件夹名", "对比结果", "工程文件夹名", "操作" };
        private float m_TableHeight = 0f;
        private float m_RowHeight = 20f;
        #endregion

        [MenuItem("工具/美术角色Spine资源迁移工具")]
        public static void ShowWindow()
        {
            var window = GetWindow<ArtCharacterSpineMigrationHelper>("美术角色Spine资源迁移工具");
            window.Show();
        }

        #region Unity生命周期方法
        private void OnEnable()
        {
            // 从EditorPrefs加载上次使用的路径
            m_ArtSpineFolderRootPath = EditorPrefs.GetString(c_EditorPrefKey, "");
        }

        private void OnGUI()
        {
            DrawHeader();

            if (string.IsNullOrEmpty(m_ArtSpineFolderRootPath))
            {
                EditorGUILayout.HelpBox("请选择美术Spine文件夹根路径", MessageType.Info);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("刷新", GUILayout.Width(60)))
            {
                m_NeedRefresh = true;
            }

            if (GUILayout.Button("创建CharacterViewPrefab", GUILayout.Width(200)))
            {
                CreateCharacterPrefabHelper.OpenHelper();
            }
            EditorGUILayout.EndHorizontal();

            if (m_NeedRefresh)
            {
                RefreshData();
                m_NeedRefresh = false;
            }

            DrawTable();
        }
        #endregion

        #region UI绘制方法
        /// <summary>
        /// 绘制顶部选择区域
        /// </summary>
        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("美术Spine文件夹根路径:", GUILayout.Width(150));
            EditorGUILayout.TextField(m_ArtSpineFolderRootPath, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("选择", GUILayout.Width(60)))
            {
                string folderPath = EditorUtility.OpenFolderPanel("选择美术Spine文件夹根路径", m_ArtSpineFolderRootPath, "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    m_ArtSpineFolderRootPath = folderPath;
                    EditorPrefs.SetString(c_EditorPrefKey, m_ArtSpineFolderRootPath);
                    m_NeedRefresh = true;
                }
            }

            if (GUILayout.Button("刷新", GUILayout.Width(60)))
            {
                m_NeedRefresh = true;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        /// <summary>
        /// 绘制表格
        /// </summary>
        private void DrawTable()
        {
            float tableWidth = position.width - 20;

            // 绘制表头
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = m_HeaderColor;

            for (int i = 0; i < m_Headers.Length; i++)
            {
                GUILayout.Box(m_Headers[i], GUILayout.Width(m_ColumnWidths[i] * 2));
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            // 滚动视图
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            // 绘制表格内容
            for (int i = 0; i < m_CompareResults.Count; i++)
            {
                var compareInfo = m_CompareResults[i];
                // 设置行背景色（奇偶行不同颜色）
                if (i % 2 == 1)
                {
                    GUI.backgroundColor = m_EvenRowColor;
                }

                EditorGUILayout.BeginHorizontal();

                // 序号
                EditorGUILayout.LabelField((i + 1).ToString(), GUILayout.Width(m_ColumnWidths[0] * 2));

                // 美术文件夹名
                EditorGUILayout.LabelField(compareInfo.ArtFolderName, GUILayout.Width(m_ColumnWidths[1] * 2));

                // 对比结果
                EditorGUILayout.LabelField(GetCompareResultText(compareInfo.CompareResult), GUILayout.Width(m_ColumnWidths[2] * 2));

                // 工程文件夹名
                EditorGUILayout.LabelField(compareInfo.ProjectFolderName, GUILayout.Width(m_ColumnWidths[3] * 2));

                // 操作按钮
                if (compareInfo.CompareResult == CompareResult.MissingInProject || compareInfo.CompareResult == CompareResult.Different)
                {
                    if (GUILayout.Button("拷贝", GUILayout.Width(m_ColumnWidths[4] * 2)))
                    {
                        CopyFolder(compareInfo.ArtFolderPath, compareInfo.ProjectFolderPath);
                        m_NeedRefresh = true;
                    }
                }
                else if (compareInfo.CompareResult == CompareResult.OnlyInProject)
                {
                    if (GUILayout.Button("删除", GUILayout.Width(m_ColumnWidths[4] * 2)))
                    {
                        DeleteFolder(compareInfo.ProjectFolderPath);
                        m_NeedRefresh = true;
                    }
                }
                else
                {
                    GUILayout.Space(m_ColumnWidths[4] * 2);
                }

                EditorGUILayout.EndHorizontal();

                GUI.backgroundColor = Color.white;
            }

            EditorGUILayout.EndScrollView();
        }
        #endregion

        #region 数据处理方法
        /// <summary>
        /// 刷新所有数据
        /// </summary>
        private void RefreshData()
        {
            // 清除旧数据
            m_ArtFolders.Clear();
            m_ProjectFolders.Clear();
            m_CompareResults.Clear();

            // 获取美术文件夹中包含.skel.bytes的文件夹列表 (A)
            GetArtFolders();

            // 获取工程中Spine文件夹列表 (B)
            GetProjectFolders();

            // 比较文件夹并生成结果
            CompareAndGenerateResults();
        }

        /// <summary>
        /// 获取美术文件夹列表 (A)
        /// </summary>
        private void GetArtFolders()
        {
            if (!Directory.Exists(m_ArtSpineFolderRootPath))
                return;

            // 递归查找所有文件夹
            var allDirs = Directory.GetDirectories(m_ArtSpineFolderRootPath, "*", SearchOption.AllDirectories);

            foreach (var dir in allDirs)
            {
                // 查找目录中是否包含.skel.bytes文件
                if (Directory.GetFiles(dir, $"*{c_SkeletonFileExtension}", SearchOption.TopDirectoryOnly).Length > 0)
                {
                    string folderName = Path.GetFileName(dir);
                    m_ArtFolders.Add(new FolderInfo
                    {
                        FolderName = folderName,
                        FolderPath = dir,
                        FilesMD5 = CalculateFolderFilesMD5(dir)
                    });
                }
            }
        }

        /// <summary>
        /// 获取工程文件夹列表 (B)
        /// </summary>
        private void GetProjectFolders()
        {
            if (!Directory.Exists(c_ProjectSpinePath))
                return;

            // 获取Spine文件夹下的所有直接子文件夹
            var dirs = Directory.GetDirectories(c_ProjectSpinePath, "*", SearchOption.TopDirectoryOnly);

            foreach (var dir in dirs)
            {
                string folderName = Path.GetFileName(dir);
                m_ProjectFolders.Add(new FolderInfo
                {
                    FolderName = folderName,
                    FolderPath = dir,
                    FilesMD5 = CalculateFolderFilesMD5(dir)
                });
            }
        }

        /// <summary>
        /// 比较文件夹并生成结果列表
        /// </summary>
        private void CompareAndGenerateResults()
        {
            // 用于记录已处理的工程文件夹
            HashSet<string> processedProjectFolders = new HashSet<string>();

            // 先处理美术文件夹，查找它们在工程中的对应文件夹
            foreach (var artFolder in m_ArtFolders)
            {
                var projectFolder = m_ProjectFolders.FirstOrDefault(p => p.FolderName == artFolder.FolderName);

                if (projectFolder == null)
                {
                    // 工程中缺少此文件夹
                    m_CompareResults.Add(new FolderCompareInfo
                    {
                        ArtFolderName = artFolder.FolderName,
                        ArtFolderPath = artFolder.FolderPath,
                        ProjectFolderName = "",
                        ProjectFolderPath = Path.Combine(c_ProjectSpinePath, artFolder.FolderName),
                        CompareResult = CompareResult.MissingInProject
                    });
                }
                else
                {
                    // 标记此工程文件夹已处理
                    processedProjectFolders.Add(projectFolder.FolderName);

                    // 比较文件MD5
                    bool filesEqual = CompareFilesMD5(artFolder.FilesMD5, projectFolder.FilesMD5);

                    m_CompareResults.Add(new FolderCompareInfo
                    {
                        ArtFolderName = artFolder.FolderName,
                        ArtFolderPath = artFolder.FolderPath,
                        ProjectFolderName = projectFolder.FolderName,
                        ProjectFolderPath = projectFolder.FolderPath,
                        CompareResult = filesEqual ? CompareResult.Equal : CompareResult.Different
                    });
                }
            }

            // 处理工程中多出的文件夹
            foreach (var projectFolder in m_ProjectFolders)
            {
                if (!processedProjectFolders.Contains(projectFolder.FolderName))
                {
                    m_CompareResults.Add(new FolderCompareInfo
                    {
                        ArtFolderName = "",
                        ArtFolderPath = "",
                        ProjectFolderName = projectFolder.FolderName,
                        ProjectFolderPath = projectFolder.FolderPath,
                        CompareResult = CompareResult.OnlyInProject
                    });
                }
            }

            // 按序号排序结果
            m_CompareResults = m_CompareResults.OrderBy(r => r.ArtFolderName).ThenBy(r => r.ProjectFolderName).ToList();
        }

        /// <summary>
        /// 计算文件夹中所有文件的MD5值
        /// </summary>
        private Dictionary<string, string> CalculateFolderFilesMD5(string folderPath)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!Directory.Exists(folderPath))
                return result;

            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string md5 = CalculateFileMD5(file);
                result[fileName] = md5;
            }

            return result;
        }

        /// <summary>
        /// 计算单个文件的MD5值
        /// </summary>
        private string CalculateFileMD5(string filePath)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        byte[] hashBytes = md5.ComputeHash(stream);
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < hashBytes.Length; i++)
                        {
                            sb.Append(hashBytes[i].ToString("x2"));
                        }

                        return sb.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"计算文件MD5失败: {filePath}, 错误: {e.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 比较两个文件夹中文件的MD5值
        /// </summary>
        private bool CompareFilesMD5(Dictionary<string, string> artFiles, Dictionary<string, string> projectFiles)
        {
            // 不比较文件总数量，只检查A中的文件是否都在B中存在，且MD5值相等
            foreach (var artFile in artFiles)
            {
                if (!projectFiles.TryGetValue(artFile.Key, out string projectMD5) ||
                    projectMD5 != artFile.Value)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region 文件操作方法
        /// <summary>
        /// 拷贝文件夹及其内容到工程目录
        /// </summary>
        private void CopyFolder(string sourcePath, string targetPath)
        {
            try
            {
                // 先删除目标文件夹再创建
                if (Directory.Exists(targetPath))
                {
                    Directory.Delete(targetPath, true);
                }
                Directory.CreateDirectory(targetPath);

                // 拷贝文件
                foreach (var file in Directory.GetFiles(sourcePath))
                {
                    string fileName = Path.GetFileName(file);
                    string targetFile = Path.Combine(targetPath, fileName);
                    File.Copy(file, targetFile, true);
                }

                AssetDatabase.Refresh();
                Debug.Log($"成功拷贝文件夹: {sourcePath} -> {targetPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"拷贝文件夹失败: {e.Message}");
            }
        }

        /// <summary>
        /// 删除工程中的文件夹
        /// </summary>
        private void DeleteFolder(string folderPath)
        {
            try
            {
                if (Directory.Exists(folderPath))
                {
                    if (EditorUtility.DisplayDialog("删除确认",
                        $"确定要删除文件夹 {Path.GetFileName(folderPath)} 吗?", "确定", "取消"))
                    {
                        Directory.Delete(folderPath, true);
                        AssetDatabase.Refresh();
                        Debug.Log($"成功删除文件夹: {folderPath}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"删除文件夹失败: {e.Message}");
            }
        }
        #endregion

        #region 辅助类和枚举
        /// <summary>
        /// 文件夹信息类
        /// </summary>
        private class FolderInfo
        {
            public string FolderName { get; set; }
            public string FolderPath { get; set; }
            public Dictionary<string, string> FilesMD5 { get; set; } = new Dictionary<string, string>();
        }

        /// <summary>
        /// 文件夹比较结果信息类
        /// </summary>
        private class FolderCompareInfo
        {
            public string ArtFolderName { get; set; }
            public string ArtFolderPath { get; set; }
            public string ProjectFolderName { get; set; }
            public string ProjectFolderPath { get; set; }
            public CompareResult CompareResult { get; set; }
        }

        /// <summary>
        /// 比较结果枚举
        /// </summary>
        private enum CompareResult
        {
            Equal,              // 完全相同
            Different,          // 内容不同
            MissingInProject,   // 工程中缺少
            OnlyInProject       // 仅存在于工程中
        }

        /// <summary>
        /// 获取比较结果的文本描述
        /// </summary>
        private string GetCompareResultText(CompareResult result)
        {
            switch (result)
            {
                case CompareResult.Equal:
                    return "相同";
                case CompareResult.Different:
                    return "不同";
                case CompareResult.MissingInProject:
                    return "缺少";
                case CompareResult.OnlyInProject:
                    return "多余";
                default:
                    return "";
            }
        }
        #endregion
    }
}
