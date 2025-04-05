using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 用于监听壳和视图的显示
    /// </summary>
    public interface IShellShowHandler
    {
        /// <summary>
        /// 显示壳
        /// </summary>  
        void OnShowShell(ShellEntityLogic shellEntityLogic);

        /// <summary>
        /// 显示视图
        /// </summary>
        void OnShowView(PrefabViewEntityLogic viewEntityLogic);

        /// <summary>
        /// 当壳的实体被销毁时
        /// </summary>
        void OnHideShell(ShellEntityLogic shellEntityLogic);

        /// <summary>
        /// 当视图的实体被销毁时
        /// </summary>
        void OnHideView(PrefabViewEntityLogic viewEntityLogic);
    }
}
