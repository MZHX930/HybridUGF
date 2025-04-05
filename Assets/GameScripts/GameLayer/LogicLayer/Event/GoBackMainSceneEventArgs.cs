using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 返回主场景
    /// </summary>
    public class GoBackMainSceneEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GoBackMainSceneEventArgs).GetHashCode();

        public override int Id => EventId;

        public override void Clear()
        {
        }

        public static GoBackMainSceneEventArgs Create()
        {
            GoBackMainSceneEventArgs args = ReferencePool.Acquire<GoBackMainSceneEventArgs>();

            return args;
        }
    }
}