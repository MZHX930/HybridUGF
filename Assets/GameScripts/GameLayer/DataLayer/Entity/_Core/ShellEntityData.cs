using GameFramework;
using System;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 壳的数据
    /// </summary>
    public sealed class ShellEntityData : GameEntityData
    {
        public static ShellEntityData Create()
        {
            var data = ReferencePool.Acquire<ShellEntityData>();

            return data;
        }

        /// <summary>
        /// 业务逻辑数据
        /// </summary>
        public IUnitCtrlData UnitCtrlData;

        /// <summary>
        /// 视图模型名
        /// </summary>
        public string ViewName = null;

        /// <summary>
        /// 视图加载优先级
        /// </summary>
        public int ViewLoadPriority = 0;

        /// <summary>
        /// 当显示后的回调
        /// </summary>
        public Action<ShellEntityLogic> OnShowShellAction = null;

        /// <summary>
        /// 当显示模型后的回调
        /// </summary>
        public Action<ShellEntityLogic> OnShowViewAction = null;



        public override void Clear()
        {
            if (UnitCtrlData != null)
            {
                ReferencePool.Release(UnitCtrlData);
                UnitCtrlData = null;
            }
        }
    }
}
