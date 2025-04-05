using GameFramework.Localization;
using System;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Editor;
using GameFramework;
using System.IO;
using UnityGameFramework.Editor.ResourceTools;


namespace GameDevScript.EditorTools
{
    public static class EditorSettingData
    {
        [BuildSettingsConfigPath]
        public static string BuildSettingsConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "GameAssets/Configs/BuildSettings.xml"));

        [ResourceCollectionConfigPath]
        public static string ResourceCollectionConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "GameAssets/Configs/ResourceCollection.xml"));

        [ResourceEditorConfigPath]
        public static string ResourceEditorConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "GameAssets/Configs/ResourceEditor.xml"));

        [ResourceBuilderConfigPath]
        public static string ResourceBuilderConfig = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "GameAssets/Configs/ResourceBuilder.xml"));

        public static int ProjectHash = Application.dataPath.GetHashCode();

        #region 配置表
        public static string DataTable_XlsxPath
        {
            get
            {
                return EditorPrefs.GetString($"DataTable_XlsxPath_{ProjectHash}", Application.dataPath);
            }
            set
            {
                EditorPrefs.SetString($"DataTable_XlsxPath_{ProjectHash}", value);
            }
        }
        /// <summary>
        /// 存放加密的txt文件夹
        /// </summary>
        public static string DataTable_EncryptedTextPath
        {
            get
            {
                return EditorPrefs.GetString($"DataTable_EncryptedTextPath_{ProjectHash}", Application.dataPath);
            }
            set
            {
                EditorPrefs.SetString($"DataTable_EncryptedTextPath_{ProjectHash}", value);
            }
        }

        /// <summary>
        /// 存放未加密的txt文件夹
        /// </summary>
        public static string DataTable_UnencryptedTextPath
        {
            get
            {
                return EditorPrefs.GetString($"DataTable_UnencryptedTextPath_{ProjectHash}", Application.dataPath);
            }
            set
            {
                EditorPrefs.SetString($"DataTable_UnencryptedTextPath_{ProjectHash}", value);
            }
        }

        public static string DataTable_ScriptPath
        {
            get
            {
                return EditorPrefs.GetString($"DataTable_ScriptPath_{ProjectHash}", Application.dataPath);
            }
            set
            {
                EditorPrefs.SetString($"DataTable_ScriptPath_{ProjectHash}", value);
            }
        }
        public static string DataTable_ScriptNamespace
        {
            get
            {
                return EditorPrefs.GetString($"DataTable_ScriptNamespace_{ProjectHash}", "GameDevScript");
            }
            set
            {
                EditorPrefs.SetString($"DataTable_ScriptNamespace_{ProjectHash}", value);
            }
        }
        #endregion


        #region 多语言
        public static string Localization_XlsxFilePath
        {
            get
            {
                return EditorPrefs.GetString($"Localization_XlsxFilePath_{ProjectHash}", Application.dataPath);
            }
            set
            {
                EditorPrefs.SetString($"Localization_XlsxFilePath_{ProjectHash}", value);
            }
        }
        #endregion
    }
}
