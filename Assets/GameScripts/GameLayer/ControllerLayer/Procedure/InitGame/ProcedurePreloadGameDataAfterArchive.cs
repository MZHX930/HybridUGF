using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameDevScript
{
    /// <summary>
    /// 在存档之后加载基础数据：
    /// 内购
    /// </summary>
    public class ProcedurePreloadGameDataAfterArchive : ProcedureBase
    {
        private bool mIsSuccessLoaded = false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mIsSuccessLoaded = false;
            InitDataAfterArchive().Forget();
        }

        private async UniTaskVoid InitDataAfterArchive()
        {
            var levelStageDataTask = GameEntry.MainChapter.InitDataAfterArchive();
            var tutorialDataTask = GameEntry.Tutorial.InitDataAfterArchive();
            await UniTask.WhenAll(
                levelStageDataTask
            );

            mIsSuccessLoaded = true;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (mIsSuccessLoaded)
            {
                ChangeState<ProcedureMainUIScene>(procedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.UI.OpenUIForm(FloatTipsFormData.Create());
        }
    }
}
