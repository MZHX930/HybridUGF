using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameDevScript
{
    /// <summary>
    ///子弹的"状态"，用来管理当前应该怎么移动、应该怎么旋转、应该怎么播放动画的。
    /// </summary>
    public class UnitBulletCtrl : MonoBehaviour, IUnitCtrl<UnitBulletCtrlData>, IShellShowHandler
    {
        #region 组件
        private UnitRotate m_UnitRotate;
        private UnitMove m_UnitMove;
        private ViewContainer m_ViewContainer;
        private UnitCollider m_UnitCollider;
        public ShellEntityLogic ShellEntityLogic { get; private set; }
        #endregion


        #region private data
        /// <summary>
        /// 是否加载视图中
        /// </summary>
        private bool m_WaitLoadView = true;
        #endregion


        #region public data
        ///<summary>
        ///子弹命中纪录
        ///</summary>
        public List<BulletHitRecord> HitRecordList = new List<BulletHitRecord>();

        ///<summary>
        ///角色移动力，单位：米/秒
        ///</summary>
        public float MoveSpeed
        {
            get
            {
                return CtrlData.MoveSpeedFactor * 5.600f / (CtrlData.MoveSpeedFactor + 100.000f) + 0.200f;
            }
        }
        #endregion

        /// <summary>
        /// 实体的数据
        /// </summary>
        public UnitBulletCtrlData CtrlData { get; private set; }
        public IUnitCtrlData BaseCtrlData => CtrlData;


        void Awake()
        {
            m_UnitRotate = gameObject.GetComponent<UnitRotate>();
            m_UnitMove = gameObject.GetComponent<UnitMove>();
            m_UnitCollider = gameObject.GetComponent<UnitCollider>();
            m_ViewContainer = gameObject.GetComponentInChildren<ViewContainer>();

            m_WaitLoadView = true;
        }

        public void OnShowShell(ShellEntityLogic shellEntityLogic)
        {
            ShellEntityLogic = shellEntityLogic;
            CtrlData = ShellEntityLogic.ShellData.UnitCtrlData as UnitBulletCtrlData;

            //初始角度方向
            transform.position = CtrlData.FirePosition;
            transform.up = CtrlData.FireDireNorm;

            m_WaitLoadView = true;
        }

        public void OnShowView(PrefabViewEntityLogic viewEntityLogic)
        {
            m_UnitCollider.ActiveCollision(CtrlData.GameSide == GameSideEnum.Player ? CollisionLayer.PlayerBullet : CollisionLayer.EnemyBullet);

            m_WaitLoadView = false;
        }

        public void OnHideShell(ShellEntityLogic shellEntityLogic)
        {
            m_WaitLoadView = true;
        }

        public void OnHideView(PrefabViewEntityLogic viewEntityLogic)
        {
            m_UnitCollider.RemoveCollision();
            m_WaitLoadView = true;
        }

        ///<summary>
        ///子弹是否碰到了碰撞
        ///</summary>
        public bool HitObstacle()
        {
            return m_UnitMove == null ? false : m_UnitMove.hitObstacle;
        }

        ///<summary>
        ///控制子弹移动，这应该是由bulletSystem来调用的
        ///</summary>
        public void SetMoveForce(Vector3 mf)
        {
            mf = mf * MoveSpeed;
            mf.z = 0;
            float moveDeg = Vector3.SignedAngle(Vector3.up, mf, Vector3.forward);

            m_UnitMove.MoveBy(mf);
            m_UnitRotate.RotateTo(moveDeg);
        }

        ///<summary>
        ///判断子弹是否还能击中某个GameObject
        ///<param name="target">目标gameObject</param>
        ///</summary>
        public bool CanHit(ShellEntityLogic target)
        {
            if (CtrlData.CanHitAfterCreated > 0)
                return false;

            for (int i = 0; i < this.HitRecordList.Count; i++)
            {
                if (HitRecordList[i].target == target.gameObject)
                {
                    return false;
                }
            }

            if (target && (target.UnitCtrl.BaseCtrlData as UnitCharacterCtrlData).ImmuneTime > 0)
                return false;

            return true;
        }

        ///<summary>
        ///添加命中纪录
        ///<param name="target">目标GameObject</param>
        ///</summary>
        public void AddHitRecord(GameObject target)
        {
            HitRecordList.Add(new BulletHitRecord(
                target,
                this.CtrlData.Model.SameTargetDelay
            ));
        }

        void FixedUpdate()
        {
            if (m_WaitLoadView || CtrlData.ResidualHitTimes <= 0)
                return;

            float timePassed = Time.fixedDeltaTime;
            //如果是刚创建的，那么就要处理刚创建的事情
            if (CtrlData.TimeElapsed <= 0 && CtrlData.Model.OnCreate != null)
            {
                CtrlData.Model.OnCreate(this);
            }

            //处理子弹命中纪录信息
            int hIndex = 0;
            while (hIndex < HitRecordList.Count)
            {
                HitRecordList[hIndex].timeToCanHit -= timePassed;
                if (HitRecordList[hIndex].timeToCanHit <= 0 || HitRecordList[hIndex].target == null)
                {
                    //理论上应该支持可以鞭尸，所以即使target dead了也得留着……
                    HitRecordList.RemoveAt(hIndex);
                }
                else
                {
                    hIndex += 1;
                }
            }

            //处理子弹的移动信息
            SetMoveForce(CtrlData.MoveTrackTween == null ? transform.up : CtrlData.MoveTrackTween(CtrlData.TimeElapsed, this, null));

            //处理子弹的碰撞信息，如果子弹可以碰撞，才会执行碰撞逻辑
            if (CtrlData.CanHitAfterCreated > 0)
            {
                CtrlData.CanHitAfterCreated -= timePassed;
            }
            else
            {
                if (GameEntry.CollisionCal.CheckAdverseCollisions(m_UnitCollider, out List<UnitCollider> otherColliderList))
                {
                    foreach (var adverseCollider in otherColliderList)
                    {
                        if (!CanHit(adverseCollider.ShellEntityLogic))
                            continue;

                        if (adverseCollider == null)
                            continue;
                        var adverseChaCtrl = (adverseCollider.ShellEntityLogic.UnitCtrl as IUnitCtrl<UnitCharacterCtrlData>) as UnitCharacterCtrl;
                        if (adverseChaCtrl.IsDead == true || adverseChaCtrl.CtrlData.ImmuneTime > 0)
                            continue;

                        //命中了
                        CtrlData.ResidualHitTimes -= 1;

                        if (CtrlData.Model.OnHit != null)
                        {
                            CtrlData.Model.OnHit(this, adverseChaCtrl);
                        }

                        if (CtrlData.ResidualHitTimes > 0)
                        {
                            AddHitRecord(adverseCollider.gameObject);
                            // break;
                        }
                        else
                        {
                            GameEntry.Entity.HideEntity(ShellEntityLogic.Entity);
                            return;
                        }
                    }
                }
            }

            ///生命周期的结算
            CtrlData.Duration -= timePassed;
            CtrlData.TimeElapsed += timePassed;
            if (CtrlData.Duration <= 0 || HitObstacle() == true)
            {
                if (CtrlData.Model.OnRemoved != null)
                {
                    CtrlData.Model.OnRemoved(this);
                }
                GameEntry.Entity.HideEntity(ShellEntityLogic.Entity);
            }
        }
    }
}
