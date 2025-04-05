using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public sealed class UnitDamageFloatText
    {
        #region 组件
        public GameObject gameObject;
        public RectTransform TrsRoot;
        public TextMeshProUGUI TxtValue;
        #endregion

        #region 数据
        private bool m_IsIdle = true;
        private Vector3 m_HideLocalPos = new Vector3(0, 9999, 0);
        private float m_MaxMoveSeconds = 1f;
        private DamageFloatTextData m_Data;

        private float m_ElapsedSeconds = 0;
        private Vector3 m_TempBornLocalPos;
        #endregion

        public bool IsIdle
        {
            get
            {
                return m_IsIdle;
            }
            private set
            {
                m_IsIdle = value;
                if (m_IsIdle)
                {
                    TrsRoot.localPosition = m_HideLocalPos;
                    m_Data = DamageFloatTextData.None;
                }
                else
                {
                    m_ElapsedSeconds = 0;
                }
            }
        }

        public UnitDamageFloatText(GameObject obj)
        {
            gameObject = obj;
            TrsRoot = gameObject.GetComponent<RectTransform>();
            TxtValue = TrsRoot.Find("Value").GetComponent<TextMeshProUGUI>();

            IsIdle = true;
        }


        public bool StartAnim(DamageFloatTextData data)
        {
            if (data.IsNone)
                return false;

            if (data.targetChaCtrl == null || data.targetChaCtrl.IsDead)
                return false;

            var bindPoint = data.targetChaCtrl.GetBindPoint(Constant.GameLogic.BindPoint_CenterPoint);
            if (bindPoint == null)
                return false;

            m_Data = data;
            IsIdle = false;

            var showWorldPos = GameEntry.UI.UICamera.ScreenToWorldPoint(FightSceneRoot.Ins.FightCamera.WorldToScreenPoint(bindPoint.transform.position));

            TrsRoot.position = new Vector3(showWorldPos.x, showWorldPos.y, TrsRoot.position.z);
            m_TempBornLocalPos = TrsRoot.localPosition;

            TxtValue.color = data.IsCritical ? Color.red : Color.white;
            m_MaxMoveSeconds = Utility.Random.GetRandom(5, 15) * 0.1f;
            TxtValue.text = $"{data.Value}";

            return true;
        }

        public void OnUpdate(float elapseSeconds)
        {
            m_ElapsedSeconds += elapseSeconds;
            if (m_ElapsedSeconds > m_MaxMoveSeconds)
            {
                IsIdle = true;
                return;
            }

            // 使用加速度运动公式: s = v0*t + (1/2)*a*t^2
            TrsRoot.localPosition = m_TempBornLocalPos + new Vector3(
                m_Data.XMoveSpeed * m_ElapsedSeconds + 0.5f * m_Data.XMoveAcceleration * m_ElapsedSeconds * m_ElapsedSeconds,
                m_Data.YMoveSpeed * m_ElapsedSeconds + 0.5f * m_Data.YMoveAcceleration * m_ElapsedSeconds * m_ElapsedSeconds,
                0
            );
        }
    }

    public struct DamageFloatTextData
    {
        /// <summary>
        /// 是否默认空值
        /// </summary>
        public bool IsNone;
        /// <summary>
        /// 飘字对象
        /// </summary>
        public UnitCharacterCtrl targetChaCtrl;
        /// <summary>
        /// 变化值
        /// </summary>
        public int Value;
        /// <summary>
        /// 是否治疗
        /// </summary>
        public bool IsHeal;
        /// <summary>
        /// 是否暴击
        /// </summary>
        public bool IsCritical;
        /// <summary>
        /// y轴初始速度
        /// </summary>
        public float YMoveSpeed;
        /// <summary>
        /// y轴加速度
        /// </summary>
        public float YMoveAcceleration;
        /// <summary>
        /// x轴初始速度
        /// </summary>
        public float XMoveSpeed;
        /// <summary>
        /// x轴加速度
        /// </summary>
        public float XMoveAcceleration;

        /// <summary>
        /// 空值
        /// </summary>
        public readonly static DamageFloatTextData None = new DamageFloatTextData() { IsNone = true };
    }
}
