using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public class CreateEventArgsHelper : EditorToolsRootWindow
    {
        private string m_EventName;
        private string m_EventRemark;

        [MenuItem("工具/创建EventArgs", priority = 1001)]
        public static void OpenHelper()
        {
            var window = GetWindow<CreateEventArgsHelper>("创建EventArgs");
            window.minSize = new Vector2(500, 500);
            window.Show();
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            EditorGUILayout.Space(10);

            GUI.enabled = !EditorApplication.isCompiling;
            // 事件名称输入
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("事件名称");
            m_EventName = EditorGUILayout.TextField(m_EventName, GUILayout.Width(150));
            EditorGUILayout.LabelField("EventArgs");
            EditorGUILayout.EndHorizontal();

            //备注
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("备注");
            m_EventRemark = EditorGUILayout.TextField(m_EventRemark, GUILayout.Width(300));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);

            if (GUILayout.Button("创建", GUILayout.Height(30)))
            {
                if (CheckSameFile())
                {
                    return;
                }
                CreateEventArgsFile();
            }
        }

        private bool CheckSameFile()
        {
            if (string.IsNullOrEmpty(m_EventName))
            {
                EditorUtility.DisplayDialog("提示", "事件名称不能为空", "确定");
                return true;
            }

            string eventArgsPath = $"Assets/GameScripts/GameLayer/LogicLayer/Event/{m_EventName}EventArgs.cs";
            if (File.Exists(eventArgsPath))
            {
                EditorUtility.DisplayDialog("提示", "事件参数类已存在", "确定");
                return true;
            }
            return false;
        }

        private void CreateEventArgsFile()
        {
            string eventArgsName = "";
            if (m_EventName.EndsWith("EventArgs"))
                eventArgsName = m_EventName;
            else
                eventArgsName = $"{m_EventName}EventArgs";

            string eventArgsPath = $"Assets/GameScripts/GameLayer/LogicLayer/Event/{eventArgsName}.cs";

            // 读取模板文件
            string template = File.ReadAllText("Assets/Editor default resources/TemplateEventArgs.cs.txt");

            // 替换模板中的占位符
            template = template.Replace("_#ClassName#_", eventArgsName);
            template = template.Replace("_#Remark#_", m_EventRemark);

            // 确保目录存在
            string directory = Path.GetDirectoryName(eventArgsPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 写入文件
            File.WriteAllText(eventArgsPath, template);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("提示", "创建成功", "确定");
        }
    }
}
