using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 军营
    /// 管理士兵的招募、解雇、合成升级的流程
    /// </summary>
    public class MilitaryCampMgr : MonoBehaviour
    {
        #region public data
        public MilitaryCampStateEnum CurState { get; private set; } = MilitaryCampStateEnum.Close;
        #endregion

        #region private data
        /// <summary>
        /// 当前激活的酒馆建筑
        /// </summary>
        public UnitBuildingCtrl BuildingCtrl { get; private set; }
        /// <summary>
        /// 战车车厢控制器
        /// </summary>
        private VehicleSeatContainer m_VehicleCampCtrl;
        /// <summary>
        /// 当前拖拽的士兵
        /// </summary>
        private InputDragArea m_CurInputDragArea = null;
        /// <summary>
        /// 等待加载的士兵实例序列，全部加载后置空
        /// </summary>
        private int[] m_CachedWaitCreateEntityIds = null;
        /// <summary>
        /// 闲置的等待招募的士兵网格控制器
        /// </summary>
        private List<InputDragArea> m_IdleSoldierDragAreaList = new List<InputDragArea>();
        // /// <summary>
        // /// 播放按压结束的动画中
        // /// </summary>
        // private bool m_IsPlayingTouchEndCG = false;
        // /// <summary>
        // /// 是否正在播放招募动画中？
        // /// </summary>
        // private bool m_PlayRecruitAnim = false;

        private bool m_IsPlayAnim = false;
        /// <summary>
        /// 正在播放过度动画中，比如进场动画、招募动画、合并动画
        /// </summary>
        public bool IsPlayAnim
        {
            get
            {
                return m_IsPlayAnim;
            }
            set
            {
                m_IsPlayAnim = false;
                GameEntry.UI.SetFullScreenMask(value);
            }
        }
        #endregion


        void Awake()
        {
            GameEntry.GameArchive.RegisterSaveEvent(OnSaveArchiveData);
        }

        private void OnDestroy()
        {
            GameEntry.GameArchive.UnregisterArchiveEvent(OnSaveArchiveData);
        }

        private void OnSaveArchiveData(SaveArchiveReasonTypeEnum reason)
        {
            if (CurState != MilitaryCampStateEnum.Open)
                return; //不在招募阶段，不存储进度

            Debug.Log("保存数据");
        }

        /// <summary>
        /// 进入军营
        /// >显示酒馆
        /// >显示士兵
        /// </summary>
        public async UniTask EnterCamp(int buildingDtId)
        {
            if (CurState == MilitaryCampStateEnum.Open)
            {
                Log.Error("军营已开启");
                return;
            }

            //获取战车车厢控制器
            m_VehicleCampCtrl = SceneEntityHelper.CurActivedVehicle.GetComponent<VehicleSeatContainer>();
            if (m_VehicleCampCtrl == null)
            {
                Log.Error("战车实体不存在");
            }

            CurState = MilitaryCampStateEnum.Open;
            IsPlayAnim = true;

            //加载酒馆
            BuildingCtrl = null;
            SceneEntityHelper.CreateBuildingShell(
                GameEntry.Entity.CreateEntitySerialId(),
                buildingDtId,
                FightSceneRoot.Ins.GetTavernWorldPos(),
                null,
                v => BuildingCtrl = v.GetComponent<UnitBuildingCtrl>()
            );

            await UniTask.WaitUntil(() => BuildingCtrl != null, cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 离开军营
        /// </summary>
        public void LevelCamp()
        {
            if (CurState == MilitaryCampStateEnum.Close)
            {
                Log.Error("军营已关闭");
                return;
            }

            CurState = MilitaryCampStateEnum.Close;
            IsPlayAnim = false;

            //卸载等待区的士兵
            foreach (var item in m_IdleSoldierDragAreaList)
            {
                GameEntry.Entity.HideEntity(item.ChaCtrlLogic.ShellEntityLogic.Entity);
            }
            m_IdleSoldierDragAreaList.Clear();

            //卸载酒馆
            GameEntry.Entity.HideEntity(BuildingCtrl.ShellEntityLogic.Entity);
            BuildingCtrl = null;
        }

        /// <summary>
        /// 刷新新的士兵
        /// </summary>
        public async UniTask RefreshSoliders(bool isAdRefresh)
        {
            if (CurState == MilitaryCampStateEnum.Close)
            {
                Log.Error("军营未开启");
                return;
            }

            IsPlayAnim = true;
            GenerateSoldierEntity(isAdRefresh);

            await UniTask.WaitUntilValueChanged(this, x =>
            {
                if (m_CachedWaitCreateEntityIds == null)
                    return true;
                foreach (var item in m_CachedWaitCreateEntityIds)
                {
                    if (item > 0)
                        return false;
                }
                return true;
            }, cancellationToken: this.GetCancellationTokenOnDestroy());

            m_CachedWaitCreateEntityIds = null;
            IsPlayAnim = false;
        }

        /// <summary>
        /// 按照规则生成士兵单位
        /// </summary>
        private void GenerateSoldierEntity(bool isAdRefresh)
        {
            // Vector3 bornPos = FightSceneRoot.Ins.GetMilitaryCampWorldPos();
            Vector3 bornPos = FightSceneRoot.Ins.GetTavernWorldPos();

            int createCount = isAdRefresh ? 4 : 3;
            m_CachedWaitCreateEntityIds = new int[createCount];

            UnitSoldierArchiveData[] activeSoldierList = GameEntry.GameArchive.GetActiveSoldierList();
            UnitSoldierArchiveData[] refreshSoldierList = new UnitSoldierArchiveData[createCount];

            for (int i = 0; i < activeSoldierList.Length; i++)
            {
                //注意：UnityEngine.Random是限定随机
                refreshSoldierList[i] = activeSoldierList[UnityEngine.Random.Range(0, activeSoldierList.Length)];
            }

            for (int i = activeSoldierList.Length; i < createCount; i++)
            {
                var soldierData = refreshSoldierList[i];
                int dtId = soldierData.DtId;
                int lv = soldierData.Lv;
                int mergeLv = 4;

                DRDefineSoldier configData = GameEntry.DataTable.GetDataTable<DRDefineSoldier>().GetDataRow(dtId);
                DRDefineShape shpaeConfig = GameEntry.DataTable.GetDataTable<DRDefineShape>().GetDataRow(configData.Shape);

                var serverData = UnitCharacterCtrlData.Create();
                serverData.DtId = dtId;
                serverData.GameSide = GameSideEnum.Player;
                serverData.ChaType = CharacterTypeEnum.Soldier;
                serverData.RarityType = (CharacterRarityEnum)configData.Rarity;
                serverData.CanMove = false;
                serverData.CanRotate = false;
                serverData.BornWorldPos = bornPos;
                serverData.GridInfo = new CharacterAreaInfo()
                {
                    Size = shpaeConfig.Size,
                    FillState = shpaeConfig.FillState,
                };
                serverData.Level = lv;
                serverData.MergeLv = mergeLv;
                serverData.SkillLauncherIds = configData.LauncherIds;

                int propertyLvId = lv * 10000 + mergeLv;//对应DataSoldierLvProperty_士兵等级数据.xlsx
                DRDataSoldierLvProperty lvProperty = GameEntry.DataTable.GetDataTable<DRDataSoldierLvProperty>().GetDataRow(propertyLvId);
                serverData.InitBaseProp(new ChaProperty()
                {
                    HP = 1,
                    MP = 0,
                    Attack = lvProperty.Attack,
                    MoveSpeedFactor = lvProperty.MoveSpeedFactor,
                    ActionSpeedFactor = lvProperty.ActionSpeedFactor
                });

                string viewName = (configData.ViewPrefixPath + mergeLv).ToString();
                int entityId = GameEntry.Entity.CreateEntitySerialId();
                m_CachedWaitCreateEntityIds[i] = entityId;
                SceneEntityHelper.CreateSoldierEntity(entityId, serverData, viewName, null, OnSoldierShowView);
            }
        }

        private void OnSoldierShowView(ShellEntityLogic shellEntityLogic)
        {
            InputDragArea inputDragArea = shellEntityLogic.transform.GetComponent<InputDragArea>();
            AddIdleSoldierDragArea(inputDragArea);

            //移动到指定位置
            int index = Array.IndexOf(m_CachedWaitCreateEntityIds, shellEntityLogic.Entity.Id);
            Vector3 standWorldPos = FightSceneRoot.Ins.GetMilitaryCampWorldPos() + new Vector3((index - 2) * 1.2f, 0, 0);

            UnitCharacterCtrl chaCtrl = shellEntityLogic.UnitCtrl as UnitCharacterCtrl;
            StartCoroutine(SoldierMoveTo(shellEntityLogic.Entity.Id, chaCtrl, standWorldPos));
        }

        private IEnumerator SoldierMoveTo(int entityId, UnitCharacterCtrl chaCtrl, Vector3 targetPos)
        {
            // chaCtrl.Play(EnumSpineAnimKey.Idle);
            Vector3 faceVec = targetPos - chaCtrl.transform.position;
            // chaCtrl.AddForceMove(new MovePreorder(faceVec, 0.3f));

            float moveDuration = Vector3.Distance(chaCtrl.transform.position, targetPos) / chaCtrl.MoveSpeed;
            while (moveDuration > 0)
            {
                chaCtrl.OrderMove(faceVec.normalized * chaCtrl.MoveSpeed);
                yield return new WaitForEndOfFrame();
                moveDuration -= Time.unscaledDeltaTime;
            }

            for (int i = 0; i < m_CachedWaitCreateEntityIds.Length; i++)
            {
                if (m_CachedWaitCreateEntityIds[i] == entityId)
                {
                    m_CachedWaitCreateEntityIds[i] = 0;
                }
            }
        }

        private void Update()
        {
            if (CurState != MilitaryCampStateEnum.Open)
                return;

            if (IsPlayAnim)
                return;//等待动画结束

            if (m_CachedWaitCreateEntityIds != null && m_CachedWaitCreateEntityIds.Length > 0)
                return;//等待加载士兵GameObject

            if (Input.touchCount <= 0)
                return;

            m_VehicleCampCtrl.ClearHintState();

            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    {
                        OnTouchBegin(touch);
                        break;
                    }
                case TouchPhase.Moved:
                    {
                        //是否拖动士兵
                        if (m_CurInputDragArea != null)
                        {
                            m_CurInputDragArea.OnTouchMove(FightSceneRoot.Ins.FightCamera.ScreenToWorldPoint(touch.position));
                            m_VehicleCampCtrl.OnTouchMoveWithSolider(m_CurInputDragArea);
                        }
                        break;
                    }
                case TouchPhase.Stationary:
                    {
                        if (m_CurInputDragArea != null)
                        {
                            m_CurInputDragArea.OnTouchStationary();
                            m_VehicleCampCtrl.OnTouchMoveWithSolider(m_CurInputDragArea);
                        }
                        break;
                    }
                case TouchPhase.Ended:
                    {
                        if (m_CurInputDragArea != null)
                        {
                            OnTouchEnded(touch, m_CurInputDragArea).Forget();
                            m_CurInputDragArea = null;
                        }
                        break;
                    }
            }
        }

        private void OnTouchBegin(Touch touch)
        {
            //检测是否选中士兵（等待招募的、战车上的）
            foreach (var item in m_IdleSoldierDragAreaList)
            {
                if (item.IsTouchVaildArea(touch.position))
                {
                    m_CurInputDragArea = item;
                    break;
                }
            }
            if (m_CurInputDragArea == null)
            {
                //载具区域。如果选中了士兵，则先将士兵的状态设置为等待招募
                Vector3 touchWorldPos = FightSceneRoot.Ins.FightCamera.ScreenToWorldPoint(touch.position);
                m_CurInputDragArea = m_VehicleCampCtrl.GetSoldierInTouchPos(touchWorldPos);
                if (m_CurInputDragArea != null)
                {
                    //将士兵状态设置为等待招募
                    AddIdleSoldierDragArea(m_CurInputDragArea);
                }
            }

            if (m_CurInputDragArea != null)
            {
                m_CurInputDragArea.OnStartTouch(FightSceneRoot.Ins.FightCamera.ScreenToWorldPoint(touch.position));
            }
        }

        private async UniTaskVoid OnTouchEnded(Touch touch, InputDragArea draggedSoliderArea)
        {
            // if (draggedSoliderArea == null)
            //     return;

            IsPlayAnim = true;

            draggedSoliderArea.OnEndTouch();
            Vector3 touchWorldPos = FightSceneRoot.Ins.FightCamera.ScreenToWorldPoint(touch.position);
            //判断等待区域内的合成
            for (int i = m_IdleSoldierDragAreaList.Count - 1; i >= 0; i--)
            {
                var otherSoldierArea = m_IdleSoldierDragAreaList[i];
                if (otherSoldierArea == null)
                {
                    Debug.LogError($"士兵为空??? {i}");
                    // continue;
                }
                if (otherSoldierArea == draggedSoliderArea)
                    continue;
                if (draggedSoliderArea.IsOverlapOtherSoldier(otherSoldierArea))
                {
                    if (draggedSoliderArea.IsCanMergeUp(otherSoldierArea))
                    {
                        RemoveIdleSoldierDragArea(draggedSoliderArea, true);
                        otherSoldierArea.ToMergeUp();
                        IsPlayAnim = false;
                        return;
                    }
                }
            }

            //与载具的交互
            TouchEndResultInfo resultInfo = await m_VehicleCampCtrl.OnTouchEndWithSolider(draggedSoliderArea);
            if (resultInfo.ResultCode == 0)
            {
                //全部条件都不满足，则返回原位
                await draggedSoliderArea.GoBackStandPoint();
            }
            else
            {
                RemoveIdleSoldierDragArea(draggedSoliderArea, resultInfo.ResultCode == 3);

                if (resultInfo.ResultCode == 2)
                {
                    await AddReturnSoliders(resultInfo.ReturnCampSoliderList);
                }
            }
            IsPlayAnim = false;
        }

        /// <summary>
        /// 移走等待的士兵
        /// 升级、刷新
        /// </summary>
        private void RemoveIdleSoldierDragArea(InputDragArea soldier, bool needHide)
        {
            m_IdleSoldierDragAreaList.Remove(soldier);
            if (needHide)
            {
                GameEntry.Entity.HideEntity(soldier.ChaCtrlLogic.ShellEntityLogic.Entity);
            }
        }

        /// <summary>
        /// 添加闲置（等待招募）的士兵
        /// </summary>
        private void AddIdleSoldierDragArea(InputDragArea soldier)
        {
            if (m_IdleSoldierDragAreaList.Contains(soldier))
            {
                Debug.LogError($"重复添加士兵到酒馆？");
                return;
            }
            m_IdleSoldierDragAreaList.Add(soldier);
        }

        private async UniTask AddReturnSoliders(List<InputDragArea> soldiers)
        {
            foreach (var item in soldiers)
            {
                AddIdleSoldierDragArea(item);
            }
            await UniTask.Delay(0);
        }
    }

    public enum MilitaryCampStateEnum
    {
        /// <summary>
        /// 可以招募、解雇、升级
        /// </summary>
        Open,
        /// <summary>
        /// 停止活动
        /// </summary>
        Close,
    }
}
