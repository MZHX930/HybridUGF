using GameFramework;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class FloatTipsFormData : UIFormData
    {
        public static FloatTipsFormData Create()
        {
            var data = ReferencePool.Acquire<FloatTipsFormData>();

            return data;
        }

        public override int DtId => (int)UIFormId.FloatTipsForm;

        public override void Clear()
        {
        }
    }
}