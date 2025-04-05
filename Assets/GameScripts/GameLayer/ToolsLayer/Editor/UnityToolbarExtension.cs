using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace GameDevScript.EditorTools
{
    /// <summary>
    /// 扩展Unity的按钮栏
    /// </summary>
    /// 
    [InitializeOnLoad]
    public static class UnityToolbarExtension
    {
        public static Type m_toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        public static ScriptableObject m_currentToolbar;

        static UnityToolbarExtension()
        {
            EditorApplication.delayCall += OnUpdate;
        }

        public static void OnUpdate()
        {
            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
            if (m_currentToolbar == null)
            {
                // Find toolbar
                var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
                m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

                if (m_currentToolbar != null)
                {
                    var root = m_currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    var rawRoot = root.GetValue(m_currentToolbar);
                    var mRoot = rawRoot as VisualElement;
                    RegisterCallback("ToolbarZoneLeftAlign", GUILeft);
                    RegisterCallback("ToolbarZoneRightAlign", GUIRight);

                    void RegisterCallback(string root, Action cb)
                    {
                        var toolbarZone = mRoot.Q(root);

                        var parent = new VisualElement()
                        {
                            style = {
                            flexGrow = 1,
                            flexDirection = FlexDirection.Row,
                        }
                        };
                        var container = new IMGUIContainer();
                        container.onGUIHandler += () =>
                        {
                            cb?.Invoke();
                        };
                        parent.Add(container);
                        toolbarZone.Add(parent);
                    }
                }
            }
        }


        /// <summary>
        /// 绘制左侧的元素
        /// </summary>
        private static void GUILeft()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{Application.dataPath}");
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制右侧的元素
        /// </summary>
        private static void GUIRight()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("启动"))
            {
                EditorApplication.ExitPlaymode();
                if (EditorSceneManager.SaveOpenScenes())
                {
                    // 获取构建设置中的第一个场景
                    if (EditorBuildSettings.scenes.Length > 0)
                    {
                        var firstScene = EditorBuildSettings.scenes[0];
                        if (firstScene.enabled)
                        {
                            EditorSceneManager.OpenScene(firstScene.path, OpenSceneMode.Single);
                            EditorApplication.EnterPlaymode();
                        }
                        else
                        {
                            Debug.LogError("构建设置中的第一个场景被禁用");
                        }
                    }
                    else
                    {
                        Debug.LogError("构建设置中没有场景");
                    }
                }
            }
            else if (GUILayout.Button("定位UIForm"))
            {
                string dirAssetPath = "Assets/GameAssets/UIForms/";
                string[] findGuids = AssetDatabase.FindAssets("t:prefab", new string[] { dirAssetPath });

                List<string> vaildPaths = new List<string>();
                foreach (var item in findGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(item);
                    vaildPaths.Add(path);
                }

                GenericMenu menu = new GenericMenu();
                foreach (var path in vaildPaths)
                {
                    string name = path.Replace(dirAssetPath, "");
                    menu.AddItem(new GUIContent(name), false, OpenUIPrefab, path);
                }
                menu.ShowAsContext();
            }
            else if (GUILayout.Button("打开配置表目录"))
            {
                if (!string.IsNullOrEmpty(EditorSettingData.DataTable_XlsxPath))
                {
                    System.Diagnostics.Process.Start(EditorSettingData.DataTable_XlsxPath);
                }
            }

            GUILayout.EndHorizontal();
        }

        static void OnSelectCallBack(object scenePath)
        {
            EditorApplication.ExitPlaymode();
            EditorSceneManager.OpenScene((string)scenePath);
        }

        private static void OpenUIPrefab(object path)
        {
            PrefabStageUtility.OpenPrefab((string)path);
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath((string)path, typeof(GameObject)));
        }
    }
}