using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    /// <summary>
    /// 在编辑器模式下管理Assets/GameAssets/Configs/DefaultConfig.txt
    /// </summary>
    public static class GameDefaultConfigHelper
    {
        private const string DefaultConfigAssetsPath = "Assets/GameAssets/Configs/DefaultConfig.txt";

        public static string GetValue(string key)
        {
            using (StreamReader reader = new StreamReader(DefaultConfigAssetsPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2 && parts[0] == key)
                    {
                        return parts[1];
                    }
                }
            }
            return null;
        }

        public static void SetValue(string key, string value)
        {
            // 读取所有行
            string[] lines = File.ReadAllLines(DefaultConfigAssetsPath);
            bool keyExists = false;

            // 遍历每一行检查key是否存在
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }

                string[] parts = line.Split('\t');
                if (parts.Length >= 2 && parts[0] == key)
                {
                    // 如果key存在,替换该行
                    lines[i] = $"{key}\t{value}";
                    keyExists = true;
                    break;
                }
            }

            // 如果key不存在,添加新行
            if (!keyExists)
            {
                var linesList = new List<string>(lines);
                linesList.Add($"{key}\t{value}");
                lines = linesList.ToArray();
            }

            // 写入文件
            File.WriteAllLines(DefaultConfigAssetsPath, lines, Encoding.UTF8);
        }
    }
}
