using GameFramework;
using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 敌军营地管理
    /// 按照规定产出敌人，统一Update敌人移动
    /// </summary>
    public class EnemyCampMgr : MonoBehaviour
    {
        public static EnemyCampMgr Ins { get; private set; }

        #region 组件
        /// <summary>
        /// 当前地图怪物刷新点
        /// </summary>
        private Vector3[] m_BornPosArray;//怪物出生点
        #endregion


        #region 数据
        /// <summary>
        /// 当前波次的怪物刷新规则集合
        /// </summary>
        private MonsterCreateData[] m_MonsterCreateDatas;
        /// <summary>
        /// 场上存在的怪物（存在HP<=0的怪物，因为需要播放死亡效果）
        /// </summary>
        private Dictionary<int, UnitCharacterCtrl> m_ShowMonsterShellDic = new Dictionary<int, UnitCharacterCtrl>();

        /// <summary>
        /// 当前状态
        /// </summary>
        private EnumEnemyCampState m_CurState = EnumEnemyCampState.Closed;
        #endregion


        private void Awake()
        {
            Ins = this;
            GameEntry.Event.Subscribe(HideCharacterShellEventArgs.EventId, OnHideCharacterShell);
        }

        void OnDestroy()
        {
            m_BornPosArray = null;
            m_MonsterCreateDatas = null;
            m_ShowMonsterShellDic = null;

            Ins = null;

            //这里是为了防止直接退出时,UGF已销毁,导致事件无法取消
            if (GameEntry.Event.Check(KillBoutAllMonsterEventArgs.EventId, OnHideCharacterShell))
                GameEntry.Event.Unsubscribe(HideCharacterShellEventArgs.EventId, OnHideCharacterShell);
        }

        /// <summary>
        /// 初始化当前地图的刷新点位
        /// </summary>
        public void SetPosInfo(Vector3[] bornPosArray)
        {
            m_BornPosArray = bornPosArray;
        }

        /// <summary>
        /// 设置当前波次刷新规则
        /// </summary>
        public void SetBoutRefreshRules(int[] ruleIds)
        {
            m_MonsterCreateDatas = new MonsterCreateData[ruleIds.Length];
            for (int i = 0; i < m_MonsterCreateDatas.Length; i++)
            {
                m_MonsterCreateDatas[i] = new MonsterCreateData(ruleIds[i]);
            }
        }

        /// <summary>
        /// 切换营地状态
        /// </summary>
        public void ChangeCampState(EnumEnemyCampState newState)
        {
            m_CurState = newState;
        }

        private void Update()
        {
            if (m_CurState != EnumEnemyCampState.Update)
                return;

            bool isCreating = UpdateRulesTime();
            if (isCreating)
                return;

            //判断本波次的怪全部被击杀了
            if (m_ShowMonsterShellDic.Count <= 0)
            {
                //结束
                ChangeCampState(EnumEnemyCampState.Pause);
                GameEntry.Event.Fire(this, KillBoutAllMonsterEventArgs.Create());
            }
        }

        /// <summary>
        /// 更新规则激活后的流失时间
        /// </summary>
        private bool UpdateRulesTime()
        {
            bool isCreating = false;
            foreach (var ruleInfo in m_MonsterCreateDatas)
            {
                ruleInfo.ElapseSeconds += Time.deltaTime;
                if (ruleInfo.ElapseSeconds < ruleInfo.DRRuleConfig.Delay)
                    continue;

                if (ruleInfo.HadCreatedCount >= ruleInfo.DRRuleConfig.MonsterCount)
                    continue;

                isCreating = true;
                ruleInfo.NextCreateTime -= Time.deltaTime;
                if (ruleInfo.NextCreateTime <= 0)
                {
                    ruleInfo.NextCreateTime = ruleInfo.DRRuleConfig.Interval;

                    //创建怪物
                    int createCount = Mathf.Min(ruleInfo.DRRuleConfig.EachCreateCount, ruleInfo.DRRuleConfig.MonsterCount - ruleInfo.HadCreatedCount);
                    for (int i = 0; i < createCount; i++)
                    {
                        ruleInfo.HadCreatedCount++;
                        SceneEntityHelper.CreateMonsterEntity(
                            GameEntry.Entity.CreateEntitySerialId(),
                            ruleInfo.DRMonsterConfig.Id,
                            ruleInfo.MonsterProperty,
                            m_BornPosArray[Utility.Random.GetRandom(0, m_BornPosArray.Length)],
                            null,
                            OnShowMonsterView
                        );
                    }
                }
            }
            return isCreating;
        }

        /// <summary>
        /// 显示怪物视图时
        /// </summary>
        private void OnShowMonsterView(ShellEntityLogic monster)
        {
            var chaCtrl = monster.UnitCtrl as IUnitCtrl<UnitCharacterCtrlData> as UnitCharacterCtrl;
            m_ShowMonsterShellDic.Add(chaCtrl.ShellEntityLogic.Entity.Id, chaCtrl);
            chaCtrl.SwitchFightState(true);
        }

        private void OnHideCharacterShell(object sender, GameEventArgs e)
        {
            var args = e as HideCharacterShellEventArgs;
            if (args == null)
                return;

            if (m_ShowMonsterShellDic.TryGetValue(args.HideEntityId, out var chaCtrl))
            {
                m_ShowMonsterShellDic.Remove(args.HideEntityId);
            }
        }

        /// <summary>
        /// 搜索最近的怪物实体
        /// </summary>
        public bool SearchNearestMonster(Vector3 compareWorldPos, out UnitCharacterCtrl monster)
        {
            monster = null;
            float minDistance = float.MaxValue;
            foreach (var item in m_ShowMonsterShellDic)
            {
                if (item.Value.IsDead)
                    continue;

                float sqrDistance = (compareWorldPos - item.Value.transform.position).sqrMagnitude;
                if (sqrDistance < minDistance)
                {
                    minDistance = sqrDistance;
                    monster = item.Value;
                }
            }
            return monster != null;
        }

        /// <summary>
        /// 搜索随机的怪物实体
        /// </summary>
        public bool SearchRandomMonster(out UnitCharacterCtrl monster)
        {
            monster = null;
            if (m_ShowMonsterShellDic.Count <= 0)
                return false;

            // monster = m_ShowMonsterShellDic.Values.ElementAt(UnityEngine.Random.Range(0, m_ShowMonsterShellDic.Count));
            int randomIndex = Utility.Random.GetRandom(0, (int)(m_ShowMonsterShellDic.Count * 0.3f));
            foreach (var item in m_ShowMonsterShellDic)
            {
                if (randomIndex <= 0)
                {
                    monster = item.Value;
                    return true;
                }
                randomIndex--;
            }
            return false;
        }

        /// <summary>
        /// 敌人军营状态
        /// </summary>
        public enum EnumEnemyCampState
        {
            /// <summary>
            /// 创建怪物中
            /// </summary>
            Update,

            /// <summary>
            /// 暂停状态
            /// </summary>
            Pause,

            /// <summary>
            /// 关闭状态
            /// </summary>
            Closed,
        }


        /// <summary>
        /// 怪物刷新数据
        /// </summary>
        public sealed class MonsterCreateData
        {
            /// <summary>
            /// 刷新规则配置
            /// </summary>
            public DRDefineRefreshEnemyRule DRRuleConfig;

            /// <summary>
            /// 刷新的怪物配置
            /// </summary>
            public DRDefineMonster DRMonsterConfig;

            /// <summary>
            /// 已流失时间，秒
            /// </summary>
            public float ElapseSeconds = 0;

            /// <summary>
            /// 下一次创建时间
            /// </summary>
            public float NextCreateTime = 0;

            /// <summary>
            /// 已创建的数量
            /// </summary>
            public int HadCreatedCount = 0;

            /// <summary>
            /// 怪物初始属性
            /// </summary>
            public ChaProperty MonsterProperty;

            public MonsterCreateData(int ruleId)
            {
                DRRuleConfig = GameEntry.DataTable.GetDataTable<DRDefineRefreshEnemyRule>().GetDataRow(ruleId);

                DRMonsterConfig = GameEntry.DataTable.GetDataTable<DRDefineMonster>().GetDataRow(DRRuleConfig.MonsterId);

                float chaPropertyFactor = DRRuleConfig.ChaPropertyFactor;
                MonsterProperty = new ChaProperty(
                    DRMonsterConfig.MoveSpeed,
                    (int)(chaPropertyFactor * DRMonsterConfig.HP),
                    (int)(chaPropertyFactor * DRMonsterConfig.MP),
                    (int)(chaPropertyFactor * DRMonsterConfig.Attack),
                    DRMonsterConfig.ActionSpeed
                );
            }
        }
    }
}