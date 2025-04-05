using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 建筑物壳
    /// </summary>
    public class UnitBuildingCtrlData : IUnitCtrlData
    {
        public static UnitBuildingCtrlData Create()
        {
            UnitBuildingCtrlData characterViewEntityData = ReferencePool.Acquire<UnitBuildingCtrlData>();

            return characterViewEntityData;
        }

        /// <summary>
        /// DRDefineBuilding的id
        /// </summary>
        public int DtId = 0;
        /// <summary>
        /// 出生的世界坐标
        /// </summary>
        public Vector3 BornWorldPos = default;



        public void Clear()
        {
            DtId = 0;
        }
    }
}
