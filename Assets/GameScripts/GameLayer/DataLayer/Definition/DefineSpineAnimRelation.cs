using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 播放的动画类型
    /// 枚举值也是播放权重，权重高的中中断权重低的动画
    /// </summary>
    public enum EnumSpineAnimKey : int
    {
        Null,

        /// <summary>
        /// 站立
        /// </summary>
        Idle,

        /// <summary>
        /// 行走
        /// </summary>
        Walk,

        /// <summary>
        /// 载具行走，时长2s
        /// </summary>
        VehicleWalk,

        /// <summary>
        /// 受伤
        /// </summary>
        BeHurt,

        /// <summary>
        /// 攻击
        /// </summary>
        Attack,

        /// <summary>
        /// 技能
        /// </summary>
        Skill,

        /// <summary>
        /// 死亡
        /// </summary>
        Dead,

        /// <summary>
        /// 拖拽开始
        /// </summary>
        DragStart,

        /// <summary>
        /// 拖拽结束
        /// </summary>
        DragEnd,

        /// <summary>
        /// 胜利
        /// </summary>
        Victory,

        /// <summary>
        /// 载具停止
        /// </summary>
        VehicleStop,
    }

    public static class DefineSpineAnimRelation
    {
        /// <summary>
        /// 动画容器
        /// 一个动画中包含了哪几种不同的播放动画
        /// 比如受伤动画，就可以根据受伤的次数来播放不同的动画
        /// </summary>
        public readonly static Dictionary<EnumSpineAnimKey, string[]> AnimContainerDic = new Dictionary<EnumSpineAnimKey, string[]>()
        {
            { EnumSpineAnimKey.Idle,        new string[] { "idle", "idle2" } },
            { EnumSpineAnimKey.Walk,        new string[] { "walk" } },
            { EnumSpineAnimKey.VehicleWalk,    new string[] { "walk2" } },
            { EnumSpineAnimKey.BeHurt,      new string[] { "affected", "affected2" } },
            { EnumSpineAnimKey.Attack,      new string[] { "attack", "attack2" } },
            { EnumSpineAnimKey.Skill,       new string[] { "skill" } },
            { EnumSpineAnimKey.Dead,        new string[] { "die", "die2" } },
            { EnumSpineAnimKey.DragStart,   new string[] { "carry" } },
            { EnumSpineAnimKey.DragEnd,     new string[] { "land" } },
            { EnumSpineAnimKey.Victory,     new string[] { "victory" } },
            { EnumSpineAnimKey.VehicleStop, new string[] { "start" } },
        };

        /// <summary>
        /// 动画文件的播放随机权重
        /// </summary>
        public readonly static Dictionary<string, int> AnimRandomPriority = new Dictionary<string, int>()
        {
            { "idle",      50 },
            { "idle2",     50 },
            { "walk",      50 },
            { "walk2",     50 },
            { "attack",    50 },
            { "attack2",   50 },
            { "skill",     50 },
            { "carray",    50 },
            { "land",      50 },
            { "victory",   50 },
            { "start",     50 },
            { "affected",  50 },
            { "affected2", 50 },
            { "die",       50 },
            { "die2",      50 },
        };
    }
}
