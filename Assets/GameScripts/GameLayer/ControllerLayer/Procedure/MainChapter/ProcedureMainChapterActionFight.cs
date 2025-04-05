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
    /// 主线关卡---战斗
    /// 获取当前波次的刷新规则，按照规则刷新怪物
    /// </summary>
    public class ProcedureMainChapterActionFight : ProcedureBase
    {
        private bool m_IsFinished = false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_IsFinished = false;
            FightSceneRoot.Ins.SetCameraFocusMode(FightCameraFocusModeEnum.Normal).Forget();

            EnemyCampMgr.Ins.SetBoutRefreshRules(GameEntry.DataTable.GetDataTable<DRDefineMainChapterAction>().GetDataRow(GameEntry.MainChapter.GetCurActionId()).ActionParams);

            SetFightState(true);
            EnemyCampMgr.Ins.ChangeCampState(EnemyCampMgr.EnumEnemyCampState.Update);

            GameEntry.Event.Subscribe(KillBoutAllMonsterEventArgs.EventId, OnKillBoutAllMonster);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            if (!isShutdown)
            {
                GameEntry.Event.Unsubscribe(KillBoutAllMonsterEventArgs.EventId, OnKillBoutAllMonster);

                SetFightState(false);
                EnemyCampMgr.Ins.ChangeCampState(EnemyCampMgr.EnumEnemyCampState.Closed);

                //清理技能残留
                GameEntry.Entity.HideAllLoadingEntities();
                var skills = GameEntry.Entity.GetEntityGroup(Constant.EntityGroup.Skill).GetAllEntities();
                for (int i = skills.Length - 1; i >= 0; i--)
                {
                    GameEntry.Entity.HideEntity(skills[i].Id);
                }
            }

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_IsFinished)
            {
                ChangeState<ProcedureMainChapterActionFlow>(procedureOwner);
            }
        }

        private void SetFightState(bool isFight)
        {
            SceneEntityHelper.CurActivedVehicle.SwitchFightState(isFight);
            SceneEntityHelper.CurActivedVehicle.GetComponent<VehicleSeatContainer>().SwitchFightState(isFight);
            SceneEntityHelper.CurActivedHero.SwitchFightState(isFight);
        }

        /// <summary>
        /// 当击波次怪全部被击杀且隐藏后
        /// </summary>
        private void OnKillBoutAllMonster(object sender, GameEventArgs e)
        {
            m_IsFinished = true;
        }
    }
}
