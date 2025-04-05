using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    /// <summary>
    /// 数据存档
    /// </summary>
    [Serializable]
    public sealed class GameArchiveData
    {
        /// <summary>
        /// 随机种子
        /// </summary>
        public int GlobalRandomSeed;

        /// <summary>
        /// 随机状态
        /// </summary>
        public UnityEngine.Random.State GlobalRandomState;

        /// <summary>
        /// 上一次保存时的app版本
        /// </summary>
        public string LastSaveAppVersion = null;

        /// <summary>
        /// 游戏道具存档数据
        /// </summary>
        public Dictionary<int, long> GamePropsArchiveData = new Dictionary<int, long>();

        /// <summary>
        /// 主线关卡数据
        /// </summary>
        public MainChapterArchiveData LevelStageArchiveData = new MainChapterArchiveData();

        /// <summary>
        /// 新手引导
        /// </summary>
        public TutorialArchiveData Tutorial = new TutorialArchiveData();

        /// <summary>
        /// 士兵数据
        /// </summary>
        public Dictionary<int, UnitSoldierArchiveData> SoldierInfoDict = new Dictionary<int, UnitSoldierArchiveData>();

        /*Auto Add Define*/
    }



    /// <summary>
    /// 主线章节存档数据
    /// </summary>
    [System.Serializable]
    public sealed class MainChapterArchiveData
    {
        /// <summary>
        /// 当前选中的章节id
        /// </summary>
        public int CurSelectedChapterId = 1;

        /// <summary>
        /// 最大解锁的章节id
        /// </summary>
        public int MaxUnlockChapterId = 0;

        /// <summary>
        /// 战斗临时数据
        /// </summary>
        public FightProcessData FightTempData;
    }

    /// <summary>
    /// 战斗过程数据
    /// </summary>
    /// 
    [Serializable]
    public sealed class FightProcessData
    {
        /// <summary>
        /// 当前章节阶段索引。从1开始
        /// </summary>
        public int ChapterActionId = 1;

        /// <summary>
        /// 是否是新数据，非重连数据
        /// </summary>  
        public bool IsNewData = true;

        public FightProcessData()
        {
            IsNewData = true;
            ChapterActionId = 1;
        }
    }

    /// <summary>
    /// 新手引导存档数据
    /// </summary>
    [System.Serializable]
    public sealed class TutorialArchiveData
    {
        /// <summary>
        /// 已经结束的引导组
        /// </summary>
        public List<int> FinishedGroupList = new List<int>();
    }

    /// <summary>
    /// 士兵存档数据
    /// </summary>
    [Serializable]
    public sealed class UnitSoldierArchiveData
    {
        /// <summary>
        /// DefineSoldier表ID
        /// </summary>
        public int DtId = 0;

        /// <summary>
        /// 士兵等级，0表示未解锁，解锁后等级从1开始
        /// </summary>
        public int Lv = 0;

        /// <summary>
        /// 释放激活上阵
        /// </summary>
        public bool IsActivated = false;

        /// <summary>
        /// 等级属性id
        /// </summary>
        public int LvPropertyId
        {
            get { return DtId * 10000 + Lv; }
        }

        /// <summary>
        /// 是否解锁
        /// </summary>
        public bool IsUnlock
        {
            get { return Lv > 0; }
        }
    }
}