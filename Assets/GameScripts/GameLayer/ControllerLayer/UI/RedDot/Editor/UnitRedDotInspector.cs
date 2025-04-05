using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental;
using System;

namespace GameDevScript
{
    [CustomEditor(typeof(UnitRedDot))]
    public class UnitRedDotInspector : Editor
    {
        private SerializedProperty m_RedDotKey;
        private SerializedProperty m_ObjRedDot;
        private SerializedProperty m_Script;
        private EnumRedDotKey m_SelectedEnumKey = EnumRedDotKey.Null;

        private void OnEnable()
        {
            m_RedDotKey = serializedObject.FindProperty("RedDotKey");
            m_ObjRedDot = serializedObject.FindProperty("ObjRedDot");
            m_Script = serializedObject.FindProperty("m_Script");
        }

        private void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_Script);
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(10);

            serializedObject.Update();

            //显示枚举
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("红点Key", GUILayout.Width(140));
            m_SelectedEnumKey = (EnumRedDotKey)EditorGUILayout.EnumPopup(m_SelectedEnumKey, GUILayout.Width(200));
            if (m_SelectedEnumKey != EnumRedDotKey.Null)
            {
                m_RedDotKey.stringValue = m_SelectedEnumKey.ToString();
            }
            else
            {
                m_RedDotKey.stringValue = "";
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField($"RedDotKey = {m_RedDotKey.stringValue}");

            //显示红点gameobject
            EditorGUILayout.PropertyField(m_ObjRedDot);

            if (!Application.isPlaying && m_ObjRedDot.objectReferenceValue == null)
            {
                if (GUILayout.Button("创建红点"))
                {
                    GameObject redDot = EditorResources.Load<GameObject>("RedDot.prefab");

                    // 方式2：通过GameObject访问（适用于嵌套对象）
                    var goProperty = serializedObject.FindProperty("m_GameObject");
                    Transform trsParent = (goProperty.objectReferenceValue as GameObject).transform;
                    GameObject objRedDot = Instantiate(redDot, trsParent);
                    objRedDot.name = "RedDot";
                    m_ObjRedDot.objectReferenceValue = objRedDot;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}