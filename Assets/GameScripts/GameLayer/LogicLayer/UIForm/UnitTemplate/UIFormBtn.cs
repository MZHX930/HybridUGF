using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// UI按钮组件
    /// 支持多语言索引
    /// 支持点击时播放音效
    /// 点击时发送引导触发点
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public class UIFormBtn : Button, IDyncLanguage
    {
        #region 组件
        public Image ImgCmpt;
        public TextMeshProUGUI TxtCmpt;
        #endregion

        #region 数据
        [Header("多语言Key")]
        public string LanguageKey = null;

        [Header("点击时播放的音效id")]
        public int UISoundId = 0;
        #endregion

        #region cached
        private UIBtnClickEvent m_ClickAction;
        private object[] m_ClickArgs;
        #endregion

        protected override void Awake()
        {
            base.Awake();

            onClick.AddListener(OnClickBtn);

            if (ImgCmpt == null)
                ImgCmpt = GetComponent<Image>();

            if (TxtCmpt == null)
                TxtCmpt = transform.Find("btnTxt")?.GetComponent<TextMeshProUGUI>();
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            SetLanguageKey(LanguageKey);
        }

        protected override void OnDestroy()
        {
            onClick.RemoveListener(OnClickBtn);
            m_ClickAction = null;

            base.OnDestroy();
        }

        /// <summary>
        /// 设置点击事件
        /// </summary>
        /// <param name="clickAction">点击事件</param>
        public void SetClickEvent(UIBtnClickEvent clickAction, params object[] args)
        {
            m_ClickAction = clickAction;
            m_ClickArgs = args;
        }

        private void OnClickBtn()
        {
            m_ClickAction?.Invoke(m_ClickArgs);

            if (UISoundId > 0)
            {
                GameEntry.Sound.PlayUISound(UISoundId);
            }

            GameEntry.Event.Fire(GameEntry.Tutorial, TutorialTriggerPointEventArgs.Create(
                EnumTutorialTriggerType.ClickBtn,
                new string[] { UITools.GetUIRootPath(this.transform) }
            ));
        }

        /// <summary>
        /// 设置按钮文本
        /// </summary>
        public void SetText(string text)
        {
            if (TxtCmpt == null)
                return;
            TxtCmpt.text = text;
        }

        /// <summary>
        /// 设置多语言key
        /// </summary>
        public void SetLanguageKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            LanguageKey = key;

            if (GameEntry.Localization == null)
                SetText(LanguageKey);
            else
                SetText(GameEntry.Localization.GetString(LanguageKey));
        }

        /// <summary>
        /// 按钮置灰
        /// </summary>
        public void SetGray()
        {
            foreach (var item in GetComponentsInChildren<CanvasRenderer>())
            {
                item.SetColor(new Color(0.5f, 0.5f, 0.5f, 1f));
            }
        }

        /// <summary>
        /// 按钮恢复原色
        /// </summary>
        public void SetNormal()
        {
            foreach (var item in GetComponentsInChildren<CanvasRenderer>())
            {
                item.SetColor(new Color(1f, 1f, 1f, 1f));
            }
        }

        public void OnSwitchLanguage()
        {
            SetLanguageKey(LanguageKey);
        }
    }

    public delegate void UIBtnClickEvent(object[] args);
}
