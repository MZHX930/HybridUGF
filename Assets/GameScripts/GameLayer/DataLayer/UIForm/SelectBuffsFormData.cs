using GameFramework;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class SelectBuffsFormData : UIFormData
    {
        public static SelectBuffsFormData Create(int[] optionIds)
        {
            var data = ReferencePool.Acquire<SelectBuffsFormData>();
            data.OptionIds = optionIds;
            return data;
        }

        public override int DtId => (int)UIFormId.SelectBuffsForm;

        /// <summary>
        /// 显示的选项id
        /// </summary>
        public int[] OptionIds { get; private set; }

        public override void Clear()
        {
            OptionIds = null;
        }
    }
}