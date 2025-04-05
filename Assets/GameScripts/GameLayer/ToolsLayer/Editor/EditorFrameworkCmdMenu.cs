using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    /// <summary>
    /// 编辑器框架命令行
    /// </summary>
    public static class EditorFrameworkCmdMenu
    {
        [MenuItem("工具/配置表转换面板", false, -1000)]
        public static void OpenDataTableToolWindow()
        {
            DataTableToolWindow window = EditorWindow.GetWindow<DataTableToolWindow>("配置表转换面板");
            window.minSize = new Vector2(900, 400);
            window.Show();
        }

        [MenuItem("工具/多语言转换面板", false, -1002)]
        public static void OpenLocalizationToolWindow()
        {
            LocalizationToolWindow window = EditorWindow.GetWindow<LocalizationToolWindow>("多语言转换面板");
            window.minSize = new Vector2(700, 400);
            window.Show();
        }
    }
}
