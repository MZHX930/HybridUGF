using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 击杀了本波次的全部怪物
    /// </summary>
    public class KillBoutAllMonsterEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(KillBoutAllMonsterEventArgs).GetHashCode();

        public override int Id => EventId;

        public override void Clear()
        {
        }

        public static KillBoutAllMonsterEventArgs Create()
        {
            KillBoutAllMonsterEventArgs args = ReferencePool.Acquire<KillBoutAllMonsterEventArgs>();

            return args;
        }
    }
}