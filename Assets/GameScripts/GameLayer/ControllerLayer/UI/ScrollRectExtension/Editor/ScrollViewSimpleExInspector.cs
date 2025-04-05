using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(ScrollRectSimpleEx))]
public class ScrollViewSimpleExInspector : ScrollRectEditor
{
    SerializedProperty mMaxDataCount;
    SerializedProperty mShowCellCount;
    SerializedProperty mCellWidth;
    SerializedProperty mCellHeight;
    SerializedProperty mCurHeadmostDataIndex;
    //SerializedProperty mCachedCellArray;
    //SerializedProperty mDataIndex2CellIndex;

    protected override void OnEnable()
    {
        base.OnEnable();

        mMaxDataCount = serializedObject.FindProperty("mMaxDataCount");
        mShowCellCount = serializedObject.FindProperty("mShowCellCount");
        mCellWidth = serializedObject.FindProperty("mCellWidth");
        mCellHeight = serializedObject.FindProperty("mCellHeight");
        mCurHeadmostDataIndex = serializedObject.FindProperty("mCurHeadmostDataIndex");
        //mCachedCellArray = serializedObject.FindProperty("mCachedCellArray");
        //mDataIndex2CellIndex = serializedObject.FindProperty("mDataIndex2CellIndex");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        using (new EditorGUI.DisabledScope(true))
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mMaxDataCount);
            EditorGUILayout.PropertyField(mShowCellCount);
            EditorGUILayout.PropertyField(mCellWidth);
            EditorGUILayout.PropertyField(mCellHeight);
            EditorGUILayout.PropertyField(mCurHeadmostDataIndex);
            //EditorGUILayout.PropertyField(mCachedCellArray);
            //EditorGUILayout.PropertyField(mDataIndex2CellIndex);
        }
    }

}