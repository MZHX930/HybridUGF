using GameFramework;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class FightVictoryFormData : UIFormData
    {
        public static FightVictoryFormData Create()
        {
            var data = ReferencePool.Acquire<FightVictoryFormData>();

            return data;
        }

        public override int DtId => (int)UIFormId.FightVictoryForm;

        public override void Clear()
        {
        }
    }
}