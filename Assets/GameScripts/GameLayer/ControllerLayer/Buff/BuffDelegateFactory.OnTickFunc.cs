using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    public partial class BuffDelegateFactory
    {
        ///<summary>
        ///onTick
        ///桶子每5秒对自己伤害，其实可以写一个公用的dot，不过这里表达的是，不公用也没问题
        ///</summary>
        private static void BarrelDurationLose(BuffRTData buff)
        {
            // SceneVariants.CreateDamage(buff.carrier, buff.carrier, new Damage(0, 1), 0, 0, new DamageInfoTag[] { DamageInfoTag.directDamage });
        }
    }
}
