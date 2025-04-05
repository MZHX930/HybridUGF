using GameFramework;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class LoadAssetsFormData : UIFormData
    {
        public static LoadAssetsFormData Create()
        {
            LoadAssetsFormData data = ReferencePool.Acquire<LoadAssetsFormData>();
            return data;
        }

        public override int DtId => (int)UIFormId.LoadAssetsForm;

        public override void Clear()
        {
        }
    }
}