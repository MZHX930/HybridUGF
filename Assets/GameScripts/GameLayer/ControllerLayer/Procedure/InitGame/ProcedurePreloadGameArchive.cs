using GameFramework.Event;
using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameDevScript
{
    /// <summary>
    /// 加载游戏存档
    /// </summary>
    public class ProcedurePreloadGameArchive : ProcedureBase
    {
        private bool mIsSuccessLoaded = false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mIsSuccessLoaded = false;
            GameEntry.Event.Subscribe(SuccessLoadArchiveEventArgs.EventId, OnSuccessLoadArchive);

            GameEntry.GameArchive.LoadLocalArchiveData();
        }

        private void OnSuccessLoadArchive(object sender, GameEventArgs e)
        {
            mIsSuccessLoaded = true;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (mIsSuccessLoaded)
            {
                ChangeState<ProcedurePreloadGameDataAfterArchive>(procedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEntry.Event.Unsubscribe(SuccessLoadArchiveEventArgs.EventId, OnSuccessLoadArchive);
            mIsSuccessLoaded = false;
        }
    }
}
