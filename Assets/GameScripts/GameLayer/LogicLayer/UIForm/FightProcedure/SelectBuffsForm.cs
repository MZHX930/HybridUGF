using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevScript
{
    /// <summary>
    /// 战斗3选1界面
    /// </summary>
    public class SelectBuffsForm : UGuiForm<SelectBuffsFormData>
    {
        #region 组件
        public RectTransform TrsOptions;
        public UIFormBtn BtnRefresh;
        public UIFormBtn BtnCheck;
        #endregion

        #region private data
        private OptionGrid[] m_ShowOptionGrids;
        #endregion

        #region  public data
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_ShowOptionGrids = new OptionGrid[TrsOptions.childCount];
            for (int i = 0; i < TrsOptions.childCount; i++)
            {
                m_ShowOptionGrids[i] = new OptionGrid(TrsOptions.GetChild(i));
            }

            BtnCheck.SetClickEvent(OnBtnCheckClick);
            BtnRefresh.SetClickEvent(OnBtnRefreshClick);
        }

        protected override void OnFinalOpen()
        {
            //显示
            for (int i = 0; i < UIFormData.OptionIds.Length; i++)
            {
                m_ShowOptionGrids[i].Show(UIFormData.OptionIds[i]);
            }
        }

        protected override void OnFinalClose()
        {
            GameEntry.BDH.OnCloseSelectBuffsForm().Forget();

            GameEntry.UI.SetFullScreenMask(false);
        }

        private void OnBtnRefreshClick(object[] args)
        {

        }

        private void OnBtnCheckClick(object[] args)
        {

        }


        [Serializable]
        public sealed class OptionGrid
        {
            public RectTransform TrsRect;
            public UIFormBtn BtnSelect;
            public Image ImgIcon;
            public TextMeshProUGUI TxtTitle;
            public TextMeshProUGUI TxtDesc;

            private int m_OptionId;

            public OptionGrid(Transform trsGrid)
            {
                TrsRect = trsGrid.GetComponent<RectTransform>();
                ImgIcon = trsGrid.Find("Icon").GetComponent<Image>();
                TxtTitle = trsGrid.Find("Title").GetComponent<TextMeshProUGUI>();
                TxtDesc = trsGrid.Find("Desc").GetComponent<TextMeshProUGUI>();

                BtnSelect = trsGrid.GetComponent<UIFormBtn>();
                BtnSelect.SetClickEvent(OnBtnSelectClick);
            }

            /// <summary>
            /// 显示选项
            /// </summary>
            /// <param name="optionId">DefineIntensifyOption表的id</param>
            public void Show(int optionId)
            {
                m_OptionId = optionId;

                DRDefineIntensifyOption option = GameEntry.DataTable.GetDataTable<DRDefineIntensifyOption>().GetDataRow(optionId);
                TxtDesc.text = GameEntry.Localization.GetString(option.Desc);
            }

            private void OnBtnSelectClick(object[] args)
            {
                GameEntry.UI.SetFullScreenMask(true);
                GameEntry.BDH.SelectIntensifyOption(m_OptionId);
                GameEntry.UI.CloseUIForm(UIFormId.SelectBuffsForm);
            }
        }
    }
}
