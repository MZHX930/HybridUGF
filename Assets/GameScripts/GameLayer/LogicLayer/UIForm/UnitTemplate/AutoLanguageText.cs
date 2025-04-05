using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 自动控制TextMeshProUGUI读取Localization后显示文本
    /// </summary>
    [AddComponentMenu("Localization/AutoLanguageText")]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class AutoLanguageText : MonoBehaviour, IDyncLanguage
    {
        public TextMeshProUGUI TxtCmpt;
        public string Key = string.Empty;

        private void Awake()
        {
            if (TxtCmpt == null)
                TxtCmpt = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            OnSwitchLanguage();
        }

        public void OnSwitchLanguage()
        {
            if (TxtCmpt == null || string.IsNullOrEmpty(Key))
                return;
            TxtCmpt.text = GameEntry.Localization.GetString(Key);
        }
    }
}
