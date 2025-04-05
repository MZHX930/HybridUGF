using GameFramework;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class TutorialFormData : UIFormData
    {
        public static TutorialFormData Create(DRDefineGameTutorial dataRow)
        {
            var data = ReferencePool.Acquire<TutorialFormData>();
            data.DataRow = dataRow;


            return data;
        }

        public override int DtId => (int)UIFormId.TutorialForm;

        /// <summary>
        /// 显示的引导步骤配置
        /// </summary>
        public DRDefineGameTutorial DataRow { get; private set; }

        public override void Clear()
        {
            DataRow = null;
        }
    }
}