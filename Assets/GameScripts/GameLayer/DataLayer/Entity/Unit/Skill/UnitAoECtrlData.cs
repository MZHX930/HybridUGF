using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// aoe技能实体数据
    /// 创建aoe依赖的数据都在这里了
    /// </summary>
    public class UnitAoECtrlData : IUnitCtrlData
    {
        ///<summary>
        ///要释放的aoe
        ///</summary>
        public DRDefineAoeVFX Model;

        ///<summary>
        ///释放的中心坐标
        ///</summary>
        public Vector3 FirePos;

        ///<summary>
        ///aoe的施法者
        ///释放aoe的角色的GameObject，当然可能是null的
        ///</summary>
        public Entity Caster;

        /// <summary>
        /// 阵营
        /// </summary>
        public GameSideEnum GameSide;

        // ///<summary>
        // ///aoe的半径，单位：米
        // ///目前这游戏的设计中，aoe只有圆形，所以只有一个半径，也不存在角度一说，如果需要可以扩展
        // ///</summary>
        // public float Radius;

        ///<summary>
        ///aoe存在的剩余时间，单位：秒
        ///</summary>
        public float Duration;

        ///<summary>
        ///aoe已经存在过的时间，单位：秒
        ///</summary>
        public float TimeElapsed = 0;

        ///<summary>
        ///aoe的角度
        ///</summary>
        public float Degree;

        ///<summary>
        ///aoe移动轨迹函数
        ///</summary>
        public AoeMoveTween MoveTween;

        ///<summary>
        ///Tween函数的参数
        ///</summary>
        public object[] TweenParam;

        ///<summary>
        ///aoe的轨迹运行了多少时间了，单位：秒
        ///<summary>
        public float MoveTweenRunnedTime = 0;

        ///<summary>
        ///创建时的角色属性
        ///</summary>
        public ChaProperty PropWhileCreate;

        ///<summary>
        ///aoe的传入参数，比如可以吸收次数之类的
        ///</summary>
        public Dictionary<string, object> AoeParam = new Dictionary<string, object>();


        public UnitAoECtrlData Clone()
        {
            return UnitAoECtrlData.Create(
                this.Model,
                this.Caster,
                this.GameSide,
                this.FirePos,
                this.Duration,
                this.AoeParam
            );
        }

        public void Clear()
        {
            Model = null;
            Caster = null;
            FirePos = Vector3.zero;
            // Radius = 0;
            Duration = 0;
            Degree = 0;
            MoveTween = null;
            TweenParam = null;
            MoveTweenRunnedTime = 0;
            PropWhileCreate = ChaProperty.zero;
            AoeParam.Clear();
        }

        public static UnitAoECtrlData Create(DRDefineAoeVFX model, Entity caster, GameSideEnum gameSide, Vector3 firePos, float duration, Dictionary<string, object> aoeParam = null)
        {
            UnitAoECtrlData data = ReferencePool.Acquire<UnitAoECtrlData>();
            data.Model = model;
            data.Caster = caster;
            data.GameSide = gameSide;
            data.FirePos = firePos;
            // data.Radius = radius;
            data.Duration = duration;
            data.Degree = 0;
            data.AoeParam = aoeParam;

            return data;
        }
    }
}
