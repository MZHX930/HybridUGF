using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 显示红点树状结构
    /// 根据m_RedDotNodeDic显示树状图
    /// </summary>
    [CustomEditor(typeof(UIRedDotComponent))]
    public class UIRedComponentInspector : Editor
    {
        private List<RedDotRelationNode> m_RootNodeList;
        private List<bool> m_FoldoutList;

        private void OnEnable()
        {
            RefreshTree();
        }

        private void RefreshTree()
        {
            var fieldInfo = typeof(UIRedDotComponent).GetField("m_RedDotNodeDic", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            Dictionary<string, RedDotRelationNode> redDotNodeDic = fieldInfo.GetValue(target as UIRedDotComponent) as Dictionary<string, RedDotRelationNode>;

            //解析redDotNodeDic，将数据以树状结构显示
            m_FoldoutList = new List<bool>();
            //提取所有根节点
            m_RootNodeList = new List<RedDotRelationNode>();
            foreach (var item in redDotNodeDic)
            {
                if (item.Value.ParentNode == null)
                {
                    m_RootNodeList.Add(item.Value);
                    m_FoldoutList.Add(true);
                }
            }
            //按照字符串排序
            m_RootNodeList.Sort((a, b) => a.RedDotKey.CompareTo(b.RedDotKey));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // 添加刷新按钮
            if (GUILayout.Button("刷新红点树"))
            {
                RefreshTree();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("红点树状结构", EditorStyles.boldLabel);

            // 显示树状结构
            for (int i = 0; i < m_RootNodeList.Count; i++)
            {
                m_FoldoutList[i] = EditorGUILayout.Foldout(m_FoldoutList[i], m_RootNodeList[i].RedDotKey);
                if (m_FoldoutList[i])
                {
                    DisplayNode(m_RootNodeList[i], 0);
                }
            }
        }

        private void DisplayNode(RedDotRelationNode node, int indentLevel)
        {
            // 计算缩进
            string indent = new string(' ', indentLevel * 4);

            // 节点状态颜色显示
            GUI.color = node.FinalState ? Color.red : Color.white;

            // 显示当前节点
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(indent + $"● {node.RedDotKey} = {node.FinalState}");
            EditorGUILayout.EndHorizontal();

            // 恢复颜色
            GUI.color = Color.white;

            // 递归显示子节点
            if (node.ChildNodeList != null && node.ChildNodeList.Count > 0)
            {
                foreach (var childNode in node.ChildNodeList)
                {
                    DisplayNode(childNode, indentLevel + 1);
                }
            }
        }
    }
}
