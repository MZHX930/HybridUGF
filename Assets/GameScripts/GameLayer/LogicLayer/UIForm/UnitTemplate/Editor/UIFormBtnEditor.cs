using System.Xml;
using System.IO;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace GameDevScript
{
    [CustomEditor(typeof(UIFormBtn), true)]
    public class UIFormBtnEditor : ButtonEditor
    {
        private string m_chinaXmlPath = "Assets/GameAssets/Localization/English.xml";

        private string m_LanguageKeyStr_old = null;
        private string m_ShowLanguageStr = null;
        private IEnumerable<XElement> m_XmlElements = null;

        SerializedProperty m_Script;
        SerializedProperty m_TxtCmpt;
        SerializedProperty m_ImgCmpt;
        SerializedProperty m_LanguageKey;
        SerializedProperty m_UISoundId;

        //在引导中对比的参数
        private string m_GuideBtnPath = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Script = serializedObject.FindProperty("m_Script");
            m_TxtCmpt = serializedObject.FindProperty("TxtCmpt");
            m_ImgCmpt = serializedObject.FindProperty("ImgCmpt");
            m_LanguageKey = serializedObject.FindProperty("LanguageKey");
            m_UISoundId = serializedObject.FindProperty("UISoundId");

            m_LanguageKeyStr_old = null;
            ReadXmlKey(m_LanguageKey.stringValue);

            if (Application.isPlaying)
            {
                m_GuideBtnPath = UITools.GetUIRootPath((target as UIFormBtn).transform);
            }
        }

        private void ReadXmlKey(string languageKey)
        {
            if (languageKey == m_LanguageKeyStr_old)
                return;
            m_LanguageKeyStr_old = languageKey;

            if (string.IsNullOrEmpty(languageKey))
            {
                m_ShowLanguageStr = "";
            }
            else
            {
                if (m_XmlElements == null)
                {
                    try
                    {
                        XDocument m_xml = XDocument.Load(Application.dataPath.Replace("Assets", m_chinaXmlPath));
                        m_XmlElements = m_xml.Descendants("String");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Failed to load language XML file: {e.Message}");
                        m_ShowLanguageStr = "未找到XML文件";
                        return;
                    }
                }
                if (m_XmlElements == null)
                {
                    m_ShowLanguageStr = "未找到XML文件";
                    return;
                }

                var element = m_XmlElements.FirstOrDefault(e => (string)e.Attribute("Key") == languageKey);
                if (element == null)
                {
                    m_ShowLanguageStr = "Not Define Language Key";
                }
                else
                {
                    m_ShowLanguageStr = element.Attribute("Value").Value;
                    (target as UIFormBtn).TxtCmpt.text = m_ShowLanguageStr;
                }
            }
        }

        bool showPosition = true;
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_Script);
            GUI.enabled = true;

            EditorGUILayout.Space(5);

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_ImgCmpt);

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(m_TxtCmpt);
            EditorGUILayout.PropertyField(m_LanguageKey);

            ReadXmlKey(m_LanguageKey.stringValue);
            EditorGUILayout.LabelField("Language:", m_ShowLanguageStr);

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(m_UISoundId);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(10);

            if (Application.isPlaying)
            {
                //显示发送的引导参数
                EditorGUILayout.LabelField("引导参数:(双击选中后右键复制)");
                EditorGUILayout.SelectableLabel(m_GuideBtnPath);

                EditorGUILayout.Space(10);
                //显示测试灰色的按钮
                if (showPosition = EditorGUILayout.BeginFoldoutHeaderGroup(showPosition, "按钮"))
                {
                    if (GUILayout.Button("设置为灰色"))
                    {
                        (target as UIFormBtn).SetGray();
                    }
                    if (GUILayout.Button("设置为正常"))
                    {
                        (target as UIFormBtn).SetNormal();
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            else
            {
                EditorGUILayout.HelpBox("在运行状态下查看引导参数", MessageType.Info);
            }
        }
    }
}
