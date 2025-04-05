using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 碰撞体单元
    /// </summary>
    public class UnitCollider : MonoBehaviour
    {
        /// <summary>
        /// 碰撞层级
        /// </summary>
        public CollisionLayer Layer { get; private set; }

        /// <summary>
        /// 有效的碰撞信息
        /// </summary>
        public CollisionShape ValidCollisionShape;

        /// <summary>
        /// 绑定的壳
        /// </summary>
        public ShellEntityLogic ShellEntityLogic { get; private set; }

        /// <summary>
        /// 实体逻辑控制器
        /// </summary>
        public IUnitCtrl<IUnitCtrlData> UnitCtrl;

        void Awake()
        {
            if (UnitCtrl == null)
                UnitCtrl = GetComponent<IUnitCtrl<IUnitCtrlData>>();
        }

        /// <summary>
        /// 激活碰撞
        /// </summary>
        public void ActiveCollision(CollisionLayer layer)
        {
            Layer = layer;
            ValidCollisionShape = transform.GetComponentInChildren<CollisionShape>(false);

            ShellEntityLogic = GetComponent<ShellEntityLogic>();
            GameEntry.CollisionCal.RegisterCollider(this);
        }

        /// <summary>
        /// 移除碰撞
        /// </summary>
        public void RemoveCollision()
        {
            GameEntry.CollisionCal.UnregisterCollider(this);
            ShellEntityLogic = null;
        }
    }
}
