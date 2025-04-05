using GameFramework;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class FightRuntimeFormData : UIFormData
    {
        public static FightRuntimeFormData Create(List<UIFightBoutShowData> boutShowDataList)
        {
            var data = ReferencePool.Acquire<FightRuntimeFormData>();
            data.BoutShowDataList = boutShowDataList;

            return data;
        }

        public override int DtId => (int)UIFormId.FightRuntimeForm;

        /// <summary>
        /// 显示的每波数据
        /// </summary>
        public List<UIFightBoutShowData> BoutShowDataList { get; private set; } = new List<UIFightBoutShowData>();

        public override void Clear()
        {
            BoutShowDataList.Clear();
        }
    }

    /// <summary>
    /// 在战斗中UI上显示每波数据
    /// </summary>
    public struct UIFightBoutShowData
    {
        public int BoutId;
        public int BoutType;

        public UIFightBoutShowData(int id, int type)
        {
            BoutId = id;
            BoutType = type;
        }
    }
}