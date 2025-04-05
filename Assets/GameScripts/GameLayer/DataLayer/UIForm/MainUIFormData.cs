using GameFramework;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class MainUIFormData : UIFormData
    {
        public static MainUIFormData Create()
        {
            var data = ReferencePool.Acquire<MainUIFormData>();

            return data;
        }

        public override int DtId => (int)UIFormId.MainUIForm;

        public override void Clear()
        {
        }
    }
}
