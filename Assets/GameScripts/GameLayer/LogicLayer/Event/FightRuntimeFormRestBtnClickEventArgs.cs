using GameFramework;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 战斗运行时界面在休憩时的点击事件
    /// </summary>
    public class FightRuntimeFormRestBtnClickEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(FightRuntimeFormRestBtnClickEventArgs).GetHashCode();

        public override int Id => EventId;
        /// <summary>
        /// 点击的按钮类型 1:开始战斗 2:刷新士兵 3:广告刷新
        /// </summary>
        public int ClickRestBtnType { get; private set; }


        public override void Clear()
        {
            ClickRestBtnType = 0;
        }

        /// <summary>
        /// 战斗运行时界面在休憩时的点击事件
        /// </summary>
        /// <param name="clickRestBtnType">点击的按钮类型 1:开始战斗 2:刷新士兵 3:广告刷新</param>
        /// <returns></returns>
        public static FightRuntimeFormRestBtnClickEventArgs Create(int clickRestBtnType)
        {
            FightRuntimeFormRestBtnClickEventArgs args = ReferencePool.Acquire<FightRuntimeFormRestBtnClickEventArgs>();
            args.ClickRestBtnType = clickRestBtnType;

            return args;
        }
    }
}