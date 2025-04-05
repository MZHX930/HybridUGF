using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameDevScript
{
    /// <summary>
    /// 主线关卡 - 招募阶段
    /// > 开启UI触摸遮罩；镜头缩放 ；加载酒馆
    /// > 等待酒馆加载后，从酒馆中走出士兵
    /// > 等待士兵停止移动后，运行玩家操作
    /// </summary>
    public class ProcedureMainChapterActionRest : ProcedureBase
    {
        private MilitaryCampMgr m_MilitaryCampMgr;
        private FightRuntimeForm m_FightRuntimeForm;

        private bool m_IsFinished = false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_IsFinished = false;
            m_MilitaryCampMgr = GameObject.Find("Root").GetComponent<MilitaryCampMgr>();
            m_FightRuntimeForm = GameEntry.UI.GetUIForm(UIFormId.FightRuntimeForm) as FightRuntimeForm;

            AsyncOnEnter().Forget();

            //监听按钮点击事件
            GameEntry.Event.Subscribe(FightRuntimeFormRestBtnClickEventArgs.EventId, OnRestClickBtn);
        }

        private async UniTaskVoid AsyncOnEnter()
        {
            //设置摄像机聚焦
            var task1 = FightSceneRoot.Ins.SetCameraFocusMode(FightCameraFocusModeEnum.Rest, Constant.GameLogic.FightCameraFocusAnimTime);

            //军营初始化
            int chapterActionId = GameEntry.MainChapter.GetCurActionId();
            var dtrActionData = GameEntry.DataTable.GetDataTable<DRDefineMainChapterAction>().GetDataRow(chapterActionId);
            int buildingDtId = dtrActionData.ActionParams[0];
            var task2 = m_MilitaryCampMgr.EnterCamp(buildingDtId);
            await UniTask.WhenAll(task1, task2);

            //招募士兵
            await m_MilitaryCampMgr.RefreshSoliders(false);

            GameEntry.UI.SetFullScreenMask(false);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_IsFinished)
            {
                ChangeState<ProcedureMainChapterActionFlow>(procedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(FightRuntimeFormRestBtnClickEventArgs.EventId, OnRestClickBtn);
        }

        private void OnRestClickBtn(object sender, GameEventArgs e)
        {
            var args = e as FightRuntimeFormRestBtnClickEventArgs;
            if (args.ClickRestBtnType == 1)
            {
                GameEntry.UI.SetFullScreenMask(true);
                //离开军营
                m_MilitaryCampMgr.LevelCamp();
                //开始战斗
                m_IsFinished = true;
            }
            else if (args.ClickRestBtnType == 2)
            {
                //刷新士兵
                ClickBtnRefreshSoliders(false).Forget();
            }
            else if (args.ClickRestBtnType == 3)
            {
                //广告刷新  
                ClickBtnRefreshSoliders(true).Forget();
            }
        }

        private async UniTaskVoid ClickBtnRefreshSoliders(bool isAdRefresh)
        {
            GameEntry.UI.SetFullScreenMask(true);
            await m_MilitaryCampMgr.RefreshSoliders(isAdRefresh);
            GameEntry.UI.SetFullScreenMask(false);
        }
    }
}
