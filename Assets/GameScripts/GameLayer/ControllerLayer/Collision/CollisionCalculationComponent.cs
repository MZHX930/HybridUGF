using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 碰撞检测管理器
    /// 需要在UnitMove等组件更新之后再调用
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/CollisionCalculationComponent")]
    public class CollisionCalculationComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 敌对碰撞层级
        /// </summary>
        private static Dictionary<CollisionLayer, CollisionLayer[]> m_AdverseCollisionDic = new Dictionary<CollisionLayer, CollisionLayer[]>()
        {
            { CollisionLayer.None, new CollisionLayer[] { } },
            { CollisionLayer.PlayerCha, new CollisionLayer[] { CollisionLayer.EnemyBullet, CollisionLayer.EnemyAoe } },
            { CollisionLayer.EnemyCha, new CollisionLayer[] {  CollisionLayer.PlayerBullet, CollisionLayer.PlayerAoe } },
            { CollisionLayer.PlayerBullet, new CollisionLayer[] { CollisionLayer.EnemyCha} },
            { CollisionLayer.EnemyBullet, new CollisionLayer[] { CollisionLayer.PlayerCha} },
            { CollisionLayer.PlayerAoe, new CollisionLayer[] { CollisionLayer.EnemyCha } },
            { CollisionLayer.EnemyAoe, new CollisionLayer[] { CollisionLayer.PlayerCha } },
        };

        // 四叉树节点容量
        public const int QuadTreeCapacity = 8;
        /// <summary>
        /// 碰撞体容器层级
        /// </summary>
        private Dictionary<CollisionLayer, UnitColliderContainer> m_ContainerDic = new Dictionary<CollisionLayer, UnitColliderContainer>();
        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool m_IsInited = false;

        /// <summary>
        /// 初始化检测范围
        /// </summary>
        /// <param name="centerPos">中心点</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void Init(Vector3 centerPos, float width, float height)
        {
            m_IsInited = true;
            m_ContainerDic.Clear();
            foreach (CollisionLayer layer in System.Enum.GetValues(typeof(CollisionLayer)))
            {
                m_ContainerDic[layer] = new UnitColliderContainer(layer, centerPos, width, height, QuadTreeCapacity);
            }
        }

        public void Clear()
        {
            m_IsInited = false;
            m_ContainerDic.Clear();
        }

        /// <summary>
        /// 注册碰撞体
        /// </summary>
        public void RegisterCollider(UnitCollider collider)
        {
            if (collider == null || collider.ValidCollisionShape == null)
            {
                Log.Error($"注册碰撞体失败，碰撞体为空或碰撞体形状为空: {collider}");
                return;
            }
            m_ContainerDic[collider.Layer].RegisterCollider(collider);
        }

        /// <summary>
        /// 注销碰撞体
        /// </summary>
        public void UnregisterCollider(UnitCollider collider)
        {
            if (collider == null)
            {
                Log.Error($"注销碰撞体失败，碰撞体为空或碰撞体形状为空: {collider}");
                return;
            }
            if (m_ContainerDic.TryGetValue(collider.Layer, out var container))
                container.UnregisterCollider(collider);
        }

        /// <summary>
        /// 获取当前敌对的碰撞体集合
        /// </summary>
        public bool CheckAdverseCollisions(UnitCollider enterCollider, out List<UnitCollider> adverseColliderList)
        {
            adverseColliderList = new List<UnitCollider>();
            if (enterCollider == null || enterCollider.ValidCollisionShape == null)
            {
                return false;
            }

            var adverseLayers = m_AdverseCollisionDic[enterCollider.Layer];
            foreach (var layer in adverseLayers)
            {
                var container = m_ContainerDic[layer];
                foreach (var otherCollider in container.ColliderList)
                {
                    if (otherCollider == enterCollider)
                        continue;

                    foreach (var enterVertexWorldPos in enterCollider.ValidCollisionShape.GetVertexXYPosArray())
                    {
                        if (GraphUtils.CheckPointInPolygonShape2(enterVertexWorldPos, otherCollider.ValidCollisionShape.GetVertexXYPosArray()))
                        {
                            adverseColliderList.Add(otherCollider);
                            break;
                        }
                    }
                }
            }

            return adverseColliderList.Count > 0;
        }
    }

    /// <summary>
    /// 碰撞体容器
    /// </summary>
    public sealed class UnitColliderContainer
    {
        /// <summary>
        /// 这个容器存储的碰撞体层级
        /// </summary>
        public CollisionLayer Layer { get; private set; }
        /// <summary>
        /// 组车到当前层级的碰撞体
        /// </summary>
        public List<UnitCollider> ColliderList { get; private set; } = new List<UnitCollider>();

        // 四叉树（可选，用于优化大量对象的碰撞检测）
        private QuadTree m_QuadTree;

        public UnitColliderContainer(CollisionLayer layer, Vector3 centerPos, float width, float height, int capacity)
        {
            Layer = layer;
            m_QuadTree = new QuadTree(new Rect(centerPos.x - width / 2, centerPos.y - height / 2, width, height), capacity);
        }

        public void RegisterCollider(UnitCollider collider)
        {
            ColliderList.Add(collider);
            m_QuadTree.Insert(collider);
        }

        public void UnregisterCollider(UnitCollider collider)
        {
            ColliderList.Remove(collider);
        }

        // 使用四叉树优化碰撞检测
        public void OnLateUpdateWithQuadTree()
        {
            // 重建四叉树
            m_QuadTree.Clear();
            foreach (UnitCollider collider in ColliderList)
            {
                // 只处理边界内的碰撞体
                m_QuadTree.Insert(collider);
            }
        }
    }


    /// <summary>
    /// 碰撞层级
    /// </summary>
    public enum CollisionLayer
    {
        None,
        /// <summary>
        /// 玩家角色
        /// </summary>
        PlayerCha,
        /// <summary>
        /// 敌人角色
        /// </summary>
        EnemyCha,
        /// <summary>
        /// 玩家子弹
        /// </summary>
        PlayerBullet,
        /// <summary>
        /// 敌人子弹
        /// </summary>
        EnemyBullet,
        /// <summary>
        /// 玩家AOE
        /// </summary>
        PlayerAoe,
        /// <summary>
        /// 敌人AOE
        /// </summary>
        EnemyAoe,
    }
}
