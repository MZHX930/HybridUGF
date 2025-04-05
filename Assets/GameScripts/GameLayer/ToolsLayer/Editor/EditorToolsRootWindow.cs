using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameDevScript.EditorTools
{
    public abstract class EditorToolsRootWindow : EditorWindow
    {
        protected virtual void OnGUI() { }

        public bool IsIgnoreField(string str)
        {
            if (string.IsNullOrEmpty(str) || str.StartsWith("#") || string.IsNullOrWhiteSpace(str))
                return true;
            else
                return false;
        }

        public void DrawConfigPathInfo(string title, string btnDesc, System.Action<string> onClick)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleLeft;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(title);
            if (GUILayout.Button(string.IsNullOrEmpty(btnDesc) ? "" : btnDesc, buttonStyle))
            {
                //切换路径
                string foldPath = EditorUtility.OpenFolderPanel("选择路径", Application.dataPath.Replace("Assets", ""), "");
                if (Directory.Exists(foldPath))
                {
                    onClick?.Invoke(foldPath);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public void DrawFilePathInfo(string title, string btnDesc, System.Action<string> onClick)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.alignment = TextAnchor.MiddleLeft;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(title);
            if (GUILayout.Button(string.IsNullOrEmpty(btnDesc) ? "" : btnDesc, buttonStyle))
            {
                string filePath = EditorUtility.OpenFilePanel("选择路径", Application.dataPath.Replace("Assets", ""), "");
                if (File.Exists(filePath))
                {
                    onClick?.Invoke(filePath);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
