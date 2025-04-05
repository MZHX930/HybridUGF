using GameFramework;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class SoldierHandbookFormData : UIFormData
    {
        public static SoldierHandbookFormData Create()
        {
            var data = ReferencePool.Acquire<SoldierHandbookFormData>();

            return data;
        }

        public override int DtId => (int)UIFormId.SoldierHandbookForm;

        public override void Clear()
        {
        }
    }
}