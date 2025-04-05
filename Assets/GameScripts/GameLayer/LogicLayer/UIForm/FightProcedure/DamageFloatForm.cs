using Cysharp.Threading.Tasks;
using GameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 伤害飘字界面
    /// </summary>
    public class DamageFloatForm : UGuiForm<DefaultUIFormData>
    {
        private static DamageFloatForm m_Ins = null;

        #region 组件
        public GameObject ObjTemplate;
        #endregion

        #region  public data
        [Range(10, 400)]
        public int MaxCount = 100;
        #endregion

        #region private data
        private UnitDamageFloatText[] m_TextCtrls = null;
        private DamageFloatTextData[] m_CachedTextDatas = null;
        private int m_CachedDataEnqueueIndex = 0;
        #endregion

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_CachedTextDatas = new DamageFloatTextData[MaxCount];
            m_TextCtrls = new UnitDamageFloatText[MaxCount];

            m_TextCtrls[0] = new UnitDamageFloatText(ObjTemplate);
            m_CachedTextDatas[0] = DamageFloatTextData.None;
            for (int i = 1; i < MaxCount; i++)
            {
                GameObject obj = Instantiate(ObjTemplate, BaseComponents.TrsWindow, false);
#if UNITY_EDITOR
                obj.name = "FT_" + i;
#endif
                m_TextCtrls[i] = new UnitDamageFloatText(obj);
                m_CachedTextDatas[i] = DamageFloatTextData.None;
            }
        }

        protected override void OnFinalOpen()
        {
            m_Ins = this;
        }

        protected override void OnFinalClose()
        {
            m_CachedDataEnqueueIndex = 0;
            for (int i = 0; i < MaxCount; i++)
            {
                m_CachedTextDatas[i] = DamageFloatTextData.None;
            }

            m_Ins = null;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            for (int i = 0; i < MaxCount; i++)
            {
                if (m_TextCtrls[i].IsIdle)
                {
                    m_TextCtrls[i].StartAnim(m_CachedTextDatas[i]);
                    m_CachedTextDatas[i] = DamageFloatTextData.None;
                }
                else
                {
                    m_TextCtrls[i].OnUpdate(elapseSeconds);
                }
            }
        }

        /// <summary>
        /// 弹出伤害/治疗数字
        /// </summary>
        /// <param name="chaCtrl">来源</param>
        /// <param name="value">伤害/治疗值</param>
        /// <param name="isHeal">是否是治疗</param>
        /// <param name="isCritical">是否暴击</param>
        public static void PopUpNumberOnCharacter(UnitCharacterCtrl chaCtrl, int value, bool isHeal = false, bool isCritical = false)
        {
            if (m_Ins == null)
                return;

            if (m_Ins.m_CachedDataEnqueueIndex >= m_Ins.MaxCount)
            {
                m_Ins.m_CachedDataEnqueueIndex = 0;
            }

            m_Ins.m_CachedTextDatas[m_Ins.m_CachedDataEnqueueIndex++] = new DamageFloatTextData()
            {
                IsNone = false,
                targetChaCtrl = chaCtrl,
                Value = value,
                IsHeal = isHeal,
                IsCritical = isCritical,
                YMoveSpeed = Utility.Random.GetRandom(60, 100),
                YMoveAcceleration = Utility.Random.GetRandom(100, 300),
                XMoveSpeed = Utility.Random.GetRandom(-60, 60),
                XMoveAcceleration = Utility.Random.GetRandom(-200, 200),
            };
        }
    }
}
