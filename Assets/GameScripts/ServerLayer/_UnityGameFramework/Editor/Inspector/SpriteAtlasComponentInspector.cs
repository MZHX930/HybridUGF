// using System.Collections.Generic;
// using System.IO;
// using UnityEditor;
// using UnityEngine;
// using UnityGameFramework.Runtime;

// namespace UnityGameFramework.Editor
// {
//     [CustomEditor(typeof(SpriteAtlasComponent))]
//     public class SpriteAtlasComponentInspector : GameFrameworkInspector
//     {
//         private SerializedProperty SpriteAtlasNameList;

//         void OnEnable()
//         {
//             SpriteAtlasNameList = serializedObject.FindProperty("SpriteAtlasNameList");
//         }

//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();

//             serializedObject.Update();

//             if (GUILayout.Button("更新"))
//             {
//                 string folderPath = Application.dataPath + "/GameAssets/UI/SpriteAtlas";
//                 SpriteAtlasNameList.ClearArray();
//                 string[] filePaths = Directory.GetFiles(folderPath, "*.spriteatlas", SearchOption.AllDirectories);

//                 List<string> nameList = new List<string>();
//                 foreach (var filePath in filePaths)
//                 {
//                     if (filePath.EndsWith("meta"))
//                         continue;
//                     nameList.Add(Path.GetFileNameWithoutExtension(filePath));
//                 }
//                 SpriteAtlasNameList.arraySize = nameList.Count;
//                 for (int i = 0; i < nameList.Count; i++)
//                 {
//                     SpriteAtlasNameList.GetArrayElementAtIndex(i).stringValue = nameList[i];
//                 }
//             }
//             if (GUILayout.Button("清理"))
//             {
//                 SpriteAtlasNameList.ClearArray();
//             }

//             EditorGUI.BeginDisabledGroup(true);
//             {
//                 for (int i = 0; i < SpriteAtlasNameList.arraySize; i++)
//                 {
//                     var element = SpriteAtlasNameList.GetArrayElementAtIndex(i);
//                     EditorGUILayout.PropertyField(element);
//                 }
//             }
//             EditorGUI.EndDisabledGroup();

//             serializedObject.ApplyModifiedProperties();

//             Repaint();
//         }


//     }


//     //internal class SpriteAtlasModificationProcessor : AssetModificationProcessor
//     //{
//     //    private static void OnWillCreateAsset(string assetName)
//     //    {

//     //    }
//     //}

// }