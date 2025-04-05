using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 游戏中占位实体（角色的壳，载具壳，技能特效壳）
    /// 只负责管理视图模型，业务逻辑不要在这里写。业务逻辑使用组合方式实现
    /// 可以作为一个组合管理器？
    /// </summary>
    public sealed class ShellEntityLogic : EntityLogic
    {
        #region 组件
        /// <summary>
        /// 视图模型挂载点
        /// </summary>
        private ViewContainer m_ViewContainer;

        /// <summary>
        /// 视图entity
        /// </summary>
        public PrefabViewEntityLogic CurViewEntityLogic { get; private set; }

        /// <summary>
        /// 业务逻辑控制器
        /// </summary>
        public IUnitCtrl UnitCtrl { get; private set; }
        /// <summary>
        /// 壳和视图显示通知对象集合
        /// </summary>
        public IShellShowHandler[] ShellShowHandlers { get; private set; }
        #endregion

        /// <summary>
        /// 运行时数据
        /// </summary>
        public ShellEntityData ShellData { get; private set; }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_ViewContainer = GetComponentInChildren<ViewContainer>();
            UnitCtrl = GetComponent<IUnitCtrl>();
            ShellShowHandlers = GetComponents<IShellShowHandler>();
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

#if UNITY_EDITOR
            gameObject.name = $"{gameObject.name.Replace("(Clone)", "").Split('_')[0]}_{Entity.Id}";
#endif

            ShellData = userData as ShellEntityData;

            //需要先执行Ctrl的显示，避免OnShowShellAction中需要用到CtrlData
            foreach (var handler in ShellShowHandlers)
            {
                handler.OnShowShell(this);
            }
            ShellData.OnShowShellAction?.Invoke(this);

            ToLoadViewEntity();
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            if (!isShutdown)
            {
                foreach (var handler in ShellShowHandlers)
                {
                    handler.OnHideShell(this);
                }
            }

            base.OnHide(isShutdown, userData);

            if (!isShutdown && ShellData != null)
            {
                ReferencePool.Release(ShellData);
                ShellData = null;
            }

            CurViewEntityLogic = null;
        }

        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);

            if (childEntity is PrefabViewEntityLogic)
            {
                CurViewEntityLogic = childEntity as PrefabViewEntityLogic;
                //需要先执行Ctrl的显示，避免OnShowViewAction中需要用到CtrlData
                foreach (var handler in ShellShowHandlers)
                {
                    handler.OnShowView(CurViewEntityLogic);
                }
                ShellData.OnShowViewAction?.Invoke(this);
            }
        }

        protected override void OnDetached(EntityLogic childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);

            if (childEntity is PrefabViewEntityLogic hidePrefabLogic)
            {
                foreach (var handler in ShellShowHandlers)
                {
                    handler.OnHideView(hidePrefabLogic);
                }
                if (CurViewEntityLogic == hidePrefabLogic)
                {
                    CurViewEntityLogic = null;
                }
            }
        }

        /// <summary>
        /// 开始加载视图模型
        /// </summary>
        /// <returns>是否有视图模型需要加载</returns>
        private void ToLoadViewEntity()
        {
            if (CurViewEntityLogic != null)
            {
                GameEntry.Entity.HideEntity(CurViewEntityLogic.Entity);
                CurViewEntityLogic = null;
            }

            PrefabViewEntityData viewEntityData = PrefabViewEntityData.Create(Entity, m_ViewContainer.transform, Vector3.zero);
            GameEntry.Entity.ShowEntity<PrefabViewEntityLogic>(
                GameEntry.Entity.CreateEntitySerialId(),
                AssetPathUtility.GetPrefabViewPath(ShellData.ViewName),
                Constant.EntityGroup.TmpPrefab,
                ShellData.ViewLoadPriority,
                viewEntityData
            );
        }


        /// <summary>
        /// 切换模型视图
        /// </summary>
        public void ChangeModel(string viewName)
        {
            ShellData.ViewName = viewName;
            ToLoadViewEntity();
        }
    }
}
