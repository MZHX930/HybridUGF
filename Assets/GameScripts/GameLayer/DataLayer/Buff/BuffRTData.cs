using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    ///<summary>
    ///游戏中运行的、角色身上存在的buff
    ///</summary>
    public class BuffRTData
    {
        ///<summary>
        ///这是个什么buff
        ///</summary>
        public BuffModel Model;

        ///<summary>
        ///剩余多久，单位：秒
        ///</summary>
        public float Duration;

        ///<summary>
        ///是否是一个永久的buff，永久的duration不会减少，但是timeElapsed还会增加
        ///</summary>
        public bool IsPermanent;

        ///<summary>
        ///当前层数
        ///</summary>
        public int Stack;

        ///<summary>
        ///buff的施法者是谁，当然可以是空的
        ///</summary>
        public System.Object Caster;

        ///<summary>
        ///buff的携带者，实际上是作为参数传递给脚本用，具体是谁，可定是所在控件的this.gameObject了
        ///</summary>
        public System.Object Carrier;

        ///<summary>
        ///buff已经存在了多少时间了，单位：秒
        ///</summary>
        public float TimeElapsed = 0.00f;

        ///<summary>
        ///buff执行了多少次onTick了，如果不会执行onTick，那将永远是0
        ///</summary>
        public int Ticked = 0;

        ///<summary>
        ///buff的一些参数，这些参数是逻辑使用的，比如wow中牧师的盾还能吸收多少伤害，就可以记录在buffParam里面
        ///</summary>
        public Dictionary<string, string> BuffParam = new Dictionary<string, string>();

        /// <summary>
        /// buff运行时数据
        /// </summary>
        /// <param name="model">buff的定义</param>
        /// <param name="caster">buff的施法者是谁</param>
        /// <param name="carrier">buff的携带者</param>
        /// <param name="duration">剩余多久，单位：秒</param>
        /// <param name="stack">当前层数</param>
        /// <param name="isPermanent">是否是一个永久的buff</param>
        /// <param name="buffParam">buff的一些参数</param>
        public BuffRTData(BuffModel model, System.Object caster, System.Object carrier, float duration, int stack, bool isPermanent = false, Dictionary<string, string> buffParam = null)
        {
            this.Model = model;
            this.Caster = caster;
            this.Carrier = carrier;
            this.Duration = duration;
            this.Stack = stack;
            this.IsPermanent = isPermanent;
            if (buffParam != null)
            {
                foreach (KeyValuePair<string, string> kv in buffParam)
                {
                    this.BuffParam.Add(kv.Key, kv.Value);
                }
            }
        }
    }
}
