using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace GameDevScript.EditorTools
{
    [CustomEditor(typeof(UnitBindManager))]
    public class UnitBindManagerInspector : Editor
    {
        private bool m_ShowBindPoints = true;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            UnitBindManager bindManager = (UnitBindManager)target;

            // 使用反射获取私有字段mBindPointDic
            FieldInfo bindPointDicField = typeof(UnitBindManager).GetField("mBindPointDic",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (bindPointDicField != null)
            {
                Dictionary<string, UnitBindPoint> bindPointDic =
                    bindPointDicField.GetValue(bindManager) as Dictionary<string, UnitBindPoint>;

                if (bindPointDic != null)
                {
                    m_ShowBindPoints = EditorGUILayout.Foldout(m_ShowBindPoints, $"绑点列表 ({bindPointDic.Count})");

                    if (m_ShowBindPoints)
                    {
                        EditorGUI.indentLevel++;

                        if (bindPointDic.Count == 0)
                        {
                            EditorGUILayout.HelpBox("没有找到绑点", MessageType.Info);
                        }
                        else
                        {
                            foreach (var kvp in bindPointDic)
                            {
                                EditorGUILayout.BeginVertical(GUI.skin.box);

                                // 绑点名称与路径
                                EditorGUILayout.LabelField("绑点名称", kvp.Key);

                                if (kvp.Value != null)
                                {
                                    EditorGUI.indentLevel++;

                                    // 绑点GameObject名称
                                    EditorGUILayout.ObjectField("绑点对象", kvp.Value.gameObject, typeof(GameObject), true);

                                    // 绑点偏移量
                                    EditorGUILayout.Vector3Field("偏移量", kvp.Value.offset);

                                    // 获取bindGameObject字典（如果需要显示）
                                    FieldInfo bindGameObjField = typeof(UnitBindPoint).GetField("bindGameObject",
                                        BindingFlags.NonPublic | BindingFlags.Instance);

                                    if (bindGameObjField != null)
                                    {
                                        Dictionary<string, BindGameObjectInfo> bindGameObjDic =
                                            bindGameObjField.GetValue(kvp.Value) as Dictionary<string, BindGameObjectInfo>;

                                        if (bindGameObjDic != null && bindGameObjDic.Count > 0)
                                        {
                                            EditorGUILayout.LabelField($"已挂载对象 ({bindGameObjDic.Count})");
                                            EditorGUI.indentLevel++;

                                            foreach (var gameObjKvp in bindGameObjDic)
                                            {
                                                EditorGUILayout.BeginVertical(GUI.skin.box);
                                                EditorGUILayout.LabelField("标识", gameObjKvp.Key);

                                                if (gameObjKvp.Value != null)
                                                {
                                                    EditorGUILayout.ObjectField("游戏对象", gameObjKvp.Value.gameObject, typeof(GameObject), true);
                                                    EditorGUILayout.LabelField("永久存在", gameObjKvp.Value.forever.ToString());

                                                    if (!gameObjKvp.Value.forever)
                                                    {
                                                        EditorGUILayout.LabelField("剩余时间", gameObjKvp.Value.duration.ToString("F2") + " 秒");
                                                    }
                                                }

                                                EditorGUILayout.EndVertical();
                                            }

                                            EditorGUI.indentLevel--;
                                        }
                                    }

                                    EditorGUI.indentLevel--;
                                }

                                EditorGUILayout.EndVertical();
                                EditorGUILayout.Space();
                            }
                        }

                        EditorGUI.indentLevel--;
                    }
                }
            }
        }
    }
}