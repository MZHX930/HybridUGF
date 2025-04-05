using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using UnityEditor.Experimental;
using UnityEngine.UI;
using System.Text;

namespace GameDevScript.EditorTools
{
    public class CreateNewUIFormHelper : EditorToolsRootWindow
    {
        private string m_UIFormName = "";
        private string m_UIFormRemark = "";
        private bool m_NeedFormData = true;
        private bool m_NeedMask = true;
        private bool m_NeedSubFolder = true;
        private string m_SubFolderName = "";

        [MenuItem("工具/创建新的UIForm", priority = 1000)]
        public static void OpenHelper()
        {
            ClearPrefs();
            var window = GetWindow<CreateNewUIFormHelper>("创建新的UIForm");
            window.minSize = new Vector2(500, 500);
            window.Show();
        }

        void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        public void OnAfterAssemblyReload()
        {
            CreatePrefab();
        }

        protected override void OnGUI()
        {
            EditorGUILayout.Space(10);

            GUI.enabled = !EditorApplication.isCompiling;
            // 界面名称输入
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("界面名称");
            m_UIFormName = EditorGUILayout.TextField(m_UIFormName, GUILayout.Width(150));
            EditorGUILayout.LabelField("Form");
            EditorGUILayout.EndHorizontal();

            //备注
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("备注");
            m_UIFormRemark = EditorGUILayout.TextField(m_UIFormRemark, GUILayout.Width(300));
            EditorGUILayout.EndHorizontal();

            // 是否生成FormData
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("生成FormData");
            m_NeedFormData = EditorGUILayout.Toggle(m_NeedFormData, GUILayout.Width(60));
            if (m_NeedFormData)
            {
                EditorGUILayout.LabelField($"{m_UIFormName}FormData");
            }
            EditorGUILayout.EndHorizontal();

            // 是否需要Mask
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("需要Mask");
            m_NeedMask = EditorGUILayout.Toggle(m_NeedMask, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            // 是否需要二级文件夹
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("需要二级文件夹");
            m_NeedSubFolder = EditorGUILayout.Toggle(m_NeedSubFolder, GUILayout.Width(60));
            if (m_NeedSubFolder)
            {
                m_SubFolderName = EditorGUILayout.TextField(m_SubFolderName, GUILayout.Width(150));
            }
            else
            {
                m_SubFolderName = "";
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            if (GUILayout.Button("生成脚本", GUILayout.Width(300)))
            {
                CreateUIFormAssets();
            }
        }

        private void CreateUIFormAssets()
        {
            if (!CheckCanCreate())
            {
                return;
            }

            string prefabName = $"{m_UIFormName}Form";
            string scriptName = $"{m_UIFormName}Form";
            string dataName = $"{m_UIFormName}FormData";

            //创建数据脚本
            if (m_NeedFormData)
            {
                string scriptPath = Path.Combine("Assets/GameScripts/GameLayer/DataLayer/UIForm", dataName);
                scriptPath = scriptPath.Replace("\\", "/");
                scriptPath += ".cs";
                TextAsset datacsTxt = EditorResources.Load<TextAsset>("TemplateDataForm.cs.txt");
                string dataFilePath = Application.dataPath.Replace("Assets", scriptPath);
                File.WriteAllText(dataFilePath, datacsTxt.text.Replace("_#Key0#_", dataName).Replace("_#Key1#_", prefabName), Encoding.UTF8);
            }
            else
            {
                dataName = "DefaultUIFormData";
            }

            //判断UIFormId中是否存在枚举
            if (!Enum.IsDefined(typeof(UIFormId), prefabName))
            {
                //Assets/GameScripts/GameLayer/DataLayer/Definition/UIFormId.cs

                // 读取UIFormId.cs文件
                string uiFormIdPath = "Assets/GameScripts/GameLayer/DataLayer/Definition/UIFormId.cs";
                string[] lines = File.ReadAllLines(uiFormIdPath);

                // 找到最后一个枚举值
                int lastEnumIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("}"))
                    {
                        lastEnumIndex = i;
                        break;
                    }
                }

                // 在最后一个枚举值后插入新的枚举
                if (lastEnumIndex != -1)
                {
                    List<string> newLines = new List<string>(lines);
                    string newEnumLine = $"        {prefabName} = {prefabName.GetHashCode()},";
                    newLines.Insert(lastEnumIndex, newEnumLine);
                    File.WriteAllLines(uiFormIdPath, newLines);
                }
                else
                {
                    Debug.LogError("无法找到合适的插入位置");
                }
            }

            //创建UI脚本
            string uiScriptPath = Path.Combine("Assets/GameScripts/GameLayer/LogicLayer/UIForm", m_SubFolderName, scriptName);
            uiScriptPath = uiScriptPath.Replace("\\", "/");
            uiScriptPath += ".cs";
            string subFolderPath = Path.GetDirectoryName(uiScriptPath);
            if (!Directory.Exists(subFolderPath))
            {
                Directory.CreateDirectory(subFolderPath);
            }
            TextAsset uicsTxt = EditorResources.Load<TextAsset>("TemplateUIForm.cs.txt");
            File.WriteAllText(Application.dataPath.Replace("Assets", uiScriptPath), uicsTxt.text.Replace("_#Key0#_", scriptName).Replace("_#Key1#_", dataName).Replace("_#Remark#_", m_UIFormRemark), Encoding.UTF8);

            // 获取模板文件路径
            string prefabSaveAssetsPath = Path.Combine("Assets/GameAssets/UIForms", m_SubFolderName, prefabName);
            prefabSaveAssetsPath = prefabSaveAssetsPath.Replace("\\", "/");
            prefabSaveAssetsPath += ".prefab";
            string prefabFolderPath = Application.dataPath.Replace("Assets", Path.GetDirectoryName(prefabSaveAssetsPath));
            if (!Directory.Exists(prefabFolderPath))
            {
                Directory.CreateDirectory(prefabFolderPath);
            }

            //保存数据，等待编译结束
            EditorPrefs.SetString("CreateNewUIFormHelper_ScriptName", scriptName);
            EditorPrefs.SetString("CreateNewUIFormHelper_PrefabName", prefabName);
            EditorPrefs.SetString("CreateNewUIFormHelper_PrefabSaveAssetsPath", prefabSaveAssetsPath);
            EditorPrefs.SetBool("CreateNewUIFormHelper_NeedMask", m_NeedMask);
            EditorPrefs.SetBool("CreateNewUIFormHelper_WaitAddComponent", true);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static void CreatePrefab()
        {
            string scriptName = EditorPrefs.GetString("CreateNewUIFormHelper_ScriptName");
            string prefabName = EditorPrefs.GetString("CreateNewUIFormHelper_PrefabName");
            string prefabSaveAssetsPath = EditorPrefs.GetString("CreateNewUIFormHelper_PrefabSaveAssetsPath");
            bool needMask = EditorPrefs.GetBool("CreateNewUIFormHelper_NeedMask");

            if (string.IsNullOrEmpty(scriptName) || string.IsNullOrEmpty(prefabName) || string.IsNullOrEmpty(prefabSaveAssetsPath))
            {
                return;
            }

            //创建预制件
            GameObject sourcePrefab = EditorResources.Load<GameObject>("TemplateUIForm.prefab");
            GameObject newPrefab = Instantiate<GameObject>(sourcePrefab);
            newPrefab.name = prefabName;
            if (!needMask)
            {
                Transform trsMask = newPrefab.transform.Find("Mask");
                DestroyImmediate(trsMask.gameObject.GetComponent<Image>());
                DestroyImmediate(trsMask.gameObject.GetComponent<CanvasRenderer>());
            }


            try
            {
                Debug.Log($"GameDevScript.{scriptName}");

                Type scriptType = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp")
                ?.GetType($"GameDevScript.{scriptName}", true, true);

                if (scriptType == null)
                {
                    Debug.LogError($"找不到类型: GameDevScript.{scriptName}");
                }
                else
                {
                    Debug.Log($"添加组件: {scriptType.Name}");
                    newPrefab.AddComponent(scriptType);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabSaveAssetsPath);
                DestroyImmediate(newPrefab);
                ClearPrefs();
            }
        }

        private bool CheckCanCreate()
        {
            if (string.IsNullOrEmpty(m_UIFormName))
            {
                EditorUtility.DisplayDialog("错误", "界面名称不能为空", "确定");
                return false;
            }

            if (m_UIFormName.EndsWith("Form"))
            {
                EditorUtility.DisplayDialog("错误", "重复Form结尾", "确定");
                return false;
            }

            // 检查是否已存在同名文件
            // 检查程序集中是否存在同名类
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                // 检查GameDevScript命名空间下的类
                var types = assembly.GetTypes().Where(t => t.Namespace == "GameDevScript" && (t.Name == $"{m_UIFormName}Form" || t.Name == $"{m_UIFormName}FormData"));

                if (types.Any())
                {
                    EditorUtility.DisplayDialog("错误", $"已存在类 {types.First().Name}", "确定");
                    return false;
                }
            }

            return true;
        }


        private static void ClearPrefs()
        {
            EditorPrefs.DeleteKey("CreateNewUIFormHelper_ScriptName");
            EditorPrefs.DeleteKey("CreateNewUIFormHelper_PrefabName");
            EditorPrefs.DeleteKey("CreateNewUIFormHelper_PrefabSaveAssetsPath");
            EditorPrefs.DeleteKey("CreateNewUIFormHelper_NeedMask");
            EditorPrefs.DeleteKey("CreateNewUIFormHelper_WaitAddComponent");
        }
    }
}

