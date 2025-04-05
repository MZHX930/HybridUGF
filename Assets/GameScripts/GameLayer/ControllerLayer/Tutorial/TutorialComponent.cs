using Cysharp.Threading.Tasks;
using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 新手引导
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/TutorialComponent")]
    public partial class TutorialComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 激活的引导组
        /// 0表示没有激活的引导组
        /// </summary>
        public int CurGuideGroupId = 0;
        /// <summary>
        /// 当前执行中的步骤
        /// 每个引导组的步骤从1开始
        /// </summary>
        public int CurGuideStepId = 1;
        /// <summary>
        /// 当前引导状态
        /// </summary>
        public EnumGuideState CurGuideState { get; private set; } = EnumGuideState.WaitStartGroup;

        /// <summary>
        /// 等待开始的引导组
        /// </summary>
        private Dictionary<int, int> m_WaitGroupDic = new Dictionary<int, int>();

        void Start()
        {
            CurGuideState = EnumGuideState.WaitStartGroup;
            GameEntry.Event.Subscribe(TutorialTriggerPointEventArgs.EventId, ChecktTriggerPoint);
        }

        public async UniTask InitDataAfterArchive()
        {
            var FinishedGroupDic = GameEntry.GameArchive.Data.Tutorial.FinishedGroupList.ToHashSet<int>();
            //初始化引导数据
            foreach (var item in GameEntry.DataTable.GetDataTable<DRDefineGameTutorial>().GetAllDataRows())
            {
                if (FinishedGroupDic.Contains(item.Group))
                {
                    continue;
                }
                if (m_WaitGroupDic.TryGetValue(item.Group, out int stepId))
                {
                    if (stepId < item.Step)
                        m_WaitGroupDic[item.Group] = item.Step;
                }
                else
                {
                    m_WaitGroupDic.Add(item.Group, item.Step);
                    await UniTask.NextFrame();
                }
            }
            await UniTask.NextFrame();
        }

        /// <summary>
        /// 引导触发点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChecktTriggerPoint(object sender, GameEventArgs e)
        {
            var eventArgs = e as TutorialTriggerPointEventArgs;
            switch (CurGuideState)
            {
                case EnumGuideState.WaitStartGroup:
                    {
                        //当前不处于引导中,检测是否开启某个引导组
                        foreach (var item in m_WaitGroupDic)
                        {
                            var dataRow = GameEntry.DataTable.GetDataTable<DRDefineGameTutorial>().GetDataRow(GetDtId(item.Key, 1));
                            if (EqualsTriggerPoint(dataRow.StartType, dataRow.StartParams, eventArgs))
                            {
                                CurGuideState = EnumGuideState.WaitEndStep;
                                CurGuideGroupId = item.Key;
                                CurGuideStepId = 1;
                                ActiveGuideStep(dataRow);
                                break;
                            }
                        }
                        break;
                    }
                case EnumGuideState.WaitStartStep:
                    {
                        //当前处于引导中，检测是否开启下一步引导
                        var dataRow = GameEntry.DataTable.GetDataTable<DRDefineGameTutorial>().GetDataRow(GetDtId(CurGuideGroupId, CurGuideStepId));
                        if (EqualsTriggerPoint(dataRow.StartType, dataRow.StartParams, eventArgs))
                        {
                            CurGuideState = EnumGuideState.WaitEndStep;
                            ActiveGuideStep(dataRow);
                        }
                        break;
                    }
                case EnumGuideState.WaitEndStep:
                    {
                        //当前处于引导中，检测是否结束引导步骤或组
                        var dataRow = GameEntry.DataTable.GetDataTable<DRDefineGameTutorial>().GetDataRow(GetDtId(CurGuideGroupId, CurGuideStepId));
                        if (EqualsTriggerPoint(dataRow.EndType, dataRow.EndParams, eventArgs))
                        {
                            //判断是否是当前组的最后一步
                            if (m_WaitGroupDic[CurGuideGroupId] == CurGuideStepId)
                            {
                                CurGuideState = EnumGuideState.WaitStartGroup;
                                //记录存档
                                GameEntry.GameArchive.Data.Tutorial.FinishedGroupList.Add(CurGuideGroupId);
                                m_WaitGroupDic.Remove(CurGuideGroupId);

                                CurGuideGroupId = 0;
                                CurGuideStepId = 1;
                            }
                            else
                            {
                                CurGuideState = EnumGuideState.WaitStartStep;
                                CurGuideStepId++;
                            }
                            GameEntry.UI.CloseUIForm(UIFormId.TutorialForm);

                            GameEntry.Event.Fire(this, TutorialTriggerPointEventArgs.Create(EnumTutorialTriggerType.WaitPreStep));
                        }
                        break;
                    }
                default:
                    {
                        Debug.LogError($"未定义的引导状态:{CurGuideState}");
                        break;
                    }
            }
        }

        private bool EqualsTriggerPoint(EnumTutorialTriggerType triggerType, string[] triggerParams, TutorialTriggerPointEventArgs eventArgs)
        {
            if (triggerType != eventArgs.TriggerType)
                return false;

            switch (eventArgs.TriggerType)
            {
                case EnumTutorialTriggerType.ClickBtn:
                case EnumTutorialTriggerType.ShowUI:
                case EnumTutorialTriggerType.CloseUI:
                    {
                        for (int i = 0; i < triggerParams.Length; i++)
                        {
                            if (triggerParams[i] != eventArgs.TriggerParams[i])
                                return false;
                        }
                        return true;
                    }
                case EnumTutorialTriggerType.WaitPreStep:
                case EnumTutorialTriggerType.ClickFullMask:
                    {
                        return true;
                    }
                default:
                    break;
            }

            return false;
        }


        private void ActiveGuideStep(DRDefineGameTutorial dataRow)
        {
            GameEntry.UI.OpenUIForm(TutorialFormData.Create(dataRow));
        }

        /// <summary>
        /// 获取引导表ID
        /// </summary>
        public int GetDtId(int groupId, int stepId)
        {
            return groupId * 1000 + stepId;
        }
    }

    /// <summary>
    /// 当前引导状态
    /// </summary>
    public enum EnumGuideState
    {
        /// <summary>
        /// 等待开启新的引导组
        /// </summary>
        WaitStartGroup,
        /// <summary>
        /// 等待执行新的引导步骤
        /// </summary>
        WaitStartStep,
        /// <summary>
        /// 等待当前引导步骤结束
        /// </summary>
        WaitEndStep,
    }
}