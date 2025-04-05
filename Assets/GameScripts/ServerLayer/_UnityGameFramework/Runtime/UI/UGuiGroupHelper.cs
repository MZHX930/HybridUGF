//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// uGUI 界面组辅助器。
    /// </summary>
    public class UGuiGroupHelper : UIGroupHelperBase
    {
        public const int DepthOffset = -1000;

        //Order in Layer的最大值为[-32768,32767]
        public const int DepthFactor = 1000;

        private int m_Depth = 0;
        private Canvas m_CachedCanvas = null;

        private bool m_IsInitRectTransformAnchor = false;
        private DeviceOrientation m_LastDeviceOrientation = DeviceOrientation.Unknown;

        /// <summary>
        /// 设置界面组深度。
        /// </summary>
        /// <param name="depth">界面组深度。</param>
        public override void SetDepth(int depth)
        {
            m_Depth = depth;
            m_CachedCanvas.overrideSorting = true;
            m_CachedCanvas.sortingOrder = DepthFactor * depth + DepthOffset;
        }

        private void Awake()
        {
            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }

        private void Start()
        {
            m_CachedCanvas.overrideSorting = true;
            m_CachedCanvas.sortingOrder = DepthFactor * m_Depth + DepthOffset;

            m_IsInitRectTransformAnchor = true;

            AdjusetRectTransformAnchors();
        }

        private void AdjusetRectTransformAnchors()
        {
            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.localPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            Vector2 offsetMax = Vector2.zero;//修改right top
            Vector2 offsetMin = Vector2.zero;//修改left bottom

            transform.offsetMax = offsetMax;
            transform.offsetMin = offsetMin;
        }
    }
}
