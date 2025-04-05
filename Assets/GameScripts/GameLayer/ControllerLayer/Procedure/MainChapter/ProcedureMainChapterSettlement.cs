using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameDevScript
{
    /// <summary>
    /// 结算流程
    /// </summary>
    public class ProcedureMainChapterSettlement : ProcedureBase
    {
        private bool m_GoBackMain = false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_GoBackMain = false;
            bool isWin = procedureOwner.GetData<VarBoolean>("IsWin").Value;

            if (isWin)
            {
                GameEntry.UI.OpenUIForm(FightVictoryFormData.Create());
            }
            else
            {
                GameEntry.UI.OpenUIForm(FightFailureFormData.Create());
            }

            GameEntry.Event.Subscribe(GoBackMainSceneEventArgs.EventId, OnGoBackMainScene);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntry.Event.Unsubscribe(GoBackMainSceneEventArgs.EventId, OnGoBackMainScene);
            procedureOwner.RemoveData("IsWin");
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_GoBackMain)
            {
                ChangeState<ProcedureMainUIScene>(procedureOwner);
            }
        }

        private void OnGoBackMainScene(object sender, GameEventArgs e)
        {
            m_GoBackMain = true;
        }
    }
}
