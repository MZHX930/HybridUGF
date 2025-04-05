using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{

    /// <summary>
    /// 红点关系节点
    /// </summary>
    public class RedDotRelationNode
    {
        /// <summary>
        /// 匹配字段
        /// </summary>
        public string RedDotKey { get; private set; }

        /// <summary>
        /// 父级
        /// </summary>
        private RedDotRelationNode m_ParentNode;

        /// <summary>
        /// 子级列表
        /// </summary>
        private List<RedDotRelationNode> m_ChildNodeList = new List<RedDotRelationNode>();

        /// <summary>
        /// 绑定的UI对象
        /// </summary>
        private List<UnitRedDot> m_UnitRedDotList = new List<UnitRedDot>();

        /// <summary>
        /// 自身状态
        /// </summary>
        private bool m_OwnState = false;


        /// <summary>
        /// 父级
        /// </summary>
        public RedDotRelationNode ParentNode
        {
            get
            {
                return m_ParentNode;
            }
        }

        /// <summary>
        /// 子级列表
        /// </summary>
        public List<RedDotRelationNode> ChildNodeList
        {
            get
            {
                return m_ChildNodeList;
            }
        }

        /// <summary>
        /// 最终状态
        /// </summary>
        public bool FinalState
        {
            get
            {
                if (m_OwnState)
                    return true;
                else
                    return GetChildState();
            }
        }

        public RedDotRelationNode(string redDotKey)
        {
            RedDotKey = redDotKey;
            m_OwnState = false;
        }

        /// <summary>
        /// 设置父级关系
        /// </summary>
        public void AddParentRelation(RedDotRelationNode parentNode)
        {
            m_ParentNode = parentNode;
        }

        /// <summary>
        /// 移除父级关系
        /// </summary>
        public void RemoveParentRelation()
        {
            m_ParentNode = null;
        }

        /// <summary>
        /// 添加子级关系
        /// </summary>
        public void AddChildRelation(RedDotRelationNode childNode)
        {
            if (m_ChildNodeList.Contains(childNode))
                return;
            m_ChildNodeList.Add(childNode);
        }

        /// <summary>
        /// 移除子级关系
        /// </summary>
        public void RemoveChildRelation(RedDotRelationNode childNode)
        {
            if (!m_ChildNodeList.Contains(childNode))
                return;
            m_ChildNodeList.Remove(childNode);
        }

        public void AddUINode(UnitRedDot uiRedDot)
        {
            if (m_UnitRedDotList.Contains(uiRedDot))
                return;
            m_UnitRedDotList.Add(uiRedDot);

            uiRedDot.SetState(FinalState);
        }

        public void SetUINodeState(bool state)
        {
            foreach (var uiRedDot in m_UnitRedDotList)
            {
                uiRedDot.SetState(state);
            }
        }


        public void RemoveUINode(UnitRedDot uiRedDot)
        {
            if (!m_UnitRedDotList.Contains(uiRedDot))
                return;
            m_UnitRedDotList.Remove(uiRedDot);
        }

        /// <summary>
        /// 设置自身红点状态
        /// </summary>
        public void SetSelfRedDotState(bool state)
        {
            if (m_OwnState == state)
                return;

            m_OwnState = state;
            SetUINodeState(FinalState);

            //通知父级变化
            if (m_ParentNode != null)
                m_ParentNode.OnChildStateChanged();
        }

        /// <summary>
        /// 当子级状态变化时
        /// </summary>
        public void OnChildStateChanged()
        {
            SetUINodeState(FinalState);

            //通知父级变化
            if (m_ParentNode != null)
                m_ParentNode.OnChildStateChanged();
        }

        /// <summary>
        /// 获取子级集合状态
        /// </summary>
        private bool GetChildState()
        {
            foreach (var childNode in m_ChildNodeList)
            {
                if (childNode.FinalState)
                    return true;
            }
            return false;
        }
    }
}
