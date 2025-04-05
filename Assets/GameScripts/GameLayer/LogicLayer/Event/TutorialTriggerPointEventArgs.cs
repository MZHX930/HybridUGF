using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 新手引导触发点
    /// </summary>
    public class TutorialTriggerPointEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(TutorialTriggerPointEventArgs).GetHashCode();

        public override int Id => EventId;

        public EnumTutorialTriggerType TriggerType;
        public string[] TriggerParams;

        public override void Clear()
        {
            TriggerType = EnumTutorialTriggerType.ClickBtn;
            TriggerParams = null;
        }


        /// <summary>
        /// 创建新手引导触发点
        /// </summary>
        /// <param name="triggerType">触发类型</param>
        /// <param name="triggerParams">触发参数</param>
        /// <returns></returns>
        public static TutorialTriggerPointEventArgs Create(EnumTutorialTriggerType triggerType, string[] triggerParams = null)
        {
            TutorialTriggerPointEventArgs args = ReferencePool.Acquire<TutorialTriggerPointEventArgs>();
            args.TriggerType = triggerType;
            args.TriggerParams = (triggerParams == null) ? new string[] { } : triggerParams;

            return args;
        }
    }
}