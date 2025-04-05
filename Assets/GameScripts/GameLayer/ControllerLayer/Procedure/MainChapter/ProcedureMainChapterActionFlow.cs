using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameDevScript
{
    /// <summary>
    /// 主线章节波次，根据配置表切换波次的类型
    /// > 模拟前进移动
    /// > 根据配置表进入不同状态机
    /// </summary>
    public class ProcedureMainChapterActionFlow : ProcedureBase
    {
        private bool m_IsPlayAnim = false;
        private FightRuntimeForm m_FightRuntimeForm;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.GameArchive.RecordRandomState();
            GameEntry.UI.SetFullScreenMask(true);

            m_IsPlayAnim = true;
            FightSceneRoot.Ins.StartScrollGround(Constant.GameLogic.EachChapterActionMoveTime, OnGroundScrollEnd);
            FightSceneRoot.Ins.SetCameraFocusMode(FightCameraFocusModeEnum.Normal, Constant.GameLogic.FightCameraFocusAnimTime).Forget();

            m_FightRuntimeForm = GameEntry.UI.GetUIForm(UIFormId.FightRuntimeForm) as FightRuntimeForm;
            if (procedureOwner.GetData<VarBoolean>("IsJustStarted") == false)
            {
                m_FightRuntimeForm.GoToNextBout();
                GameEntry.MainChapter.OnCompleteBoutAction();
            }
            else
            {
                procedureOwner.SetData<VarBoolean>("IsJustStarted", false);
            }
        }

        private void OnGroundScrollEnd()
        {
            //进入对应状态机
            m_IsPlayAnim = false;
            m_FightRuntimeForm.ShowCurBoutUIElements();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_IsPlayAnim)
                return;

            int actionId = GameEntry.MainChapter.GetCurActionId();
            var dtrAction = GameEntry.DataTable.GetDataTable<DRDefineMainChapterAction>().GetDataRow(actionId);
            if (dtrAction == null)
            {
                //没有下一个动作
                ChangeState<ProcedureMainChapterSettlement>(procedureOwner);
            }
            else
            {
                switch (dtrAction.BoutType)
                {
                    case 0:
                        //招募
                        ChangeState<ProcedureMainChapterActionRest>(procedureOwner);
                        break;
                    case 1:
                        //战斗
                        ChangeState<ProcedureMainChapterActionFight>(procedureOwner);
                        break;
                    // case 2:
                    //     //加血事件
                    //     // ChangeState<ProcedureMainChapterActionAddHp>(procedureOwner);
                    //     break;
                    default:
                        Log.Error("波次的行为类型未定义，{0}", actionId);
                        break;
                }
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
}