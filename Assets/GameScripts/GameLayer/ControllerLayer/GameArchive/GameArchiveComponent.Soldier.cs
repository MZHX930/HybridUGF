using System.Linq;
using UnityEngine;

namespace GameDevScript
{
    /*
    士兵养成
    士兵上阵情况、等级
    */
    public partial class GameArchiveComponent
    {
        /// <summary>
        /// 提升士兵等级
        /// </summary>
        /// <param name="dtId">DefineSoldier表ID</param>
        /// <returns>是否成功</returns>
        public bool UpgradeSoliderLv(int dtId)
        {
            var soldierData = GetSoldierArchiveData(dtId);

            if (soldierData.Lv >= Constant.GameLogic.Soldier_Max_Lv)
                return false;

            DRDataSoldierLvProperty dtSoldierLvProperty = GameEntry.DataTable.GetDataTable<DRDataSoldierLvProperty>().GetDataRow(soldierData.LvPropertyId);
            if (dtSoldierLvProperty == null)
                return false;

            //检查升级所需道具是否足够
            for (int i = 0; i < dtSoldierLvProperty.UpgradePropsArray.Length; i++)
            {
                if (!CheckGamePropsIsEnough(dtSoldierLvProperty.UpgradePropsArray[i], dtSoldierLvProperty.UpgradePropsCount[i]))
                    return false;
            }

            //扣除升级所需道具
            for (int i = 0; i < dtSoldierLvProperty.UpgradePropsArray.Length; i++)
            {
                ModifyGamePropsCount(dtSoldierLvProperty.UpgradePropsArray[i], -dtSoldierLvProperty.UpgradePropsCount[i]);
            }

            soldierData.Lv++;

            return true;
        }

        /// <summary>
        /// 获取士兵存档数据
        /// </summary>
        /// <param name="dtId">DefineSoldier表ID</param>
        /// <returns>士兵存档数据</returns>
        public UnitSoldierArchiveData GetSoldierArchiveData(int dtId)
        {
            if (Data.SoldierInfoDict.ContainsKey(dtId))
                return Data.SoldierInfoDict[dtId];

            var soldierData = new UnitSoldierArchiveData();
            soldierData.DtId = dtId;
            soldierData.Lv = 0;
            soldierData.IsActivated = false;

            Data.SoldierInfoDict[dtId] = soldierData;

            return soldierData;
        }

        /// <summary>
        /// 修改士兵上阵状态
        /// </summary>
        public void ModifySoldierActivatedState(int dtId, bool isActivated)
        {
            var soldierData = GetSoldierArchiveData(dtId);
            if (soldierData.Lv <= 0)
                return;

            soldierData.IsActivated = isActivated;
        }

        /// <summary>
        /// 获取上阵士兵列表
        /// </summary>
        /// <returns>上阵士兵列表</returns>
        public UnitSoldierArchiveData[] GetActiveSoldierList()
        {
            return Data.SoldierInfoDict.Values.Where(data => data.IsActivated).ToArray();
        }
    }
}
