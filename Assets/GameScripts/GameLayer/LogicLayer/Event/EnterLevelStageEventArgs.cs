using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 进入关卡挑战事件
    /// </summary>
    public class EnterLevelStageEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(EnterLevelStageEventArgs).GetHashCode();

        public override int Id => EventId;

        public override void Clear()
        {
        }

        public static EnterLevelStageEventArgs Create()
        {
            EnterLevelStageEventArgs args = ReferencePool.Acquire<EnterLevelStageEventArgs>();

            return args;
        }
    }
}