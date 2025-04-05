using GameFramework;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class FightFailureFormData : UIFormData
    {
        public static FightFailureFormData Create()
        {
            var data = ReferencePool.Acquire<FightFailureFormData>();

            return data;
        }

        public override int DtId => (int)UIFormId.FightFailureForm;

        public override void Clear()
        {
        }
    }
}