using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    ///<summary>
    ///用于添加一条buff的信息
    ///</summary>
    public struct AddBuffInfo
    {
        ///<summary>
        ///buff的负责人是谁，可以是null
        ///</summary>
        public System.Object Caster;

        ///<summary>
        ///buff要添加给谁，这个必须有
        ///</summary>
        public System.Object Target;

        ///<summary>
        ///buff的model，这里当然可以从数据里拿，也可以是逻辑脚本现生成的
        ///</summary>
        public BuffModel BuffModel;

        ///<summary>
        ///要添加的层数，负数则为减少
        ///</summary>
        public int AddStack;

        ///<summary>
        ///关于时间，是改变还是设置为, true代表设置为(覆盖)，false代表改变(叠加)
        ///</summary>
        public bool DurationSetTo;

        ///<summary>
        ///是否是一个永久的buff，即便=true，时间设置也是有意义的，因为时间如果被减少到0以下，即使是永久的也会被删除
        ///</summary>
        public bool Permanent;

        ///<summary>
        ///时间值，设置为这个值，或者加上这个值，单位：秒
        ///</summary>
        public float Duration;

        ///<summary>
        ///buff的一些参数，这些参数是逻辑使用的，比如wow中牧师的盾还能吸收多少伤害，就可以记录在buffParam里面
        ///</summary>
        public Dictionary<string, string> BuffParam;

        public AddBuffInfo(
            BuffModel model, System.Object caster, System.Object target,
            int stack, float duration, bool durationSetTo = true,
            bool permanent = false,
            Dictionary<string, string> buffParam = null
        )
        {
            this.BuffModel = model;
            this.Caster = caster;
            this.Target = target;
            this.AddStack = stack;
            this.Duration = duration;
            this.DurationSetTo = durationSetTo;
            this.BuffParam = buffParam;
            this.Permanent = permanent;
        }
    }
}
