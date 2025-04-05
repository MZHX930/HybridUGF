using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameDevScript
{
    /// <summary>
    /// 主线战斗场景资源加载
    /// 步骤：
    /// >加载场景和地板
    /// >加载战车
    /// >加载驾驶员
    /// >将驾驶员绑定到战车上
    /// 
    /// </summary>
    public class ProcedureLoadMainChapterScene : ProcedureChangeScene
    {
        /// <summary>
        /// 当前步骤
        /// 0：准备开始
        /// 1：等待加载基础资源
        /// 2：等待加载载具和驾驶员的模型
        /// 3：等待调整资源布局
        /// 4：进入下一阶段
        /// </summary>
        private int m_CurStep = 0;
        private LoadAssetsForm m_LoadAssetsForm = null;
        private bool m_VehicleModelIsLoaded = false;
        private bool m_DriverModelIsLoaded = false;


        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.UI.CloseAllLoadedUIForms();
            GameEntry.BDH.StartNew();

            m_CurStep = 1;
            m_VehicleModelIsLoaded = false;
            m_DriverModelIsLoaded = false;

            //显示加载进度界面
            GameEntry.UI.OpenUIForm(LoadAssetsFormData.Create());

            //加载场景
            LoadSceneAssets(GameEntry.MainChapter.CurSelectedChapterId);

            //加载战斗界面
            List<UIFightBoutShowData> boutShowDataList = new List<UIFightBoutShowData>();
            for (int i = 0; i < GameEntry.MainChapter.GetChapterActionCount(GameEntry.MainChapter.CurSelectedChapterId); i++)
            {
                int boutId = i + 1;
                int actionId = GameEntry.MainChapter.GetActionId(GameEntry.MainChapter.CurSelectedChapterId, boutId);
                var drChapterAction = GameEntry.DataTable.GetDataTable<DRDefineMainChapterAction>().GetDataRow(actionId);
                int boutType = drChapterAction.BoutType;
                boutShowDataList.Add(new UIFightBoutShowData(boutId, boutType));
            }
            GameEntry.UI.OpenUIForm(FightRuntimeFormData.Create(boutShowDataList));

            //打开伤害飘字界面
            GameEntry.UI.OpenUIForm(DefaultUIFormData.Create(UIFormId.DamageFloatForm));

            //加载载具和驾驶员
            LoadFighters();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_CurStep == 0 || m_CurStep == 3)
                return;

            //显示进度条
            if (m_CurStep == 1)
            {
                if (LoadedFlag.Count <= 0)
                    return;

                if (m_LoadAssetsForm == null)
                {
                    m_LoadAssetsForm = GameEntry.UI.GetUIForm(UIFormId.LoadAssetsForm) as LoadAssetsForm;
                }

                if (m_LoadAssetsForm == null)
                    return;

                float progress = LoadedFlag.Values.Count(v => v);
                m_LoadAssetsForm.SetProgress(progress / (LoadedFlag.Count + 2));
            }
            else if (m_CurStep == 2)
            {
                //检测载具和驾驶员的模型是否已加载
                if (m_VehicleModelIsLoaded && m_DriverModelIsLoaded)
                {
                    if (m_LoadAssetsForm != null)
                        m_LoadAssetsForm.SetProgress(1);
                    m_CurStep = 3;
                    SetAssetPosition();
                }
            }
            else if (m_CurStep == 4)
            {
                var fightRuntimeForm = GameEntry.UI.GetUIForm(UIFormId.FightRuntimeForm);
                if (fightRuntimeForm == null)
                    return;

                //TODO:断线重连时这里要改写
                (fightRuntimeForm as FightRuntimeForm).OnStartChallenge(0);

                procedureOwner.SetData<VarBoolean>("IsJustStarted", true);
                ChangeState<ProcedureMainChapterActionFlow>(procedureOwner);
                m_CurStep = 0;
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.UI.CloseUIForm(UIFormId.LoadAssetsForm);
        }

        /// <summary>
        /// 加载必备的战斗人员
        /// </summary>
        void LoadFighters()
        {
            int vehicleEntityId = GameEntry.Entity.CreateEntitySerialId();
            AddLoadFlag(vehicleEntityId.ToString());
            //加载载具
            SceneEntityHelper.CreateVehicleEntity(
                vehicleEntityId,
                1,
                v => SceneEntityHelper.CurActivedVehicle = v.GetComponent<UnitCharacterCtrl>(),
                v => m_VehicleModelIsLoaded = true
            );

            //加载驾驶员
            int driverEntityId = GameEntry.Entity.CreateEntitySerialId();
            AddLoadFlag(driverEntityId.ToString());
            SceneEntityHelper.CreateHeroEntity(
                driverEntityId,
                1,
                v => SceneEntityHelper.CurActivedHero = v.GetComponent<UnitCharacterCtrl>(),
                v => m_DriverModelIsLoaded = true
            );
        }

        protected override void OnLoadAssetsComplete()
        {
            base.OnLoadAssetsComplete();

            m_CurStep = 2;
        }

        /// <summary>
        /// 设置资源位置和关系
        /// </summary>
        private void SetAssetPosition()
        {
            //设置地板
            FightSceneRoot.Ins.SetGroundSprites(GroundTextures);

            //调整载具位置
            SceneEntityHelper.CurActivedVehicle.ShellEntityLogic.transform.position = FightSceneRoot.Ins.GetVehiclePointPos(0);

            //调整驾驶员位置
            SceneEntityHelper.CurActivedHero.ShellEntityLogic.transform.position = SceneEntityHelper.CurActivedVehicle.GetPortPointWorldPos(Constant.GameLogic.BindPoint_DriverPoint);

            //初始化敌人军营
            EnemyCampMgr.Ins.SetPosInfo(FightSceneRoot.Ins.GetMonsterBornWorldPosArray());

            //拉远视角
            FightSceneRoot.Ins.SetCameraFocusMode(FightCameraFocusModeEnum.Normal).Forget();

            m_CurStep = 4;
        }
    }
}