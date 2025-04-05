using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 隐藏角色的壳时
    /// </summary>
    public class HideCharacterShellEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(HideCharacterShellEventArgs).GetHashCode();

        public override int Id => EventId;

        public int HideEntityId { get; private set; }

        public override void Clear()
        {
            HideEntityId = 0;
        }

        public static HideCharacterShellEventArgs Create(int hideEntityId)
        {
            HideCharacterShellEventArgs args = ReferencePool.Acquire<HideCharacterShellEventArgs>();
            args.HideEntityId = hideEntityId;

            return args;
        }
    }
}