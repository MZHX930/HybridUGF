using GameFramework;
using GameFramework.Localization;
using GameFramework.Resource;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public interface IDyncLanguage
    {
        /// <summary>
        /// 当切换多语言后
        /// </summary>
        public void OnSwitchLanguage();
    }
}