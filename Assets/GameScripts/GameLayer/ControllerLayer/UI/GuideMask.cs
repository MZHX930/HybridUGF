using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevScript
{
    public class GuideMask : Image
    {
        private float m_MaxRange = 2000;
        private float m_AnimOffset = 10f;

        private EnumGuideState m_CurState = EnumGuideState.None;

        private Vector2 m_Center;
        private float m_HalfWidth;
        private float m_HalfHeight;
        private float m_Radius;
        private float m_AnimTime;
        private float m_ElapsedTime;


        /// <summary>
        /// 设置矩形穿透区域
        /// </summary>
        public void SetRectangularPenetration(RectTransform trsRectHighlight, float animTime = 1f)
        {
            m_CurState = EnumGuideState.Rectangular;
            m_Center = rectTransform.InverseTransformPoint(trsRectHighlight.position);
            m_HalfWidth = trsRectHighlight.rect.width * 0.5f;
            m_HalfHeight = trsRectHighlight.rect.height * 0.5f;
            m_AnimTime = animTime;
            m_ElapsedTime = 0;
            StartCoroutine(PlayRectangularAnim());
        }

        private IEnumerator PlayRectangularAnim()
        {
            m_ElapsedTime = 0;
            material.SetVector("_Center", m_Center);
            material.SetFloat("_HalfWidth", m_MaxRange);
            material.SetFloat("_HalfHeight", m_MaxRange);

            while (m_ElapsedTime <= m_AnimTime)
            {
                material.SetFloat("_HalfWidth", Mathf.Lerp(m_MaxRange, m_HalfWidth + m_AnimOffset, m_ElapsedTime / m_AnimTime));
                material.SetFloat("_HalfHeight", Mathf.Lerp(m_MaxRange, m_HalfHeight + m_AnimOffset, m_ElapsedTime / m_AnimTime));
                yield return new WaitForEndOfFrame();
                m_ElapsedTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// 设置圆形穿透区域
        /// </summary>
        public void SetCirclePenetration(RectTransform trsRectHighlight, float animTime = 1f)
        {
            m_CurState = EnumGuideState.Circle;
            m_Center = rectTransform.InverseTransformPoint(trsRectHighlight.position);
            m_Radius = Mathf.Max(trsRectHighlight.rect.width, trsRectHighlight.rect.height) * 0.5f;
            m_AnimTime = animTime;
            m_ElapsedTime = 0;
            StartCoroutine(PlayCircleAnim());
        }

        private IEnumerator PlayCircleAnim()
        {
            m_ElapsedTime = 0;
            material.SetVector("_Center", m_Center);
            material.SetFloat("_Radius", m_MaxRange);

            while (m_ElapsedTime <= m_AnimTime)
            {
                material.SetFloat("_Radius", Mathf.Lerp(m_MaxRange, m_Radius + m_AnimOffset, m_ElapsedTime / m_AnimTime));
                yield return new WaitForEndOfFrame();
                m_ElapsedTime += Time.deltaTime;
            }
        }

        public void Clear()
        {
            m_CurState = EnumGuideState.None;
        }

        //计算此图像的射线位置是否为有效的命中位置
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            switch (m_CurState)
            {
                case EnumGuideState.None:
                    return true;
                case EnumGuideState.Rectangular:
                    {
                        // 判断点是否在矩形区域内(像素坐标)。这个矩形是没有旋转角度的！！！
                        bool inX = Mathf.Abs(screenPoint.x - m_Center.x) <= m_HalfWidth;
                        bool inY = Mathf.Abs(screenPoint.y - m_Center.y) <= m_HalfHeight;
                        return inX && inY;
                    }
                case EnumGuideState.Circle:
                    {
                        // 判断点是否在圆形区域内(像素坐标)
                        float sqrDis = Vector2.SqrMagnitude(screenPoint - m_Center);
                        return m_Radius * m_Radius >= sqrDis;
                    }
            }
            return true;
        }


        public enum EnumGuideState
        {
            None,
            Rectangular,
            Circle,
        }

    }
}
