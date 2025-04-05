using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental;

public static class UIElementMenuHelper
{
    [MenuItem("GameObject/UI/UIFormBtn", priority = 1000)]
    public static void CreateUIElement(MenuCommand menuComman)
    {
        GameObject sourcePrefab = EditorResources.Load<GameObject>("UIElementTemplate/UIFormBtn.prefab");
        GameObject newPrefab = GameObject.Instantiate<GameObject>(sourcePrefab);
        newPrefab.name = "Btn";
        GameObjectUtility.SetParentAndAlign(newPrefab, menuComman.context as GameObject);
    }


    [MenuItem("GameObject/UI/AutoLanguageText", priority = 1000)]
    public static void CreateAutoLanguageText(MenuCommand menuComman)
    {
        GameObject sourcePrefab = EditorResources.Load<GameObject>("UIElementTemplate/AutoLanguageText.prefab");
        GameObject newPrefab = GameObject.Instantiate<GameObject>(sourcePrefab);
        newPrefab.name = "#AutoText";
        GameObjectUtility.SetParentAndAlign(newPrefab, menuComman.context as GameObject);
    }
}