using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        /// <summary>
        /// 图集
        /// </summary>
        public static SpritesComponent Sprites { get; private set; }

        /// <summary>
        /// 红点提醒
        /// </summary>
        public static UIRedDotComponent UIRedDot { get; private set; }

        /// <summary>
        /// 游戏存档
        /// </summary>
        public static GameArchiveComponent GameArchive { get; private set; }

        /// <summary>
        /// 碰撞计算
        /// </summary>
        public static CollisionCalculationComponent CollisionCal { get; private set; }

        /// <summary>
        /// 伤害计算
        /// </summary>
        public static DamageCalculationComponent DamageCal { get; private set; }

        /// <summary>
        /// 主线关卡数据
        /// </summary>
        public static MainChapterComponent MainChapter { get; private set; }

        /// <summary>
        /// 新手引导
        /// </summary>
        public static TutorialComponent Tutorial { get; private set; }

        /// <summary>
        /// 战场数据中心
        /// </summary>
        public static BattlefieldDataHubComponent BDH { get; private set; }


        private static void InitCustomComponents()
        {
            Sprites = UnityGameFramework.Runtime.GameEntry.GetComponent<SpritesComponent>();
            UIRedDot = UnityGameFramework.Runtime.GameEntry.GetComponent<UIRedDotComponent>();
            GameArchive = UnityGameFramework.Runtime.GameEntry.GetComponent<GameArchiveComponent>();
            CollisionCal = UnityGameFramework.Runtime.GameEntry.GetComponent<CollisionCalculationComponent>();
            DamageCal = UnityGameFramework.Runtime.GameEntry.GetComponent<DamageCalculationComponent>();
            MainChapter = UnityGameFramework.Runtime.GameEntry.GetComponent<MainChapterComponent>();
            Tutorial = UnityGameFramework.Runtime.GameEntry.GetComponent<TutorialComponent>();
            BDH = UnityGameFramework.Runtime.GameEntry.GetComponent<BattlefieldDataHubComponent>();
        }
    }
}
