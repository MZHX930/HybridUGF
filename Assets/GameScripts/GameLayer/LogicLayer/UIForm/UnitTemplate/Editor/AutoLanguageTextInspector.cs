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
    [CustomEditor(typeof(AutoLanguageText))]
    public class AutoLanguageTextInspector : Editor
    {
        private string m_chinaXmlPath = "Assets/GameAssets/Localization/English.xml";

        SerializedProperty m_Script;
        SerializedProperty m_TxtCmpt;
        SerializedProperty m_LanguageKey;

        private string m_LanguageKeyStr_old = null;
        private string m_ShowLanguageStr = null;

        private IEnumerable<XElement> m_XmlElements = null;

        protected void OnEnable()
        {
            m_Script = serializedObject.FindProperty("m_Script");
            m_TxtCmpt = serializedObject.FindProperty("TxtCmpt");
            m_LanguageKey = serializedObject.FindProperty("Key");

            m_LanguageKeyStr_old = null;
            ReadXmlKey(m_LanguageKey.stringValue);
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
                    XDocument m_xml = XDocument.Load(Application.dataPath.Replace("Assets", m_chinaXmlPath));
                    m_XmlElements = m_xml.Descendants("String");
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
                    (target as AutoLanguageText).TxtCmpt.text = m_ShowLanguageStr;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_Script);
            GUI.enabled = true;

            EditorGUILayout.Space(5);

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_TxtCmpt);
            EditorGUILayout.PropertyField(m_LanguageKey);
            serializedObject.ApplyModifiedProperties();

            ReadXmlKey(m_LanguageKey.stringValue);
            EditorGUILayout.LabelField("Language:", m_ShowLanguageStr);
        }
    }
}
