using GameFramework;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class SoldierDetailInfoFormData : UIFormData
    {
        public static SoldierDetailInfoFormData Create(DRDefineSoldier dtrConfig)
        {
            var data = ReferencePool.Acquire<SoldierDetailInfoFormData>();
            data.DtrDefineSoldier = dtrConfig;

            return data;
        }

        public override int DtId => (int)UIFormId.SoldierDetailInfoForm;
        public DRDefineSoldier DtrDefineSoldier;

        public override void Clear()
        {
            DtrDefineSoldier = null;
        }
    }
}