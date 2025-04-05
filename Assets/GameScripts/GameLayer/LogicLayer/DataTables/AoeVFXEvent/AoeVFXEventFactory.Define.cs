using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    public static partial class AoeSkillDelegateFactory
    {
        /// <summary>
        /// aoe创建时的事件
        /// </summary>
        public static Dictionary<string, AoeOnCreate> OnCreateFuncDic = new Dictionary<string, AoeOnCreate>()
        {
            {"CreateSightEffect", CreateSightEffect}
        };

        /// <summary>
        /// aoe移除时的事件
        /// </summary>
        public static Dictionary<string, AoeOnRemoved> OnRemovedFuncDic = new Dictionary<string, AoeOnRemoved>()
        {
            // {"DoDamageOnRemoved", DoDamageOnRemoved},
            // {"CreateAoeOnRemoved", CreateAoeOnRemoved},
            // {"BarrelExplosed", BarrelExplosed}
        };

        /// <summary>
        /// aoe每一跳的事件
        /// </summary>
        public static Dictionary<string, AoeOnTick> OnTickFuncDic = new Dictionary<string, AoeOnTick>()
        {
            // {"BlackHole", BlackHole}
        };

        /// <summary>
        /// 当有角色进入aoe范围的时候触发
        /// </summary>
        public static Dictionary<string, AoeOnCharacterEnter> OnChaEnterFuncDic = new Dictionary<string, AoeOnCharacterEnter>()
        {
            // {"DoDamageToEnterCha", DoDamageToEnterCha}
        };

        /// <summary>
        /// 当有角色离开aoe范围的时候触发
        /// </summary>
        public static Dictionary<string, AoeOnCharacterLeave> OnChaLeaveFuncDic = new Dictionary<string, AoeOnCharacterLeave>()
        {

        };

        // /// <summary>
        // /// 当有子弹进入aoe范围的时候触发
        // /// </summary>
        // public static Dictionary<string, AoeOnBulletEnter> onBulletEnterFunc = new Dictionary<string, AoeOnBulletEnter>()
        // {
        //     // {"BlockBullets", BlockBullets},
        //     // {"SpaceMonkeyBallHit", SpaceMonkeyBallHit}
        // };

        // /// <summary>
        // /// 当有子弹离开aoe范围的时候触发
        // /// </summary>
        // public static Dictionary<string, AoeOnBulletLeave> onBulletLeaveFunc = new Dictionary<string, AoeOnBulletLeave>()
        // {
        // };

        /// <summary>
        /// aoe的tween事件
        /// </summary>
        public static Dictionary<string, AoeMoveTween> AoeTweenFuncDic = new Dictionary<string, AoeMoveTween>()
        {
            // {"AroundCaster", AroundCaster},
            // {"SpaceMonkeyBallRolling", SpaceMonkeyBallRolling}
        };
    }

    ///<summary>
    ///aoe的移动信息
    ///</summary>
    public class AoeMoveInfo
    {
        ///<summary>
        ///此时此刻的移动方式
        ///</summary>
        public MoveType MoveType;

        ///<summary>
        ///此时aoe移动的力量，在这个游戏里，y坐标依然无效，如果要做手雷一跳一跳的，请使用其他的component绑定到特效的gameobject上，而非aoe的
        ///</summary>
        public Vector3 Velocity;

        ///<summary>
        ///aoe的角度变成这个值
        ///</summary>
        public float RotateToDegree;

        public AoeMoveInfo(MoveType moveType, Vector3 velocity, float rotateToDegree)
        {
            this.MoveType = moveType;
            this.Velocity = velocity;
            this.RotateToDegree = rotateToDegree;
        }
    }

    ///<summary>
    ///aoe创建时的事件
    ///<param name="aoe">被创建出来的aoe的gameObject</param>
    ///</summary>
    public delegate void AoeOnCreate(UnitAoECtrl aoe);

    ///<summary>
    ///aoe移除时候的事件
    ///<param name="aoe">被创建出来的aoe的gameObject</param>
    ///</summary>
    public delegate void AoeOnRemoved(UnitAoECtrl aoe);

    ///<summary>
    ///aoe每一跳的事件
    ///<param name="aoe">被创建出来的aoe的gameObject</param>
    ///</summary>
    public delegate void AoeOnTick(UnitAoECtrl aoe);

    ///<summary>
    ///当有角色进入aoe范围的时候触发
    ///<param name="aoe">被创建出来的aoe的gameObject</param>
    ///<param name="cha">进入aoe范围的那些角色，他们现在还不在aoeState的角色列表里</param>
    ///</summary>
    public delegate void AoeOnCharacterEnter(UnitAoECtrl aoe, List<UnitCharacterCtrl> cha);

    ///<summary>
    ///当有角色离开aoe范围的时候
    ///<param name="aoe">离开aoe的gameObject</param>
    ///<param name="cha">离开aoe范围的那些角色，他们现在已经不在aoeState的角色列表里</param>
    ///</summary>
    public delegate void AoeOnCharacterLeave(UnitAoECtrl aoe, List<UnitCharacterCtrl> cha);

    // ///<summary>
    // ///当有子弹进入aoe范围的时候
    // ///<param name="aoe">被创建出来的aoe的gameObject</param>
    // ///<param name="bullet">离开aoe范围的那些子弹，他们现在已经不在aoeState的子弹列表里</param>
    // ///</summary>
    // public delegate void AoeOnBulletEnter(GameObject aoe, List<GameObject> bullet);

    // ///<summary>
    // ///当有子弹离开aoe范围的时候
    // ///<param name="aoe">离开的aoe的gameObject</param>
    // ///<param name="bullet">离开aoe范围的那些子弹，他们现在已经不在aoeState的子弹列表里</param>
    // ///</summary>
    // public delegate void AoeOnBulletLeave(GameObject aoe, List<GameObject> bullet);

    ///<summary>
    ///aoe的移动轨迹函数
    ///<param name="aoe">要执行的aoeObj</param>
    ///<param name="t">这个tween在aoe中运行了多久了，单位：秒</param>
    ///<return>aoe在这时候的移动信息</param>
    public delegate AoeMoveInfo AoeMoveTween(UnitAoECtrl aoe, float t);
}
