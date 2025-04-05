using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 角色稀有度
    /// </summary>
    public enum CharacterRarityEnum : int
    {
        /// <summary>
        /// 普通
        /// </summary>
        N = 0,
        /// <summary>
        /// 稀有
        /// </summary>
        R,
        /// <summary>
        /// 超稀有
        /// </summary>
        SR,
        /// <summary>
        /// 神话
        /// </summary>
        SSR,
        /// <summary>
        /// 传说
        /// </summary>
        UR,
    }

    /// <summary>
    /// 阵营区分
    /// </summary>
    public enum GameSideEnum : int
    {
        Player = 0,
        Enemy = 1,
    }

    /// <summary>
    /// 角色类型
    /// </summary>
    public enum CharacterTypeEnum : int
    {
        /// <summary>
        /// 指挥官
        /// </summary>
        Hero = 0,

        /// <summary>
        /// 士兵
        /// </summary>
        Soldier,

        /// <summary>
        /// 士兵载具
        /// </summary>
        Vehicle,

        /// <summary>
        /// 怪物
        /// </summary>
        Monster,
    }
}
