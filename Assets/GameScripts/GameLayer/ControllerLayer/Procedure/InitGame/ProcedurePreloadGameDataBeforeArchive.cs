using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameDevScript
{
    /// <summary>
    /// 在存档之前加载基础数据：
    /// 配置表的解析转换
    /// </summary>
    public class ProcedurePreloadGameDataBeforeArchive : ProcedureBase
    {
        private bool mIsSuccessLoaded = false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            mIsSuccessLoaded = false;
            InitDataBeforeArchive().Forget();
        }


        private async UniTaskVoid InitDataBeforeArchive()
        {
            await UniTask.WhenAll();

            mIsSuccessLoaded = true;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (mIsSuccessLoaded)
            {
                ChangeState<ProcedurePreloadGameArchive>(procedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }
    }
}
