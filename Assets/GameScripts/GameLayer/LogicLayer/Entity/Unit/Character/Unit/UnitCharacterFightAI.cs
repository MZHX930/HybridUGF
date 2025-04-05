using GameFramework;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    ///<summary>
    ///角色战斗AI
    ///</summary>
    [RequireComponent(typeof(UnitCharacterCtrl))]
    [DisallowMultipleComponent]
    public class UnitCharacterFightAI : MonoBehaviour
    {
        /// <summary>
        /// 激活的战斗AI
        /// </summary>
        private IFighAIHelper m_ActiveAIHelper;

        /// <summary>
        /// 交互的角色控制器
        /// </summary>
        private UnitCharacterCtrl m_ChaLogic;

        private void Awake()
        {
            m_ChaLogic = this.gameObject.GetComponent<UnitCharacterCtrl>();
        }

        /// <summary>
        /// 激活AI
        /// </summary>
        public void ActiveAI()
        {
            if (m_ChaLogic.CtrlData.GameSide == GameSideEnum.Player)
            {
                m_ActiveAIHelper = IFighAIHelper.Create<PlayerFightAIHelper>();
            }
            else
            {
                m_ActiveAIHelper = IFighAIHelper.Create<MonsterFightAIHelper>();
            }
            m_ActiveAIHelper.Init(m_ChaLogic);

            GameEntry.OnUpdateEvent += OnUpdate;
        }

        /// <summary>
        /// 注销AI
        /// </summary>
        public void DeactiveAI()
        {
            if (m_ActiveAIHelper != null)
            {
                ReferencePool.Release(m_ActiveAIHelper);
                m_ActiveAIHelper = null;
            }
            GameEntry.OnUpdateEvent -= OnUpdate;
        }

        private void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (m_ActiveAIHelper == null || m_ChaLogic == null)
                return;

            if (m_ChaLogic.IsFighting)
            {
                m_ActiveAIHelper.OnUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }
    }
}