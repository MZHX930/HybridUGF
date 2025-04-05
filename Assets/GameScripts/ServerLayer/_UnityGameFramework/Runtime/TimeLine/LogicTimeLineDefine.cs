using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    ///<summary>
    ///时间线壳，用于存储时间线的基础信息
    ///</summary>
    public class LogicTimeLineShell : IReference
    {
        ///<summary>
        ///Timeline的基础信息
        ///</summary>
        public LogicTimeLineModel Model;

        ///<summary>
        ///Timeline的焦点对象也就是创建timeline的负责人，比如技能产生的timeline，就是技能的施法者
        ///</summary>
        public Entity Caster;

        ///<summary>
        ///倍速，1=100%，0.1=10%是最小值
        ///</summary>
        public float TimeScale
        {
            get
            {
                return m_TimeScale;
            }
            set
            {
                m_TimeScale = Mathf.Max(0.100f, value);
            }
        }
        private float m_TimeScale = 1.00f;

        ///<summary>
        ///Timeline的创建参数，如果是一个技能，这就是一个skillObj
        ///</summary>
        public object Param;

        ///<summary>
        ///Timeline已经运行了多少秒了
        ///</summary>
        public float TimeElapsed = 0;

        ///<summary>
        ///一些重要的逻辑参数，是根据游戏机制在程序层提供的，这里目前需要的是
        ///[faceDegree] 发生时如果有caster，则caster企图面向的角度（主动）。
        ///[moveDegree] 发生时如果有caster，则caster企图移动向的角度（主动）。
        ///</summary>
        public Dictionary<string, object> Values = new Dictionary<string, object>();

        ///<summary>
        ///尝试从values获得某个值
        ///<param name="key">这个值的key{faceDegree, moveDegree}</param>
        ///<return>取出对应的值，如果不存在就是null</return>
        ///</summary>
        public object GetValue(string key)
        {
            if (Values.ContainsKey(key) == false)
                return null;
            return Values[key];
        }

        public void Clear()
        {
            Model = default;
            Caster = null;
            Values.Clear();
            TimeElapsed = 0;
            Param = null;
            m_TimeScale = 1.00f;
        }

        public static LogicTimeLineShell Create(LogicTimeLineModel model, Entity caster, object param)
        {
            LogicTimeLineShell timeline = ReferencePool.Acquire<LogicTimeLineShell>();
            timeline.Model = model;
            timeline.Caster = caster;
            timeline.Param = param;
            timeline.Values.Clear();
            timeline.TimeScale = 1.00f;

            return timeline;
        }

        public void ClearNodesParamsRef()
        {
            Debug.Log("逻辑时间轴 创建Shell");
        }
    }

    ///<summary>
    ///策划预先填表制作的，就是这个东西，同样她也是被clone到obj当中去的
    ///</summary>
    public struct LogicTimeLineModel
    {
        public string Id;

        ///<summary>
        ///Timeline运行多久之后发生，单位：秒
        ///</summary>
        public LogicTimeLineNode[] Nodes;

        ///<summary>
        ///Timeline一共多长时间（到时间了就丢掉了），单位秒
        ///</summary>
        public float Duration;

        ///<summary>
        ///如果有caster，并且caster处于蓄力状态，则可能会经历跳转点
        ///</summary>
        public LogicTimeLineGoTo ChargeGoBack;

        public LogicTimeLineModel(string id, LogicTimeLineNode[] nodes, float duration, LogicTimeLineGoTo chargeGoBack)
        {
            this.Id = id;
            this.Nodes = nodes;
            this.Duration = duration;
            this.ChargeGoBack = chargeGoBack;
        }
    }

    ///<summary>
    ///Timeline每一个节点上要发生的事情
    ///</summary>
    public struct LogicTimeLineNode
    {
        ///<summary>
        ///Timeline运行多久之后发生，单位：秒
        ///</summary>
        public float TimeElapsed;

        ///<summary>
        ///要执行的脚本函数
        ///</summary>
        public LogicTimeLineNodeEvent DoEvent;

        ///<summary>
        ///要执行的函数的参数
        ///</summary>
        public Variable[] EveParams { get; }

        public LogicTimeLineNode(float time, LogicTimeLineNodeEvent doEvet, params Variable[] eveArgs)
        {
            this.TimeElapsed = time;
            this.DoEvent = doEvet;
            this.EveParams = eveArgs;
        }

        public static LogicTimeLineNode Null = new LogicTimeLineNode(float.MaxValue, null);
    }

    ///<summary>
    ///Timeline的一个跳转点信息
    ///</summary>
    public struct LogicTimeLineGoTo
    {
        ///<summary>
        ///自身处于时间点
        ///</summary>
        public float AtDuration;

        ///<summary>
        ///跳转到时间点
        ///</summary>
        public float GotoDuration;

        public LogicTimeLineGoTo(float atDuration, float gotoDuration)
        {
            this.AtDuration = atDuration;
            this.GotoDuration = gotoDuration;
        }

        // 这是一个静态的TimelineGoTo类型的属性，命名为Null
        // 它创建了一个新的TimelineGoTo实例，并将atDuration和gotoDuration都设置为float类型的最大值
        // 这种写法通常用于表示一个"空"或"无效"的TimelineGoTo对象
        // 使用float.MaxValue是为了确保这个时间点永远不会被触发到
        public static LogicTimeLineGoTo Null = new LogicTimeLineGoTo(float.MaxValue, float.MaxValue);
    }

    /// <summary>
    /// 逻辑时间轴节点事件
    /// </summary>
    /// <param name="timeline">节点所在的时间轴</param>
    /// <param name="args">节点事件参数</param>
    public delegate void LogicTimeLineNodeEvent(LogicTimeLineShell timeline, params object[] args);
}