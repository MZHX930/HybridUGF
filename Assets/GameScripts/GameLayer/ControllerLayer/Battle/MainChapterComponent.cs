using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 主线章节控制
    /// </summary>  
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/MainChapterComponent")]
    public sealed class MainChapterComponent : GameFrameworkComponent
    {
        #region public数据
        public MainChapterArchiveData Data { get; private set; }
        /// <summary>
        /// 最大关卡数
        /// </summary>
        public int MaxChapterId { get; private set; }

        /// <summary>
        /// 当前选中关卡ID
        /// </summary>
        public int CurSelectedChapterId
        {
            get { return Data.CurSelectedChapterId; }
            set { Data.CurSelectedChapterId = value; }
        }
        #endregion

        #region private数据
        private Dictionary<int, int> m_ChapterActionCountDict = new Dictionary<int, int>();
        #endregion

        public async UniTask InitDataAfterArchive()
        {
            MaxChapterId = GameEntry.DataTable.GetDataTable<DRDefineMainChapter>().Count;
            Data = GameEntry.GameArchive.Data.LevelStageArchiveData;

            GameEntry.GameArchive.RegisterSaveEvent(OnSaveArchiveData);
            await UniTask.NextFrame();
        }

        private void OnSaveArchiveData(SaveArchiveReasonTypeEnum reason)
        {
        }

        /// <summary>
        /// 切换关卡
        /// </summary>
        public bool SwitchSelectedChapter(int chapterDtId)
        {
            if (chapterDtId <= Data.MaxUnlockChapterId && chapterDtId >= 1)
            {
                CurSelectedChapterId = chapterDtId;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取这个关卡总共有多少波次
        /// </summary>
        public int GetChapterActionCount(int chapterDtId)
        {
            if (m_ChapterActionCountDict.ContainsKey(chapterDtId))
            {
                return m_ChapterActionCountDict[chapterDtId];
            }

            var dtrChapter = GameEntry.DataTable.GetDataTable<DRDefineMainChapter>().GetDataRow(chapterDtId);
            if (dtrChapter == null)
            {
                return 0;
            }

            int actionCount = 0;
            int actionIndex = 1;
            while (true)
            {
                // int actionDtId = chapterDtId * 10000 + actionIndex;
                int actionDtId = GetActionId(chapterDtId, actionIndex);
                if (GameEntry.DataTable.GetDataTable<DRDefineMainChapterAction>().GetDataRow(actionDtId) == null)
                {
                    break;
                }
                actionCount++;
                actionIndex++;
            }

            m_ChapterActionCountDict.Add(chapterDtId, actionCount);
            return actionCount;
        }


        /// <summary>
        /// 准备进入主线章节
        /// </summary>
        public void ReadyToChapter()
        {
            if (GameEntry.GameArchive.GetActiveSoldierList().Length < 1)
            {
                FloatTipsForm.ShowTipsKey("To Activate Soldier");
                return;
            }

            var tempData = new FightProcessData();
            Data.FightTempData = tempData;
            GameEntry.Event.Fire(this, EnterLevelStageEventArgs.Create());
        }

        /// <summary>
        /// 当退出主线章节后
        /// </summary>
        /// <param name="isWin">是否胜利</param>
        public void ExitChapter(bool isWin)
        {
            if (isWin)
            {
                if (CurSelectedChapterId == MaxChapterId)
                {
                    //解锁更高关卡
                    Data.MaxUnlockChapterId++;
                    if (Data.MaxUnlockChapterId > MaxChapterId)
                    {
                        Data.MaxUnlockChapterId = MaxChapterId;
                    }
                }
            }
            else
            {
            }

            Data.FightTempData = null;
        }

        /// <summary>
        /// 获取当前章节阶段id
        /// </summary>
        public int GetCurActionId()
        {
            // return CurSelectedChapterId * 10000 + Data.FightTempData.ChapterActionId;
            return GetActionId(CurSelectedChapterId, Data.FightTempData.ChapterActionId);
        }

        public int GetActionId(int chapterDtId, int actionIndex)
        {
            return chapterDtId * 10000 + actionIndex;
        }

        /// <summary>
        /// 当完成波次行动后
        /// </summary>
        public void OnCompleteBoutAction()
        {
            Data.FightTempData.ChapterActionId++;
        }
    }
}