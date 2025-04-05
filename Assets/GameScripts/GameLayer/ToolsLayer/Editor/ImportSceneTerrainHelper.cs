using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System;

namespace GameDevScript
{
    public class ImportSceneTerrainHelper : EditorWindow
    {
        private List<TextureInfo> m_TextureInfos = new List<TextureInfo>();
        private Vector2 m_ScrollPosition;
        private string m_SourceFolderPath;

        [MenuItem("工具/导入地图资源")]
        public static void ShowWindow()
        {
            string folderPath = EditorUtility.OpenFolderPanel("选择地图资源文件夹", Application.dataPath, "");
            if (string.IsNullOrEmpty(folderPath))
                return;

            var window = GetWindow<ImportSceneTerrainHelper>("地图资源导入工具");
            window.minSize = new Vector2(800, 600);
            window.Initialize(folderPath);
        }

        public void Initialize(string folderPath)
        {
            m_SourceFolderPath = folderPath;
            // 获取所有图片文件
            var imageFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(file =>
                    file.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".jpg", System.StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".jpeg", System.StringComparison.OrdinalIgnoreCase))
                .OrderBy(file => Path.GetFileName(file))
                .ToArray();

            // 分析图片信息
            m_TextureInfos.Clear();
            foreach (var file in imageFiles)
            {
                var isPng = Path.GetExtension(file).Equals(".png", System.StringComparison.OrdinalIgnoreCase);
                int? width = null;
                int? height = null;
                bool? isValid = null;

                if (isPng)
                {
                    if (ImageSizeParser.TryGetImageDimensions(file, out int w, out int h))
                    {
                        width = w;
                        height = h;
                        isValid = w % 4 == 0 && h % 4 == 0;
                    }
                }
                else
                {
                    // 非PNG格式仅获取基础信息
                    width = height = null;
                    isValid = null;
                }

                m_TextureInfos.Add(new TextureInfo
                {
                    FileName = Path.GetFileNameWithoutExtension(file),
                    Format = Path.GetExtension(file).ToUpper().TrimStart('.'),
                    Width = width,
                    Height = height,
                    IsSizeValid = isValid,
                    IsPNG = isPng
                });
            }
        }

        private void OnGUI()
        {
            if (string.IsNullOrEmpty(m_SourceFolderPath))
                return;
            else
            {
                EditorGUILayout.LabelField("当前选择的地形资源文件夹：" + m_SourceFolderPath);
                if (m_TextureInfos == null || m_TextureInfos.Count == 0)
                {
                    Initialize(m_SourceFolderPath);
                }
            }

            // 命名规则提示
            DrawNamingRuleHint();

            //导入地形资源
            if (GUILayout.Button("导入地形资源"))
            {
                CopyTerrainTexture();
            }

            EditorGUILayout.LabelField("地图资源检查结果", EditorStyles.boldLabel);

            // 表格标题
            GUILayout.BeginHorizontal();
            GUILayout.Label("序号", GUILayout.Width(50));
            GUILayout.Label("图片名", GUILayout.Width(120));
            GUILayout.Label("图片格式", GUILayout.Width(120));
            GUILayout.Label("是否PNG", GUILayout.Width(120));
            GUILayout.Label("图片宽高", GUILayout.Width(120));
            GUILayout.Label("4的倍数", GUILayout.Width(120));
            GUILayout.EndHorizontal();

            // 滚动视图
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

            // 表格内容
            for (int i = 0; i < m_TextureInfos.Count; i++)
            {
                var info = m_TextureInfos[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label((i + 1).ToString(), GUILayout.Width(50));
                GUILayout.Label(info.FileName, GUILayout.Width(120));
                GUILayout.Label(info.Format, GUILayout.Width(120));
                GUILayout.Label(info.IsPNG ? "✔" : "✖", GUILayout.Width(120));

                // 宽高信息
                if (info.Width.HasValue && info.Height.HasValue)
                    GUILayout.Label($"{info.Width}_{info.Height}", GUILayout.Width(120));
                else
                    GUILayout.Label("-", GUILayout.Width(120));

                // 4的倍数检查
                if (info.IsSizeValid.HasValue)
                    GUILayout.Label(info.IsSizeValid.Value ? "✔" : "✖", GUILayout.Width(120));
                else
                    GUILayout.Label("-", GUILayout.Width(120));

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 拷贝地形资源到Unity工程中
        /// </summary>
        private void CopyTerrainTexture()
        {
            if (string.IsNullOrEmpty(m_SourceFolderPath))
            {
                EditorUtility.DisplayDialog("错误", "未选择源文件夹", "确定");
                return;
            }

            // 目标路径
            string targetRoot = Path.Combine(Application.dataPath, "GameAssets/Textures/Terrain");
            string folderName = Path.GetFileName(m_SourceFolderPath);
            string targetPath = Path.Combine(targetRoot, folderName);

            // 检查目标文件夹是否存在
            if (Directory.Exists(targetPath))
            {
                if (EditorUtility.DisplayDialog("冲突处理", $"文件夹 {folderName} 已存在，选择操作：", "覆盖", "取消"))
                {
                    Directory.Delete(targetPath, true);
                    // 删除meta文件
                    string metaFile = $"{targetPath}.meta";
                    if (File.Exists(metaFile))
                        File.Delete(metaFile);
                }
                else
                {
                    return;
                }
            }

            try
            {
                // 创建目标目录
                Directory.CreateDirectory(targetRoot);

                // 开始拷贝
                CopyDirectory(m_SourceFolderPath, targetPath);

                // 刷新资源数据库
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("成功", $"资源已导入到：Assets/GameAssets/Textures/Map/{folderName}", "确定");
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("错误", $"导入失败：{e.Message}", "确定");
                Debug.LogError(e);
            }
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            // 创建目标目录
            Directory.CreateDirectory(targetDir);

            // 拷贝文件
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                if (Path.GetExtension(file) == ".meta")
                    continue;
                string dest = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, dest, true);
            }

            // 递归拷贝子目录
            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dir);
                CopyDirectory(dir, Path.Combine(targetDir, dirName));
            }
        }

        private void DrawNamingRuleHint()
        {
            // 背景框
            GUILayout.BeginVertical(GUI.skin.box);
            GUI.color = Color.yellow;
            EditorGUILayout.LabelField("命名规则提示：a_b。 a表示切图顺序，b表示层级", EditorStyles.boldLabel);
            GUI.color = Color.white;

            // 层级说明
            GUILayout.BeginHorizontal();
            GUILayout.Label("层级说明", GUILayout.Width(100));
            GUILayout.Label("地板", GUILayout.Width(100));
            GUILayout.Label("建筑低层、花草", GUILayout.Width(120));
            GUILayout.Label("怪物层", GUILayout.Width(100));
            GUILayout.Label("建筑高层", GUILayout.Width(100));
            GUILayout.Label("云朵", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            // 数值对应
            GUILayout.BeginHorizontal();
            GUILayout.Label("对应数值", GUILayout.Width(100));
            GUI.color = Color.cyan;
            GUILayout.Label("0", GUILayout.Width(100));
            GUILayout.Label("2", GUILayout.Width(120));
            GUILayout.Label("5", GUILayout.Width(100));
            GUILayout.Label("7", GUILayout.Width(100));
            GUILayout.Label("9", GUILayout.Width(100));
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        private struct TextureInfo
        {
            public string FileName;
            public string Format;
            public int? Width;
            public int? Height;
            public bool? IsSizeValid;
            public bool IsPNG;
        }

        // 新增图片尺寸解析类
        private static class ImageSizeParser
        {
            public static bool TryGetImageDimensions(string filePath, out int width, out int height)
            {
                width = height = 0;
                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    using (var reader = new BinaryReader(stream))
                    {
                        var extension = Path.GetExtension(filePath).ToLower();

                        if (extension == ".png")
                            return TryParsePng(reader, out width, out height);
                        if (extension == ".jpg" || extension == ".jpeg")
                            return TryParseJpeg(reader, out width, out height);

                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

            private static bool TryParsePng(BinaryReader reader, out int width, out int height)
            {
                width = height = 0;

                // PNG文件头校验
                byte[] header = reader.ReadBytes(8);
                if (header[0] != 137 || header[1] != 80 || header[2] != 78 || header[3] != 71)
                    return false;

                // 查找IHDR块
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int chunkLength = SwapEndian(reader.ReadInt32());
                    string chunkType = new string(reader.ReadChars(4));

                    if (chunkType == "IHDR")
                    {
                        width = SwapEndian(reader.ReadInt32());
                        height = SwapEndian(reader.ReadInt32());
                        return true;
                    }

                    reader.BaseStream.Position += chunkLength + 4; // 跳过数据块和CRC
                }
                return false;
            }

            private static bool TryParseJpeg(BinaryReader reader, out int width, out int height)
            {
                width = height = 0;

                // JPEG文件头校验
                if (reader.ReadUInt16() != 0xFFD8)
                    return false;

                // 查找SOF标记
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    ushort marker = reader.ReadUInt16();
                    if (marker >= 0xFFC0 && marker <= 0xFFCF && marker != 0xFFC4 && marker != 0xFFC8)
                    {
                        ushort length = SwapEndian(reader.ReadUInt16());
                        reader.BaseStream.Position += 1; // 跳过精度字节
                        height = SwapEndian(reader.ReadUInt16());
                        width = SwapEndian(reader.ReadUInt16());
                        return true;
                    }
                    reader.BaseStream.Position += SwapEndian(reader.ReadUInt16()) - 2;
                }
                return false;
            }

            private static int SwapEndian(int value)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Reverse(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }

            private static ushort SwapEndian(ushort value)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Reverse(bytes);
                return BitConverter.ToUInt16(bytes, 0);
            }
        }
    }
}
