using GameFramework;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class MainChapterFormData : UIFormData
    {
        public static MainChapterFormData Create()
        {
            var data = ReferencePool.Acquire<MainChapterFormData>();

            return data;
        }

        public override int DtId => (int)UIFormId.MainChapterForm;

        public override void Clear()
        {
        }
    }
}