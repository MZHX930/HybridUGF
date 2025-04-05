using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevScript
{
    /// <summary>
    /// 关卡界面
    /// </summary>
    public class MainChapterForm : UGuiForm<MainChapterFormData>
    {
        #region 组件
        public UIFormBtn BtnStart;
        public TextMeshProUGUI TxtStageName;
        #endregion

        #region private data
        #endregion

        #region  public data
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            BtnStart.SetClickEvent(OnBtnStartClick, null);
        }

        protected override void OnFinalOpen()
        {
            TxtStageName.text = $"Stage {GameEntry.MainChapter.CurSelectedChapterId}";
        }
        protected override void OnFinalClose()
        {
        }

        private void OnBtnStartClick(params object[] args)
        {
            GameEntry.MainChapter.ReadyToChapter();
        }


        private void OnBtnLeftStage()
        {
            if (GameEntry.MainChapter.SwitchSelectedChapter(GameEntry.MainChapter.CurSelectedChapterId - 1))
            {
                TxtStageName.text = $"Stage {GameEntry.MainChapter.CurSelectedChapterId}";
            }
        }

        private void OnBtnRightStage()
        {
            if (GameEntry.MainChapter.SwitchSelectedChapter(GameEntry.MainChapter.CurSelectedChapterId + 1))
            {
                TxtStageName.text = $"Stage {GameEntry.MainChapter.CurSelectedChapterId}";
            }
        }
    }
}