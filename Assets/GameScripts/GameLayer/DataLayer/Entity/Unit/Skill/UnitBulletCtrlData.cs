using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    /// 子弹技能运行时数据
    /// </summary>
    public sealed class UnitBulletCtrlData : IUnitCtrlData
    {
        ///<summary>
        ///要发射的子弹
        ///</summary>
        public DRDefineBulletVFX Model;

        ///<summary>
        ///要发射子弹的这个人的gameObject，这里就认角色（拥有ChaState的）
        ///当然可以是null发射的，但是写效果逻辑的时候得小心caster是null的情况
        ///</summary>
        public UnitCharacterCtrl Caster;

        /// <summary>
        /// 阵营
        /// </summary>
        public GameSideEnum GameSide;

        ///<summary>
        ///发射的坐标
        ///</summary>
        public Vector3 FirePosition;

        ///<summary>
        ///发射的初始方向
        ///</summary>
        public Vector3 FireDireNorm;

        ///<summary>
        ///子弹当前的移动速度因子
        ///</summary>
        public float MoveSpeedFactor;

        ///<summary>
        ///子弹的生命周期，单位：秒
        ///</summary>
        public float Duration;

        ///<summary>
        ///子弹在发射瞬间，可以捕捉一个GameObject作为目标，并且将这个目标传递给BulletTween，作为移动参数
        ///<param name="bullet">是当前的子弹GameObject，不建议公式中用到这个</param>
        ///<param name="targets">所有可以被选作目标的对象，这里是GameManager的逻辑决定的传递过来谁，比如这个游戏子弹只能捕捉角色作为对象，那就是只有角色的GameObject，当然如果需要，加入子弹也不麻烦</param>
        ///<return>在创建子弹的瞬间，根据这个函数获得一个GameObject作为followingTarget</return>
        ///</summary>
        public BulletTargettingFunction TargetFunc;

        ///<summary>
        ///子弹的轨迹函数，传入一个时间点，返回出一个Vector3，作为这个时间点的速度和方向，这是个相对于正在飞行的方向的一个偏移（*speed的）
        ///正在飞行的方向按照z轴，来算，也就是说，当你只需要子弹匀速行动的时候，你可以让这个函数只做一件事情——return Vector3.forward。
        ///如果这个值是null，就会跟return Vector3.forward一样处理，性能还高一些。
        ///虽然是vector3，但是y坐标是无效的，只是为了统一单位
        ///比如手榴弹这种会一跳一跳的可不得y变化吗？是要变化，但是这个变化归我管，这是render的事情
        ///简单地说就是做一个跳跳的Component，update（而非fixedupdate）里面去管理跳吧
        ///<param name="t">子弹飞行了多久的时间点，单位秒。</param>
        ///<return>返回这一时间点上的速度和偏移，Vector3就是正常速度正常前进</return>
        ///</summary>
        public BulletTween MoveTrackTween;

        ///<summary>
        ///子弹的移动轨迹是否严格遵循发射出来的角度
        ///如果是true，则子弹每一帧Tween返回的角度是按照fireDegree来偏移的
        ///如果是false，则会根据子弹正在飞的角度(transform.rotation)来算下一帧的角度
        ///</summary>
        public bool UseFireDegreeForever;

        ///<summary>
        ///子弹创建后多久是没有碰撞的，这样比如子母弹之类的，不会在创建后立即命中目标，但绝大多子弹还应该是0的
        ///单位：秒
        ///</summary>
        public float CanHitAfterCreated;

        ///<summary>
        ///子弹的一些特殊逻辑使用的参数，可以在创建子的时候传递给子弹
        ///</summary>
        public Dictionary<string, object> Param;

        ///<summary>
        ///子弹发射时候，caster的属性，如果caster不存在，就会是一个ChaProperty.zero
        ///在一些设计中，比如wow的技能中，技能效果是跟发出时候的角色状态有关的，之后即使获得或者取消了buff，更换了装备，数值一样不会受到影响，所以得记录这个释放当时的值
        ///</summary>
        public ChaProperty PropWhileCast;

        ///<summary>
        ///子弹已经存在了多久了，单位：秒
        ///毕竟duration是可以被重设的，比如经过一个aoe，生命周期减半了
        ///</summary>
        public float TimeElapsed = 0;

        ///<summary>
        ///子弹还能命中几次
        ///</summary>
        public int ResidualHitTimes = 0;


        public void Clear()
        {
            Model = default;
            Caster = null;
            FirePosition = default;
            FireDireNorm = default;
            MoveSpeedFactor = 0;
            Duration = 0;
            TargetFunc = null;
            MoveTrackTween = null;
            UseFireDegreeForever = false;
            CanHitAfterCreated = 0;
            if (Param != null)
            {
                Param.Clear();
            }
            Param = null;
        }

        /// <summary>
        /// 克隆数据，用于创建技能镜像。SerialId是不同的了
        /// </summary>
        public UnitBulletCtrlData Clone()
        {
            var clonedData = Create(
                this.Model,
                this.Caster,
                this.GameSide,
                this.FirePosition,
                this.FireDireNorm,
                this.MoveSpeedFactor,
                this.Duration,
                this.CanHitAfterCreated,
                this.MoveTrackTween,
                this.TargetFunc,
                this.UseFireDegreeForever,
                this.Param
            );
            return clonedData;
        }

        public static UnitBulletCtrlData Create(
            DRDefineBulletVFX model,
            UnitCharacterCtrl caster,
            GameSideEnum gameSide,
            Vector3 firePos,
            Vector3 fireDireNorm,
            float speed,
            float duration,
            float canHitAfterCreated = 0,
            BulletTween tween = null,
            BulletTargettingFunction targetFunc = null,
            bool useFireDegree = false,
            Dictionary<string, object> param = null
        )
        {
            UnitBulletCtrlData entityData = ReferencePool.Acquire<UnitBulletCtrlData>();
            entityData.Model = model;
            entityData.Caster = caster;
            entityData.GameSide = gameSide;
            entityData.FirePosition = firePos;
            entityData.FireDireNorm = fireDireNorm;
            entityData.MoveSpeedFactor = speed;
            entityData.Duration = duration;
            entityData.CanHitAfterCreated = canHitAfterCreated;
            entityData.MoveTrackTween = tween;
            entityData.TargetFunc = targetFunc;
            entityData.UseFireDegreeForever = useFireDegree;
            entityData.Param = param;

            ///初始化数据
            if (caster)
            {
                entityData.PropWhileCast = caster.CtrlData.CurProperty;
            }
            entityData.TimeElapsed = 0;
            entityData.ResidualHitTimes = model.HitTimes;

            return entityData;
        }
    }
}
