using UnityEngine;
using UnityEditor;

namespace GameDevScript
{
    [CustomEditor(typeof(UnitCharacterFightAI))]
    public class UnitCharacterFightAIInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var targetCsIns = target as UnitCharacterFightAI;
            // 通过反射获取私有字段m_ActiveAIHelper
            var m_ActiveAIHelperField = typeof(UnitCharacterFightAI).GetField("m_ActiveAIHelper", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var m_ActiveAIHelper = m_ActiveAIHelperField.GetValue(targetCsIns);
            if (m_ActiveAIHelper != null)
            {
                EditorGUILayout.LabelField("ActiveAIHelper", m_ActiveAIHelper.GetType().ToString());
            }
        }
    }
}
