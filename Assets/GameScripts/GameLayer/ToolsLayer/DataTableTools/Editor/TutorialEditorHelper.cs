using UnityEngine;
using UnityEditor;


namespace GameDevScript
{
    public static class TutorialEditorHelper
    {
        [MenuItem("GameObject/CopyUIPath", false, 0)]
        private static void CopyUIPath()
        {
            GameObject selectedObj = Selection.activeGameObject;
            if (selectedObj == null)
            {
                Debug.LogWarning("请先选择一个GameObject");
                return;
            }

            RectTransform trsSelected = selectedObj.GetComponent<RectTransform>();
            if (trsSelected == null)
            {
                Debug.LogWarning("请先选择一个UI");
                return;
            }
            GUIUtility.systemCopyBuffer = UITools.GetUIRootPath(trsSelected);
            Debug.Log($"已复制路径: {GUIUtility.systemCopyBuffer}");
        }

    }
}
