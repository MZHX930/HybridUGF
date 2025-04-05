using UnityEngine;
using UnityEditor;
namespace GameDevScript
{
    [CustomEditor(typeof(UISimpleSlider))]
    public class UISimpleSliderInspector : Editor
    {
        private SerializedProperty m_Value;

        void OnEnable()
        {
            m_Value = serializedObject.FindProperty("m_FinalSliderRatio");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            (target as UISimpleSlider).FinalSliderRatio = EditorGUILayout.Slider(m_Value.floatValue, 0, 1);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
