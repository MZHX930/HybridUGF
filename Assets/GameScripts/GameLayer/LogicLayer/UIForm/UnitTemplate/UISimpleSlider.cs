using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevScript
{
    /// <summary>
    /// 简单滑块
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class UISimpleSlider : MonoBehaviour
    {
        #region 组件
        public RectMask2D RectMask2D;
        public TextMeshProUGUI TxtValue;
        #endregion

        private float m_RectWidth { get { return RectMask2D.rectTransform.rect.width; } }

        [SerializeField]
        [HideInInspector]
        private float m_FinalSliderRatio = 0;
        /// <summary>
        /// 最终滑块值，[0,1]
        /// </summary>
        public float FinalSliderRatio
        {
            get
            {
                return m_FinalSliderRatio;
            }
            set
            {
                if (value > 1)
                    m_FinalSliderRatio = value - Mathf.Floor(value);
                else if (value < 0)
                    m_FinalSliderRatio = 0;
                else
                    m_FinalSliderRatio = value;

                RectMask2D.padding = new Vector4(0, 0, m_RectWidth * (1 - m_FinalSliderRatio), 0);
            }
        }

        /// <summary>
        /// 设置滑块值
        /// </summary>
        /// <param name="sliderRatio">滑块值：[0,1]</param>
        /// <param name="valueDesc">显示值</param>
        public void SetValue(float sliderRatio, string valueDesc = "")
        {
            if (TxtValue != null)
                TxtValue.text = valueDesc;

            StopAllCoroutines();
            FinalSliderRatio = sliderRatio;
        }

        /// <summary>
        /// 设置滑块值
        /// </summary>
        /// <param name="sliderRatio">最终滑块值[0,?]</param>
        /// <param name="animTime">动画时间</param>
        /// <param name="valueDesc">显示值</param>
        public void SetValueAnim(float sliderRatio, float animTime = 0.2f, string valueDesc = "")
        {
            if (TxtValue != null)
                TxtValue.text = valueDesc;

            if (animTime <= 0)
            {
                SetValue(sliderRatio, valueDesc);
                return;
            }

            StopAllCoroutines();
            // if (m_AnimResidualSec > 0)
            // {
            //     FinalSliderRatio = m_AnimFinalSliderRatio;
            // }

            m_AnimDiffValueSec = (sliderRatio - FinalSliderRatio) / animTime;
            m_AnimResidualSec = animTime;
            m_AnimFinalSliderRatio = sliderRatio;

            StartCoroutine(SliderAnimCoroutine());
        }

        /// <summary>
        /// 每一帧的滑块差值
        /// </summary>
        private float m_AnimDiffValueSec = 0;
        private float m_AnimResidualSec = 0;
        private float m_AnimFinalSliderRatio = 0;

        private IEnumerator SliderAnimCoroutine()
        {
            while (m_AnimResidualSec > 0)
            {
                m_AnimResidualSec -= Time.deltaTime;
                FinalSliderRatio += m_AnimDiffValueSec * Time.deltaTime;
                yield return null;
            }

            FinalSliderRatio = m_AnimFinalSliderRatio;

            m_AnimDiffValueSec = 0;
            m_AnimResidualSec = 0;
            m_AnimFinalSliderRatio = 0;
        }
    }
}
