using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 游戏道具数量变化事件
    /// </summary>
    public class GamePropsCountChangeEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GamePropsCountChangeEventArgs).GetHashCode();

        public override int Id => EventId;

        public int DtId { get; private set; }

        public long OldCount { get; private set; }

        public long NewCount { get; private set; }

        public override void Clear()
        {
            DtId = 0;
            OldCount = 0;
            NewCount = 0;
        }

        public static GamePropsCountChangeEventArgs Create(int dtId, long oldCount, long newCount)
        {
            GamePropsCountChangeEventArgs args = ReferencePool.Acquire<GamePropsCountChangeEventArgs>();
            args.DtId = dtId;
            args.OldCount = oldCount;
            args.NewCount = newCount;

            return args;
        }
    }
}