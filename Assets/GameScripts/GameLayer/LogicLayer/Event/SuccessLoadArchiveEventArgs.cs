using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 成功加载存档数据
    /// </summary>
    public class SuccessLoadArchiveEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(SuccessLoadArchiveEventArgs).GetHashCode();

        public override int Id => EventId;

        public override void Clear()
        {
        }

        public static SuccessLoadArchiveEventArgs Create()
        {
            SuccessLoadArchiveEventArgs args = ReferencePool.Acquire<SuccessLoadArchiveEventArgs>();
            return args;
        }
    }
}