using GameFramework.Event;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevScript
{
    /// <summary>
    /// 战斗通用界面
    /// </summary>
    public class FightRuntimeForm : UGuiForm<FightRuntimeFormData>
    {
        #region Top组件
        [Header("Top组件")]
        public GameObject ObjTop;
        public UIFormBtn BtnPause;
        public FightProgressNode TopFightProgressNode;
        public UICurrencyElement UiCurrencyElement;
        #endregion


        #region Rest组件
        [Header("Rest组件")]
        public GameObject ObjRest;
        public UIFormBtn BtnRestStart;
        public UIFormBtn BtnRestRefresh;
        public UIFormBtn BtnRestAdRefresh;
        public HeroFightInfoBar RestHeroFightInfoBar;
        #endregion


        #region  War组件
        [Header("War组件")]
        public GameObject ObjWar;
        public HeroFightInfoBar WarHeroFightInfoBar;
        public UISimpleSlider SliderWarHp;
        public UISimpleSlider SliderWarShield;
        public UIFormBtn BtnWarSpeed;
        public UISimpleSlider SliderWarExp;
        #endregion



        #region private data
        /// <summary>
        /// 顶部显示的波次索引
        /// </summary>
        private int m_CurShowBoutIndex = 0;
        #endregion

        #region  public data
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            BtnRestStart.SetClickEvent(OnBtnStartClick, null);
            BtnRestRefresh.SetClickEvent(OnBtnRefreshClick, null);
            BtnRestAdRefresh.SetClickEvent(OnBtnAdRefreshClick, null);
        }

        protected override void OnFinalOpen()
        {
        }

        protected override void OnFinalClose()
        {
        }

        /// <summary>
        /// 开始挑战，初始化状态
        /// </summary>
        public void OnStartChallenge(int fromBoutIndex)
        {
            m_CurShowBoutIndex = fromBoutIndex;
            TopFightProgressNode.ShowSmall(UIFormData.BoutShowDataList[m_CurShowBoutIndex]);
            UiCurrencyElement.SetCurrencyId(4);//局内银币
            //隐藏rest和war
            ObjRest.SetActive(false);
            ObjWar.SetActive(false);
        }

        /// <summary>
        /// 前往下一波次
        /// </summary>
        public void GoToNextBout()
        {
            m_CurShowBoutIndex++;
            //播放动画
            UIFightBoutShowData[] showDatas = new UIFightBoutShowData[4];
            for (int i = -1; i < 3; i++)
            {
                int index = m_CurShowBoutIndex + i;
                if (index < 0 || index >= UIFormData.BoutShowDataList.Count)
                {
                    showDatas[i + 1] = new UIFightBoutShowData(-1, -1);
                }
                else
                {
                    showDatas[i + 1] = UIFormData.BoutShowDataList[index];
                }
            }
            TopFightProgressNode.ShowBig(showDatas);

            //隐藏rest和war
            ObjRest.SetActive(false);
            ObjWar.SetActive(false);
        }

        /// <summary>
        /// 显示当前波次的UI操作按钮
        /// </summary>
        public void ShowCurBoutUIElements()
        {
            int boutType = UIFormData.BoutShowDataList[m_CurShowBoutIndex].BoutType;
            ObjRest.SetActive(boutType == 0);
            ObjWar.SetActive(boutType == 1);
        }

        #region Top


        #endregion


        #region  Rest阶段
        private void OnBtnStartClick(params object[] args)
        {
            GameEntry.Event.Fire(this, FightRuntimeFormRestBtnClickEventArgs.Create(1));
        }

        private void OnBtnRefreshClick(params object[] args)
        {
            GameEntry.Event.Fire(this, FightRuntimeFormRestBtnClickEventArgs.Create(2));
        }

        private void OnBtnAdRefreshClick(object[] args)
        {
            GameEntry.Event.Fire(this, FightRuntimeFormRestBtnClickEventArgs.Create(3));
        }
        #endregion


        #region  War阶段

        #endregion


        #region 内部定义

        #endregion
    }
}
