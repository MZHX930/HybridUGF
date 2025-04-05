using UnityEngine;
using UnityGameFramework.Runtime;
using System.Collections.Generic;

namespace GameDevScript
{
    /// <summary>
    /// UI红点管理
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/UIRedDotComponent")]
    public partial class UIRedDotComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 是否检查死循环关系
        /// </summary>
        [SerializeField]
        [Tooltip("是否检查死循环关系")]
        private bool m_CheckCircularRelation = true;
        /// <summary>
        /// 缓存的红点节点
        /// (redKey,[parentRedKey,childRedKeys])
        /// </summary>
        private Dictionary<string, RedDotRelationNode> m_RedDotNodeDic = new Dictionary<string, RedDotRelationNode>();

        void Start()
        {
            //注册父子节点
            foreach (var item in m_RedDotRelationMap)
            {
                if (item.Key == EnumRedDotKey.Null)
                    continue;

                AddRedDotDataNode(item.Key.ToString(), item.Value.ToString());
            }
        }

        /// <summary>
        /// 设置红点状态
        /// </summary>
        public void SetRedDotState(EnumRedDotKey key, bool state)
        {
            if (key == EnumRedDotKey.Null)
                return;
            SetRedDotState(key.ToString(), state);
        }

        /// <summary>
        /// 设置红点状态
        /// </summary>
        /// <param name="key">红点key</param>
        /// <param name="state">红点状态</param>
        public void SetRedDotState(string key, bool state)
        {
            if (m_RedDotNodeDic.TryGetValue(key, out RedDotRelationNode node))
            {
                node.SetSelfRedDotState(state);
            }
        }

        /// <summary>
        /// 添加红点数据节点
        /// </summary>
        public bool AddRedDotDataNode(string childRedKey, string parentRedKey = null)
        {
            if (string.IsNullOrEmpty(childRedKey))
            {
                Log.Error("添加红点数据节点失败，子节点为空值");
                return false;
            }

            //添加子节点
            RedDotRelationNode childNode = null;
            if (!m_RedDotNodeDic.TryGetValue(childRedKey, out childNode))
            {
                childNode = new RedDotRelationNode(childRedKey);
                m_RedDotNodeDic.Add(childRedKey, childNode);
            }

            //添加父节点
            RedDotRelationNode parentNode = null;
            if (!string.IsNullOrEmpty(parentRedKey) && parentRedKey != EnumRedDotKey.Null.ToString())
            {
                if (!m_RedDotNodeDic.TryGetValue(parentRedKey, out parentNode))
                {
                    parentNode = new RedDotRelationNode(parentRedKey);
                    m_RedDotNodeDic.Add(parentRedKey, parentNode);
                }
            }

            if (childNode.ParentNode == parentNode)
            {
                return true;
            }

            //解除旧的父子关系
            if (childNode.ParentNode != null)
                childNode.ParentNode.RemoveChildRelation(childNode);
            childNode.RemoveParentRelation();

            //设置新的父子关系
            childNode.AddParentRelation(parentNode);
            if (parentNode != null)
                parentNode.AddChildRelation(childNode);

            //验证是否存在死循环关系
            if (m_CheckCircularRelation)
            {
                RedDotRelationNode tempNode = parentNode.ParentNode;
                while (tempNode != null)
                {
                    if (tempNode == parentNode)
                    {
                        Log.Error($"存在死循环关系：{childRedKey}，{parentRedKey}");
                        RemoveRedDotDataNode(childRedKey);
                        return false;
                    }
                    tempNode = tempNode.ParentNode;
                }
            }

            return true;
        }

        /// <summary>
        /// 移除红点数据节点
        /// </summary>
        public void RemoveRedDotDataNode(string redDotKey)
        {
            if (string.IsNullOrEmpty(redDotKey))
                return;

            if (m_RedDotNodeDic.TryGetValue(redDotKey, out RedDotRelationNode node))
            {
                //清理父级联系
                node.ParentNode.RemoveChildRelation(node);
                node.RemoveParentRelation();

                //清理子级联系
                for (int i = node.ChildNodeList.Count - 1; i >= 0; i--)
                {
                    var childNode = node.ChildNodeList[i];
                    childNode.RemoveParentRelation();
                    node.RemoveChildRelation(childNode);
                }

                m_RedDotNodeDic.Remove(redDotKey);
            }
        }

        /// <summary>
        /// 绑定UI节点
        /// </summary>
        public void BindUINode(UnitRedDot uiRedDot)
        {
            if (m_RedDotNodeDic.TryGetValue(uiRedDot.RedDotKey, out RedDotRelationNode node))
            {
                node.AddUINode(uiRedDot);
            }
            else
            {
                Debug.LogError($"绑定UI节点失败：{uiRedDot.RedDotKey}，没有注册key！");
            }
        }

        /// <summary>
        /// 解绑UI节点
        /// </summary>
        public void UnbindUINode(UnitRedDot uiRedDot)
        {
            if (m_RedDotNodeDic.TryGetValue(uiRedDot.RedDotKey, out RedDotRelationNode node))
            {
                node.RemoveUINode(uiRedDot);
            }
        }
    }
}
