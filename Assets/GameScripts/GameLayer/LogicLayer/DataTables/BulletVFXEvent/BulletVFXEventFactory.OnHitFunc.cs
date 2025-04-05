using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    //OnHitFuncDic
    public static partial class BulletSkillDelegateFactory
    {
        ///<summary>
        ///onHit
        ///普通子弹命中效果，参数：
        ///[0]伤害倍数
        ///[1]基础暴击率
        ///[2]命中视觉特效
        ///[3]播放特效位于目标的绑点，默认Body
        ///</summary>
        private static void CommonBulletHit(UnitBulletCtrl bulletLogic, UnitCharacterCtrl target)
        {
            if (!bulletLogic)
                return;
            string[] onHitParam = bulletLogic.CtrlData.Model.OnHitParam;
            float damageTimes = onHitParam.Length > 0 ? float.Parse(onHitParam[0]) : 1.00f;
            float critRate = onHitParam.Length > 1 ? float.Parse(onHitParam[1]) : 0.00f;
            string sightEffect = onHitParam.Length > 2 ? onHitParam[2] : "";
            string bpName = onHitParam.Length > 3 ? onHitParam[3] : "Body";
            GameEntry.DamageCal.DoDamage(
                bulletLogic.CtrlData.Caster,
                target,
                new Damage(Mathf.CeilToInt(damageTimes * bulletLogic.CtrlData.PropWhileCast.Attack)),
                bulletLogic.transform.eulerAngles.y,
                critRate,
                new DamageInfoTag[] { DamageInfoTag.DirectDamage, }
            );
        }


        ///<summary>
        ///onHit
        ///在子弹位置创建一个aoe，所以aoe的始作俑者肯定是caster了，位置也是子弹位置，填写什么都无效，角度也是子弹角度，参数：
        ///[0]AoeLauncher：aoe的发射器，caster在这里被重新赋值，position则作为增量加给现在的角色坐标
        ///</summary>
        private static void CreateAoEOnHit(UnitBulletCtrl bulletState, UnitCharacterCtrl target)
        {
            if (!bulletState)
                return;
            // string[] onHitParams = bulletState.EntityData.Model.OnHitParam;
            // if (onHitParams.Length <= 0)
            //     return;

            // int aoeId = int.Parse(onHitParams[0]);

            // var aoeDefineConfig = GameEntry.DataTable.GetDataTable<DRDefineAoeVFX>().GetDataRow(aoeId);

            // AoeVFXEntityData vfxData = AoeVFXEntityData.Create(
            //     aoeDefineConfig,
            //     bulletState.EntityData.Caster,
            //     bulletState.transform.position,
            //     aoeDefineConfig.Radius,
            //     aoeDefineConfig.LifeTime,
            //     aoeDefineConfig.Degree,
            //     null,
            //     null,
            //     null
            // );

            // SceneEntityHelper.CreateAoeShell(vfxData);
        }


        ///<summary>
        ///onHit
        ///氪漏氪回力标命中效果，除了普通效果，就是命中自己的时候移除子弹，参数：
        ///[0]伤害倍数
        ///[1]基础暴击率
        ///[2]命中视觉特效
        ///[3]播放特效位于目标的绑点，默认Body
        ///</summary>
        private static void CloakBoomerangHit(UnitBulletCtrl bulletLogic, UnitCharacterCtrl target)
        {
            UnitCharacterCtrl ccs = bulletLogic.CtrlData.Caster;
            UnitCharacterCtrl tcs = target;
            if (ccs != null && tcs != null)
            {
                CommonBulletHit(bulletLogic, target);
            }
            else
            {
                float backTime = bulletLogic.CtrlData.Param.ContainsKey("backTime") ? (float)bulletLogic.CtrlData.Param["backTime"] : 1.0f; //默认1秒 
                if (bulletLogic.CtrlData.TimeElapsed > backTime && target.Equals(bulletLogic.CtrlData.Caster))
                {
                    // SceneVariants.RemoveBullet(bullet);
                    GameEntry.Entity.HideEntity(bulletLogic.ShellEntityLogic.Entity);
                    if (ccs)
                        ccs.PlaySightEffect("Body", "Effect/Heart");
                }
            }
        }
    }
}