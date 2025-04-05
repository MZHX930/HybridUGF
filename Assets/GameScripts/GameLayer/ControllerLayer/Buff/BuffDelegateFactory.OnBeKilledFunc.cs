using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /*
    BeKilledFuncDic
    在伤害流程中，持有这个buff的人被杀死了，会触发的事情
    */
    public partial class BuffDelegateFactory
    {

        ///<summary>
        ///beKilled
        ///死亡后爆炸，对敌人造成伤害，其他桶子也是其他敌人，所以不必特殊处理，beHurt已经特殊处理了，当然还要立即清除掉这个桶子。
        ///</summary>
        private static void BarrelExplosed(BuffRTData buff, DamageInfo damageInfo, UnitCharacterCtrl attacker)
        {
            // GameObject aoeCaster = buff.caster != null ? buff.caster : buff.carrier;
            // //AoeModel是可以动态生成的
            // SceneVariants.CreateAoE(new AoeLauncher(
            //     new AoeModel(
            //         "BoomExplosive", "", new string[0], 0, false,
            //         "CreateSightEffect", new object[] { "Effect/Explosion_A" },
            //         "BarrelExplosed", new object[0],
            //         "", new object[0],  //tick
            //         "", new object[0],  //chaEnter
            //         "", new object[0],  //chaLeave
            //         "", new object[0],  //bulletEnter
            //         "", new object[0]   //bulletLeave
            //     ),
            //     aoeCaster, buff.carrier.transform.position, 2.2f, 0.5f, 0,
            //     null, null, new Dictionary<string, object>(){
            //         {"Barrel", buff.carrier}
            //     }
            // ));
            // //隐藏自己，反正后面会被Remover移走
            // buff.carrier.transform.localScale = Vector3.zero;
        }
    }
}

