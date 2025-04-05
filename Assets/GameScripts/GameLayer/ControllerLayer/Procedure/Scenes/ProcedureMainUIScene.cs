using GameFramework.Event;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameDevScript
{
    /// <summary>
    /// 主UI场景
    /// </summary>
    public class ProcedureMainUIScene : ProcedureChangeScene
    {
        private bool mGoFightScene = false;

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            //关闭所有UI
            GameEntry.UI.CloseAllLoadedUIForms();
            mGoFightScene = false;
            LoadSceneAssets(0);//MainUIScene

            //清空当前激活的英雄和载具
            SceneEntityHelper.CurActivedHero = null;
            SceneEntityHelper.CurActivedVehicle = null;

            GameEntry.Event.Subscribe(EnterLevelStageEventArgs.EventId, OnEnterLevelStage);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (mGoFightScene)
            {
                mGoFightScene = false;
                ChangeState<ProcedureLoadMainChapterScene>(procedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(EnterLevelStageEventArgs.EventId, OnEnterLevelStage);
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnLoadAssetsComplete()
        {
            GameEntry.UI.OpenUIForm(MainUIFormData.Create());
        }

        private void OnEnterLevelStage(object sender, GameEventArgs e)
        {
            mGoFightScene = true;
        }
    }
}
