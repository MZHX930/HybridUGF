using GameFramework.Event;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public class MainUIForm : UGuiForm<MainUIFormData>
    {
        #region 组件
        public UIFormBtn BtnMain;
        public UIFormBtn BtnSoldier;
        public UIFormBtn BtnHero;
        #endregion

        #region private data
        private UIFormId m_CloseOpenUIFormId = UIFormId.Undefined;
        private UIFormId m_CurOpenUIFormId = UIFormId.Undefined;
        private int? m_CurOpenUIFormSerialId = null;
        #endregion

        #region  public data
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            BtnMain.SetClickEvent(OnClickBtnMain);
            BtnSoldier.SetClickEvent(OnClickSoldier);
            BtnHero.SetClickEvent(OnClickBtnHero);
        }

        protected override void OnFinalOpen()
        {
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            SwitchUIForm(UIFormId.MainChapterForm, () =>
            {
                return GameEntry.UI.OpenUIForm(MainChapterFormData.Create());
            });
        }

        protected override void OnFinalClose()
        {
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
        }


        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            GameEntry.UI.SetFullScreenMask(false);

            OpenUIFormSuccessEventArgs args = (OpenUIFormSuccessEventArgs)e;
            if (m_CloseOpenUIFormId != UIFormId.Undefined && args.UserData != null && args.UIForm.SerialId == (int)m_CurOpenUIFormSerialId)
            {
                GameEntry.UI.CloseUIForm(m_CloseOpenUIFormId);
                m_CloseOpenUIFormId = UIFormId.Undefined;
            }
        }

        private void OnClickBtnHero(object[] args)
        {
        }

        private void OnClickSoldier(object[] args)
        {
            SwitchUIForm(UIFormId.SoldierHandbookForm, () =>
            {
                return GameEntry.UI.OpenUIForm(SoldierHandbookFormData.Create());
            });
        }

        private void OnClickBtnMain(object[] args)
        {
            SwitchUIForm(UIFormId.MainChapterForm, () =>
            {
                return GameEntry.UI.OpenUIForm(MainChapterFormData.Create());
            });
        }

        private void SwitchUIForm(UIFormId uiFormId, System.Func<int?> action)
        {
            if (m_CurOpenUIFormId == uiFormId)
                return;

            m_CloseOpenUIFormId = m_CurOpenUIFormId;
            m_CurOpenUIFormId = uiFormId;

            m_CurOpenUIFormSerialId = action?.Invoke();
        }

    }
}
