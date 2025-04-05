using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    public static partial class LogicTimeLineNodeEventFactory
    {
        /// <summary>
        /// 在Caster的某个绑点(Muzzle/Head/Body)上发射一个子弹出来
        /// </summary>
        /// <param name="timeLineShell">触发的时间轴</param>
        /// <param name="args">参数集 [0]子弹id</param>
        private static void FireBulletFromCha(LogicTimeLineShell timeLineShell, params object[] args)
        {
            if (timeLineShell.Caster == null || timeLineShell.Caster.isActiveAndEnabled == false)
                return;

            var characterLogic = timeLineShell.Caster.GetComponent<UnitCharacterCtrl>();
            if (characterLogic == null)
                return;

            int bulletId = ((VarInt32)args[0]).Value;
            var bulletModel = GameEntry.DataTable.GetDataTable<DRDefineBulletVFX>().GetDataRow(bulletId);
            //这里从配置表读取后再赋值，是为了后续buff影响这些参数
            float speed = bulletModel.MoveSpeedFactor;
            float duration = bulletModel.LifeTime;

            Vector3 fireDireNorm = (Vector3)timeLineShell.Values[Constant.GameLogic.Skill_FireDire];
            Vector3 firePos = (Vector3)timeLineShell.Values[Constant.GameLogic.Skill_FirePos];

            var shellData = UnitBulletCtrlData.Create(
                bulletModel,
                characterLogic,
                characterLogic.CtrlData.GameSide,
                firePos,
                fireDireNorm,
                speed,
                duration,
                bulletModel.DelayCollision,
                null,
                null,
                false,
                null
            );

            SceneEntityHelper.CreateBulletShell(GameEntry.Entity.CreateEntitySerialId(), shellData);
        }

        ///<summary>
        ///从角色的某个绑点发射一个aoe特效，并移动到目标点
        /// </summary>
        /// <param name="timeLineShell">触发的时间轴</param>
        /// <param name="args">参数集 [0]aoe特效id [1]是否从发射点开始位移</param>
        private static void FireAoeFromCha(LogicTimeLineShell timeLineShell, params object[] args)
        {
            if (timeLineShell.Caster == null)
            {
                Log.Error("FireAoeFromCha: Caster is null");
                return;
            }
            if (args.Length < 2)
            {
                Log.Error("FireAoeFromCha: args.Length <= 2");
                return;
            }

            var characterLogic = timeLineShell.Caster.GetComponent<UnitCharacterCtrl>();
            if (characterLogic == null)
                return;

            int dtId = ((VarInt32)args[0]).Value;
            bool needMove = ((VarInt32)args[1]).Value == 1;
            var aoeDefineConfig = GameEntry.DataTable.GetDataTable<DRDefineAoeVFX>().GetDataRow(dtId);

            Vector3 firePos;
            if (needMove)
            {
                firePos = (Vector3)timeLineShell.Values[Constant.GameLogic.Skill_FirePos];
            }
            else
            {
                firePos = (Vector3)timeLineShell.Values[Constant.GameLogic.Skill_TargetPos];
            }

            UnitAoECtrlData aLauncher = UnitAoECtrlData.Create(
                aoeDefineConfig,
                timeLineShell.Caster,
                characterLogic.CtrlData.GameSide,
                firePos,
                aoeDefineConfig.LifeTime,
                timeLineShell.Values
            );

            SceneEntityHelper.CreateAoeShell(GameEntry.Entity.CreateEntitySerialId(), aLauncher);
        }


        private static void BasicAttack(LogicTimeLineShell tlo, params object[] args)
        {
            Debug.Log("普通攻击");
        }
    }
}
