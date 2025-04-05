using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /*
    BeHurtFuncDic
    在伤害流程中，持有这个buff的人作为挨打者会发生的事情
    */
    public partial class BuffDelegateFactory
    {

        ///<summary>
        ///beHurt
        ///buff的Carrier只能受到1点直接伤害，免疫其他一切，桶子就是这样的
        ///</summary>
        private static void OnlyTakeOneDirectDamage(BuffRTData buff, DamageInfo damageInfo, UnitCharacterCtrl attacker)
        {
            bool isDirectDamage = false;
            for (int i = 0; i < damageInfo.Tags.Length; i++)
            {
                if (damageInfo.Tags[i] == DamageInfoTag.DirectDamage)
                {
                    isDirectDamage = true;
                    break;
                }
            }
            if (isDirectDamage == true && damageInfo.DamageValue(false) > 0)
            {
                int finalDV = 1;
                //来自另外一个桶子（不包含自己）的伤害为9999，其他的都是1
                if (attacker != null && attacker.HasTag("Barrel") == true && attacker.Equals(buff.Carrier) == false)
                {
                    finalDV = 9999;
                }
                damageInfo.Damage = new Damage(0, finalDV);
            }
            else
            {
                damageInfo.Damage = new Damage(0);
            }
        }
    }
}
