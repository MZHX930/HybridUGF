using System;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 时间线组件
    /// 一个实体同一时间内只能存在一个逻辑时间线
    /// https://zhuanlan.zhihu.com/p/416805924
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/LogicTimeLineComponent")]
    public class LogicTimeLineComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 正在更新中的逻辑时间线列表
        /// </summary>
        private List<LogicTimeLineShell> m_Timelines = new List<LogicTimeLineShell>();
        /// <summary>
        /// 等待删除的逻辑时间线列表
        /// </summary>
        private List<LogicTimeLineShell> m_WaitRemoveList = new List<LogicTimeLineShell>();

        protected override void Awake()
        {
            base.Awake();
        }

        ///<summary>
        ///添加一个timeline
        ///<param name="timelineModel">要添加的timeline</param>
        ///</summary>
        public void AddTimeline(LogicTimeLineShell timeline)
        {
            if (timeline.Caster != null && CasterHasTimeline(timeline.Caster) == true)
            {
                Log.Warning($"施法者{timeline.Caster.name} 已经有时间线了. {Time.realtimeSinceStartupAsDouble}");
                ReferencePool.Release(timeline);
                return;
            }
            this.m_Timelines.Add(timeline);
        }

        /// <summary>
        /// 取消一个timeline
        /// </summary>
        public void CancelTimeLine(LogicTimeLineShell shell)
        {
            // if (shell == null)
            //     return;

            // if (this.mTimelines.Contains(shell))
            //     this.mTimelines.Remove(shell);

            // ReferencePool.Release(shell);
            if (m_WaitRemoveList.Contains(shell))
                return;
            m_WaitRemoveList.Add(shell);
        }

        /// <summary>
        /// 检查指定的施法者是否已经有正在执行的时间线
        /// </summary>
        /// <param name="caster">要检查的施法者实体</param>
        /// <returns>如果施法者已有时间线则返回true,否则返回false</returns>
        public bool CasterHasTimeline(Entity caster)
        {
            for (var i = 0; i < m_Timelines.Count; i++)
            {
                if (m_Timelines[i].Caster == caster)
                {
                    // Debug.Log($"剩余时间长度 {m_Timelines[i].Model.Duration - m_Timelines[i].TimeElapsed}");
                    return true;
                }
            }
            return false;
        }




        private LogicTimeLineShell m_TmpTimeLine = null;
        private void FixedUpdate()
        {
            // 删除等待删除的timeline
            if (m_WaitRemoveList.Count > 0)
            {
                foreach (var item in m_WaitRemoveList)
                {
                    m_Timelines.Remove(item);
                    ReferencePool.Release(item);
                }
                m_WaitRemoveList.Clear();
            }

            if (this.m_Timelines.Count <= 0)
                return;

            int idx = 0;

            while (idx < this.m_Timelines.Count)
            {
                m_TmpTimeLine = m_Timelines[idx];
                float wasTimeElapsed = m_TmpTimeLine.TimeElapsed;
                m_TmpTimeLine.TimeElapsed += Time.fixedDeltaTime * m_TmpTimeLine.TimeScale;

                //判断有没有返回点
                if (m_TmpTimeLine.Model.ChargeGoBack.AtDuration < m_TmpTimeLine.TimeElapsed
                    && m_TmpTimeLine.Model.ChargeGoBack.AtDuration >= wasTimeElapsed)
                {
                    if (m_TmpTimeLine.Caster != null && m_TmpTimeLine.Caster.Logic.IsCharging == true)
                    {
                        m_TmpTimeLine.TimeElapsed = m_TmpTimeLine.Model.ChargeGoBack.GotoDuration;
                        continue;
                    }
                }
                //执行时间点内的事情
                for (int i = 0; i < m_TmpTimeLine.Model.Nodes.Length; i++)
                {
                    if (m_TmpTimeLine.Model.Nodes[i].TimeElapsed < m_TmpTimeLine.TimeElapsed
                        && m_TmpTimeLine.Model.Nodes[i].TimeElapsed >= wasTimeElapsed)
                    {
                        m_TmpTimeLine.Model.Nodes[i].DoEvent(
                            m_TmpTimeLine,
                            m_TmpTimeLine.Model.Nodes[i].EveParams
                        );
                    }
                }

                //判断timeline是否终结
                if (m_TmpTimeLine.Model.Duration <= m_TmpTimeLine.TimeElapsed)
                {
                    ReferencePool.Release(m_TmpTimeLine);
                    m_Timelines.RemoveAt(idx);
                }
                else
                {
                    idx++;
                }
            }
            m_TmpTimeLine = null;
        }

    }
}