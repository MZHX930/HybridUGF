using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 建筑控制器
    /// </summary>
    public class UnitBuildingCtrl : MonoBehaviour, IUnitCtrl<UnitBuildingCtrlData>, IShellShowHandler
    {
        #region 组件
        public ShellEntityLogic ShellEntityLogic { get; private set; }
        #endregion

        #region private data
        #endregion

        #region public data
        public UnitBuildingCtrlData CtrlData { get; private set; }

        public IUnitCtrlData BaseCtrlData => CtrlData;
        #endregion

        public void OnShowShell(ShellEntityLogic shellEntityLogic)
        {
            ShellEntityLogic = shellEntityLogic;
            CtrlData = ShellEntityLogic.ShellData.UnitCtrlData as UnitBuildingCtrlData;

            //设置建筑位置
            transform.position = CtrlData.BornWorldPos;
        }

        public void OnShowView(PrefabViewEntityLogic viewEntityLogic)
        {
        }

        public void OnHideShell(ShellEntityLogic shellEntityLogic)
        {
        }

        public void OnHideView(PrefabViewEntityLogic viewEntityLogic)
        {
        }
    }
}
